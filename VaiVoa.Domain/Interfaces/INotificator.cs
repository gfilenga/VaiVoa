using System;
using System.Collections.Generic;
using System.Text;
using VaiVoa.Domain.Notifications;

namespace VaiVoa.Domain.Interfaces
{
    public interface INotificator
    {
        bool HasNotification();
        List<Notification> ObtainNotifications();
        void Handle(Notification notificacao);
    }
}
