// 
// Copyright © 2010-2019, Sinclair Community College
// Licensed under the GNU General Public License, version 3.
// See the LICENSE file in the project root for full license information.  
//
// This file is part of Make Me Admin.
//
// Make Me Admin is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3.
//
// Make Me Admin is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Make Me Admin. If not, see <http://www.gnu.org/licenses/>.
//

namespace SinclairCC.MakeMeAdmin
{
    using Microsoft.Diagnostics.Tracing.Parsers;
    using Microsoft.Diagnostics.Tracing.Parsers.Kernel;
    using Microsoft.Diagnostics.Tracing.Session;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Collections.Generic;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceProcess;
    using System.Threading.Tasks;
    using System.Security.Cryptography;

    /// <summary>
    /// This class is the Windows Service, which does privileged work
    /// on behalf of the an unprivileged user.
    /// </summary>
    public partial class MakeMeAdminService : ServiceBase
    {
        /// <summary>
        /// A timer to monitor when administrator rights should be removed.
        /// </summary>
        private readonly System.Timers.Timer removalTimer;

        /// <summary>
        /// A Windows Communication Foundation (WCF) service host which communicates over named pipes.
        /// </summary>
        /// <remarks>
        /// This service host exists for communication on the local computer. It is not accessible
        /// from remote computers, and it is therefore always enabled.
        /// </remarks>
        private ServiceHost namedPipeServiceHost = null;

        /// <summary>
        /// A Windows Communication Foundation (WCF) service host which communicates over TCP.
        /// </summary>
        /// <remarks>
        /// This service host exists for communication from remote computers. It is only
        /// created if the remote administrator rights setting is enabled (true).
        /// </remarks>
        private ServiceHost tcpServiceHost = null;

        private TraceEventSession processWatchSession = null;

        private readonly static List<ProcessInformation> processList = new List<ProcessInformation>();

        private readonly static Queue<ElevatedProcessInformation> elevatedProcessList = new Queue<ElevatedProcessInformation>();

        // TODO: Is this name the same on non-English Windows?
        private readonly string portSharingServiceName = "NetTcpPortSharing";



        /// <summary>
        /// Instantiate a new instance of the Make Me Admin Windows service.
        /// </summary>
        public MakeMeAdminService()
        {
            InitializeComponent();

            /*
            this.CanHandleSessionChangeEvent = true;
            */

            this.EventLog.Source = "Make Me Admin";
            this.AutoLog = false;

            this.removalTimer = new System.Timers.Timer()
            {
                Interval = 10000,   // Raise the Elapsed event every ten seconds.
                AutoReset = true    // Raise the Elapsed event repeatedly.
            };
            this.removalTimer.Elapsed += RemovalTimerElapsed;
        }

