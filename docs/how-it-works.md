## How It Works

The user launches the Make Me Admin application. If they are authorized to obtain administrator rights, and they are not already an administrator, the “Grant Me Administrator Rights” button is enabled.

![Make Me Admin UI](images/makemeadminui-238.png)

When the user clicks the Grant button, a service, running in the background, adds the user to the Administrators group. The user application notifies the user, and it minimizes to the notification area.

![Added to the Administrators group](images/make-me-admin-added.png)

At this point, the user can perform the actions that require administrator rights. When prompted for credentials, they simply enter their current Windows credentials.

After a configurable period, the user is removed from the Administrators group by the service. The user is notified of this, and the application closes.

![Removed from the Administrators group](images/make-me-admin-removed.png)

Optionally, a prompt for the user to provide justification for administrator rights can be configured.

![Prompt for Administrator Rights Justification](images/promptforreason.png)

[home](/ "Make Me Admin home page")