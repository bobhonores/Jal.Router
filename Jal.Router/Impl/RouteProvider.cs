using System;
using System.Collections.Generic;
using System.Linq;
using Jal.Router.Interface;
using Jal.Router.Model;

namespace Jal.Router.Impl
{
    public class RouteProvider : IRouteProvider
    {
        private readonly Route[] _routes;

        private readonly Saga[] _sagas;
        public RouteProvider(IRouterConfigurationSource[] sources)
        {
            var routes = new List<Route>(); 

            foreach (var source in sources)
            {
                routes.AddRange(source.GetRoutes());
            }

            _routes = routes.ToArray();
        }

        public Route[] Provide(Type type, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return _routes.Where(x => x.BodyType == type).ToArray();
            }
            else
            {
                return _routes.Where(x => x.Name == name && x.BodyType == type).ToArray();
            }
            
        }
    }
}