        private void Dynamic_All(Microsoft.Diagnostics.Tracing.TraceEvent obj)
        {
            // TODO: Is this name the same on non-English Windows?
            if ((obj.Opcode == Microsoft.Diagnostics.Tracing.TraceEventOpcode.Start) && (string.Compare(obj.TaskName, "ProcessStart", true) == 0))
            {
                int processIsElevated = 0;
                int processElevationType = 0;
                int processId = int.MinValue;
                int sessionId = int.MinValue;
                DateTime createTime = DateTime.MinValue;
                int index = int.MinValue;

                // TODO: Are these strings the same on non-English Windows?
                index = obj.PayloadIndex("ProcessTokenIsElevated");
                if (index >= 0)
                {
                    processIsElevated = (int)obj.PayloadValue(index);
                }

                if (processIsElevated == 1)
                {
                    // TODO: Are these strings the same on non-English Windows?
                    index = obj.PayloadIndex("ProcessID");
                    if (index >= 0)
                    {
                        processId = (int)obj.PayloadValue(index);
                    }

                    ElevatedProcessInformation elevatedProcess = new ElevatedProcessInformation
                    {
                        ProcessID = processId
                    };

                    // TODO: Are these strings the same on non-English Windows?
                    index = obj.PayloadIndex("ProcessTokenElevationType");
                    if (index >= 0)
                    {
                        processElevationType = (int)obj.PayloadValue(index);
                        elevatedProcess.ElevationType = (TokenElevationType)processElevationType;
                    }

                    // TODO: Are these strings the same on non-English Windows?
                    index = obj.PayloadIndex("SessionID");
                    if (index >= 0)
                    {
                        sessionId = (int)obj.PayloadValue(index);
                        elevatedProcess.SessionID = sessionId;
                    }

                    // TODO: Are these strings the same on non-English Windows?
                    index = obj.PayloadIndex("CreateTime");
                    if (index >= 0)
                    {
                        createTime = (DateTime)obj.PayloadValue(index);
                        elevatedProcess.CreateTime = createTime;
                    }

                    // Determine whether the process should be logged. It should be logged if
                    // 1. The process logging setting is set to always, or
                    // 2. The process logging is set to "Only When Admin" and the user is in the admins group.
                    bool processShouldBeLogged = (Settings.LogElevatedProcesses == ElevatedProcessLogging.Always);
                    if (Settings.LogElevatedProcesses == ElevatedProcessLogging.OnlyWhenAdmin)
                    {
                        NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
                        ChannelFactory<IAdminGroup> namedPipeFactory = new ChannelFactory<IAdminGroup>(binding, Settings.NamedPipeServiceBaseAddress);
                        IAdminGroup channel = namedPipeFactory.CreateChannel();
                        processShouldBeLogged = channel.UserSessionIsInList(elevatedProcess.SessionID);
                        namedPipeFactory.Close();
                    }

                    if (processShouldBeLogged)
                    {
                        elevatedProcessList.Enqueue(elevatedProcess);
                    }
                }
            }
        }

        private void Kernel_ProcessStart(ProcessTraceData processInfo)
        {
            if (processInfo.Opcode == Microsoft.Diagnostics.Tracing.TraceEventOpcode.Start)
            {
                ProcessInformation startedProcess = new ProcessInformation()
                {
                    CommandLine = processInfo.CommandLine,
                    ImageFileName = processInfo.ImageFileName,
                    ProcessID = processInfo.ProcessID,
                    SessionID = processInfo.SessionID,
                    CreateTime = processInfo.TimeStamp,
                };

                SecurityIdentifier sid = LsaLogonSessions.LogonSessions.GetSidForSessionId(startedProcess.SessionID);
                if (sid != null)
                {
                    startedProcess.UserSIDString = sid.Value;
                }

                processList.Add(startedProcess);
            }
        }

        /// <summary>
        /// Handles the Elapsed event for the rights removal timer.
        /// </summary>
        /// <param name="sender">
        /// The timer whose Elapsed event is firing.
        /// </param>
        /// <param name="e">
        /// Data related to the event.
        /// </param>
        private void RemovalTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            const int ID_YES = 0x00000006;
            int MB_YESNO = 0x00000004;
            int MB_ICONINFORMATION = 0x00000040;
            int MB_DEFBUTTON2 = 0x00000100;

            EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);

            User[] expiredUsers = encryptedSettings.GetExpiredUsers();

