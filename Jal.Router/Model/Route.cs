﻿using System;
using System.Collections.Generic;
using Jal.Router.Interface;
using Jal.Router.Interface.Inbound;

namespace Jal.Router.Model
{
    public class Route
    {
        public Route(string name, Type contenttype, Type consumerinterfacetype)
        {
            ContentType = contenttype;
            ConsumerInterfaceType = consumerinterfacetype;
            MiddlewareTypes = new List<Type>();
            Name = name;
            Paths = new List<RoutePath>();
        }

        //public Type ConnectionStringExtractorType { get; set; }

        //public object ToConnectionStringExtractor { get; set; }

        //public string ToConnectionString { get; set; }

        //public string ToPath { get; set; }

        //public string ToSubscription { get; set; }

        public IList<RoutePath> Paths { get; set; }

        public Type ConsumerInterfaceType { get; set; }

        public string Name { get; set; }

        public Type ContentType { get; set; }

        public Type RetryExceptionType { get; set; }

        public Type RetryExtractorType { get; set; }

        public string OnRetryEndPoint { get; set; }

        public string OnErrorEndPoint { get; set; }

        public Func<IValueSettingFinder, IRetryPolicy> RetryPolicyExtractor { get; set; }

        public IList<Type> MiddlewareTypes { get; set; }

        public string ForwardEndPoint { get; set; }

        //public Action ShutdownAction { get; set; }

        public Func<MessageContext, bool> When { get; set; }
    }

    public class Route<TContent, TConsumer> : Route
    {
        public List<RouteMethod<TContent, TConsumer>> RouteMethods { get; set; }

        public Type ConsumerType { get; set; }

        public Route(string name) : base(name, typeof(TContent), typeof(TConsumer))
        {
            RouteMethods = new List<RouteMethod<TContent, TConsumer>>();
        }
    }
}