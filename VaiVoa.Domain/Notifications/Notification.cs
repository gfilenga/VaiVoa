using System;
using System.Collections.Generic;
using System.Text;

namespace VaiVoa.Domain.Notifications
{
    public class Notification
    {
        public Notification(string mensagem)
        {
            Mensagem = mensagem;
        }

        public string Mensagem { get; }
    }
}
