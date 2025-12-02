// 
// Copyright © 2010-2025, Sinclair Community College
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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Deployment.WindowsInstaller;

    public class CustomActions
    {
        [CustomAction]
        public static ActionResult RemoveUserList(Session session)
        {
            // TODO: i18n
            session.Log(string.Format("Removing saved user list \"{0}\".", EncryptedSettings.SettingsFilePath));

            int tries = 5;
            TimeSpan sleepTime = new TimeSpan(0, 0, 5);
            while ((tries > 0) && (System.IO.File.Exists(EncryptedSettings.SettingsFilePath)))
            {
                try
                {
                    EncryptedSettings.RemoveAllSettings();
                }
                catch (Exception e)
                {
                    // TODO: i18n
                    session.Log("Error while trying to remove saved user list.");
                    session.Log(e.Message);
                }

                tries--;
                if (tries > 0)
                {
                    // TODO: i18n
                    session.Log(string.Format("{0:N0} tries remaining.", tries));
                    System.Threading.Thread.Sleep(sleepTime);
                }
            }

            // TODO: i18n
            session.Log("Finished removing saved user list.");

            return ActionResult.Success;
        }
    }
}