            if (expiredUsers != null)
            {
                foreach (User prin in expiredUsers)
                {
                    // Get a WindowsIdentity object for the user matching the added user SID.
                    WindowsIdentity sessionIdentity = null;
                    WindowsIdentity userIdentity = null;
                    int response;
                    int sendToSessionId = 0;
                    bool performRemoval = true;
                    int[] currentSessionIds = LsaLogonSessions.LogonSessions.GetLoggedOnUserSessionIds();
                    foreach (int sessionId in currentSessionIds)
                    {
                        if (LsaLogonSessions.LogonSessions.GetSidForSessionId(sessionId) == prin.Sid)
                        {
                            sendToSessionId = sessionId;
                        }
                        sessionIdentity = LsaLogonSessions.LogonSessions.GetWindowsIdentityForSessionId(sessionId);
                        if ((sessionIdentity != null) && (sessionIdentity.User == prin.Sid))
                        {
                            userIdentity = sessionIdentity;
                        }
                    }

                    if ((Settings.RenewalsAllowed > 0) && (prin.RenewalsUsed < Settings.RenewalsAllowed))
                    {
                        // TODO: Might want something other than a hard 30 seconds here.
                        int returnValue = LsaLogonSessions.LogonSessions.SendMessageToSession(sendToSessionId, Properties.Resources.RenewalAllowed, (MB_YESNO + MB_ICONINFORMATION + MB_DEFBUTTON2), 30, out response);
                        switch (response)
                        {
                            case ID_YES:
                                int timeoutMinutes = AdminGroupManipulator.GetTimeoutForUser(userIdentity);
                                encryptedSettings.AddedUsers[prin.Sid].ExpirationTime = DateTime.Now.AddMinutes(timeoutMinutes);
                                encryptedSettings.AddedUsers[prin.Sid].RenewalsUsed++;
                                encryptedSettings.Save();
                                performRemoval = false;
                                break;
                        }
                    }

                    if (performRemoval)
                    {
                        LocalAdministratorGroup.RemoveUser(prin.Sid, RemovalReason.Timeout);

                        if ((Settings.EndRemoteSessionsUponExpiration) && (!string.IsNullOrEmpty(prin.RemoteAddress)))
                        {
                            string userName = prin.Name;
                            while (userName.LastIndexOf("\\") >= 0)
                            {
                                userName = userName.Substring(userName.LastIndexOf("\\") + 1);
                            }

                            // TODO: Log this return code if it's not a success?
                            int returnCode = 0;
                            if (!string.IsNullOrEmpty(userName))
                            {
                                returnCode = LocalAdministratorGroup.EndNetworkSession(string.Format(@"\\{0}", prin.RemoteAddress), userName);
                            }
                        }
                    }
                }
            }

            LocalAdministratorGroup.ValidateAllAddedUsers();

