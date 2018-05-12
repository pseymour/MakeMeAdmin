// <copyright file="syslog.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{

    /// <summary>
    /// This class allows simple logging of application events.
    /// </summary>
    public class syslog
    {        
        // TODO: i18n.
        /// <summary>
        /// The name of the Windows Event Log to which events will be written.
        /// </summary>
        private const string EventLogName = "Application";

        // TODO: i18n.
        private const string AppName = "Make Me Admin";

        // TODO: Make this configurable.
        private const string SyslogVersion = "5424";

        // TODO: Make this configurable.
        private const string NetworkProtocol = "udp";

        // TODO: Make this configurable.
        private const string SyslogServerHostname = "patrick-syslog";

        // TODO: Make this configurable.
        private const int SyslogServerPort = 514;


        /// <summary>
        /// Constructor
        /// </summary>
        static syslog()
        {
        }

        /*
        /// <summary>
        /// Adds an event source to the event log on the local computer.
        /// </summary>
        public static void CreateSource()
        {
            // If the specified source name does not exist, create it.
            if (!System.Diagnostics.EventLog.SourceExists(SourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(SourceName, EventLogName);
            }
        }

        /// <summary>
        /// Removes, from the local computer, the event source for this service.
        /// </summary>
        public static void RemoveSource()
        {
            // If the specified source name exists, remove it.
            if (System.Diagnostics.EventLog.SourceExists(SourceName))
            {
                System.Diagnostics.EventLog.DeleteEventSource(SourceName);
            }
        }
        */


        // TODO: Try to use the property from the Shared class, so we don't have duplication of effort.
        private static string FullyQualifiedHostName
        {
            get
            {
                // TODO: i18n for localhost?
                string hostName = System.Environment.MachineName;
                try
                {
                    hostName = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).HostName.ToLowerInvariant();
                }
                catch (System.Net.Sockets.SocketException) { hostName = System.Environment.MachineName; }
                catch (System.ArgumentNullException) { hostName = System.Environment.MachineName; }
                catch (System.ArgumentOutOfRangeException) { hostName = System.Environment.MachineName; }
                catch (System.ArgumentException) { hostName = System.Environment.MachineName; }
                return hostName;
            }
        }

        /// <summary>
        /// Writes the specified message to the event log as an information event
        /// with the specified event ID.
        /// </summary>
        /// <param name="message">
        /// The message to be written to the log.
        /// </param>
        /// <param name="id">
        /// The event ID to use for the event being written.
        /// </param>
        public static void WriteInformationEvent(string message, int processId, string messageId)
        {
            // TODO: Maybe we want to pass the process name here, rather than the numeric ID.

            SyslogNet.Client.SyslogMessage syslogMessage = new SyslogNet.Client.SyslogMessage(
                System.DateTimeOffset.Now,
                SyslogNet.Client.Facility.UserLevelMessages,
                SyslogNet.Client.Severity.Informational,
                FullyQualifiedHostName,
                AppName,
                processId.ToString(),
                messageId,
                message);

            Sender.Send(syslogMessage, Serializer);
        }


        private static SyslogNet.Client.Serialization.ISyslogMessageSerializer Serializer
        {
            get
            {
                return SyslogVersion == "5424"
                    ? (SyslogNet.Client.Serialization.ISyslogMessageSerializer)new SyslogNet.Client.Serialization.SyslogRfc5424MessageSerializer()
                    : SyslogVersion == "3164"
                        ? (SyslogNet.Client.Serialization.ISyslogMessageSerializer)new SyslogNet.Client.Serialization.SyslogRfc3164MessageSerializer()
                        : (SyslogNet.Client.Serialization.ISyslogMessageSerializer)new SyslogNet.Client.Serialization.SyslogLocalMessageSerializer();
            }
        }


        private static SyslogNet.Client.Transport.ISyslogMessageSender Sender
        {
            get
            {
                return NetworkProtocol == "tcp"
                    ? (SyslogNet.Client.Transport.ISyslogMessageSender)new SyslogNet.Client.Transport.SyslogEncryptedTcpSender(SyslogServerHostname, SyslogServerPort)
                    : NetworkProtocol == "udp"
                        ? (SyslogNet.Client.Transport.ISyslogMessageSender)new SyslogNet.Client.Transport.SyslogUdpSender(SyslogServerHostname, SyslogServerPort)
                        : (SyslogNet.Client.Transport.ISyslogMessageSender)new SyslogNet.Client.Transport.SyslogLocalSender();
            }
        }


        private bool HostIsAvailableViaTcp(double timeout)
        {
            bool returnValue = false;
            using (System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient())
            {
                System.IAsyncResult asyncResult = tcp.BeginConnect(SyslogServerHostname, SyslogServerPort, null, null);
                System.Threading.WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
                try
                {
                    if (!asyncResult.AsyncWaitHandle.WaitOne(System.TimeSpan.FromSeconds(timeout), false))
                    {
                        tcp.Close();
                        returnValue = false;
                        //throw new TimeoutException();
                    }

                    if (tcp.Client != null)
                    {
                        if (tcp.Client.Connected)
                        {
                            tcp.EndConnect(asyncResult);
                            returnValue = true;
                        }
                        else
                        {
                            returnValue = false;
                        }
                    }
                }
                finally
                {
                    waitHandle.Close();
                }
            }

            return returnValue;
        }

        /*
        /// <summary>
        /// Writes the specified message to the event log as an error event with the
        /// specified event ID.
        /// </summary>
        /// <param name="message">
        /// The message to be written to the log.
        /// </param>
        /// <param name="id">
        /// The event ID to use for the event being written.
        /// </param>
        public static void WriteErrorEvent(string message, EventID id)
        {
            log.WriteEntry(message, System.Diagnostics.EventLogEntryType.Error, (int)id);
        }

        /// <summary>
        /// Writes the specified message to the event log as a warning event with the
        /// specified event ID.
        /// </summary>
        /// <param name="message">
        /// The message to be written to the log.
        /// </param>
        /// <param name="id">
        /// The event ID to use for the event being written.
        /// </param>
        public static void WriteWarningEvent(string message, EventID id)
        {
            log.WriteEntry(message, System.Diagnostics.EventLogEntryType.Warning, (int)id);
        }
        */
    }
}
