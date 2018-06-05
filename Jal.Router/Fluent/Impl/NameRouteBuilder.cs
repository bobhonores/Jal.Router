using System;
using System.Collections.Generic;
using Jal.Router.Fluent.Interface;
using Jal.Router.Interface;
using Jal.Router.Model;

namespace Jal.Router.Fluent.Impl
{
    public class NameRouteBuilder<THandler> : INameRouteBuilder<THandler>, IListenerRouteBuilder<THandler>, IListenerChannelBuilder<THandler>
    {
        //private string _tosubscription;

        //private string _topath;

        //private Type _connectionstringextractortype;

        //private object _toconnectionstringextractor;

        private readonly List<Route> _routes;

        private readonly string _name;

        private IList<RoutePath> _paths;

        private Action<IListenerChannelBuilder<THandler>> _channelbuilder;

        public NameRouteBuilder(List<Route> routes, string name)
        {
            _routes = routes;

            _name = name;

            _paths = new List<RoutePath>();
        }

        public IHandlerBuilder<TContent, THandler> ForMessage<TContent>()
        {
            _channelbuilder?.Invoke(this);

            var value = new Route<TContent, THandler>(_name) {Paths = _paths};

            var builder = new HandlerBuilder<TContent, THandler>(value);

            _routes.Add(value);

            return builder;
        }

        public void Add<TExtractorConectionString>(string path, Func<IValueSettingFinder, string> connectionstringextractor) where TExtractorConectionString : IValueSettingFinder
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (connectionstringextractor == null)
            {
                throw new ArgumentNullException(nameof(connectionstringextractor));
            }

            _paths.Add(new RoutePath
            {
                ToPath = path,

                ToConnectionStringExtractor = connectionstringextractor,

                ConnectionStringExtractorType = typeof(TExtractorConectionString)
            });
        }

        public void Add<TExtractorConectionString>(string path, string subscription, Func<IValueSettingFinder, string> connectionstringextractor) where TExtractorConectionString : IValueSettingFinder
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (connectionstringextractor == null)
            {
                throw new ArgumentNullException(nameof(connectionstringextractor));
            }
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }

            _paths.Add(new RoutePath
            {
                ToPath = path,

                ToConnectionStringExtractor = connectionstringextractor,

                ConnectionStringExtractorType = typeof(TExtractorConectionString),

                ToSubscription = subscription
            });
        }

        public INameRouteBuilder<THandler> ToListenChannels(Action<IListenerChannelBuilder<THandler>> channelbuilder)
        {
            if (channelbuilder == null)
            {
                throw new ArgumentNullException(nameof(channelbuilder));
            }

            _channelbuilder = channelbuilder;

            return this;
        }

        public INameRouteBuilder<THandler> ToListenPointToPointChannel<TExtractorConectionString>(string path, Func<IValueSettingFinder, string> connectionstringextractor) where TExtractorConectionString : IValueSettingFinder
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (connectionstringextractor == null)
            {
                throw new ArgumentNullException(nameof(connectionstringextractor));
            }

            _paths.Add(new RoutePath
            {
                ToPath = path,

                ToConnectionStringExtractor = connectionstringextractor,

                ConnectionStringExtractorType = typeof(TExtractorConectionString)
            });

            return this;
        }

        public INameRouteBuilder<THandler> ToListenPublishSubscribeChannel<TExtractorConectionString>(string path, string subscription, Func<IValueSettingFinder, string> connectionstringextractor) where TExtractorConectionString : IValueSettingFinder
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (connectionstringextractor == null)
            {
                throw new ArgumentNullException(nameof(connectionstringextractor));
            }
            if (subscription == null)
            {
                throw new ArgumentNullException(nameof(subscription));
            }

            _paths.Add(new RoutePath
            {
                ToPath = path,

                ToConnectionStringExtractor = connectionstringextractor,

                ConnectionStringExtractorType = typeof(TExtractorConectionString),

                ToSubscription = subscription
            });
            
            return this; 
        }
    }
}