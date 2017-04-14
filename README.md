# Jal.Router
Just another library to route in/out messages

## How to use?

### Routing

When a messages arrives (event or command) this feature allows us to handle it based on a routing configuration.
For instance, if we want to handle a Transfer command by the following class:
```
public class TransferMessageHandler : IMessageHandler<Transfer>
{
    public void Handle(Transfer transfer)
    {
	//Do something
    }
}
```
When the message is listen by the standard Azure Webjob SDK
```
public class Listener
{
    public void Listen([ServiceBusTrigger("somequeue")] BrokeredMessage message)
    {
	//Route message somewhere
    }
}
```

In order to achive that we need to use the class IRouter&lt;T&gt; where T will be the BrokeredMessage class).
The next step is use the method Route&lt;TContent&gt;> where TContent will be the object under the brokered message.
```
public class Listener
{
    private readonly IRouter<BrokeredMessage> _router;

    public Listener(IRouter<BrokeredMessage> router)
    {
        _router = router;
    }

    public void Listen([ServiceBusTrigger("somequeue")] BrokeredMessage message)
    {
        _router.Route<Transfer>(message);
    }
}
```
The last step is setup the routing itself.
```
public class RouterConfigurationSource : AbstractRouterConfigurationSource
{
    public RouterConfigurationSource()
    {
        RegisterRoute<IMessageHandler<Transfer>>().ForMessage<Transfer>().ToBeHandledBy<TransferMessageHandler>(x =>
        {
            x.With(((transfer, handler) => handler.Handle(transfer)));
        });
    }
}
```
Let's see every method used in the class above:

* RegisterRoute, allows us to start the creation of a new route handled by the interface specified in the generic parameter.
* ForMessage, indicates the object under the brokered message to be routed
* ToBeHandledBy, This method will tell us the concrete class on charge to handle the message how and to handle this message usign this class (you can add as many ways as you want there).

Suppose that now you're handler has two method to handle the message in different ways under different circumstances.
```
public class TransferMessageHandler : IMessageHandler<Transfer>
{
    public void HandleWay1(Transfer transfer)
    {
	//Do something
    }

    public bool IsWay1(Transfer transfer)
    {
	//Do something
    }

    public void HandleWay2(Transfer transfer)
    {
	//Do something
    }
}
```

You can have the following configuration to handle this scenario.
```
public class RouterConfigurationSource : AbstractRouterConfigurationSource
{
    public RouterConfigurationSource()
    {
        RegisterRoute<IMessageHandler<Transfer>>().ForMessage<Transfer>().ToBeHandledBy<TransferMessageHandler>(x =>
        {
			x.With(((transfer, handler) => handler.HandleWay1(transfer))).When(((message, handler) => handler.IsWay1(transfer)));
			x.With(((transfer, handler) => handler.HandleWay2(transfer))).When(((message, handler) => !handler.IsWay1(transfer)));
        });
    }
}
```

### Send

### Publish

### Retry

### Castle Windsor Integration

Note: The Jal.Locator.CastleWindsor and Jal.Finder library are needed

Setup the Jal.Finder library

	var directory = AppDomain.CurrentDomain.BaseDirectory;

	var finder = AssemblyFinder.Builder.UsePath(directory).Create;

	var assemblies = finder.GetAssembliesTagged<AssemblyTagAttribute>();

Setup the Castle Windsor container

	var container = new WindsorContainer();

	container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel));

Install the Jal.Locator.CastleWindsor library

	container.Install(new ServiceLocatorInstaller());

Install the Jal.Router library, use the RouterInstaller class included

	container.Install(new RouterInstaller(assemblies));

Create a Handler interface and class

	public class MessageHandler : IMessageHandler<Message>
	{
		public void Handle(Message message)
		{
			Console.WriteLine("Sender"+ message.Name);
		}
	}

	public interface IMessageHandler<in T>
	{
		void Handle(T message);
	}

Create a class to setup the Jal.Route library

	public class RouterConfigurationSource : AbstractRouterConfigurationSource
	{
		public RouterConfigurationSource()
		{
			RegisterRoute<IMessageHandler<Message>>().ForMessage<Message>().ToBeHandledBy<MessageHandler>(x =>
			{
				x.With(((request, handler) => handler.Handle(request)));
			});
		}
	}

Tag the assembly container of the router configuration source classes in order to be read by the library

	[assembly: AssemblyTag]
	
Resolve an instance of the interface IRouter

	var router = container.Resolve<IRouter>();

Use the Router class

	var message = new Message();

	router.Route<Message>(message);

### Azure Service Bus Brokered Message Routing

Note: The Jal.Router.ServiceBus library is needed

Install the Jal.Router library, use the BrokeredMessageRouterInstaller class included

	container.Install(new BrokeredMessageRouterInstaller());

Create a Handler interface and class. The InboundMessageContext class could be used as a parameter, this class will contain useful data from the orginal brokered message.

	public class MessageHandler : IMessageHandler<Message>
	{
		public void Handle(Message message, InboundMessageContext context)
		{
			Console.WriteLine("Sender"+ message.Name);
		}
	}

	public interface IMessageHandler<in T>
	{
		void Handle(T message, InboundMessageContext context);
	}

Create a class to setup the library

	public class RouterConfigurationSource : AbstractRouterConfigurationSource
	{
		public RouterConfigurationSource()
		{
			RegisteFrom("From");

			RegisterRoute<IMessageHandler<Message>>().ForMessage<Message>().ToBeHandledBy<MessageHandler>(x =>
			{
				x.With<InboundMessageContext>(((request, handler, context) => handler.Handle(request, context)));
			});
		}
	}

Resolve an instance of the interface IBrokeredMessageRouter

	var router = container.Resolve<IBrokeredMessageRouter>();

Use the BrokeredMessageRouter class to handle brokered messages from a queue or topic. The message should be serialized as string from the origin.

	var brokeredmessage = new BrokeredMessage(@"{""Name"":""Test""}");

	router.Route<Message>(brokeredmessage);

Register an endpoint in the setup class

	public class RouterConfigurationSource : AbstractRouterConfigurationSource
	{
		public RouterConfigurationSource()
		{
			RegisteFrom("From");

			RegisterRoute<IMessageHandler<Message>>().ForMessage<Message>().ToBeHandledBy<MessageHandler>(x =>
			{
				x.With<InboundMessageContext>(((request, handler, context) => handler.Handle(request, context)));
			});

			RegisterEndPoint<AppSettingEndPointValueSettingFinder>()
				.ForMessage<Message>()
				.To(x => x.Find("toconnectionstring"), x => x.Find("topath"));
			   
		}
	}

Resolve an instance of the interface IBus

	var bus = container.Resolve<IBus>();

Use the BrokeredMessageBus class to send brokered messages

	var message = new Message();

	_bus.Send(message, new Options());

Use the BrokeredMessageBus class to publish brokered messages

	var message = new Message();

	_bus.Publish(message, new Options());