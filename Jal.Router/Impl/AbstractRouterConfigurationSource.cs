﻿using System;
using System.Collections.Generic;
using Jal.Router.Fluent.Impl;
using Jal.Router.Fluent.Interface;
using Jal.Router.Interface;
using Jal.Router.Interface.Outbound;
using Jal.Router.Model;
using Jal.Router.Model.Management;

namespace Jal.Router.Impl
{
    public abstract class AbstractRouterConfigurationSource : IRouterConfigurationSource
    {
        private readonly List<Route> _routes = new List<Route>();

        private readonly List<EndPoint> _enpoints = new List<EndPoint>();

        private readonly List<SubscriptionToPublishSubscribeChannel> _subscriptions = new List<SubscriptionToPublishSubscribeChannel>();

        private readonly List<PublishSubscribeChannel> _publishsubscriberchannels = new List<PublishSubscribeChannel>();

        private readonly List<PointToPointChannel> _pointtopointchannels = new List<PointToPointChannel>();

        private readonly List<Saga> _sagas = new List<Saga>();

        private readonly Origin _origin = new Origin();

        public Route[] GetRoutes()
        {
            return _routes.ToArray();
        }

        public Saga[] GetSagas()
        {
            return _sagas.ToArray();
        }

        public EndPoint[] GetEndPoints()
        {
            foreach (var endPoint in _enpoints)
            {
                endPoint.Origin = _origin;
            }
            return _enpoints.ToArray();
        }

        public SubscriptionToPublishSubscribeChannel[] GetSubscriptionsToPublishSubscribeChannel()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Origin = _origin;
            }
            return _subscriptions.ToArray();
        }

        public PointToPointChannel[] GetPointToPointChannels()
        {
            return _pointtopointchannels.ToArray();
        }

        public PublishSubscribeChannel[] GetPublishSubscribeChannels()
        {
            return _publishsubscriberchannels.ToArray();
        }


        public IListenerRouteBuilder<THandler> RegisterHandler<THandler>(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var builder = new NameRouteBuilder<THandler>(_routes, name);

            return builder;
        }

        public void RegisterSubscriptionToPublishSubscriberChannel<TExtractorConectionString>(string subscription, string path, Func<IValueSettingFinder, string> connectionstringextractor, bool all=false)
            where TExtractorConectionString : IValueSettingFinder
        {
            if (string.IsNullOrWhiteSpace(subscription))
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (connectionstringextractor == null)
            {
                throw new ArgumentNullException(nameof(connectionstringextractor));
            }
            var channel = new SubscriptionToPublishSubscribeChannel(subscription, path)
            {
                ConnectionStringExtractorType = typeof(TExtractorConectionString),
                ToConnectionStringExtractor = connectionstringextractor,
                All = all
            };

            _subscriptions.Add(channel);
        }

        public void RegisterSubscriptionToPublishSubscriberChannel(string subscription, string path, string connectionstring, bool all = false)
        {
            if (string.IsNullOrWhiteSpace(subscription))
            {
                throw new ArgumentNullException(nameof(subscription));
            }
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (string.IsNullOrWhiteSpace(connectionstring))
            {
                throw new ArgumentNullException(nameof(connectionstring));
            }

            Func<IValueSettingFinder, string> extractor = x => connectionstring;

            var channel = new SubscriptionToPublishSubscribeChannel(subscription, path)
            {
                ConnectionStringExtractorType = typeof(NullValueSettingFinder),
                ToConnectionStringExtractor = extractor,
                All = all
            };

            _subscriptions.Add(channel);
        }


        public void RegisterPointToPointChannel<TExtractorConectionString>(string path, Func<IValueSettingFinder, string> connectionstringextractor)
            where TExtractorConectionString : IValueSettingFinder
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (connectionstringextractor == null)
            {
                throw new ArgumentNullException(nameof(connectionstringextractor));
            }
            var channel = new PointToPointChannel(path)
            {
                ConnectionStringExtractorType = typeof (TExtractorConectionString),
                ToConnectionStringExtractor = connectionstringextractor
            };

            _pointtopointchannels.Add(channel);
        }

        public void RegisterPointToPointChannel(string path, string connectionstring)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (string.IsNullOrWhiteSpace(connectionstring))
            {
                throw new ArgumentNullException(nameof(connectionstring));
            }

            Func<IValueSettingFinder, string> extractor = x => connectionstring;

            var channel = new PointToPointChannel(path)
            {
                ConnectionStringExtractorType = typeof(NullValueSettingFinder),
                ToConnectionStringExtractor = extractor
            };

            _pointtopointchannels.Add(channel);
        }

        public void RegisterPublishSubscriberChannel<TExtractorConectionString>(string path, Func<IValueSettingFinder, string> connectionstringextractor)
            where TExtractorConectionString : IValueSettingFinder
        { 
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (connectionstringextractor == null)
            {
                throw new ArgumentNullException(nameof(connectionstringextractor));
            }
            var channel = new PublishSubscribeChannel(path)
            {
                ConnectionStringExtractorType = typeof(TExtractorConectionString),
                ToConnectionStringExtractor = connectionstringextractor
            };

            _publishsubscriberchannels.Add(channel);
        }

        public void RegisterPublishSubscriberChannel(string path, string connectionstring)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (string.IsNullOrWhiteSpace(connectionstring))
            {
                throw new ArgumentNullException(nameof(connectionstring));
            }

            Func<IValueSettingFinder, string> extractor = x => connectionstring;

            var channel = new PublishSubscribeChannel(path)
            {
                ConnectionStringExtractorType = typeof(NullValueSettingFinder),
                ToConnectionStringExtractor = extractor
            };

            _publishsubscriberchannels.Add(channel);
        }

        public ITimeoutBuilder RegisterSaga<TData>(string name, Action<IStartingRouteBuilder<TData>> start, Action<INextRouteBuilder<TData>> @continue=null, Action<IEndingRouteBuilder<TData>> end = null) where TData : class, new()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (start==null)
            {
                throw new ArgumentNullException(nameof(start));
            }


            var saga = new Saga(name, typeof(TData));

            start(new StartingRouteBuilder<TData>(saga));

            @continue?.Invoke(new NextRouteBuilder<TData>(saga));

            end?.Invoke(new EndingRouteBuilder<TData>(saga));

            _sagas.Add(saga);

            var timeoutbuilder = new TimeoutBuilder(saga);

            return timeoutbuilder;
        }

        public void RegisterOrigin(string name, string key="")
        {
            _origin.From = name;

            _origin.Key = key;
        }

        public INameEndPointBuilder RegisterEndPoint(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var endpoint = new EndPoint(name);

            _enpoints.Add(endpoint);

            var builder = new EndPointBuilder(endpoint);

            return builder;
        }

        public void RegisterEndPoint<TExtractor, T>(string name) where TExtractor : IEndPointSettingFinder<T>
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            var endpoint = new EndPoint(name);

            _enpoints.Add(endpoint);

            endpoint.ExtractorType = typeof(TExtractor);

            endpoint.MessageType = typeof(T);
        }
    }
}
