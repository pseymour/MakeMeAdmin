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
    // TODO: Finish commenting this class.
    /// <summary>
    /// This class allows simple logging of application events.
    /// </summary>
    public class Syslog
    {
        private const string AppName = "Make Me Admin";

        /// <summary>
        /// The name or IP address of the syslog server.
        /// </summary>
        private string hostname;

        /// <summary>
        /// The port on which the server communicates.
        /// </summary>
        private int port;

        /// <summary>
        /// The protocol used by the server (UDP or TCP).
        /// </summary>
        private string protocol;

        /// <summary>
        /// The syslog RFC to which the server conforms.
        /// </summary>
        private string syslogRFC;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// The protocol is assumed to be UDP.
        /// </remarks>
        public Syslog(string Hostname) : this(Hostname, "udp")
        {
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public Syslog(string Hostname, string Protocol) : this(Hostname, 0, Protocol, "3164")
        {
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public Syslog(string Hostname, int Port, string Protocol) : this(Hostname, Port, Protocol, "3164")
        {
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public Syslog(string Hostname, int Port, string Protocol, string RFC)
        {
            hostname = Hostname;
            port = Port;
            protocol = Protocol.ToLowerInvariant();
            syslogRFC = RFC;

            // If the port is zero, choose a default port based on the protocol being used.
            if (0 == port)
            {
                switch (protocol)
                {
                    case "tcp":
                        port = 1468;
                        break;
                    case "udp":
                        port = 514;
                        break;
                    default:
                        port = int.MaxValue;
                        break;
                }
            }
        }

        /*
        /// <summary>
        /// Writes the specified message to syslog as an information event.
        /// </summary>
        /// <param name="message">
        /// The message to be written.
        /// </param>
        /// <param name="messageId">
        /// Identifies the type of message being sent.
        /// </param>
        public void WriteInformationEvent(string message, string messageId)
        {
            SendMessage(message, messageId, SyslogNet.Client.Severity.Informational);
        }

        public void WriteWarningEvent(string message, string messageId)
        {
            SendMessage(message, messageId, SyslogNet.Client.Severity.Warning);
        }

        public void WriteErrorEvent(string message, string messageId)
        {
            SendMessage(message, messageId, SyslogNet.Client.Severity.Error);
        }
        */

        public void SendMessage(string message,  string messageId, SyslogNet.Client.Severity severity)
        {

            System.Net.IPHostEntry hostEntry = null;
            try
            {
                hostEntry = System.Net.Dns.GetHostEntry(hostname);
            }
            catch (System.Net.Sockets.SocketException)
            {
                hostEntry = null;
            }

            if (hostEntry == null)
            {
                // TODO: Cache these events.                    
            }
            else
            {
                SyslogNet.Client.SyslogMessage syslogMessage = new SyslogNet.Client.SyslogMessage(
                    System.DateTimeOffset.Now,
                    SyslogNet.Client.Facility.UserLevelMessages,
                    severity,
                    Shared.FullyQualifiedHostName,
                    AppName,
                    null,
                    messageId,
                    message);

                if (string.Compare(protocol, "TCP", true) == 0)
                {
                    if (HostIsAvailableViaTcp(5))
                    {
                        Sender.Send(syslogMessage, Serializer);
                    }
                    else
                    {
                        // TODO: Cache these events.
                    }
                }
                else if (string.Compare(protocol, "UDP", true) == 0)
                {
                    Sender.Send(syslogMessage, Serializer);
                }
            }
        }


        private SyslogNet.Client.Serialization.ISyslogMessageSerializer Serializer
        {
            get
            {
                return syslogRFC == "5424"
                    ? (SyslogNet.Client.Serialization.ISyslogMessageSerializer)new SyslogNet.Client.Serialization.SyslogRfc5424MessageSerializer()
                    : syslogRFC == "3164"
                        ? (SyslogNet.Client.Serialization.ISyslogMessageSerializer)new SyslogNet.Client.Serialization.SyslogRfc3164MessageSerializer()
                        : (SyslogNet.Client.Serialization.ISyslogMessageSerializer)new SyslogNet.Client.Serialization.SyslogLocalMessageSerializer();
            }
        }


        private SyslogNet.Client.Transport.ISyslogMessageSender Sender
        {
            get
            {
                return (string.Compare(protocol, "TCP", true) == 0)
                    ? (SyslogNet.Client.Transport.ISyslogMessageSender)new SyslogNet.Client.Transport.SyslogTcpSender(hostname, port)
                    : (string.Compare(protocol, "UDP", true) == 0)
                        ? (SyslogNet.Client.Transport.ISyslogMessageSender)new SyslogNet.Client.Transport.SyslogUdpSender(hostname, port)
                        : (SyslogNet.Client.Transport.ISyslogMessageSender)new SyslogNet.Client.Transport.SyslogLocalSender();
            }
        }


        private bool HostIsAvailableViaTcp(double timeout)
        {
            bool returnValue = false;
            using (System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient())
            {
                System.IAsyncResult asyncResult = tcp.BeginConnect(hostname, port, null, null);
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
    }
}
