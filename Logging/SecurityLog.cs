// <copyright file="SecurityLog.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;

    public class SecurityLog
    {        
        /// <summary>
        /// The name of the Windows Event Log to which events will be written.
        /// </summary>
        private const string EventLogName = "Application";

        /// <summary>
        /// The source name to use when writing events to the Event Log.
        /// </summary>
        private const string SourceName = "Make Me Admin Security";        

        /// <summary>
        /// An EventLog object for interacting with this service's event log.
        /// </summary>
        private System.Diagnostics.EventLog log;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>Creates an event source if one does not already exist on the machine.</remarks>
        public SecurityLog()
        {            
            // Get an EventLog object for this service's log.
            this.log = new System.Diagnostics.EventLog(EventLogName);

            // Specify the source name for this event log.
            this.log.Source = SourceName;
        }

        static public void CreateSource()
        {
            // If the specified source name does not exist, create it.
            if (!System.Diagnostics.EventLog.SourceExists(SourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(SourceName, EventLogName);
            }
        }

        /// <summary>
        /// Removes from the local computer the event log and source for this service.
        /// </summary>
        static public void RemoveSource()
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
        /// <param name="Message">The message to be written to the log.</param>
        /// <param name="ID">The event ID to use for the event being written.</param>
        virtual public void WriteInformationEvent(string Message, EventID ID)
        {
            log.WriteEntry(Message, System.Diagnostics.EventLogEntryType.Information, (int)ID);
        }

        /// <summary>
        /// Writes the specified message to the event log as an error event with the
        /// specified event ID.
        /// </summary>
        /// <param name="Message">The message to be written to the log.</param>
        /// <param name="ID">The event ID to use for the event being written.</param>
        virtual public void WriteErrorEvent(string Message, EventID ID)
        {
            log.WriteEntry(Message, System.Diagnostics.EventLogEntryType.Error, (int)ID);
        }

        /// <summary>
        /// Writes the specified message to the event log as a warning event with the
        /// specified event ID.
        /// </summary>
        /// <param name="Message">The message to be written to the log.</param>
        /// <param name="ID">The event ID to use for the event being written.</param>
        virtual public void WriteWarningEvent(string Message, EventID ID)
        {
            log.WriteEntry(Message, System.Diagnostics.EventLogEntryType.Warning, (int)ID);
        }

        virtual public void WriteSuccessAuditEvent(string Message, EventID ID)
        {
            log.WriteEntry(Message, System.Diagnostics.EventLogEntryType.SuccessAudit, (int)ID);
        }

        virtual public void WriteFailureAuditEvent(string Message, EventID ID)
        {
            log.WriteEntry(Message, System.Diagnostics.EventLogEntryType.FailureAudit, (int)ID);
        }
    }
}