            if (Settings.LogElevatedProcesses != ElevatedProcessLogging.Never)
            {
                if (this.processWatchSession == null) { StartTracing(); }
                if (this.processWatchSession != null) { LogProcesses(); }
            }
        }


        private void LogProcesses()
        {
            processList.RemoveAll(pi => DateTime.Now.Subtract(pi.CreateTime).TotalMinutes > 2);

            bool itemDequeued = false;
            do
            {
                itemDequeued = false;
                if (elevatedProcessList.Count > 0)
                {
                    ElevatedProcessInformation nextProcess = elevatedProcessList.Peek();
                    if (DateTime.Now.Subtract(nextProcess.CreateTime).TotalSeconds >= 60)
                    {
                        // TODO: Process has been in the queue longer than 60 seconds. Log that it could not be matched, and move on.
                        elevatedProcessList.Dequeue();
                        itemDequeued = true;
                    }
                    else
                    {
                        nextProcess = elevatedProcessList.Dequeue();
                        itemDequeued = true;

                        processList.FindAll(p => (p.ProcessID == nextProcess.ProcessID) && (p.SessionID == nextProcess.SessionID)).ForEach(action =>
                        {
                            LoggingProvider.Log.ElevatedProcessDetected(nextProcess.ElevationType, action);
                        });
                    }
                }
            } while ((itemDequeued) && (elevatedProcessList.Count > 0));
        }


        /// <summary>
        /// Creates the WCF Service Host which is accessible via named pipes.
        /// </summary>
        private void OpenNamedPipeServiceHost()
        {
            if (null != this.namedPipeServiceHost)
            {
                this.namedPipeServiceHost.Close();
            }
            this.namedPipeServiceHost = new ServiceHost(typeof(AdminGroupManipulator), new Uri(Settings.NamedPipeServiceBaseAddress));
            this.namedPipeServiceHost.Faulted += ServiceHostFaulted;
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
            this.namedPipeServiceHost.AddServiceEndpoint(typeof(IAdminGroup), binding, Settings.NamedPipeServiceBaseAddress);
            this.namedPipeServiceHost.Open();
        }


        /// <summary>
        /// Creates the WCF Service Host which is accessible via TCP.
        /// </summary>
        private void OpenTcpServiceHost()
        {
            if ((null != this.tcpServiceHost) && (this.tcpServiceHost.State == CommunicationState.Opened))
            {
                this.tcpServiceHost.Close();
            }

            this.tcpServiceHost = new ServiceHost(typeof(AdminGroupManipulator), new Uri(Settings.TcpServiceBaseAddress));
            this.tcpServiceHost.Faulted += ServiceHostFaulted;
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport)
            {
                PortSharingEnabled = true
            };

            // If port sharing is enabled, then the Net.Tcp Port Sharing Service must be available as well.
            if (PortSharingServiceExists)
            {
                ServiceController controller = new ServiceController(portSharingServiceName);
                switch (controller.StartType)
                {
                    case ServiceStartMode.Disabled:
                        ApplicationLog.WriteEvent(Properties.Resources.PortSharingServiceDisabledMessage, EventID.RemoteAccessFailure, System.Diagnostics.EventLogEntryType.Warning);
                        return;
                    /*
                    case ServiceStartMode.Automatic:
                        break;
                    case ServiceStartMode.Manual:
                        int waitCount = 0;
                        while ((controller.Status != ServiceControllerStatus.Running) && (waitCount < 10))
                        {
                            switch (controller.Status)
                            {
                                case ServiceControllerStatus.Paused:
                                    controller.Continue();
                                    controller.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 5));
                                    break;
                                case ServiceControllerStatus.Stopped:
                                    try
                                    {
                                        controller.Start();
                                        controller.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 5));
                                    }
                                    catch (Win32Exception win32Exception)
                                    {
                                        ApplicationLog.WriteEvent(win32Exception.Message, EventID.RemoteAccessFailure, System.Diagnostics.EventLogEntryType.Error);
                                    }
                                    catch (InvalidOperationException invalidOpException)
                                    {
                                        ApplicationLog.WriteEvent(invalidOpException.Message, EventID.RemoteAccessFailure, System.Diagnostics.EventLogEntryType.Error);
                                    }
                                    break;
                            }
                            System.Threading.Thread.Sleep(1000);
                            waitCount++;
                        }

                        if (controller.Status != ServiceControllerStatus.Running)
                        {
                            // TODO: i18n.
                            ApplicationLog.WriteEvent(string.Format("Port {0} is already in use, but the Net.Tcp Port Sharing Service is not running. Remote access will not be available.", Settings.TCPServicePort), EventID.RemoteAccessFailure, System.Diagnostics.EventLogEntryType.Warning);
                        }

                        break;
                    */
                }
                controller.Close();
            }
            else
            {
                // TODO: i18n.  
                ApplicationLog.WriteEvent(string.Format("Port {0} is already in use, but the Net.Tcp Port Sharing Service does not exist. Remote access will not be available.", Settings.TCPServicePort), EventID.RemoteAccessFailure, System.Diagnostics.EventLogEntryType.Warning);
                return;
            }

            this.tcpServiceHost.AddServiceEndpoint(typeof(IAdminGroup), binding, Settings.TcpServiceBaseAddress);

            try
            {
                this.tcpServiceHost.Open();
            }
            catch (ObjectDisposedException)
            {
                // TODO: i18n.
                ApplicationLog.WriteEvent("The communication object is in a Closing or Closed state and cannot be modified.", EventID.RemoteAccessFailure, System.Diagnostics.EventLogEntryType.Warning);
            }
            catch (InvalidOperationException)
            {
                // TODO: i18n.
                ApplicationLog.WriteEvent("The communication object is not in a Opened or Opening state and cannot be modified.", EventID.RemoteAccessFailure, System.Diagnostics.EventLogEntryType.Warning);
            }
            catch (CommunicationObjectFaultedException)
            {
                // TODO: i18n.
                ApplicationLog.WriteEvent("The communication object is in a Faulted state and cannot be modified.", EventID.RemoteAccessFailure, System.Diagnostics.EventLogEntryType.Warning);
            }
            catch (System.TimeoutException)
            {
                // TODO: i18n.
                ApplicationLog.WriteEvent("The default interval of time that was allotted for the operation was exceeded before the operation was completed.", EventID.RemoteAccessFailure, System.Diagnostics.EventLogEntryType.Warning);
            }
        }

        private bool PortSharingServiceExists
        {
            get
            {
                bool serviceExists = false;
                ServiceController[] services = ServiceController.GetServices();
                for (int i = 0; (i < services.Length) && (!serviceExists); i++)
                {
                    serviceExists |= (string.Compare(services[i].ServiceName, portSharingServiceName, true) == 0);
                }
                return serviceExists;
            }
        }

        /*
        private bool TcpPortInUse
        {
            get
            {
                System.Net.NetworkInformation.IPGlobalProperties globalIPProps = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
                return globalIPProps.GetActiveTcpListeners().Where(n => n.Port == Settings.TCPServicePort).Count() > 0;
            }
        }
        */

        /// <summary>
        /// Handles the faulted event for a WCF service host.
        /// </summary>
        /// <param name="sender">
        /// The service host that has entered the faulted state.
        /// </param>
        /// <param name="e">
        /// Data related to the event.
        /// </param>
        private void ServiceHostFaulted(object sender, EventArgs e)
        {
            ApplicationLog.WriteEvent(Properties.Resources.ServiceHostFaulted, EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Warning);
        }


        /// <summary>
        /// Handles the startup of the service. 
        /// </summary>
        /// <param name="args">
        /// Data passed the start command.
        /// </param>
        /// <remarks>
        /// This function executes when a Start command is sent to the service by the
        /// Service Control Manager (SCM) or when the operating system starts
        /// (for a service that starts automatically).
        /// </remarks>
        protected override void OnStart(string[] args)
        {
            try
            {
                base.OnStart(args);
            }
            catch (Exception) { };

            // Create the Windows Event Log source for this application.
            ApplicationLog.CreateSource();

            // Open the service host which is accessible via named pipes.
            this.OpenNamedPipeServiceHost();

            // If remote requests are allowed, open the service host which
            // is accessible via TCP.
            if (Settings.AllowRemoteRequests)
            {
                try
                {
                    this.OpenTcpServiceHost();
                }
                catch (AddressAlreadyInUseException addressInUseException)
                {
                    System.Text.StringBuilder logMessage = new System.Text.StringBuilder(addressInUseException.Message);
                    logMessage.Append(System.Environment.NewLine);
                    // TODO: i18n.
                    logMessage.Append(string.Format("Determine whether another application is using TCP port {0:N0}.", Settings.TCPServicePort));
                    ApplicationLog.WriteEvent(logMessage.ToString(), EventID.RemoteAccessFailure, System.Diagnostics.EventLogEntryType.Warning);
                }
                catch (Exception)
                {
                    // TODO: i18n.
                    ApplicationLog.WriteEvent("Unhandled exception while opening the remote request handler. Remote requests may not be honored.", EventID.RemoteAccessFailure, System.Diagnostics.EventLogEntryType.Warning);
                }
            }

            if ((Settings.LogElevatedProcesses != ElevatedProcessLogging.Never) && (this.processWatchSession == null))
            {
                StartTracing();
            }

            // Start the timer that watches for expired administrator rights.
            this.removalTimer.Start();
        }

        private void StartTracing()
        {
            Task.Factory.StartNew(
                () =>
                {
                    if (this.processWatchSession == null)
                    {
                        processWatchSession = new TraceEventSession("Make Me Admin Process Watch")
                        {
                            StopOnDispose = true
                        };

                        try
                        {
                            // Turn on the process events (includes starts and stops).  
                            processWatchSession.EnableKernelProvider(KernelTraceEventParser.Keywords.Process);

                            // Enable the Microsoft-Windows-Kernel-Process provider.
                            processWatchSession.EnableProvider(Guid.Parse("22fb2cd6-0e7b-422b-a0c7-2fad1fd0e716"));

                            // Ask the DynamicTraceEventParser to raise an event for all events that it knows about.
                            processWatchSession.Source.Dynamic.All += Dynamic_All;

                            // Raise an event for every process start event that the kernel knows about.
                            processWatchSession.Source.Kernel.ProcessStart += Kernel_ProcessStart;

                            // Listen for events.
                            processWatchSession.Source.Process();
                        }
                        catch (Exception exxx)
                        {
                            ApplicationLog.WriteEvent(exxx.Message, EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Error);
                        }
                    }
                },
                TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Handles the stopping of the service.
        /// </summary>
        /// <remarks>
        /// Executes when a stop command is sent to the service by the Service Control Manager (SCM).
        /// </remarks>
        protected override void OnStop()
        {
            EncryptedSettings.RemoveOldUsersFile();

            if ((this.namedPipeServiceHost != null) && (this.namedPipeServiceHost.State == CommunicationState.Opened))
            {
                this.namedPipeServiceHost.Close();
            }

            if ((this.tcpServiceHost != null) && (this.tcpServiceHost.State == CommunicationState.Opened))
            {
                this.tcpServiceHost.Close();
            }

            this.removalTimer.Stop();

            EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
            SecurityIdentifier[] sids = encryptedSettings.AddedUserSIDs;
            for (int i = 0; i < sids.Length; i++)
            {
                LocalAdministratorGroup.RemoveUser(sids[i], RemovalReason.ServiceStopped);
            }

            if (processWatchSession != null)
            {
                try
                {
                    processWatchSession.Dispose();
                }
                catch (Exception e)
                {
                    // TODO: i18n.
                    ApplicationLog.WriteEvent(string.Format("{0} exception while stopping process watcher. Message: {1}", e.GetType().Name, e.Message), EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Error);
                }
            }

            base.OnStop();
        }


        /// <summary>
        /// Executes when a change event is received from a Terminal Server session.
        /// </summary>
        /// <param name="changeDescription">
        /// Identifies the type of session change and the session to which it applies.
        /// </param>
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            switch (changeDescription.Reason)
            {
                // The user has logged off from a session, either locally or remotely.
                case SessionChangeReason.SessionLogoff:

                    EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
                    System.Collections.Generic.List<SecurityIdentifier> sidsToRemove = new System.Collections.Generic.List<SecurityIdentifier>(encryptedSettings.AddedUserSIDs);

                    int[] sessionIds = LsaLogonSessions.LogonSessions.GetLoggedOnUserSessionIds();

                    // For any user that is still logged on, remove their SID from the list of
                    // SIDs to be removed from Administrators. That is, let the users who are still
                    // logged on stay in the Administrators group.
                    foreach (int id in sessionIds)
                    {
                        SecurityIdentifier sid = LsaLogonSessions.LogonSessions.GetSidForSessionId(id);
                        if (sid != null)
                        {
                            if (sidsToRemove.Contains(sid))
                            {
                                sidsToRemove.Remove(sid);
                            }
                        }
                    }

                    // Process the list of SIDs to be removed from Administrators.
                    for (int i = 0; i < sidsToRemove.Count; i++)
                    {
                        if (
                            // If the user is not remote.
                            (!(encryptedSettings.ContainsSID(sidsToRemove[i]) && encryptedSettings.IsRemote(sidsToRemove[i])))
                            &&
                            // If admin rights are to be removed on logoff, or the user's rights do not expire.
                            (Settings.RemoveAdminRightsOnLogout || !encryptedSettings.GetExpirationTime(sidsToRemove[i]).HasValue)
                            )
                        {
                            LocalAdministratorGroup.RemoveUser(sidsToRemove[i], RemovalReason.UserLogoff);
                        }
                    }

                    /*
                     * In theory, this code should remove the user associated with the logoff, but it doesn't work.
                    SecurityIdentifier sid = LsaLogonSessions.LogonSessions.GetSidForSessionId(changeDescription.SessionId);
                    if (!(UserList.ContainsSID(sid) && UserList.IsRemote(sid)))
                    {
                        LocalAdministratorGroup.RemoveUser(sid, RemovalReason.UserLogoff);
                    }
                    */

                    break;

                // The user has logged on to a session, either locally or remotely.
                case SessionChangeReason.SessionLogon:


                    WindowsIdentity userIdentity = LsaLogonSessions.LogonSessions.GetWindowsIdentityForSessionId(changeDescription.SessionId);

                    /*
#if DEBUG
                    ApplicationLog.WriteEvent(string.Format("Session Logon : {0}", userIdentity.Name), EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
#endif
                    */

                    if (null != userIdentity)
                    {
                        AdminGroupManipulator adminGroupManipulator = new AdminGroupManipulator();
                        bool userIsAuthorizedForAutoAdd = adminGroupManipulator.UserIsAuthorized(userIdentity, Settings.AutomaticAddAllowed, Settings.AutomaticAddDenied);
                        /*
                        NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
                        ChannelFactory<IAdminGroup> namedPipeFactory = new ChannelFactory<IAdminGroup>(binding, Settings.NamedPipeServiceBaseAddress);
                        IAdminGroup channel = namedPipeFactory.CreateChannel();
                        bool userIsAuthorizedForAutoAdd = channel.UserIsAuthorized(Settings.AutomaticAddAllowed, Settings.AutomaticAddDenied);
                        namedPipeFactory.Close();
                        */

                        // If the user is in the automatic add list, then add them to the Administrators group.
                        if (
                            (Settings.AutomaticAddAllowed != null) &&
                            (Settings.AutomaticAddAllowed.Length > 0) &&
                            (userIsAuthorizedForAutoAdd)
                           )
                        {
                            LocalAdministratorGroup.AddUser(userIdentity, null, null);
                        }
                    }
                    else
                    {
                        ApplicationLog.WriteEvent(Properties.Resources.UserIdentifyIsNull, EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Warning);
                    }

                    break;

                    /*
                    // The user has reconnected or logged on to a remote session.
                    case SessionChangeReason.RemoteConnect:
                        ApplicationLog.WriteInformationEvent(string.Format("Remote connect. Session ID: {0}", changeDescription.SessionId), EventID.SessionChangeEvent);
                        break;
                    */

                    /*
                    // The user has disconnected or logged off from a remote session.
                    case SessionChangeReason.RemoteDisconnect:
                        ApplicationLog.WriteInformationEvent(string.Format("Remote disconnect. Session ID: {0}", changeDescription.SessionId), EventID.SessionChangeEvent);
                        break;
                    */
                        
                    /*
                    // The user has locked their session.
                    case SessionChangeReason.SessionLock:
                        ApplicationLog.WriteInformationEvent(string.Format("Session lock. Session ID: {0}", changeDescription.SessionId), EventID.SessionChangeEvent);
                        break;
                    */

                    /*
                    // The user has unlocked their session.
                    case SessionChangeReason.SessionUnlock:
                        ApplicationLog.WriteInformationEvent(string.Format("Session unlock. Session ID: {0}", changeDescription.SessionId), EventID.SessionChangeEvent);
                        break;
                    */

            }

            base.OnSessionChange(changeDescription);
        }
    }
}
