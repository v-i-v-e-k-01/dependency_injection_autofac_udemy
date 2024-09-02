using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using Autofac.Features.Indexed;
using Autofac.Features.Metadata;
using Autofac.Features.OwnedInstances;


namespace Implicit_Relationship_Types
{
    public interface ILog : IDisposable
    {
        void Write(string message);
    }

    public class ConsoleLog : ILog
    {
        public ConsoleLog()
        {
            Console.WriteLine("ConsoleLog object created at: "+ DateTime.Now);
        }
        public void Write(string message)
        {
            Console.WriteLine($"Console Logged: {message} ");
        }

        public void Dispose()
        {
            Console.WriteLine("Console logger no longer required");
        }
    }

    /////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SMSLog : ILog
    {
        string _phoneNumber;
        public SMSLog(string phoneNumber)
        {
            Console.WriteLine("SMSLog object created at " + DateTime.Now);
            _phoneNumber = phoneNumber;
        }
        public void Write(string message)
        {
            Console.WriteLine($"SMS to [{_phoneNumber}]: {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("SMSLog is no longer needed");
        }
    }


    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////






    ////Delayed Instantiation
    //public class Reporting
    //{
    //    private Lazy<ConsoleLog> _log;
    //    // using Lazy<ConsoleLog> instantiates the dependency at a later time
    //    // which is equivalent to Lambda function at resolve time : new Lazy<ConsoleLog>(() => new ConsoleLog());

    //    public Reporting(Lazy<ConsoleLog> log)
    //    {
    //        if (log == null)
    //        {
    //            throw new ArgumentNullException(paramName: nameof(log));
    //        }
    //        _log = log;
    //        Console.WriteLine("Reporting component created");
    //    }

    //    public void Report()
    //    {
    //        Console.WriteLine("Calling the dependency object ConsoleLog");
    //        _log.Value.Write("Log started");
    //    }
    //}


    //// Controlled instantiation
    //public class Reporting
    //{
    //    private Owned<ConsoleLog> _log;
    //    public Reporting(Owned<ConsoleLog> log)
    //    {
    //        if (log == null)
    //        {
    //            throw new ArgumentNullException(paramName: nameof(log));
    //        }
    //        _log = log;
    //        Console.WriteLine("Reporting object created");
    //    }

    //    public void ReportOnce()
    //    {
    //        Console.WriteLine("Calling the dependency object ConsoleLog");
    //        _log.Value.Write("Log started");
    //        _log.Dispose(); // disposing the dependency -- Dispose() comes from Owned<T> 
    //    }
    //}





    ////Dynamic instantiation(ConsoleLog) and Parameterized Instantiation(SMSLog)
    //public class Reporting
    //{
    //    Func<ConsoleLog> _consoleLog;
    //    Func<string, SMSLog> _smsLog;
    //    public Reporting(Func<ConsoleLog> consoleLog, Func<string, SMSLog> smsLog)
    //    {
    //        _consoleLog = consoleLog;
    //        _smsLog = smsLog;
    //    }

    //    public void Report()
    //    {
    //        Console.WriteLine("Reporting started");
    //        _consoleLog().Write("Reporting to console");
    //        _consoleLog().Write("And again");
    //        // two different ConsoleLog objects are created for above two commands dynamically and runtime

    //        _smsLog("+123456").Write("This is SMS.");
    //        // Parameter "+123456" is passed to _smsLog dependency -- parameterized instantiation
    //    }
    //}



    //// Enumeration
    //public class Reporting
    //{
    //    private IList<ILog> _allLogs;
    //    public Reporting(IList<ILog> allLogs)
    //    {
    //        _allLogs = allLogs;
    //    }

    //    public void Report()
    //    {
    //        foreach (ILog log in _allLogs)
    //        {
    //            log.Write($"Hello this is {log.GetType().Name}");
    //        }
    //    }
    //}




    ////Metadata Interrogation
    //public class Settings
    //{
    //    public string LogMode { get; set; }
    //    //uncomment to get error
    //    //public string SomethingElse {get; set; }
    //}

    //public class Reporting
    //{
    //    //private Meta<ConsoleLog> _log;
    //    private Meta<ConsoleLog, Settings> _log;
    //    public Reporting(Meta<ConsoleLog, Settings> log)
    //    {
    //        if(log == null)
    //        {
    //            throw new ArgumentNullException(paramName: nameof(log));
    //        }
    //        _log = log;
    //    }

    //    public void Report()
    //    {
    //        _log.Value.Write("ConsoleLogged here");
    //        //if (_log.Metadata["mode"] as string == "verbose")
    //        if(_log.Metadata.LogMode == "verbose")
    //        {
    //            _log.Value.Write($"Verbose Mode: Logger started at {DateTime.Now}");
    //        }
    //    }

    //}



    //Keyed Service Lookup
    public class Reporting
    {
        IIndex<string, ILog> _logs;

        public Reporting(IIndex<string, ILog> logs)
        {
            if (logs == null)
            {
                throw new ArgumentNullException(paramName: nameof(logs));
            }

            _logs = logs;
        }

        public void Report()
        {
            _logs["cmd"].Write("Starting report Output Console");
            _logs["sms"].Write("Starting report Output SMS");
        }
    }


    internal class Program
    {    
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            ////Delayed Instantiation
            //builder.RegisterType<ConsoleLog>();
            //builder.RegisterType<Reporting>();
            //using (var container = builder.Build())
            //{
            //    container.Resolve<Reporting>().Report();
            //}


            //// Controlled Instantiation
            //builder.RegisterType<ConsoleLog>();
            //builder.RegisterType<Reporting>();
            //using (var container = builder.Build())
            //{
            //    container.Resolve<Reporting>().ReportOnce();
            //    Console.WriteLine("Done reporting");
            //}


            ////Dynamic Instantiation and Parameterized Instantiation
            //builder.RegisterType<ConsoleLog>();
            //builder.RegisterType<SMSLog>();
            //builder.RegisterType<Reporting>();
            //using(var container  = builder.Build())
            //{
            //    container.Resolve<Reporting>().Report();
            //}


            ////Enumeration
            //builder.RegisterType<ConsoleLog>().As<ILog>();
            ////builder.Register((c,p)=> new SMSLog(p.Named<string>("phoneNumber"))).As<ILog>();
            //builder.Register(c => new SMSLog("+456789")).As<ILog>();
            //builder.RegisterType<Reporting>();

            //using(var container = builder.Build())
            //{
            //    //container.Resolve<ILog>(new NamedParameter("phoneNumber", "+456789"));
            //    container.Resolve<Reporting>().Report();
            //}


            ////Metadata Interrogation
            ////builder.RegisterType<ConsoleLog>().WithMetadata("mode", "verbose"); // -- weakly typed
            //builder.RegisterType<ConsoleLog>()
            //    .WithMetadata<Settings>(c => c.For(x => x.LogMode, "verbose")); // -- strongly typed
            //builder.RegisterType<Reporting>();
            //using(var container  = builder.Build())
            //{
            //    container.Resolve<Reporting>().Report();
            //}

            builder.RegisterType<ConsoleLog>().Keyed<ILog>("cmd");
            builder.Register(c => new SMSLog("+456789")).Keyed<ILog>("sms");
            builder.RegisterType<Reporting>();
            using (var container = builder.Build())
            {
                container.Resolve<Reporting>().Report();
            }


        }
    }
}
