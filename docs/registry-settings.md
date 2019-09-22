## Settings

The following settings can be configured in the registry to control the behavior of Make Me Admin. Settings should be stored in the following key:

`HKEY_LOCAL_MACHINE\SOFTWARE\Sinclair Community College\Make Me Admin`

To enforce settings, you should use the Group Policy templates, which are located in the installation directory. However, policy settings can be manually set in the following key:

`HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Sinclair Community College\Make Me Admin`


| Setting Name | Default Value | Format | Explanation |
| ------------ | ------------- | ------ | ----------- |
| Allowed Entities                    | *empty*   | `REG_MULTI_SZ`       | List of SIDs or names for users or groups that are allowed to obtain administrator rights on the local machine. |
| Denied Entities                     | *empty*   | `REG_MULTI_SZ`       | List of SIDs or names for users or groups that are not allowed to obtain administrator rights on the local machine. Denials take precedence over allowed entities. |
| Automatic Add Allowed               | *empty*   | `REG_MULTI_SZ`       | List of SIDs or names for users or groups that are automatically added to the Administrators group upon logon. |
| Automatic Add Denied                | *empty*   | `REG_MULTI_SZ`       | List of SIDs or names for users or groups that are never allowed to be added automatically to the Administrators group upon logon. Denials take precedence over allowed entities. |
| Remote Allowed Entities             | *empty*   | `REG_MULTI_SZ`       | List of SIDs or names for users or groups that are allowed to obtain administrator rights from a remote computer. |
| Remote Denied Entities              | *empty*   | `REG_MULTI_SZ`       | List of SIDs or names for users or groups that are not allowed to obtain administrator rights from a remote computer. Denials take precedence over allowed entities. |
| syslog servers                      | *empty*   | `REG_MULTI_SZ`       | See the [Syslog Configuration](syslog-configuration.md) page for a detailed explanation. |
| Timeout Overrides                   | *empty*   | `REG_SZ`<sup>1</sup> | Specifies different timeout values for users or groups. For example, you can allow your help desk 60 minutes while allowing everyone else 15 minutes. The highest timeout value that applies to a given user wins. |
| Admin Rights Timeout                | 10        | `REG_DWORD`          | The default number of minutes that the user will be added to the Administrators group. |
| Remove Admin Rights On Logout       | false (0) | `REG_DWORD`          | Specifies whether to remove administrator rights if a user logs off of their Windows session. |
| Override Removal By Outside Process | false (0) | `REG_DWORD`          | Specifies whether to re-add a user to the Administrators group, if they are removed by another process, e.g., a Group Policy refresh. |
| Allow Remote Requests               | false (0) | `REG_DWORD`          | Specifies whether to allow requests for administrator rights from remote computers. |
| End Remote Sessions Upon Expiration | true (1)  | `REG_DWORD`          | Specifies whether remote sessions are terminated when the user’s administrator rights expire. |

<sup>1</sup> : Create a separate `REG_SZ` value for each user or group. The name of the registry value will be the SID or name of the user or group, and the value will be the desired timeout, in minutes.

[home](/ "Make Me Admin home page")