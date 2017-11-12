﻿using System;
using Jal.Router.AzureServiceBus.Extensions;
using Jal.Router.AzureServiceBus.Impl;
using Jal.Router.AzureStorage.Impl;
using Jal.Router.Impl;
using Jal.Router.Impl.Inbound;
using Jal.Router.Model;
using Jal.Router.Tests.Model;
using Microsoft.ServiceBus.Messaging;

namespace Jal.Router.Tests.Impl
{
    public class RouterConfigurationSource : AbstractRouterConfigurationSource
    {
        public RouterConfigurationSource()
        {
            RegisterSaga<Data>("saga", start =>
            {
                start.RegisterRoute<IMessageHandler<Message>>().ForMessage<Message>().ToBeHandledBy<MessageHandler>(x =>
                {
                    x.With((request, handler, data) => handler.Handle(request, data));
                });
            }, @continue =>
            {
                @continue.RegisterRoute<IMessageHandler<Message1>>().ForMessage<Message1>().ToBeHandledBy<Message1Handler>(x =>
                {
                    x.With(((request, handler, data) => handler.Handle(request, data)));
                });
            }
            );

            RegisterRoute<IMessageHandler<Message>>().ForMessage<Message>().ToBeHandledBy<MessageHandler>(x =>
            {
                x.With(((request, handler) => handler.Handle(request, null)));
            }).When(((message, context) => context.Origin.Key == "A")); ;


            RegisterRoute<IMessageHandler<Message>>().ForMessage<Message>().ToBeHandledBy<OtherMessageHandler>(x =>
            {
                x.With(((request, handler) => handler.Handle(request, null)));
            })
                .OnExceptionRetryFailedMessageTo<ApplicationException>("retry")
                .Using<AppSettingValueSettingFinder>(y => new LinearRetryPolicy(10, 5))
                .OnErrorSendFailedMessageTo("error").ForwardMessageTo("");
                //.UsingStorage<AzureTableStorage>()
                //.UsingMessageChannel<AzureServiceBusQueue, AzureServiceBusTopic, AzureServiceBusManager>();

            RegisterOrigin("From", "2CE8F3B2-6542-4D5C-8B08-E7E64EF57D22");

            RegisterEndPoint("retry")
                .ForMessage<Message>()
                .To<AppSettingValueSettingFinder, AppSettingValueSettingFinder>(x => "dadsa", x => "asdaxxx");

            RegisterEndPoint("error")
                .ForMessage<Message>()
                .To<AppSettingValueSettingFinder>(x => "11111", "topath");

            RegisterEndPoint<EndPointSettingFinder, Message>();

            RegisterSubscriptionToPublishSubscriberChannel<AppSettingValueSettingFinder>("subscripcion12", "testtopic", x => "Endpoint=sb://raulqueuetests.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=8WpD2e6cWAW3Qj4AECuzdKCySM4M+ZAIW2VGRHvvXlo=");

            RegisterPointToPointChannel<AppSettingValueSettingFinder>("errorqueue12", x => "Endpoint=sb://raulqueuetests.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=8WpD2e6cWAW3Qj4AECuzdKCySM4M+ZAIW2VGRHvvXlo=");

            this.RegisterQueue<AppSettingValueSettingFinder>("errorqueue12", x => "Endpoint=sb://raulqueuetests.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=8WpD2e6cWAW3Qj4AECuzdKCySM4M+ZAIW2VGRHvvXlo=");

            RegisterPublishSubscriberChannel<AppSettingValueSettingFinder>("errortopic12", x => "Endpoint=sb://raulqueuetests.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=8WpD2e6cWAW3Qj4AECuzdKCySM4M+ZAIW2VGRHvvXlo=");

            //RegisterSubscription();
            //RegisterFlow<Data>(s => s.Id).StartedByMessage<Request>(h => h.Name).UsingRoute("Tag1").FollowingBy(y =>
            //{
            //    y.Message<Request>(r => r.Id).UsingRoute("");
            //    y.Message<Request>(r => r.Id).UsingRoute("");
            //});
        }
    }
}
