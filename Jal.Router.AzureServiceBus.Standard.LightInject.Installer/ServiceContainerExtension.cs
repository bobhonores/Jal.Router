﻿using System;
using Jal.Router.AzureServiceBus.Standard.Impl;
using Jal.Router.Interface;
using Jal.Router.Interface.Inbound;
using Jal.Router.Interface.Management;
using LightInject;

namespace Jal.Router.AzureServiceBus.Standard.LightInject.Installer
{
    public static class ServiceContainerExtension
    {
        public static void RegisterAzureServiceBusRouter(this IServiceContainer container, int maxconcurrentcalls = 0, TimeSpan? autorenewtimeout = null)
        {
            container.Register<IMessageAdapter, MessageAdapter>(typeof(MessageAdapter).FullName, new PerContainerLifetime());

            container.Register<IChannelManager, AzureServiceBusManager>(typeof(AzureServiceBusManager).FullName, new PerContainerLifetime());

            container.Register<IMessageSerializer, JsonMessageSerializer>(typeof(JsonMessageSerializer).FullName, new PerContainerLifetime());

            container.Register<IPointToPointChannel>(x => new AzureServiceBusQueue(x.GetInstance<IComponentFactory>(), x.GetInstance<IConfiguration>(), x.GetInstance<IChannelPathBuilder>(), maxconcurrentcalls, autorenewtimeout), typeof(AzureServiceBusQueue).FullName, new PerContainerLifetime());

            container.Register<IRequestReplyChannel>(x => new AzureServiceBusSession(x.GetInstance<IComponentFactory>(), x.GetInstance<IConfiguration>(), x.GetInstance<IChannelPathBuilder>()), typeof(AzureServiceBusSession).FullName, new PerContainerLifetime());

            container.Register<IPublishSubscribeChannel>(x => new AzureServiceBusTopic(x.GetInstance<IComponentFactory>(), x.GetInstance<IConfiguration>(), x.GetInstance<IChannelPathBuilder>(), maxconcurrentcalls, autorenewtimeout), typeof(AzureServiceBusTopic).FullName, new PerContainerLifetime());
        }
    }
}
