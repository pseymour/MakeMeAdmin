// 
// Copyright © 2010-2019, Sinclair Community College
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

namespace SinclairCC.MakeMeAdmin
{
    using System.Management;

    public delegate void ProcessEventHandler(WMI.Win32.Process proc);

    public class ProcessWatcher : System.Management.ManagementEventWatcher 
    {
        // Process Events
        public event ProcessEventHandler ProcessCreated;
        public event ProcessEventHandler ProcessDeleted;
        public event ProcessEventHandler ProcessModified;

        // WMI WQL process query string
        static readonly string processEventQuery = @"SELECT * FROM __InstanceOperationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'";
        
        public ProcessWatcher()
        {
            Init();
        }

        private void Init()
        {
            Query.QueryLanguage = "WQL";
            Query.QueryString = processEventQuery;
            EventArrived += new EventArrivedEventHandler(watcher_EventArrived);
        }

        private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string eventType = e.NewEvent.ClassPath.ClassName;
            WMI.Win32.Process proc = new WMI.Win32.Process(e.NewEvent["TargetInstance"] as ManagementBaseObject);

            switch (eventType)
            {
                case "__InstanceCreationEvent":
                    if (ProcessCreated != null)
                    {
                        ProcessCreated(proc);
                    }
                    break;

                case "__InstanceDeletionEvent":
                    if (ProcessDeleted != null)
                    {
                        ProcessDeleted(proc);
                    }
                    break;

                case "__InstanceModificationEvent":
                    if (ProcessModified != null)
                    {
                        ProcessModified(proc);
                    }
                    break;
            }
        }
    }
}
