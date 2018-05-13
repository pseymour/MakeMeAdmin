// 
// Copyright © 2010-2018, Sinclair Community College
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
    //using System.Collections.Generic;
    using System.Linq;
    //using System.Text;
    //using System.Threading.Tasks;

    // TODO: How do we know we are being passed a valid SID?

    internal class PrincipalList
    {
        private static System.Collections.Generic.Dictionary<string, DateTime> principals = null;

        private PrincipalList()
        {
        }

        static PrincipalList()
        {
            principals = new System.Collections.Generic.Dictionary<string, DateTime>();

            // Retrieve the stored SID list from the settings.
            string[] storedSIDs = Settings.SIDs;
            if (storedSIDs != null)
            {
                for (int i = 0; i < storedSIDs.Length; i++)
                {
                    PrincipalList.AddSID(storedSIDs[i], DateTime.Now.AddMinutes(Settings.AdminRightsTimeout * -1));
                }
            }
            Settings.SIDs = null;
        }

        public static void AddSID(string sid)
        {
            AddSID(sid, DateTime.Now);
        }

        public static void AddSID(string sid, DateTime timeAdded)
        {
            if (!principals.ContainsKey(sid))
            {
                principals.Add(sid, timeAdded);
            }
        }

        public static void RemoveSID(string sid)
        {
            if (principals.ContainsKey(sid))
            {
                principals.Remove(sid);
            }
        }

        public static string[] GetSIDs()
        {
            return principals.Select(p => p.Key).ToArray<string>();
        }

        public static string[] GetExpiredSIDs()
        {
            return principals.Where(p => p.Value <= DateTime.Now.AddMinutes(-1 * Settings.AdminRightsTimeout)).Select(p => p.Key).ToArray<string>();
        }
    }

}
