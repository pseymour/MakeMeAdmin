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
    // TODO: This entire class needs to be commented.
    public class SyslogServerInfo
    {
        private readonly string domainNamePattern = @"^(([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]*[a-zA-Z0-9])\.)*([A-Za-z0-9]|[A-Za-z0-9][A-Za-z0-9\-]*[A-Za-z0-9])$";
        private readonly string ipAddressPattern = @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}";
        private readonly string protocolPattern = "^(udp|tcp)$";
        private readonly string versionPattern = "^(3164|5424)$";

        private int serverPort;
        private string protocol;
        private string syslogRFCNumber;

        public SyslogServerInfo(string hostName, int port, string protocol, string syslogRFC)
        {
            Hostname = hostName;
            this.protocol = protocol;
            syslogRFCNumber = syslogRFC;
            Port = port;
        }

        public string Hostname { get; set; }

        public string Protocol
        {
            get
            {
                return protocol;
            }

            set
            {
                protocol = value.ToLowerInvariant();
            }
        }

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
        
        public bool IsValid
        {
            get
            {
                bool returnValue = true;

                returnValue &= (
                                System.Text.RegularExpressions.Regex.IsMatch(Hostname, domainNamePattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase) ||
                                System.Text.RegularExpressions.Regex.IsMatch(Hostname, ipAddressPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                               );

                returnValue &= System.Text.RegularExpressions.Regex.IsMatch(protocol, protocolPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                returnValue &= System.Text.RegularExpressions.Regex.IsMatch(syslogRFCNumber, versionPattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                return returnValue;
            }
        }
    }
}
