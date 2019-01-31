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
    /// <summary>
    /// Contains information about a syslog server.
    /// </summary>
    public class SyslogServerInfo
    {
        /// <summary>
        /// RegEx for validating fully-qualified domain names.
        /// </summary>
        private readonly string domainNamePattern = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";

        /// <summary>
        /// RegEx for validating IP addresses.
        /// </summary>
        private readonly string ipAddressPattern = @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}";

        /// <summary>
        /// RegEx for validating protocol initialisms.
        /// </summary>
        private readonly string protocolPattern = "^(udp|tcp)$";
        
        /// <summary>
        /// RegEx for validating the RFC to which the server conforms.
        /// </summary>
        private readonly string versionPattern = "^(3164|5424)$";


        /// <summary>
        /// The port on which the server is listening.
        /// </summary>
        private int serverPort;

        /// <summary>
        /// The protocol via which the server communicates.
        /// </summary>
        private string protocol;

        /// <summary>
        /// The RFC number to which the syslog server conforms.
        /// </summary>
        private string syslogRFCNumber;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <param name="protocol"></param>
        /// <param name="syslogRFC"></param>
        public SyslogServerInfo(string hostName, int port, string protocol, string syslogRFC)
        {
            Hostname = hostName;
            this.protocol = protocol;
            syslogRFCNumber = syslogRFC;
            Port = port;
        }

        /// <summary>
        /// Gets or sets the host name or IP address of the syslog server.
        /// </summary>
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the protocol via which the server communicates.
        /// </summary>
        public string Protocol
        {
            get
            {
                return protocol;
            }

            set
            {
                // TODO: Probably should do some verification here, don't you think?
                protocol = value.ToLowerInvariant();
            }
        }

        /// <summary>
        /// Gets or sets the RFC number to which the syslog server conforms.
        /// </summary>
        public string RFC
        {
            get
            {
                return syslogRFCNumber;
            }
            set
            {
                syslogRFCNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the port on which the server is listening.
        /// </summary>
        /// <remarks>
        /// If zero (0) is specified for the port, the value will automatically
        /// be changed based on the protocol in use. TCP will use 1468, and
        /// UDP will use 514.
        /// </remarks>
        public int Port
        {
            get
            {
                return serverPort;
            }
            set
            {
                serverPort = value;
                if (0 == serverPort)
                {
                    switch (protocol)
                    {
                        case "tcp":
                            serverPort = 1468;
                            break;
                        case "udp":
                            serverPort = 514;
                            break;
                        default:
                            serverPort = int.MaxValue;
                            break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets a value indicating whether the information for this object
        /// represents valid syslog server information that we can use for
        /// sending events.
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool returnValue = true;

                // Check the hostname.
                returnValue &= (
                                System.Text.RegularExpressions.Regex.IsMatch(Hostname, domainNamePattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                                System.Text.RegularExpressions.Regex.IsMatch(Hostname, ipAddressPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                               );

                // Check the protocol.
                returnValue &= System.Text.RegularExpressions.Regex.IsMatch(protocol, protocolPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                // Check the RFC number.
                returnValue &= System.Text.RegularExpressions.Regex.IsMatch(syslogRFCNumber, versionPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                return returnValue;
            }
        }
    }
}
