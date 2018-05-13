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
    /// <summary>
    /// This class allows simple logging of application events.
    /// </summary>
    public class ApplicationLog
    {        
        // TODO: i18n.
        /// <summary>
        /// The name of the Windows Event Log to which events will be written.
        /// </summary>
        private const string EventLogName = "Application";

        // TODO: i18n.
        /// <summary>
        /// The source name to use when writing events to the Event Log.
        /// </summary>
        private const string SourceName = "Make Me Admin";        

        /// <summary>
        /// An EventLog object for interacting with this service's event log.
        /// </summary>
        private static System.Diagnostics.EventLog log;
        
        /// <summary>
        /// Constructor
        /// </summary>
        static ApplicationLog()
        {
            // Get an EventLog object for this service's log.
            log = new System.Diagnostics.EventLog(EventLogName)
            {
                // Specify the source name for this event log.
                Source = SourceName
            };
        }

        /// <summary>
        /// Adds an event source to the event log on the local computer.
        /// </summary>
        public static void CreateSource()
        {
            // If the specified source name does not exist, create it.
            if (!System.Diagnostics.EventLog.SourceExists(SourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(SourceName, EventLogName);
            }
        }

        /// <summary>
        /// Removes, from the local computer, the event source for this service.
        /// </summary>
        public static void RemoveSource()
        {
            // If the specified source name exists, remove it.
            if (System.Diagnostics.EventLog.SourceExists(SourceName))
            {
                System.Diagnostics.EventLog.DeleteEventSource(SourceName);
            }
        }

        /// <summary>
        /// Writes the specified message to the event log as an information event
        /// with the specified event ID.
        /// </summary>
        /// <param name="message">
        /// The message to be written to the log.
        /// </param>
        /// <param name="id">
        /// The event ID to use for the event being written.
        /// </param>
        public static void WriteInformationEvent(string message, EventID id)
        {
            log.WriteEntry(message, System.Diagnostics.EventLogEntryType.Information, (int)id);
        }

        /// <summary>
        /// Writes the specified message to the event log as an error event with the
        /// specified event ID.
        /// </summary>
        /// <param name="message">
        /// The message to be written to the log.
        /// </param>
        /// <param name="id">
        /// The event ID to use for the event being written.
        /// </param>
        public static void WriteErrorEvent(string message, EventID id)
        {
            log.WriteEntry(message, System.Diagnostics.EventLogEntryType.Error, (int)id);
        }

        /// <summary>
        /// Writes the specified message to the event log as a warning event with the
        /// specified event ID.
        /// </summary>
        /// <param name="message">
        /// The message to be written to the log.
        /// </param>
        /// <param name="id">
        /// The event ID to use for the event being written.
        /// </param>
        public static void WriteWarningEvent(string message, EventID id)
        {
            log.WriteEntry(message, System.Diagnostics.EventLogEntryType.Warning, (int)id);
        }
    }
}
