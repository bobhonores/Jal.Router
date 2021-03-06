using System;
using System.Collections.Generic;
using Jal.Router.Interface;
using Jal.Router.Interface.Management;
using Jal.Router.Interface.Outbound;
using Jal.Router.Model;
using Jal.Router.Model.Outbound;

namespace Jal.Router.Impl.Outbound
{
    public class Bus : IBus
    {
        private readonly IEndPointProvider _provider;

        private readonly IComponentFactory _factory;

        private readonly IConfiguration _configuration;

        public Bus(IEndPointProvider provider, IComponentFactory factory, IConfiguration configuration)
        {
            _provider = provider;
            _factory = factory;
            _configuration = configuration;
        }

        private TResult Reply<TResult>(MessageContext message, Options options)
        {
            var interceptor = _factory.Create<IBusInterceptor>(_configuration.BusInterceptorType);

            interceptor.OnEntry(message, options);

            try
            {
                if (!string.IsNullOrWhiteSpace(message.ToConnectionString) && !string.IsNullOrWhiteSpace(message.ToPath) &&
                    !string.IsNullOrWhiteSpace(message.ToReplyPath) && !string.IsNullOrWhiteSpace(message.ToReplyConnectionString)
                    && !string.IsNullOrWhiteSpace(message.ReplyToRequestId))
                {
                    var middlewares = new List<Type>();

                    middlewares.AddRange(_configuration.OutboundMiddlewareTypes);

                    middlewares.Add(typeof(RequestReplyHandler));

                    var parameter = new MiddlewareParameter() { Options = options, OutboundType = "Reply", ResultType = typeof(TResult)};

                    var pipeline = new Pipeline(_factory, middlewares.ToArray(), message, parameter);

                    pipeline.Execute();

                    interceptor.OnSuccess(message, options);

                    return (TResult)parameter.Result;
                }
                else
                {
                    throw new ApplicationException($"Endpoint {message.EndPointName}, invalid connection string and/or path and/or reply connection string and/or replypath and/or replytorequestid");
                }
            }
            catch (Exception ex)
            {
                interceptor.OnError(message, options, ex);

                throw;
            }
            finally
            {
                interceptor.OnExit(message, options);
            }
        }
        public TResult Reply<TContent, TResult>(TContent content, Options options)
        {
            var endpoint = _provider.Provide(options.EndPointName, content.GetType());

            var setting = _provider.Provide(endpoint, content);

            return Reply<TContent, TResult>(content, setting, endpoint.Origin, options);
        }
        public TResult Reply<TContent, TResult>(TContent content, EndPointSetting endpoint, Origin origin, Options options)
        {
            var serializer = _factory.Create<IMessageSerializer>(_configuration.MessageSerializerType);

            var message = new MessageContext
            {
                Id = options.Id,
                //Content = content,
                ToConnectionString = endpoint.ToConnectionString,
                ToPath = endpoint.ToPath,
                Origin = origin,
                Headers = options.Headers,
                Version = options.Version,
                ScheduledEnqueueDateTimeUtc = options.ScheduledEnqueueDateTimeUtc,
                RetryCount = options.RetryCount,
                SagaContext = options.SagaContext,
                ContentType = content.GetType(),
                DateTimeUtc = DateTime.UtcNow,
                ContentAsString = serializer.Serialize(content),
                ToReplyConnectionString = endpoint.ToReplyConnectionString,
                ToReplySubscription = endpoint.ToReplySubscription,
                ToReplyPath = endpoint.ToReplyPath,
                ReplyToRequestId = options.ReplyToRequestId,
                RequestId = options.RequestId,
                ToReplyTimeOut = endpoint.ToReplyTimeOut,
                EndPointName = endpoint.EndPointName,
                ResultType = typeof(TResult),
                Tracks = options.Tracks
            };

            return Reply<TResult>(message, options);
        }
        public TResult Reply<TContent, TResult>(TContent content, Origin origin, Options options)
        {
            var endpoint = _provider.Provide(options.EndPointName, content.GetType());

            var setting = _provider.Provide(endpoint, content);

            if (string.IsNullOrWhiteSpace(origin.From))
            {
                origin.From = endpoint.Origin.From;
            }

            if (string.IsNullOrWhiteSpace(origin.Key))
            {
                origin.Key = endpoint.Origin.Key;
            }

            return Reply<TContent, TResult>(content, setting, origin, options);
        }
        private void Send(MessageContext message, Options options)
        {
            var interceptor = _factory.Create<IBusInterceptor>(_configuration.BusInterceptorType);

            interceptor.OnEntry(message, options);

            try
            {
                if (!string.IsNullOrWhiteSpace(message.ToConnectionString) && !string.IsNullOrWhiteSpace(message.ToPath))
                {
                    var middlewares = new List<Type>();

                    middlewares.AddRange(_configuration.OutboundMiddlewareTypes);

                    middlewares.Add(typeof(PointToPointHandler));

                    var parameter = new MiddlewareParameter() { Options = options, OutboundType = "Send"};

                    var pipeline = new Pipeline(_factory, middlewares.ToArray(), message, parameter);

                    pipeline.Execute();

                    interceptor.OnSuccess(message, options);
                }
                else
                {
                    throw new ApplicationException($"Endpoint {message.EndPointName}, invalid connection string and/or path");
                }
            }
            catch (Exception ex)
            {
                interceptor.OnError(message, options, ex);

                throw;
            }
            finally
            {
                interceptor.OnExit(message, options);
            }
        }

