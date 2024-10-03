## Settings

The following settings can be configured in the registry to control the behavior of Make Me Admin. Settings should be stored in the following key:

`HKEY_LOCAL_MACHINE\SOFTWARE\Sinclair Community College\Make Me Admin`

To enforce settings, you should use the Group Policy templates, which are located in the installation directory. However, policy settings can be manually set in the following key:

`HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Sinclair Community College\Make Me Admin`


| Setting Name | Default Value | Format | Explanation |
| ------------ | ------------- | ------ | ----------- |
| Allowed Entities                      | *empty*   | `REG_MULTI_SZ`       | List of SIDs or names<sup>2</sup> for users or groups that are allowed to obtain administrator rights on the local machine. |
| Denied Entities                       | *empty*   | `REG_MULTI_SZ`       | List of SIDs or names<sup>2</sup> for users or groups that are not allowed to obtain administrator rights on the local machine. Denials take precedence over allowed entities. |
| Automatic Add Allowed                 | *empty*   | `REG_MULTI_SZ`       | List of SIDs or names<sup>2</sup> for users or groups that are automatically added to the Administrators group upon logon. |
| Automatic Add Denied                  | *empty*   | `REG_MULTI_SZ`       | List of SIDs or names<sup>2</sup> for users or groups that are never allowed to be added automatically to the Administrators group upon logon. Denials take precedence over allowed entities. |
| Remote Allowed Entities               | *empty*   | `REG_MULTI_SZ`       | List of SIDs or names<sup>2</sup> for users or groups that are allowed to obtain administrator rights from a remote computer. |
| Remote Denied Entities                | *empty*   | `REG_MULTI_SZ`       | List of SIDs or names<sup>2</sup>  for users or groups that are not allowed to obtain administrator rights from a remote computer. Denials take precedence over allowed entities. |
| syslog servers                        | *empty*   | `REG_MULTI_SZ`       | See the [Syslog Configuration](syslog-configuration.md) page for a detailed explanation. |
| Timeout Overrides                     | *empty*   | `REG_SZ`<sup>1</sup> | Specifies different timeout values for users or groups. For example, you can allow your help desk 60 minutes while allowing everyone else 15 minutes. The highest timeout value that applies to a given user wins. |
| Admin Rights Timeout                  | 10        | `REG_DWORD`          | The default number of minutes that the user will be added to the Administrators group. |
| Remove Admin Rights On Logout         | false (0) | `REG_DWORD`          | Specifies whether to remove administrator rights if a user logs off of their Windows session. |
| Override Removal By Outside Process   | false (0) | `REG_DWORD`          | Specifies whether to re-add a user to the Administrators group, if they are removed by another process, e.g., a Group Policy refresh. |
| Allow Remote Requests                 | false (0) | `REG_DWORD`          | Specifies whether to allow requests for administrator rights from remote computers. |
| End Remote Sessions Upon Expiration   | true (1)  | `REG_DWORD`          | Specifies whether remote sessions are terminated when the user’s administrator rights expire. |
| Renewals Allowed                      | 0         | `REG_DWORD`          | Specifies the number of times that a user can renew their administrative rights. |
| Log Off After Expiration              | false (0) | `REG_DWORD`          | Specifies the number of seconds after which a user is logged off, once their administrator privileges expire. A value of zero (0) prevents the logoff. |
| Log Off Message                       | *empty*   | `REG_MULTI_SZ`       | Specifies a message to be displayed before a user is logged off, once their administrator privileges expire. |
| TCP Service Port                      | 808       | `REG_DWORD`          | Specifies the TCP port to be used for remote administrative rights requests. |
| Require Authentication For Privileges | false (0) | `REG_DWORD`          | Specifies whether the user must authenticate before being granted administrative privileges. |
| Close Application Upon Expiration     | true (1)  | `REG_DWORD`          | Specifies whether the application is closed when the user's rights expire. |
| Prompt For Reason                     | false (0) | `REG_DWORD`          | Specifies whether to prompt the user for the reason that they require administrator privileges. 0 = None, 1 = Optional, 2 = Required |
| Allow Free-Form Reason                | true (1)  | `REG_DWORD`          | Allow the user to enter a free-form text string in the administrator rights reason dialog box. |
| Canned Reasons                        | *empty*   | `REG_MULTI_SZ`       | Pre-populated reasons to be shown on the administrator rights reason dialog box. |
| Maximum Reason Length                 | 333       | `REG_DWORD`          | Specifies the maximum number of characters that a user may enter in the reason dialog box. |
| Log Elevated Processes                | false (0) | `REG_DWORD`          | Determines when Make Me Admin logs processes that are run with elevated privileges. 0 = Never, 1 = Only While Administrator, 2 = Always |

<sup>1</sup> : Create a separate `REG_SZ` value for each user or group. The name of the registry value will be the SID or name of the user or group, and the value will be the desired timeout, in minutes.

<sup>2</sup> : Names of users or groups should be in the format `DOMAIN\Name`. User principal names (e.g., user@<span></span>domain.com) will not work.  
**NOTE:** If you are using local groups, `DOMAIN` should be either a single dot (`.`), the name of the computer (not recommended), or `%COMPUTERNAME%`.

[home](/ "Make Me Admin home page")