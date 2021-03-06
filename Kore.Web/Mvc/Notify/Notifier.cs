﻿using System.Collections.Generic;
using Castle.Core.Logging;
using Kore.Logging;

namespace Kore.Web.Mvc.Notify
{
    /// <summary>
    /// Notification manager for UI notifications
    /// </summary>
    /// <remarks>
    /// Where such notifications are displayed depends on the theme used. Default themes contain a
    /// Messages zone for this.
    /// </remarks>
    public interface INotifier
    {
        /// <summary>
        /// Adds a new UI notification
        /// </summary>
        /// <param name="type">
        /// The type of the notification (notifications with different types can be displayed differently)</param>
        /// <param name="message">A localized message to display</param>
        void Add(NotifyType type, string message);

        /// <summary>
        /// Get all notifications added
        /// </summary>
        IEnumerable<NotifyEntry> List();
    }

    public class Notifier : INotifier
    {
        private readonly ICollection<NotifyEntry> entries;

        public Notifier()
        {
            Logger = LoggingUtilities.Resolve();
            entries = new List<NotifyEntry>();
        }

        public ILogger Logger { get; set; }

        public void Add(NotifyType type, string message)
        {
            //Logger.InfoFormat("Notification {0} message: {1}", type, message);
            entries.Add(new NotifyEntry { Type = type, Message = message });
        }

        public IEnumerable<NotifyEntry> List()
        {
            return entries;
        }
    }
}