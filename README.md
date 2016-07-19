# Server Monitor
([servermonitor.online](https://servermonitor.online))
Server Monitor is a secure remote monitoring system for Windows-based servers and workstations. At this moment the project is at early development stage. Server Monitor is easy to install and configure. It is also secure: no back-doors and no remote access.

Monitoring features:
- statistics of memory and CPU usage for several hours;
- tracking the status of selected services;
- statistics for errors in Windows Log;
- disk space monitoring;
- tracking the most memory/CPU consuming processes.

The project includes at client side:
- [Server Monitor Agent](SrvMon-Agent) - windows monitoring service;
- [Configuration utility](SrvMon-Agent-Configurator).

The server side:
- [WCF-based web-service](SrvMon-WebService) receiving data from Server Monitor Agent;
- [ASP.NET MVC-based web-application](SrvMon-WebApp) as remote monitoring interface ([the website](https://servermonitor.online) is in Russian only at the moment).

Also the project includes:
- [common libraries](SrvMon-Common);
- MySQL database.

An acceptable level of safety implemented:
- data transfer from Server Monitor Agent to WCF service using SSL transport security with client credentials;
- exclusive use of HTTPS protocol in web-application
- ASP.NET Identity system reduces the risk of leakage of credentials to a minimum (based on [MySql.AspNet.Identity](https://github.com/radenkozec/MySqlIdentity))
- two-factor authentication with Google Authenticator or e-mail code;
- confirmation of new accounts via e-mail (based on [SendGrid](https://github.com/sendgrid/sendgrid-csharp));
- authentication using third-party identity providers (only Google at the moment);
- account lock-out when brute-force password hacking.

Further development:
- extension of monitoring features range;
- implementation of user notification features;
- web-application internationalization (only Russian at the moment; configuration utility is globalized);
- web-application design development (basic at the moment).

Client software system requirements:
- Windows Sever 2008 (Windows 7) or later;
- .NET Framework 4.5.2 or later.

[Download Server Monitor Agent](https://github.com/laberko/ServerMonitor/releases/latest).