        public void Send<TContent>(TContent content, EndPointSetting endpoint, Origin origin, Options options)
        {
            var serializer = _factory.Create<IMessageSerializer>(_configuration.MessageSerializerType);

            var message = new MessageContext
            {
                Id = options.Id,
                //Content = content,
                ToConnectionString = endpoint.ToConnectionString,
                ToPath = endpoint.ToPath,
                Origin = origin,
                Headers = options.Headers,
                Version = options.Version,
                ScheduledEnqueueDateTimeUtc = options.ScheduledEnqueueDateTimeUtc,
                RetryCount = options.RetryCount,
                SagaContext = options.SagaContext,
                ContentType = content.GetType(),
                DateTimeUtc = DateTime.UtcNow,
                ContentAsString = serializer.Serialize(content),
                ReplyToRequestId = options.ReplyToRequestId,
                RequestId = options.RequestId,
                ToReplyTimeOut = endpoint.ToReplyTimeOut,
                EndPointName = endpoint.EndPointName,
                Tracks = options.Tracks
            };

            Send(message, options);
        }

        public void Send<TContent>(TContent content, Options options)
        {
            var endpoint = _provider.Provide(options.EndPointName, content.GetType());

            var setting = _provider.Provide(endpoint, content);

            var origin = endpoint.Origin;

            Send(content, setting, origin, options);
        }
        public void Send<TContent>(TContent content, Origin origin, Options options)
        {
            var endpoint = _provider.Provide(options.EndPointName, content.GetType());

            var setting = _provider.Provide(endpoint, content);

            if (string.IsNullOrWhiteSpace(origin.From))
            {
                origin.From = endpoint.Origin.From;
            }

            if (string.IsNullOrWhiteSpace(origin.Key))
            {
                origin.Key = endpoint.Origin.Key;
            }

            Send(content, setting, origin, options);
        } 
        public void Publish<TContent>(TContent content, Options options)
        {
            var endpoint = _provider.Provide(options.EndPointName, content.GetType());

            var setting = _provider.Provide(endpoint, content);

            var origin = endpoint.Origin;

            Publish(content, setting, origin, options);
        }
        public void Publish<TContent>(TContent content, Origin origin, Options options)
        {
            var endpoint = _provider.Provide(options.EndPointName, content.GetType());

            var setting = _provider.Provide(endpoint, content);

            if (string.IsNullOrWhiteSpace(origin.From))
            {
                origin.From = endpoint.Origin.From;
            }

            if (string.IsNullOrWhiteSpace(origin.Key))
            {
                origin.Key = endpoint.Origin.Key;
            }

            Publish(content, setting, origin, options);
        }
        public void Publish<TContent>(TContent content, EndPointSetting endpoint, Origin origin, Options options)
        {
            var serializer = _factory.Create<IMessageSerializer>(_configuration.MessageSerializerType);

            var message = new MessageContext
            {
                Id = options.Id,
                //Content = content,
                ToConnectionString = endpoint.ToConnectionString,
                ToPath = endpoint.ToPath,
                Origin = origin,
                Headers = options.Headers,
                Version = options.Version,
                ScheduledEnqueueDateTimeUtc = options.ScheduledEnqueueDateTimeUtc,
                RetryCount = options.RetryCount,
                SagaContext = options.SagaContext,
                ContentType = content.GetType(),
                DateTimeUtc = DateTime.UtcNow,
                ContentAsString = serializer.Serialize(content),
                ReplyToRequestId = options.ReplyToRequestId,
                RequestId = options.RequestId,
                ToReplyTimeOut = endpoint.ToReplyTimeOut,
                EndPointName = endpoint.EndPointName,
                Tracks = options.Tracks
            };

            Publish(message, options);
        }

