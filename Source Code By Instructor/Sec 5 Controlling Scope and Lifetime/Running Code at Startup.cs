﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Module = Autofac.Module;

namespace AutofacSamples
{
  public interface ILog
  {
    void Write(string message);
  }

  public interface IConsole
  {

  }

  public class ConsoleLog : ILog, IConsole, IDisposable
  {
    public void Dispose()
    {
      Console.WriteLine("Console log no longer needed");
    }

    public ConsoleLog()
    {
      Console.WriteLine("Creating a console log!");
    }

    public void Write(string message)
    {
      Console.WriteLine(message);
    }
  }

  public class EmailLog : ILog
  {
    private const string adminEmail = "admin@foo.com";

    public void Write(string message)
    {
      Console.WriteLine($"Email sent to {adminEmail} : {message}");
    }
  }

  public class Engine
  {
    private ILog log;
    private int id;

    public Engine(ILog log)
    {
      this.log = log;
      id = new Random().Next();
    }

    public Engine(ILog log, int id)
    {
      this.log = log;
      this.id = id;
    }

    public void Ahead(int power)
    {
      log.Write($"Engine [{id}] ahead {power}");
    }
  }

  public class SMSLog : ILog
  {
    private string phoneNumber;

    public SMSLog(string phoneNumber)
    {
      this.phoneNumber = phoneNumber;
    }

    public void Write(string message)
    {
      Console.WriteLine($"SMS to {phoneNumber} : {message}");
    }
  }

  public class Car
  {
    private Engine engine;
    private ILog log;

    public Car(Engine engine)
    {
      this.engine = engine;
      this.log = new EmailLog();
    }

    public Car(Engine engine, ILog log)
    {
      this.engine = engine;
      this.log = log;
    }

    public void Go()
    {
      engine.Ahead(100);
      log.Write("Car going forward...");
    }
  }

  public class Parent
  {
    public override string ToString()
    {
      return "I am your father";
    }
  }

  public class Child
  {
    public string Name { get; set; }
    public Parent Parent { get; set; }


    public Child()
    {
      Console.WriteLine("Child being created");
    }

    public void SetParent(Parent parent)
    {
      Parent = parent;
    }

    public override string ToString()
    {
      return "Hi there";
    }
  }

  class BadChild : Child
  {
    public override string ToString()
    {
      return "I hate you";
    }
  }

  public class ParentChildModule : Module
  {
    protected override void Load(ContainerBuilder builder)
    {
      builder.RegisterType<Parent>();
      builder.Register(c => new Child() {Parent = c.Resolve<Parent>()});
    }
  }

  public class MyClass : IStartable
  {
    public MyClass()
    {
      Console.WriteLine("MyClass ctor");
    }

    public void Start()
    {
      Console.WriteLine("Container being built");
    }
  }

  internal class Program
  {
    public static void Main(string[] args)
    {
      var builder = new ContainerBuilder();
      builder.RegisterType<MyClass>()
        .AsSelf()
        .As<IStartable>()
        .SingleInstance();
      var container = builder.Build();
      container.Resolve<MyClass>();
    }
  }
}