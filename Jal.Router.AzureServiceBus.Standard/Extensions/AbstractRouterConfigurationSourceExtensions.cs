using System;
using Jal.Router.AzureServiceBus.Standard.Model;
using Jal.Router.Impl;
using Jal.Router.Interface;
using Newtonsoft.Json;
// ReSharper disable ConvertToLocalFunction

namespace Jal.Router.AzureServiceBus.Standard.Extensions
{
    public static class AbstractRouterConfigurationSourceExtensions
    {
        public static void RegisterQueue<TExtractorConectionString>(this AbstractRouterConfigurationSource configuration, string queue, Func<IValueSettingFinder, ServiceBusConfiguration> connectionstringextractor)
            where TExtractorConectionString : IValueSettingFinder
        {

            Func<IValueSettingFinder, string> extractor = finder =>
            {
                var servicebusconfiguration = connectionstringextractor(finder);

                return JsonConvert.SerializeObject(servicebusconfiguration);
            };

            configuration.RegisterPointToPointChannel<TExtractorConectionString>(queue, extractor);
        }

        public static void RegisterQueue(this AbstractRouterConfigurationSource configuration, string queue, ServiceBusConfiguration servicebusconfiguration)
        {
            configuration.RegisterPointToPointChannel(queue, JsonConvert.SerializeObject(servicebusconfiguration));
        }

        public static void RegisterTopic<TExtractorConectionString>(this AbstractRouterConfigurationSource configuration, string topic,
            Func<IValueSettingFinder, ServiceBusConfiguration> connectionstringextractor)
            where TExtractorConectionString : IValueSettingFinder
        {
            Func<IValueSettingFinder, string> extractor = finder =>
            {
                var servicebusconfiguration = connectionstringextractor(finder);

                return JsonConvert.SerializeObject(servicebusconfiguration);
            };

            configuration.RegisterPublishSubscriberChannel<TExtractorConectionString>(topic, extractor);
        }

        public static void RegisterTopic(this AbstractRouterConfigurationSource configuration, string topic, ServiceBusConfiguration servicebusconfiguration)
        {
            configuration.RegisterPublishSubscriberChannel(topic, JsonConvert.SerializeObject(servicebusconfiguration));
        }

        public static void RegisterSubscriptionToTopic<TExtractorConectionString>(this AbstractRouterConfigurationSource configuration, string subscription, string topic,
            Func<IValueSettingFinder, ServiceBusConfiguration> connectionstringextractor, bool all = false)
            where TExtractorConectionString : IValueSettingFinder
        {
            Func<IValueSettingFinder, string> extractor = finder =>
            {
                var servicebusconfiguration = connectionstringextractor(finder);

                return JsonConvert.SerializeObject(servicebusconfiguration);
            };

            configuration.RegisterSubscriptionToPublishSubscriberChannel<TExtractorConectionString>(subscription, topic, extractor, all);
        }

        public static void RegisterSubscriptionToTopic(this AbstractRouterConfigurationSource configuration, string subscription, string topic, ServiceBusConfiguration servicebusconfiguration, bool all = false)
        {
            configuration.RegisterSubscriptionToPublishSubscriberChannel(subscription, topic, JsonConvert.SerializeObject(servicebusconfiguration), all);
        }
    }
}