        private void Publish(MessageContext message, Options options)
        {
            var interceptor = _factory.Create<IBusInterceptor>(_configuration.BusInterceptorType);

            interceptor.OnEntry(message, options);

            try
            {
                if (!string.IsNullOrWhiteSpace(message.ToConnectionString) && !string.IsNullOrWhiteSpace(message.ToPath))
                {
                    var middlewares = new List<Type>();

                    middlewares.AddRange(_configuration.OutboundMiddlewareTypes);

                    middlewares.Add(typeof(PublishSubscribeHandler));

                    var parameter = new MiddlewareParameter() { Options = options, OutboundType = "Publish" };

                    var pipeline = new Pipeline(_factory, middlewares.ToArray(), message, parameter);

                    pipeline.Execute();

                    interceptor.OnSuccess(message, options);
                }
                else
                {
                    throw new ApplicationException($"Endpoint {message.EndPointName}, invalid connection string and/or path");
                }
            }
            catch (Exception ex)
            {
                interceptor.OnError(message, options, ex);

                throw;
            }
            finally
            {
                interceptor.OnExit(message, options);
            }
        }

        public void FireAndForget<TContent>(TContent content, EndPointSetting endpoint, Origin origin, Options options)
        {
            var serializer = _factory.Create<IMessageSerializer>(_configuration.MessageSerializerType);

            var message = new MessageContext
            {
                Id = options.Id,
                //Content = content,
                Origin = origin,
                ToConnectionString = endpoint.ToConnectionString,
                ToPath = endpoint.ToPath,
                Headers = options.Headers,
                Version = options.Version,
                ScheduledEnqueueDateTimeUtc = options.ScheduledEnqueueDateTimeUtc,
                RetryCount = options.RetryCount,
                SagaContext = options.SagaContext,
                ContentType = content.GetType(),
                DateTimeUtc = DateTime.UtcNow,
                ContentAsString = serializer.Serialize(content),
                EndPointName = endpoint.EndPointName,
                Tracks = options.Tracks
            };

            message.Origin.Key = string.Empty;

            Send(message, options);
        }

        public void FireAndForget<TContent>(TContent content, Options options)
        {
            var endpoint = _provider.Provide(options.EndPointName, content.GetType());

            var setting = _provider.Provide(endpoint, content);

            FireAndForget(content, setting, new Origin() { Key = endpoint.Origin.Key, From = endpoint.Origin.From }, options);
        }

        public void FireAndForget<TContent>(TContent content, Origin origin, Options options)
        {
            var endpoint = _provider.Provide(options.EndPointName, content.GetType());

            var setting = _provider.Provide(endpoint, content);

            if (string.IsNullOrWhiteSpace(origin.From))
            {
                origin.From = endpoint.Origin.From;
            }

            if (string.IsNullOrWhiteSpace(origin.Key))
            {
                origin.Key = endpoint.Origin.Key;
            }

            FireAndForget(content, setting, origin, options);
        }
    }
}