using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VaiVoa.Domain.Interfaces;

namespace VaiVoa.Domain.Notifications
{
    public class Notificator : INotificator
    {
        private List<Notification> _notifications;

        public Notificator()
        {
            _notifications = new List<Notification>();
        }

        public void Handle(Notification notificacao)
        {
            _notifications.Add(notificacao);
        }

        public bool HasNotification()
        {
            return _notifications.Any();
        }

        public List<Notification> ObtainNotifications()
        {
            return _notifications;
        }
    }
}
