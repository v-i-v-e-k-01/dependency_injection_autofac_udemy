using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Autofac;
using Autofac.Features.Metadata;
using Autofac.Configuration;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;
using Autofac.Extras.AggregateService;
using Autofac.Features.ResolveAnything;
using Microsoft.Extensions.Configuration;

namespace AutofacSamples
{
  public interface IService1 {}
  public interface IService2 {}
  public interface IService3 {}
  public interface IService4 {}

  public class Class1 : IService1 {}
  public class Class2 : IService2 {}
  public class Class3 : IService3 {}

  public class Class4 : IService4
  {
    private string name;

    public Class4(string name)
    {
      if (name == null)
      {
        throw new ArgumentNullException(paramName: nameof(name));
      }
      this.name = name;
    }
  }

  public interface IMyAggregateService
  {
    IService1 Service1 { get; }
    IService2 Service2 { get; }
    IService3 Service3 { get; }
    //IService4 Service4 { get; }

    IService4 GetFourthService(string name);
  }

  public class Consumer
  {
    public IMyAggregateService AllServices;

    public Consumer(IMyAggregateService allServices)
    {
      if (allServices == null)
      {
        throw new ArgumentNullException(paramName: nameof(allServices));
      }
      AllServices = allServices;
    }
  }

  public class Program
  {
    static void Main(string[] args)
    {
      var cb = new ContainerBuilder();
      cb.RegisterAggregateService<IMyAggregateService>();
      cb.RegisterAssemblyTypes(typeof(Program).Assembly)
        .Where(t => t.Name.StartsWith("Class"))
        .AsImplementedInterfaces();
      cb.RegisterType<Consumer>();

      using (var container = cb.Build())
      {
        var consumer = container.Resolve<Consumer>();
        //Console.WriteLine(consumer.AllServices.Service3.GetType().Name);n
        Console.WriteLine(consumer.AllServices.GetFourthService("test").GetType().Name);
      }
    }
  }
}