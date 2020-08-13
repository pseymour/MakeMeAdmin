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
//


namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Diagnostics.Tracing;

    [EventSource(Name = "MakeMeAdmin")]
    public sealed class LoggingProvider : EventSource
    {
        /*
        // Rich payloads only work when the self-describing format 
        private LoggingProvider() : base(EventSourceSettings.EtwSelfDescribingEventFormat) { }
        */

        public static LoggingProvider Log = new LoggingProvider();

        /*
        [Event(1, Message = "User added to Administrators group. \"{0}\"", Level = EventLevel.Informational)]
        public void UserAddedToAdmins(string userSid) { WriteEvent(1, userSid); }

        [Event(2, Message = "User removed from Administrators group. \"{0}\"", Level = EventLevel.Informational)]
        public void UserRemovedFromAdmins(string userSid) { WriteEvent(2, userSid); }

        [Event(69, Message = "This is event 69. \"{0}\"", Level = EventLevel.Informational)]
        public void ServiceStart(string message) { WriteEvent(69, message); }
        */

        [NonEvent]
        public void ElevatedProcessDetected(TokenElevationType elevationType, ProcessInformation elevatedProcess /*, ProcessInformation parentProcess */)
        {
            string elevationTypeString = "Unknown";
            switch (elevationType)
            {
                case TokenElevationType.Full:
                    elevationTypeString = "Full";
                    break;
                case TokenElevationType.Limited:
                    elevationTypeString = "Limited";
                    break;
                case TokenElevationType.Default:
                    elevationTypeString = "Default";
                    break;
            }

            /*
            Console.WriteLine("\t\tCommandLine: \"{0}\"", parentProcess.CommandLine);
            Console.WriteLine("\t\tImageFileName: \"{0}\"", parentProcess.ImageFileName);
            Console.WriteLine("\t\tProcessID: {0}", parentProcess.ProcessID);
            Console.WriteLine("\t\tProcessName: \"{0}\"", parentProcess.ProcessName);
            Console.WriteLine("\t\tSessionID: {0}", parentProcess.SessionID);
            Console.WriteLine("\t\tTimeStamp: {0}", parentProcess.TimeStamp);
            */

            ElevatedProcessDetected(elevatedProcess.ImageFileName, elevatedProcess.TimeStamp, elevatedProcess.SessionID, elevationTypeString, elevatedProcess.CommandLine, elevatedProcess.ProcessID);
        }


        /*
        [Event(
            101,
            Message = "{0}",
            Channel = EventChannel.Operational,
            Level = EventLevel.Informational
            )]
        private void ElevatedProcessDetected(string imageFileName, DateTime creationTime, int sessionId, string elevationType, string commandLine, int processId)
        {
            if (IsEnabled())
            {
                System.Text.StringBuilder message = new System.Text.StringBuilder();
                message.Append("Process ");
                message.Append(imageFileName);
                message.Append(" created at ");
                message.Append(creationTime);
                message.Append(" in session ");
                message.Append(sessionId);
                message.Append(" with an elevation type of ");
                message.Append(elevationType);
                message.Append(".");
                message.Append(System.Environment.NewLine);
                message.Append("command line: \"");
                message.Append(commandLine);
                message.Append("\"");
                message.Append(System.Environment.NewLine);
                message.Append("process ID: \"");
                message.Append(processId);
                message.Append("\"");
                WriteEvent(101, message.ToString());
            }
        }
        */

        
        [Event(
            101,
            Message = "Process {0} created at {1} in session {2} with an elevation type of {3}." + "\r\ncommand line: \"{4}\"" + "\r\nprocess ID: \"{5}\"",
            Channel = EventChannel.Operational,
            Level = EventLevel.Informational
            )]
        private void ElevatedProcessDetected(string imageFileName, DateTime creationTime, int sessionId, string elevationType, string commandLine, int processId)
        {
            if (IsEnabled())
            {
                WriteEvent(101, imageFileName, creationTime, sessionId, elevationType, commandLine, processId);
            }
        }
        
    }
}


/*

    enum MyColor { Red, Yellow, Blue };

    [EventSource(Name = "MyCompany")]
    class LoggingProvider : EventSource
    {
        public class Keywords
        {
            public const EventKeywords Page = (EventKeywords)1;
            public const EventKeywords DataBase = (EventKeywords)2;
            public const EventKeywords Diagnostic = (EventKeywords)4;
            public const EventKeywords Perf = (EventKeywords)8;
        }

        public class Tasks
        {
            public const EventTask Page = (EventTask)1;
            public const EventTask DBQuery = (EventTask)2;
        }

        [Event(1, Message = "Application Failure: {0}", Level = EventLevel.Error, Keywords = Keywords.Diagnostic)]
        public void Failure(string message) { WriteEvent(1, message); }

        [Event(2, Message = "Starting up.", Keywords = Keywords.Perf, Level = EventLevel.Informational)]
        public void Startup() { WriteEvent(2); }

        [Event(3, Message = "loading page {1} activityID={0}", Opcode = EventOpcode.Start, 
            Task = Tasks.Page, Keywords = Keywords.Page, Level = EventLevel.Informational)]
        public void PageStart(int ID, string url) { if (IsEnabled()) WriteEvent(3, ID, url); }

        [Event(4, Opcode = EventOpcode.Stop, Task = Tasks.Page, Keywords = Keywords.Page, Level = EventLevel.Informational)]
        public void PageStop(int ID) { if (IsEnabled()) WriteEvent(4, ID); }

        [Event(5, Opcode = EventOpcode.Start, Task = Tasks.DBQuery, Keywords = Keywords.DataBase, Level = EventLevel.Informational)]
        public void DBQueryStart(string sqlQuery) { WriteEvent(5, sqlQuery); }

        [Event(6, Opcode = EventOpcode.Stop, Task = Tasks.DBQuery, Keywords = Keywords.DataBase, Level = EventLevel.Informational)]
        public void DBQueryStop() { WriteEvent(6); }

        [Event(7, Level = EventLevel.Verbose, Keywords = Keywords.DataBase)]
        public void Mark(int ID) { if (IsEnabled()) WriteEvent(7, ID); }

        [Event(8)]
        public void LogColor(MyColor color) { WriteEvent(8, (int) color); }

        public static LoggingProvider Log = new LoggingProvider();
    }
*/
