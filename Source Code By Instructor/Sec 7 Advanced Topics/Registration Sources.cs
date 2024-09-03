using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Autofac;
using Autofac.Configuration;

using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;
using Autofac.Features.ResolveAnything;
using Microsoft.Extensions.Configuration;

namespace AutofacSamples
{
  public abstract class BaseHandler
  {
    public virtual string Handle(string message)
    {
      return "Handled: " + message;
    }
  }

  public class HandlerA : BaseHandler
  {
    public override string Handle(string message)
    {
      return "Handled by A: " + message;
    }
  }

  public class HandlerB : BaseHandler
  {
    public override string Handle(string message)
    {
      return "Handled by B: " + message;
    }
  }

  public interface IHandlerFactory
  {
    T GetHandler<T>() where T : BaseHandler;
  }

  class HandlerFactory : IHandlerFactory
  {
    public T GetHandler<T>() where T : BaseHandler
    {
      return Activator.CreateInstance<T>();
    }
  }

  public class ConsumerA
  {
    private HandlerA handlerA;

    public ConsumerA(HandlerA handlerA)
    {
      if (handlerA == null)
      {
        throw new ArgumentNullException(paramName: nameof(handlerA));
      }
      this.handlerA = handlerA;
    }

    public void DoWork()
    {
      Console.WriteLine(handlerA.Handle("ConsumerA"));
    }
  }

  public class ConsumerB
  {
    private HandlerB handlerB;


    public ConsumerB(HandlerB handlerB)
    {
      if (handlerB == null)
      {
        throw new ArgumentNullException(paramName: nameof(handlerB));
      }
      this.handlerB = handlerB;
    }

    public void DoWork()
    {
      Console.WriteLine(handlerB.Handle("ConsumerB"));
    }
  }

  public class HandlerRegistrationSource : IRegistrationSource
  {
    public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
    {
      var swt = service as IServiceWithType;
      if (swt == null
          || swt.ServiceType == null
          || !swt.ServiceType.IsAssignableTo<BaseHandler>())
      {
        yield break;
      }

      yield return new ComponentRegistration(
          Guid.NewGuid(),
          new DelegateActivator(
            swt.ServiceType,
            (c, p) =>
            {
              var provider = c.Resolve<IHandlerFactory>();
              var method = provider.GetType().GetMethod("GetHandler").MakeGenericMethod(swt.ServiceType);
              return method.Invoke(provider, null);
            }
          ),
          new CurrentScopeLifetime(),
          InstanceSharing.None,
          InstanceOwnership.OwnedByLifetimeScope,
          new[]{service},
          new ConcurrentDictionary<string, object>());
    }

    public bool IsAdapterForIndividualComponents => false;
  }

  public class Program
  {
    static void Main(string[] args)
    {
      var b = new ContainerBuilder();
      b.RegisterType<HandlerFactory>().As<IHandlerFactory>();
      b.RegisterSource(new HandlerRegistrationSource());
      b.RegisterType<ConsumerA>();
      b.RegisterType<ConsumerB>();

      using (var c = b.Build())
      {
        c.Resolve<ConsumerA>().DoWork();
        c.Resolve<ConsumerB>().DoWork();
      }
    }
  }
}