using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core.Activators.Reflection;
using Autofac.Features.Indexed;
using Autofac.Features.Metadata;
using Autofac.Features.OwnedInstances;

namespace AutofacDemos
{
  public class Reporting
  {
    private readonly Lazy<ConsoleLog> log;

    public Reporting(Lazy<ConsoleLog> log)
    {
      this.log = log;
      Console.WriteLine("Reporting component created");
    }

    public void Report()
    {
      log.Value.Write($"Log started");
    }
  }

  public class Reporting2
  {
    private Owned<ConsoleLog> log;

    public Reporting2(Owned<ConsoleLog> log)
    {
      this.log = log;
    }

    public void ReportOnce()
    {
      log.Value.Write("Log started");
      log.Dispose();
    }
  }

  public class Reporting3
  {
    private Func<ConsoleLog> consoleLog;
    private Func<string, SmsLog> smsLog; // 2

    public Reporting3(Func<ConsoleLog> consoleLog, Func<string, SmsLog> smsLog)
    {
      this.consoleLog = consoleLog;
      this.smsLog = smsLog; // 2
    }

    public void Report()
    {
      consoleLog().Write("Reporting to console");
      consoleLog().Write("And again");

      smsLog("+1234567").Write("Texting admins...");
    }
  }

  public class Reporting4
  {
    private readonly IList<ILog> allLogs;

    public Reporting4(IList<ILog> allLogs)
    {
      this.allLogs = allLogs;
    }

    public void Report()
    {
      foreach (var log in allLogs)
      {
        log.Write($"Hello, this is {log.GetType().Name}");
      }
    }
  }

  public class Settings
  {
    public string LogMode { get; set; }

    // uncomment to get an error
    //public string SomethingElse { get; set; }
  }

  public class Reporting5
  {
    private Meta<ConsoleLog,Settings> log;

    public Reporting5(Meta<ConsoleLog,Settings> log)
    {
      this.log = log;
    }

    public void Report()
    {
      log.Value.Write("Starting report");
      //if (log.Metadata["mode"] as string == "verbose")
      if (log.Metadata.LogMode == "verbose")
        log.Value.Write($"VERBOSE MODE: Logger started on {DateTime.Now}");
    }
  }

  public class Reporting6
  {
    public IIndex<string, ILog> logs;

    public Reporting6(IIndex<string, ILog> logs)
    {
      this.logs = logs;
    }

    public void Report()
    {
      logs["sms"].Write("Starting report output");
    }
  }

  public class ImplicitRelationships
  {
    // keyed service lookup
    static void KeyedServiceLookup(string[] args)
    {
      var builder = new ContainerBuilder();
      builder.RegisterType<ConsoleLog>().Keyed<ILog>("cmd");
      builder.Register(c => new SmsLog("+1234567")).Keyed<ILog>("sms");
      builder.RegisterType<Reporting6>();
      using (var c = builder.Build())
      {
        c.Resolve<Reporting6>().Report();
      }
    }

    static void Metadata(string[] args)
    {
      var builder = new ContainerBuilder();
      //builder.RegisterType<ConsoleLog>().WithMetadata("mode", "verbose");
      builder.RegisterType<ConsoleLog>()
        .WithMetadata<Settings>(c => c.For(x => x.LogMode, "verbose")); // insane
      builder.RegisterType<Reporting5>();
      using (var c = builder.Build())
      {
        c.Resolve<Reporting5>().Report();
      }
    }

    // enumeration
    static void Enumeration(string[] args)
    {
      var builder = new ContainerBuilder();
      builder.RegisterType<ConsoleLog>().As<ILog>();
      builder.Register(c => new SmsLog("+1234567")).As<ILog>();
      builder.RegisterType<Reporting4>();
      using (var c = builder.Build())
      {
        c.Resolve<Reporting4>().Report();
      }
    }

    // dynamic instantiation and parameterized instantiation
    static void DynamicAndParameterizedInstantiation(string[] args)
    {
      var builder = new ContainerBuilder();
      builder.RegisterType<ConsoleLog>();
      builder.RegisterType<SmsLog>();
      builder.RegisterType<Reporting3>();
      using (var c = builder.Build())
      {
        c.Resolve<Reporting3>().Report();
      }
    }

    static void ControlledInstantiation(string[] args)
    {
      var builder = new ContainerBuilder();
      builder.RegisterType<ConsoleLog>();
      builder.RegisterType<Reporting2>();

      using (var container = builder.Build())
      {
        container.Resolve<Reporting2>().ReportOnce();
        Console.WriteLine("Done reporting");
      }

    }

    static void DelayedInstantiation(string[] args)
    {
      var builder = new ContainerBuilder();
      builder.RegisterType<ConsoleLog>();
      builder.RegisterType<Reporting>();

      using (var container = builder.Build())
      {
        container.Resolve<Reporting>().Report();
      }

    }
  }
}