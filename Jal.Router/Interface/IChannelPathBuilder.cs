using Jal.Router.Model;

namespace Jal.Router.Interface
{
    public interface IChannelPathBuilder
    {
        string BuildFromRoute(string routeName, RoutePath route);

        string BuildFromSagaAndRoute(Saga saga, string routeName, RoutePath route);

        string BuildFromContext(MessageContext context);

        string BuildReplyFromContext(MessageContext context);
    }
}