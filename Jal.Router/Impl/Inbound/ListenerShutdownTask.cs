using System;
using Jal.Router.Interface;
using Jal.Router.Interface.Management;
using Jal.Router.Model;

namespace Jal.Router.Impl.Inbound
{
    public class ListenerShutdownTask : IShutdownTask
    {
        private readonly IRouterConfigurationSource[] _sources;

        private readonly IChannelPathBuilder _builder;

        public ListenerShutdownTask(IRouterConfigurationSource[] sources, IChannelPathBuilder builder)
        {
            _sources = sources;
            _builder = builder;
        }

        public void Run()
        {
            foreach (var source in _sources)
            {
                foreach (var route in source.GetRoutes())
                {
                    Shutdown(route);
                }

                foreach (var saga in source.GetSagas())
                {
                    Shutdown(saga, saga.StartingRoute);

                    foreach (var nextroute in saga.NextRoutes)
                    {
                        Shutdown(saga, nextroute);
                    }
                }
            }
        }

        private void Shutdown(Route route)
        {
            foreach (var routePath in route.Paths)
            {
                if (routePath.ShutdownAction != null)
                {
                    if (!string.IsNullOrWhiteSpace(routePath.ToPath) && string.IsNullOrWhiteSpace(routePath.ToSubscription))
                    {
                        routePath.ShutdownAction();

                        var channelpath = _builder.BuildFromRoute(route.Name, routePath);

                        Console.WriteLine($"Shutdown {channelpath} point to point channel");
                    }

                    if (!string.IsNullOrWhiteSpace(routePath.ToPath) && !string.IsNullOrWhiteSpace(routePath.ToSubscription))
                    {
                        routePath.ShutdownAction();

                        var channelpath = _builder.BuildFromRoute(route.Name, routePath);

                        Console.WriteLine($"Shutdown {channelpath} publish subscriber channel");
                    }
                }
            }
        }

        private void Shutdown(Saga saga, Route route)
        {
            foreach (var routePath in route.Paths)
            {
                if (routePath.ShutdownAction != null)
                {
                    if (!string.IsNullOrWhiteSpace(routePath.ToPath) && string.IsNullOrWhiteSpace(routePath.ToSubscription))
                    {
                        routePath.ShutdownAction();

                        var channelpath = _builder.BuildFromSagaAndRoute(saga, route.Name, routePath);

                        Console.WriteLine($"Shutdown {channelpath} point to point channel");
                    }

                    if (!string.IsNullOrWhiteSpace(routePath.ToPath) && !string.IsNullOrWhiteSpace(routePath.ToSubscription))
                    {
                        routePath.ShutdownAction();

                        var channelpath = _builder.BuildFromSagaAndRoute(saga, route.Name, routePath);

                        Console.WriteLine($"Shutdown {channelpath} publish subscriber channel");
                    }
                }
            }
        }
    }
}