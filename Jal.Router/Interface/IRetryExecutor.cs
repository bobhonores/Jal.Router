using System;
using Jal.Router.Model;

namespace Jal.Router.Interface
{
    public interface IRetryExecutor
    {
        void Execute<TContent, THandler>(Action action, RouteMethod<TContent, THandler> routeMethod, InboundMessageContext<TContent> context) where THandler : class;
    }
}