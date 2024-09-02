using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Module = Autofac.Module;

namespace AutofacSamples
{
  public interface IResource
  {

  }

  class SingletonResource : IResource
  {
  }

  public class InstancePerDependencyResource : IResource, IDisposable
  {
    public InstancePerDependencyResource()
    {
      Console.WriteLine("Instance per dep created");
    }

    public void Dispose()
    {
      Console.WriteLine("Instance per dep destroyed");
    }
  }

  public class ResourceManager
  {
    public ResourceManager(IEnumerable<IResource> resources)
    {
      if (resources == null)
      {
        throw new ArgumentNullException(paramName: nameof(resources));
      }
      Resources = resources;
    }

    public IEnumerable<IResource> Resources { get; set; }
  }

  internal class Program
  {
    public static void Main(string[] args)
    {
      var builder = new ContainerBuilder();
      builder.RegisterType<ResourceManager>().SingleInstance();
      builder.RegisterType<SingletonResource>()
        .As<IResource>().SingleInstance();
      builder.RegisterType<InstancePerDependencyResource>()
        .As<IResource>();

      using (var container = builder.Build())
      using (var scope = container.BeginLifetimeScope())
      {
        scope.Resolve<ResourceManager>();
      }
    }
  }
}