// <copyright file="ApplicationLog.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    /// <summary>
    /// This class allows simple logging of application events.
    /// </summary>
    public class ApplicationLog
    {        
        /// <summary>
        /// The name of the Windows Event Log to which events will be written.
        /// </summary>
        private const string EventLogName = "Application";

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
            log = new System.Diagnostics.EventLog(EventLogName);

            // Specify the source name for this event log.
            log.Source = SourceName;
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
