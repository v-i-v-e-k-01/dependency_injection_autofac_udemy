using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Features.Indexed;
using Autofac.Features.Metadata;
using Autofac.Features.OwnedInstances;
using Module = Autofac.Module;

namespace AutofacSamples
{
  public interface IVehicle
  {
    void Go();
  }

  class Truck : IVehicle
  {
    private IDriver driver;

    public Truck(IDriver driver)
    {
      if (driver == null)
      {
        throw new ArgumentNullException(paramName: nameof(driver));
      }
      this.driver = driver;
    }

    public void Go()
    {
      driver.Drive();
    }
  }

  public interface IDriver
  {
    void Drive();
  }

  class CrazyDriver : IDriver
  {
    public void Drive()
    {
      Console.WriteLine("Going too fast and crashing into a tree");
    }
  }

  public class SaneDriver : IDriver
  {
    public void Drive()
    {
      Console.WriteLine("Driving safely to destination");
    }
  }

  public class TransportModule : Module
  {
    public bool ObeySpeedLimit { get; set; }

    protected override void Load(ContainerBuilder builder)
    {
      if (ObeySpeedLimit)
        builder.RegisterType<SaneDriver>().As<IDriver>();
      else
        builder.RegisterType<CrazyDriver>().As<IDriver>();

      builder.RegisterType<Truck>().As<IVehicle>();
    }
  }

  internal class Program
  {
    public static void Main(string[] args)
    {
      var builder = new ContainerBuilder();
      builder.RegisterModule(new TransportModule{ObeySpeedLimit = true});
      using (var c = builder.Build())
      {
        c.Resolve<IVehicle>().Go();
      }
    }
  }
}