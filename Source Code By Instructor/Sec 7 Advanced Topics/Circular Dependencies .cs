using System;
using System.Collections.Generic;
using System.IO;
using Autofac;
using Autofac.Configuration;
using Microsoft.Extensions.Configuration;

namespace AutofacSamples
{
  public class ParentWithProperty
  {
    public ChildWithProperty Child { get; set; }

    public override string ToString()
    {
      return "Parent";
    }
  }

  public class ChildWithProperty
  {
    public ParentWithProperty Parent { get; set; }

    public override string ToString()
    {
      return "Child";
    }
  }

  public class ParentWithConstructor1
  {
    public ChildWithProperty1 Child;

    public ParentWithConstructor1(ChildWithProperty1 child)
    {
      if (child == null)
      {
        throw new ArgumentNullException(paramName: nameof(child));
      }
      Child = child;
    }

    public override string ToString()
    {
      return "Parent with a ChildWithProperty";
    }
  }

  public class ChildWithProperty1
  {
    public ParentWithConstructor1 Parent { get; set; }

    public override string ToString()
    {
      return "Child";
    }
  }

  public class Program
  {
    static void Main(string[] args)
    {
      var b = new ContainerBuilder();
      b.RegisterType<ParentWithConstructor1>().InstancePerLifetimeScope();
      b.RegisterType<ChildWithProperty1>()
        .InstancePerLifetimeScope()
        .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
      using (var c = b.Build())
        Console.WriteLine(c.Resolve<ParentWithConstructor1>().Child.Parent);
    }

    static void Main_(string[] args)
    {
      var b = new ContainerBuilder();
      b.RegisterType<ParentWithProperty>()
        .InstancePerLifetimeScope()
        .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
      b.RegisterType<ChildWithProperty>()
        .InstancePerLifetimeScope()
        .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

      using (var c = b.Build())
        Console.WriteLine(c.Resolve<ParentWithProperty>().Child);
    }
  }
}