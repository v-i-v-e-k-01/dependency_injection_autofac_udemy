using Autofac;
using System;
using System.Collections.Generic;

namespace Controlling_Scope_and_Lifetime
{
    public static class InstanceCount
    {
        public static int _totalInstancesCreatedInProgram { get; set; }

        public static int _instanceRemainingAtEndOfProgram { get; set; }
        public static int _instanceCreatedInScope { get; set; }

    }

    //Instance Scope
    public interface ILog : IDisposable
    {
        void Write(string message);
    }

    public class ConsoleLog : ILog
    {
        public ConsoleLog()
        {
            Console.WriteLine("ConsoleLog object created at: " + DateTime.Now);
            InstanceCount._totalInstancesCreatedInProgram++;
            InstanceCount._instanceRemainingAtEndOfProgram++;
            InstanceCount._instanceCreatedInScope++;
        }
        public void Write(string message)
        {
            Console.WriteLine($"Console Logged: {message} ");
        }

        public void Dispose()
        {
            Console.WriteLine("Console logger disposed");
            InstanceCount._instanceRemainingAtEndOfProgram--;
        }
    }

    public class SMSLog : ILog
    {
        string _phoneNumber;
        public SMSLog(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
        }
        public void Write(string message)
        {
            Console.WriteLine($"SMS to [{_phoneNumber}]: {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("SMSLogger disposed");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////

    public interface IResource : IDisposable
    {

    }

    public class SingletonResource : IResource
    {
        public SingletonResource()
        {
            Console.WriteLine("SingletonResource instance created");
        }

        public void Dispose()
        {
            Console.WriteLine("SingletonResource instance disposed");
        }
    }

    public class InstancePerDependencyResource : IResource
    {
        public InstancePerDependencyResource()
        {
            Console.WriteLine("InstancePerDependency instance created");
            InstanceCount._totalInstancesCreatedInProgram++;
            InstanceCount._instanceRemainingAtEndOfProgram++;
            InstanceCount._instanceCreatedInScope++;
        }

        public void SomeFunction()
        {
            Console.WriteLine("Some functionality of InstancePerDependencyResource");
        }
        public void Dispose()
        {
            Console.WriteLine("InstancePerDependencyResource instance disposed");
            InstanceCount._instanceRemainingAtEndOfProgram--;
        }
    }

    public class ResourceManager : IDisposable
    {
        private IEnumerable<IResource> _resources { get; set; }


        public ResourceManager(IEnumerable<IResource> resources)
        {
            Console.WriteLine("ResourceManager instance created");
            _resources = resources;

        }

        public void Dispose()
        {
            Console.WriteLine("ResourceManager instance disposed");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////
    
    public class Parent
    {
        public override string ToString()
        {
            return "Parent class";
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

    public class BadChild: Child
    {
        public override string ToString()
        {
            return "Its about time that I get my way";
        }
    }

    public class ParentChildModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Parent>();
            builder.Register(c => new Child() { Parent = c.Resolve<Parent>() });
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////

    public class RunBeforeContainerBuilt : IStartable
    {
        public RunBeforeContainerBuilt()
        {
            Console.WriteLine("IStartable class instance created");
        }

        public void Start()
        {
            Console.WriteLine("Container being built");
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();



            //************************* Instance Scope *******************************


            ///////////////////////////////////////////////////////////////////////////////////////////////////


            //// .InstancePerDependency()

            //// -- creates new instace for every resolve<>
            //// instances are disposed at end of program


            //InstanceCount._instanceRemainingAtEndOfProgram = 0;
            //InstanceCount._totalInstancesCreatedInProgram = 0;


            //builder.RegisterType<ConsoleLog>().As<ILog>().InstancePerDependency();

            //IContainer container = builder.Build();
            //InstanceCount._instanceCreatedInScope = 0;
            //using (var scope = container.BeginLifetimeScope())
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("First Scope: ");

            //    for (int i = 0; i < 2; i++)
            //    {
            //        var log = container.Resolve<ILog>();
            //        Console.WriteLine($"Iteration {i + 1} end");
            //    }
            //    Console.WriteLine($"Instances created in scope: {InstanceCount._instanceCreatedInScope}");

            //}

            //var builder2 = new ContainerBuilder();
            //builder2.RegisterType<ConsoleLog>().As<ILog>();
            //var container2 = builder2.Build();
            //InstanceCount._instanceCreatedInScope = 0;
            //using (var scope2 = container2.BeginLifetimeScope())
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("Second Scope: ");

            //    for (int i = 0; i < 2; i++)
            //    {
            //        var log = container.Resolve<ILog>();
            //        Console.WriteLine($"Iteration {i + 1} end");
            //    }
            //    Console.WriteLine($"Instances created in scope: {InstanceCount._instanceCreatedInScope}");

            //}
            //Console.WriteLine();
            //Console.WriteLine($"No. of instances created in the program: {InstanceCount._totalInstancesCreatedInProgram}");
            //Console.WriteLine($"No. of instances remaining until end of program: {InstanceCount._instanceRemainingAtEndOfProgram}");


            //////////////////////////////////////////////////////////////////////////////////////////////////


            //// .SingleInstance()


            //// --creates new instance only once no matter how many times resolve<>, no matter how many times scope created, no matter how many builders/ containers created
            //// once declared .SingleInstance(), always single instance
            //// the instance is disposed at end of program 


            //InstanceCount._totalInstancesCreatedInProgram = 0;
            //InstanceCount._instanceRemainingAtEndOfProgram = 0;


            //builder.RegisterType<ConsoleLog>().As<ILog>().SingleInstance();

            //IContainer container = builder.Build();

            //InstanceCount._instanceCreatedInScope = 0;
            //using (var scope = container.BeginLifetimeScope())
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("First Scope: ");

            //    for (int i = 0; i < 2; i++)
            //    {
            //        var log = container.Resolve<ILog>();
            //        Console.WriteLine($"Iteration {i + 1} end");
            //    }

            //    Console.WriteLine($"Instances created in scope: {InstanceCount._instanceCreatedInScope}");
            //}



            //var builder2 = new ContainerBuilder();
            //builder2.RegisterType<ConsoleLog>().As<ILog>();
            //// -notice that for second scope the previously created instance is reused instead of creating new one

            //IContainer container2 = builder2.Build();
            //InstanceCount._instanceCreatedInScope = 0;
            //using (var scope2 = container2.BeginLifetimeScope())
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("Second Scope: ");

            //    for (int i = 0; i < 2; i++)
            //    {
            //        var log = container.Resolve<ILog>();
            //        Console.WriteLine($"Iteration {i + 1} end");
            //    }
            //    Console.WriteLine($"Instances created in scope: {InstanceCount._instanceCreatedInScope}");
            //}

            //Console.WriteLine();
            //Console.WriteLine($"No. of instances created in the program: {InstanceCount._totalInstancesCreatedInProgram}");
            //Console.WriteLine($"No. of instances remaining at end of program: {InstanceCount._instanceRemainingAtEndOfProgram}");


            ////////////////////////////////////////////////////////////////////////////////////////////////

            ////.InstancePerLifetimeScope()

            //// same as .SingleInstance()

            //InstanceCount._totalInstancesCreatedInProgram = 0;
            //InstanceCount._instanceRemainingAtEndOfProgram = 0;

            //builder.RegisterType<ConsoleLog>().As<ILog>().InstancePerLifetimeScope();
            //IContainer container = builder.Build();

            //InstanceCount._instanceCreatedInScope = 0;

            //using (var scope = container.BeginLifetimeScope())
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("First Scope: ");

            //    for (int i = 0; i < 2; i++)
            //    {
            //        var log = container.Resolve<ILog>();
            //        log.Write("Writing something");
            //        Console.WriteLine($"Iteration {i + 1} end");
            //    }
            //    Console.WriteLine($"No. of instances created in scope1: {InstanceCount._instanceCreatedInScope}");
            //}



            //var builder2 = new ContainerBuilder();
            //builder2.RegisterType<ConsoleLog>().As<ILog>();
            //var container2 = builder2.Build();

            //InstanceCount._instanceCreatedInScope = 0;

            //using (var scope2 = container2.BeginLifetimeScope())
            //{
            //    Console.WriteLine();
            //    Console.WriteLine("Second Scope: ");

            //    for (int i = 0; i < 2; i++)
            //    {
            //        var log = container.Resolve<ILog>();
            //        log.Write("Writing something");
            //        Console.WriteLine($"Iteration {i + 1} end");
            //    }
            //    Console.WriteLine($"No. of instances created in scope2: {InstanceCount._instanceCreatedInScope}");
            //}

            //Console.WriteLine();
            //Console.WriteLine($"No. of instances created in the program: {InstanceCount._totalInstancesCreatedInProgram}");
            //Console.WriteLine($"No. of instances remaining until end of program: {InstanceCount._instanceRemainingAtEndOfProgram}");


            //////////////////////////////////////////////////////////////////////////////////////////////////


            ////.InstancePerMatchingLifetimeScope()


            //// --creates only one instance in matching scope
            //// disposes instance when matching scope ends

            //InstanceCount._totalInstancesCreatedInProgram = 0;
            //InstanceCount._instanceRemainingAtEndOfProgram = 0;


            //builder.RegisterType<ConsoleLog>().As<ILog>().InstancePerMatchingLifetimeScope("foo");

            //var container = builder.Build();
            //InstanceCount._instanceCreatedInScope = 0;
            //using (var scope1 = container.BeginLifetimeScope("foo"))
            //{
            //    for (int i = 0; i < 2; i++)
            //    {
            //        var log = scope1.Resolve<ILog>();
            //        log.Write("Printing Scope1 Console Line");
            //    }

            //    using (var scope2 = scope1.BeginLifetimeScope())
            //    {
            //        for (int i = 0; i < 2; i++)
            //        {
            //            var log = scope2.Resolve<ILog>();
            //            log.Write("Printing Scope2 Console Line");
            //        }

            //    }
            //    Console.WriteLine($"No. of instances created in scope1 & scope2: {InstanceCount._instanceCreatedInScope}");
            //}

            //var builder2 = new ContainerBuilder();
            //builder2.RegisterType<ConsoleLog>().As<ILog>().InstancePerMatchingLifetimeScope("bar");
            //var container2 = builder2.Build();
            //InstanceCount._instanceCreatedInScope = 0;
            //using (var scope3 = container2.BeginLifetimeScope("bar"))
            //{
            //    for (int i = 0; i < 2; i++)
            //    {
            //        var log = scope3.Resolve<ILog>();
            //        log.Write("Printing Scope1 Console Line");
            //    }

            //    using (var scope4 = scope3.BeginLifetimeScope())
            //    {
            //        for (int i = 0; i < 2; i++)
            //        {
            //            var log = scope4.Resolve<ILog>();
            //            log.Write("Printing Scope2 Console Line");
            //        }
            //    }
            //    Console.WriteLine($"No. of instances created in scope3 & scope4: {InstanceCount._instanceCreatedInScope}");
            //}

            //Console.WriteLine();
            //Console.WriteLine($"No. of instances created in the program: {InstanceCount._totalInstancesCreatedInProgram}");
            //Console.WriteLine($"No. of instances remaining until end of program: {InstanceCount._instanceRemainingAtEndOfProgram}");


            //////below region gives error as container is only able to create instance of ConsoleLog per lifetime with tag "foo" only, as we've registerd it that way above
            ////using (var scope3 = container.BeginLifetimeScope())
            ////{
            ////    for (int i = 0; i < 2; i++)
            ////    {
            ////        var log = scope3.Resolve<ILog>();
            ////        log.Write("Printing Scope3 Console Line");
            ////    }
            ////}






            //************************************* Captive Dependencies *************************************


            ////Normal Case

            //InstanceCount._totalInstancesCreatedInProgram = 0;
            //InstanceCount._instanceRemainingAtEndOfProgram = 0;

            //builder.RegisterType<InstancePerDependencyResource>();

            //IContainer container = builder.Build();
            //using (var scope = container.BeginLifetimeScope())
            //{
            //    for (int i = 0; i < 2; i++)
            //    {
            //        var resourceManager = scope.Resolve<InstancePerDependencyResource>();

            //    }
            //}
            //Console.WriteLine("Scope 1 ended");
            //Console.WriteLine();

            //using (var scope = container.BeginLifetimeScope())
            //{
            //    for (int i = 0; i < 2; i++)
            //    {
            //        var resourceManager = scope.Resolve<InstancePerDependencyResource>();

            //    }
            //}
            //Console.WriteLine("Scope 2 ended");
            //Console.WriteLine();
            //Console.WriteLine($"No. of instances of 'InstancePerDependency' created in the program: {InstanceCount._totalInstancesCreatedInProgram}");
            //Console.WriteLine($"No. of instances of 'InstancePerDependency' remaining until end of program: {InstanceCount._instanceRemainingAtEndOfProgram}");



            //////////////////////////////////////////////////////////////////////////////////////////////////


            //// Captive Dependency Case

            //InstanceCount._totalInstancesCreatedInProgram = 0;
            //InstanceCount._instanceRemainingAtEndOfProgram = 0;

            //builder.RegisterType<SingletonResource>().As<IResource>().SingleInstance();
            //builder.RegisterType<InstancePerDependencyResource>().As<IResource>();
            //// instance should dispose at end of scope but stays till end of program -- captive Dependency

            //builder.RegisterType<ResourceManager>().SingleInstance();

            //IContainer container = builder.Build();
            //using (var scope = container.BeginLifetimeScope())
            //{
            //    for (int i = 0; i < 2; i++)
            //    {
            //        var resourceManager = scope.Resolve<ResourceManager>();
            //        // lets say in this scope InstancePerDependencyResource has some functionality requirement in this scope  but not in
            //           next scope
            //    }
            //}
            //Console.WriteLine("Scope 1 ended");
            //Console.WriteLine();

            //using (var scope = container.BeginLifetimeScope())
            //{
            //    for (int i = 0; i < 2; i++)
            //    {
            //        var resourceManager = scope.Resolve<ResourceManager>();
            //    }
            //}
            //Console.WriteLine("Scope 2 ended");
            //Console.WriteLine();

            //Console.WriteLine($"No. of instances of 'InstancePerDependency' created in the program: {InstanceCount._totalInstancesCreatedInProgram}");
            //Console.WriteLine($"No. of instances of 'InstancePerDependency' remaining until end of program: {InstanceCount._instanceRemainingAtEndOfProgram}");

            //// lets say InstancePerDependencyResource was needed in only scope 1 but not in scope 2
            //// but it stays unnecessarily till end of program due to ResourceManager Instance Scope.
            //// this is called Captive Dependency
            ///




            //************************************* Disposal *************************************

            ////Automatic Disposal after scope done
            //InstanceCount._instanceRemainingAtEndOfProgram = 0;

            //builder.RegisterType<ConsoleLog>();
            //var container = builder.Build();

            //using (var scope = container.BeginLifetimeScope())
            //{
            //    var log = scope.Resolve<ConsoleLog>();
            //    log.Write("Using Console Log");
            //}
            //Console.WriteLine("Non disposed component count: " + InstanceCount._instanceRemainingAtEndOfProgram);



            ////Dispose Manually
            //builder.RegisterType<ConsoleLog>().ExternallyOwned();
            //InstanceCount._instanceRemainingAtEndOfProgram = 0;

            //var container = builder.Build();

            //using (var scope = container.BeginLifetimeScope())
            //{
            //    var log = scope.Resolve<ConsoleLog>();
            //    log.Write("Using Console Log");
            //}

            //Console.WriteLine("Non disposed component count: " + InstanceCount._instanceRemainingAtEndOfProgram);




            // ****************************** Lifetime Events ******************************

            //builder.RegisterType<Parent>();
            //builder.RegisterType<Child>()
            //    .OnActivating(a =>
            //    {
            //        Console.WriteLine("Child Activating");
            //        a.Instance.Parent = a.Context.Resolve<Parent>(); // putting in required inputs for variables/properties (here, Parent is declared as Property in Child class
            //    })
            //    .OnActivated(a =>
            //    {
            //        Console.WriteLine("Child Activated");
            //    })
            //    .OnRelease(
            //    a =>
            //    {
            //        Console.WriteLine("Child about to be removed");
            //    });

            //using (var scope = builder.Build().BeginLifetimeScope())
            //{
            //    var child = scope.Resolve<Child>();
            //    var parent = child.Parent;
            //    Console.WriteLine(parent);
            //}




            //Replacing instance with other class instance while activating

            //builder.RegisterType<Parent>();
            //builder.RegisterType<Child>()
            //    .OnActivating(a =>
            //    {
            //        Console.WriteLine("Child Activating");
            //        a.ReplaceInstance(new BadChild());
            //        a.Instance.Parent = a.Context.Resolve<Parent>();
            //    })
            //    .OnActivated(a =>
            //    {
            //        Console.WriteLine("Child Activated");
            //    })
            //    .OnRelease(
            //    a =>
            //    {
            //        Console.WriteLine();
            //        Console.WriteLine("Child about to be removed");
            //    });


            //using (var scope = builder.Build().BeginLifetimeScope())
            //{

            //    var child = scope.Resolve<Child>();
            //    var parent = child.Parent;
            //    Console.WriteLine();
            //    Console.WriteLine(child);
            //    Console.WriteLine(parent);
            //}


            // Below code gives error while replacing instance

            //builder.RegisterType<ConsoleLog>().As<ILog>()
            //    .OnActivating(a =>
            //     {
            //         a.ReplaceInstance(new SMSLog("+123456"));
            //     });

            //using(var scope =builder.Build().BeginLifetimeScope())
            //{
            //    var smsLog = scope.Resolve<ILog>();
            //    smsLog.Write("Testing SMSLog");
            //}

            ////to avoid the same declare ConsoleLog as .AsSelf(), Register ILog and resolve ConsoleLog in it and 

            //builder.RegisterType<ConsoleLog>().AsSelf();
            //builder.Register<ILog>(c => c.Resolve<ConsoleLog>())
            //    .OnActivating(a =>
            //    {
            //        a.ReplaceInstance(new SMSLog("+123456"));
            //    });

            //using (var scope = builder.Build().BeginLifetimeScope())
            //{
            //    var smsLog = scope.Resolve<ILog>();
            //    smsLog.Write("Testing SMSLog");
            //}


            // Running code at startup before container build start
            builder.RegisterType<RunBeforeContainerBuilt>().AsSelf()
                .As<IStartable>()
                .SingleInstance();

            var container = builder.Build();

            container.Resolve<IStartable>();


        }
    }
}
