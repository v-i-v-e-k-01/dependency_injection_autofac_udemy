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
using Autofac.Features.ResolveAnything;
using Microsoft.Extensions.Configuration;

namespace AutofacSamples
{
  public interface IReportingService
  {
    void Report();

  }

  public class ReportingService : IReportingService
  {
    public void Report()
    {
      Console.WriteLine("Here is your report");
    }
  }

  public class ReportingServiceWithLogging : IReportingService
  {
    private IReportingService decorated;

    public ReportingServiceWithLogging(IReportingService decorated)
    {
      this.decorated = decorated;
    }

    public void Report()
    {
      Console.WriteLine("Commencing log...");
      decorated.Report();
      Console.WriteLine("Ending log...");
    }
  }

  public class Program
  {
    static void Main(string[] args)
    {
      var b = new ContainerBuilder();
      b.RegisterType<ReportingService>().Named<IReportingService>("reporting");
      b.RegisterDecorator<IReportingService>(
        (context, service) => new ReportingServiceWithLogging(service), "reporting"
      );


      using (var c = b.Build())
      {
        var r = c.Resolve<IReportingService>();
        r.Report();
      }
    }
  }
}