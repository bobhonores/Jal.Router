using Jal.Router.Model;

namespace Jal.Router.Interface
{
    public interface IChannelPathBuilder
    {
        string BuildFromRoute(string routeName, Channel route);

        string BuildFromSagaAndRoute(Saga saga, string routeName, Channel route);

        string BuildFromContext(MessageContext context);

        string BuildReplyFromContext(MessageContext context);
    }
}