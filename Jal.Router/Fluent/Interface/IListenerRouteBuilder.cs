using System;
using System.Collections.Generic;
using Jal.Router.Interface;
using Jal.Router.Model;

namespace Jal.Router.Fluent.Interface
{
    public interface IListenerRouteBuilder<THandler>
    {
        INameRouteBuilder<THandler> ToListenChannels(Action<IListenerChannelBuilder<THandler>> channelbuilder);
        INameRouteBuilder<THandler> ToListenPointToPointChannel<TExtractorConectionString>(string path, Func<IValueSettingFinder, string> connectionstringextractor)
            where TExtractorConectionString : IValueSettingFinder;
        INameRouteBuilder<THandler> ToListenPublishSubscribeChannel<TExtractorConectionString>(string path, string subscription, Func<IValueSettingFinder, string> connectionstringextractor)
            where TExtractorConectionString : IValueSettingFinder;
    }
}