using System;
using Jal.Router.Interface;

namespace Jal.Router.Fluent.Interface
{
    public interface IListenerChannelBuilder<THandler>
    {
        void Add<TExtractorConectionString>(string path, Func<IValueSettingFinder, string> connectionstringextractor) where TExtractorConectionString : IValueSettingFinder;
        void Add<TExtractorConectionString>(string path, string subscription, Func<IValueSettingFinder, string> connectionstringextractor) where TExtractorConectionString : IValueSettingFinder;
    }
}