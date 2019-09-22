## syslog Configuration

Beginning with version 2.3, Make Me Admin allows logging to one or more syslog servers. As with other settings, this configuration is handled via the registry.

The name of the registry value is "**syslog servers**."

Syslog server entries are in the form

> `server_address:port:protocol:RFC`

where

* *server_address* is the hostname or IP address of the syslog server
* *port* is the port on which the syslog server is listening
* *protocol* is the protocol via which the syslog server communicates (TCP or UDP)
* *RFC* is the RFC number to which the syslog server conforms ([3164](https://tools.ietf.org/html/rfc3164) or [5424](https://tools.ietf.org/html/rfc5424))


### Example Valid Syslog Server Configuration Strings

* `syslogserver`
  * Logs to a server named syslogserver, using port 514, the UDP protocol and sending messages conforming to RFC3164.
* `syslogserver:udp`
  * Logs to a server named syslogserver, using port 514, the UDP protocol and sending messages conforming to RFC3164.
* `syslogserver:tcp`
  * Logs to a server named syslogserver, using port 1468, the TCP protocol and sending messages conforming to RFC3164.
* `syslogserver:514:udp`
  * Logs to a server named syslogserver, using port 514, the UDP protocol and sending messages conforming to RFC3164.
* `syslogserver.domain.edu:514:udp`
  * Logs to a server named syslogserver, using port 514, the UDP protocol and sending messages conforming to RFC3164.
* `syslogserver:1468:tcp`
  *Logs to a server named syslogserver, using port 1468, the TCP protocol and sending messages conforming to RFC3164.
* `syslogserver:1468:tcp:5424`
  * Logs to a server named syslogserver, using port 1468, the TCP protocol and sending messages conforming to RFC5424.
