# ServerMonitor
Server Monitor is a secure remote monitoring system for Windows-based servers and workstations. At this moment the project is at early development stage. Server Monitor is easy to install and configure. It is also secure: no back-doors and no remote access.

Monitoring features:
- statistics of memory and CPU usage for several hours;
- tracking the status of selected services;
- statistics for errors in Windows Log;
- disk space monitoring;
- tracking the most memory/CPU consuming processes.

The project includes at client side:
- Server Monitor Agent - windows monitoring service;
- Configuration utility.

The server side:
- WCF-based web-service receiving data from Server Monitor Agent;
- ASP.NET MVC-based web-application as remote monitoring interface.

Also the project includes:
- common libraries;
- MySQL database.

An acceptable level of safety implemented:
- data transfer from Server Monitor Agent to WCF service using SSL transport security with client credentials;
- exclusive use of HTTPS protocol in web-application
- ASP.NET Identity system reduces the risk of leakage of credentials to a minimum (based on MySql.AspNet.Identity)
- two-factor authentication with Google Authenticator or e-mail code;
- confirmation of new accounts via e-mail (based on SendGrid);
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
