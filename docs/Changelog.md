## What's New In Version 2.3.8

- Added support to extend administrator rights by prompting for renewal. This feature is configurable via Registry and GPO.

- Added an option to prompt for the reason for needing administrator rights. This is configurable to include a free-form response window or a list of canned responses.

- Added support to log elevated processes. This is configurable to never log(default), to only log while an account has been elevated, or always log.

- Added support to log off after admin rights expiration along with a configurable message.

- Added an option to close the application upon administrator rights expiration

- TCP service port is now configurable.

- Added an option to require authentication before privileges are granted.

- Remove the "Exit" button. The UI will now automatically close when either granting or removing administrator rights.


## What's New In Version 2.3

- Work on internationalization and localization is (mostly) done, with the first translation being French. Big thanks to Etienne Croteau, both for translating the English strings into French and for nudging me into internationalization.

  - If you would like to help translate this software into your language of choice, drop me a line via the contact form. I’ll send you the strings to be translated. Or you know, join me on github.

- Added support for syslog. Thanks to the SyslogNet.Client folks for making my life easier.

- The allowed and denied checks now support names, in addition to the previous option of SIDs. Name checks can be done against environment variables, as the strings are expanded before comparison.

- Added an option to add certain users to the Administrators group automatically after logon. These rights do not expire until the user logs off.

- The “Grant Me” rights button is now only enabled if the user is not an administrator at all. Previously, the app only looked to see if the user was a direct member of the Administrators group. It did not take nested group memberships into account. Now, nested group membership is checked before enabling the button.

### Other Code Changes

- This is the first release to be officially stamped with a license of GPLv3, although the license was added to the git repo a while ago.

- Added service configuration options to the installer, so that the service is automatically restarted on failure.

- Removed the default “Admin Rights Timeout” of 10 minutes from the installer. The settings class defines a default timeout of 10 minutes, so this is unnecessary.

- Creating the Event Log source name has been moved from the service installer to the service itself (on startup). It seems that internationalization has somehow caused the creation of the source name at installation time to fail.

- Added installer (`.msi`) files to source control, so that they would appear in the git repository.
