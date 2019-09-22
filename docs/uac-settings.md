## User Account Control (UAC) Settings

Because Make Me Admin allows users to perform administrative tasks, even though they have logged on to Windows without administrator-level rights, the software relies on the Windows User Account Control (UAC) feature to be enabled, at least partially.

UAC must be at least enabled, even if most of its individual features are turned off. To enable UAC, the `EnableLUA` value must be set to one (1).

Also, the `ConsentPromptBehaviorUser` value must be set to either

* one (1) : prompt for credentials
* three (3) : prompt for credentials on the secure desktop [This is the default value.]

If `ConsentPromptBehaviorUser` is set to zero (0), the user will receive an access denied message when trying to perform any tasks which require elevation.

Both of the aforementioned registry values can be found in `HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System`.

### Configuring with Group Policy

If you are using Group Policy to configure these settings, they can be found in **Computer Configuration | Policies | Windows Settings | Security Settings | Local Policies | Security Options**.

`EnableLUA` corresponds to the setting "User Account Control: Run all administrators in Admin Approval Mode." Set this policy to Enabled.

`ConsentPromptBehaviorUser` corresponds to the setting "User Account Control: Behavior of the elevation prompt for standard users." Set this policy to one of the two "Prompt for credentials" values.

[home](/ "Make Me Admin home page")