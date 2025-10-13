# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

https://github.com/pseymour/MakeMeAdmin/commits/master





## [2.4] - release pending... 2025-MM-dd

### Added - for new features.

- Added an optional dialog box to prompt the user to enter their reason for needing administrator rights. (**DOCUMENT THIS**) [2019-09-23]
- Added the ability to log off a user when their administrator rights expire. The message displayed before log off is customizable. [2019-11-15]
- Added logging of elevated processes. [2022-03-15]
- Added settings and code to prompt the user for authentication before rights are granted. [2021-04-05]
- Added a setting to control which port the remote feature uses.
- Added feature to allow renewal of rights after expiration. [2023-08-11]
- Added a setting to control whether the UI closes after the user's rights expire. [2022-09-11]
- Added Danish translation, thanks to **Bjørn Kelsen**.
- Modified the installer to remove the added user XML file upon uninstallation.

### Changed - for changes in existing functionality.

- Significant speed improvement in the code that checks the user against the allowed and denied lists, thanks to [Martin Sheppard](https://github.com/martshep).
- Changed the service's Event Log source name to "Make Me Admin," adding spacing between the words. [2020-03-08]
- Updated group policy templates to reflect change from SIDs only to SIDs or names. (**Jakob Dahl**) [2022-02-17]
- Migrated from .NET Framework version 4.5.2 to version 4.8.


### Deprecated - for soon-to-be removed features.

- 

### Removed - for now removed features.

- Removed the Exit button from the UI. [2019-09-08]
- Turned off automatic logging for service start and stop events. [2020-03-08]

### Fixed - for any bug fixes.

- Fixed an issue that would prevent syslogging from working if the syslog server was not in DNS. [2022-06-15]
- Fixed an issue where the service would check the authorization of the wrong principal, in automatic-add scenarios. (Issue #50) [2022-02-25]
- Fixed typos in French translation. [2023-03-07]
- Added better error handling in the UI app if the service is not listening. [2021-04-16]



### Security - in case of vulnerabilities.

- 



## [2.3-fr] - 2019-02-04

### Changed

- Changed French localization from fr-CA to fr.


## [2.3] - 2019-01-31

### Added

- Added localization for French. Initially, this was mistakenly coded as fr-CA, but it was later changed to fr. [2019-01-28]
- Added elements to the installer project so that the Make Me Admin service would be restarted
upon first/second/third failure. [2018-02-20]
- Added syslog functionality. [2019-01-20]
- Changed the added user file to use encryption, to discourage tampering. [2019-01-20]
- Specified a license of GPLv3. [2018-05-12]
- Added the ability to automatically add certain users when they log on. [2018-05-12]
- The UI now checks to see if the user is an administrator at all, instead of directly in the Administrators group, for the purposes of enabling "Grant Me Rights" button. [2018-05-12]
- The application now supports names of users or groups, in addition to the previous option of SIDs. [2018-05-12]

### Changed

- Changed the path of the encrypted added user list from the CommonApplicationData folder to the user's Application Data folder. In this case, the user is the System account.
- Updated code related to users whose rights do not expire. [2019-01-23]
- Simplified logging code by removing separate Information, Warning and Error logging functions. [2019-01-23]
- Changed the default timeout for admin rights to ten minutes, rather than two. [2018-05-12]
- Slight change to the way membership in the local Administrators group is checked. [2019-01-21]

### Fixed

- Added explanatory text in the ADML file for the syslog server settings. [2019-01-23]


## [2.2.1] - 2018-01-18

### Added

- Added code to use the WCF security context to identify the user being added or removed from the
Administrators group, rather than passing SIDs around as strings.
- Added remote request application, to allow users to gain administrative rights on other devices. The
feature includes its own separate allow/deny lists, a setting to permit remote requests, and a setting
to end remote sessions on a device once that user's rights have expired.

### Changed

-  The list of SIDs that have been added to the local Administrators group is now encrypted. This is really
just to prevent casual browsing of the list. It is not a secure list really, because the user gains
administrator rights and can do whatever they want to the file, including delete it.

### Removed

- Removed Sinclair-specific user manuals.


## [2.1.3] - 2017-07-17

### Added

- Added a setting to override the removal of admin rights by an outside process.
- Added different user manuals for Windows 7, 8 and 10.
- Added logic to the setup project to install the appropriate user manual based on operating system.
- Added logging for the reason that administrator rights were removed (timeout, log off, etc.)

### Deprecated

- The user manuals are deprecated, as they are somewhat specific. They will likely be removed and posted to a company website.


## [2.1.0] - 2016-02-18

### Added

- Added a setting to specify different rights timeouts for different users or groups.
- Added a setting to remove administrative rights when the user logs out.
- Added Group Policy templates (ADMX and ADML files) to control configuration settings.
- Added the ability to specify settings as preferences (mutable) or via policy (immutable).
- Added the user's name, if available, to the logging of additions to and removals from the Administrators group.
- Added logic to the installer to remove the added users list upon uninstallation.
- Added logic to the installer to remove the folder from Program Files upon uninstallation.
- Added logging for the case where a user is removed from Administrators by a process other than Make Me Admin (Group Policy, for example).


## 2015-11-20

### Added

- Modified the setup project to include the version number in the output MSI file name.
- For debug builds, the word "Debug" is now added to the end of the Windows Installer file name.

### Changed

- Renamed the shortcut for the user manual to "Make Me Admin User Manual," instead of just "User Manual."


## [2.0.0] - 2015-03-17

Initial commit to GitHub. Prior to this commit, Make Me Admin was a semi-private project at Sinclair Community College. The code was briefly available via Sinclair's public Bitbucket server, but no installers were available.


<!---
[unreleased]: https://github.com/pseymour/MakeMeAdmin/compare/v1.1.1...HEAD
[1.1.1]: https://github.com/pseymour/MakeMeAdmin/compare/v1.1.0...v1.1.1
[1.1.0]: https://github.com/pseymour/MakeMeAdmin/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/pseymour/MakeMeAdmin/compare/v0.3.0...v1.0.0
[0.3.0]: https://github.com/pseymour/MakeMeAdmin/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/pseymour/MakeMeAdmin/compare/v0.1.0...v0.2.0
[0.1.0]: https://github.com/pseymour/MakeMeAdmin/compare/v0.0.8...v0.1.0
[0.0.8]: https://github.com/pseymour/MakeMeAdmin/compare/v0.0.7...v0.0.8
[0.0.7]: https://github.com/pseymour/MakeMeAdmin/compare/v0.0.6...v0.0.7
[0.0.6]: https://github.com/pseymour/MakeMeAdmin/compare/v0.0.5...v0.0.6
[0.0.5]: https://github.com/pseymour/MakeMeAdmin/compare/v0.0.4...v0.0.5
[0.0.4]: https://github.com/pseymour/MakeMeAdmin/compare/v0.0.3...v0.0.4
[0.0.3]: https://github.com/pseymour/MakeMeAdmin/compare/v0.0.2...v0.0.3
[0.0.2]: https://github.com/pseymour/MakeMeAdmin/compare/v0.0.1...v0.0.2
[0.0.1]: https://github.com/pseymour/MakeMeAdmin/releases/tag/v0.0.1
--->

<!---

## [x.y] - 20yy-mm-dd

### Added - for new features.

- 

### Changed - for changes in existing functionality.

- 

### Deprecated - for soon-to-be removed features.

- 

### Removed - for now removed features.

- 

### Fixed - for any bug fixes.

- 

### Security - in case of vulnerabilities.

- 

--->