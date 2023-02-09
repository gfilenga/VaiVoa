using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Microsoft.Extensions.DependencyInjection;
using VaiVoa.Domain.Interfaces;
using VaiVoa.Domain.Messaging;
using VaiVoa.Domain.Notifications;
using VaiVoa.Domain.Services;
using VaiVoa.Infra.Context;
using VaiVoa.Infra.Messaging;
using VaiVoa.Infra.Repositories;

namespace VaiVoa.Api.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services)
        {
            services.AddScoped<DataContext>();
            // Client
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IClientService, ClientService>();
            // Credit Card
            services.AddScoped<ICreditCardRepository, CreditCardRepository>();
            services.AddScoped<ICreditCardService, CreditCardService>();
            //Notificator
            services.AddScoped<INotificator, Notificator>();


            // Amazon SQS
            services.AddSingleton<ISqsMessenger, SqsMessenger>();
            services.AddSingleton<IAmazonSQS, AmazonSQSClient>();

            return services;
        }
    }
}
