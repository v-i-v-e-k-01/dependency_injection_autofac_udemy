using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;


using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Delegate;
using Autofac.Core.Lifetime;
using Autofac.Core.Registration;

using Autofac.Features.ResolveAnything;
using Autofac.Features.Metadata;
using Autofac.Features.AttributeFilters;

using Autofac.Configuration;
using Autofac.Integration.Mef;

using Autofac.Extras.AggregateService;
using Autofac.Extras.AttributeMetadata;
using Autofac.Extras.DynamicProxy;


using Microsoft.Extensions.Configuration;

using Castle.DynamicProxy;




namespace Advanced_Topics
{
    //Register Source Method 1
    public interface ICanSpeak
    {
        void Speak();
    }

    public class Person : ICanSpeak
    {
        public void Speak()
        {
            Console.WriteLine("Hello");
        }
    }


    //Register Source Method 2
    public abstract class BaseHandler
    {
        public virtual string Handle(string message)
        {
            return $"Handled {message}";
        }
    }

    public class HandlerA : BaseHandler
    {
        public override string Handle(string message)
        {
            return $"Handled by A: {message}";
        }
    }
    public class HandlerB : BaseHandler
    {
        public override string Handle(string message)
        {
            return $"Handled by B: {message}";
        }
    }

    public interface IHandlerFactory
    {
        T GetHandler<T>() where T : BaseHandler;
    }

    public class HandlerFactory : IHandlerFactory
    {
        public T GetHandler<T>() where T : BaseHandler
        {
            return Activator.CreateInstance<T>();
        }
    }

    public class ConsumerA
    {
        private HandlerA _handlerA;
        public ConsumerA(HandlerA handlerA)
        {
            _handlerA = handlerA;
        }

        public void DoWork()
        {
            Console.WriteLine(_handlerA.Handle("ConsumerA"));
        }
    }

    public class ConsumerB
    {
        private HandlerB _handlerB;
        public ConsumerB(HandlerB handlerB)
        {
            _handlerB = handlerB;
        }

        public void DoWork()
        {
            Console.WriteLine(_handlerB.Handle("ConsumerB"));
        }
    }

    public class HandlerRegistrationSource : IRegistrationSource
    {
        public bool IsAdapterForIndividualComponents => false;

        public IEnumerable<IComponentRegistration> RegistrationsFor(Service service, Func<Service, IEnumerable<ServiceRegistration>> registrationAccessor)
        {
            var swt = service as IServiceWithType;
            if (swt == null || swt.ServiceType == null || !swt.ServiceType.IsAssignableTo<BaseHandler>())
            {
                yield break;
            }

            yield return new ComponentRegistration
            (
                Guid.NewGuid(),
                new DelegateActivator
                (
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
                new[] { service },
                new ConcurrentDictionary<string, object>()
            );
        }
    }


    //Adapters
    public interface ICommand
    {
        void Execute();
    }

    public class SaveCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Saved File");
        }
    }

    public class OpenCommand : ICommand
    {
        public void Execute()
        {
            Console.WriteLine("Opened File");
        }
    }

    //public class Button
    //{
    //    private ICommand _command;

    //    public Button(ICommand command)
    //    {
    //        _command = command;
    //    }

    //    public void Click()
    //    {
    //        _command.Execute();
    //    }

    //}


    //public class Editor
    //{
    //    IEnumerable<Button> _buttons;
    //    public Editor(IEnumerable<Button> buttons)
    //    {
    //        _buttons = buttons;
    //    }

    //    public void ClickAll()
    //    {
    //        foreach (var button in _buttons)
    //        {
    //            button.Click();
    //        }
    //    }
    //}


    //Adding one more dependency for Adapters -- (here string _name, taken from metadata in Main Program)
    public class Button
    {
        private ICommand _command;
        private string _name;

        public Button(ICommand command, string name)
        {
            _command = command;
            _name = name;
        }

        public void Click()
        {
            _command.Execute();
        }
        public void PrintInfo()
        {
            Console.WriteLine($"Button Name: {_name}");
        }
    }

    public class Editor
    {
        IEnumerable<Button> _buttons;

        public IEnumerable<Button> Buttons() => _buttons;
        public Editor(IEnumerable<Button> buttons)
        {
            _buttons = buttons;
        }

        public void ClickAll()
        {
            foreach (var button in _buttons)
            {
                button.Click();
            }
        }
    }




    //Decorators
    public interface IReportingService
    {
        void Report();
    }

    public class ReportingService : IReportingService
    {
        public void Report()
        {
            Console.WriteLine("Report contents");
        }
    }

    public class ReportingServiceWithLogging : IReportingService
    {
        private IReportingService _decorated;
        public ReportingServiceWithLogging(IReportingService decorated)
        {
            _decorated = decorated;
        }

        public void Report()
        {
            Console.WriteLine("Report started");
            _decorated.Report();
            Console.WriteLine("Report ended");
        }
    }



    //Circular Dependencies

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

    // Circular Dependencies with constructors 

    public class ParentWithConstructor1
    {
        public ChildWithProperty1 _child;

        public ParentWithConstructor1(ChildWithProperty1 child)
        {
            _child = child;
        }

        public override string ToString()
        {
            return "Parent with a ChildWithProperty1";
        }
    }

    public class ChildWithProperty1
    {
        public ParentWithConstructor1 Parent { get; set; }

        public override string ToString()
        {
            return "ChildWithProperty1";
        }
    }



    //Attribute Based Metadata 

    [MetadataAttribute]
    public class AgeMetadataAttribute : Attribute
    {
        public int Age { get; set; }

        public AgeMetadataAttribute(int age)
        {
            Age = age;
        }
    }

    public interface IArtwork
    {
        void Display();
    }

    [AgeMetadata(100)]
    public class CenturyArtWork : IArtwork
    {
        public void Display()
        {
            Console.WriteLine("Displaying a century-old piece");
        }
    }

    [AgeMetadata(1000)]
    public class MillenialArtwork : IArtwork
    {
        public void Display()
        {
            Console.WriteLine("Displaying a really old piece of art");
        }
    }

    public class ArtDisplay
    {
        private IArtwork _artwork;

        public ArtDisplay([MetadataFilter("Age", 100)] IArtwork artwork)
        {
            _artwork = artwork;
        }

        public void Display()
        {
            _artwork.Display();
        }

    }



    //Aggregate Services
    public interface IService1 { }
    public interface IService2 { }
    public interface IService3 { }
    public interface IService4 { }

    public class Class1 : IService1 { }
    public class Class2 : IService2 { }
    public class Class3 : IService3 { }
    public class Class4 : IService4
    {
        public string _name;
        public Class4(string name)
        {
            _name = name;
        }
    }

    public interface IMyAggregateService
    {
        IService1 Service1 { get; }
        IService2 Service2 { get; }
        IService3 Service3 { get; }
        IService4 GetService4(string name);
    }


    public class Consumer
    {
        public IMyAggregateService _allServices;

        public Consumer(IMyAggregateService allServices)
        {
            _allServices = allServices;
        }
    }



    //Type Interceptors
    public class CallLogger : IInterceptor
    {
        private TextWriter _output;
        public CallLogger(TextWriter output)
        {
            _output = output;
        }

        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Method.Name;
            _output.WriteLine("Calling method {0} with arguments {1}",
                methodName,
                string.Join(
                    ",",
                    invocation.Arguments.Select(a => (a ?? "").ToString())
                )
            );

            invocation.Proceed();

            _output.WriteLine("Done calling {0}, result was {1}", methodName, invocation.ReturnValue);
            ;
        }
    }

    public interface IAudit
    {
        int Start(DateTime reportDate);
    }

    [Intercept(typeof(CallLogger))]
    public class Audit : IAudit
    {
        public virtual int Start(DateTime reportDate)
        {
            Console.WriteLine($"Starting report on {reportDate}");
            return 42;
        }
    }



    internal class Program
    {
        static void Main(string[] args)
        {

            //Register Source 



            ////Method 1

            //var builder = new ContainerBuilder();

            //builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource()); // registers unregistered components automatically

            //using (var container = builder.Build())
            //{
            //    container.Resolve<Person>().Speak();
            //}




            //// Method 2

            //var builder = new ContainerBuilder();
            //builder.RegisterType<HandlerFactory>().As<IHandlerFactory>();
            //builder.RegisterSource(new HandlerRegistrationSource());
            //builder.RegisterType<ConsumerA>();
            //builder.RegisterType<ConsumerB>();

            //using (var container = builder.Build())
            //{
            //    container.Resolve<ConsumerA>().DoWork();
            //    container.Resolve<ConsumerB>().DoWork();
            //}





            // Adapters

            //var builder = new ContainerBuilder();
            //builder.RegisterType<SaveCommand>().As<ICommand>();
            //builder.RegisterType<OpenCommand>().As<ICommand>();
            ////builder.RegisterType<Button>(); -- registers button of only one type out of two types -- 1.Button with open command Dependency 2. Button with Save Command dependency -- two different buttons are needed
            //// here we use adapters
            //builder.RegisterAdapter<ICommand, Button>(cmd => new Button(cmd)); // the lambda function creates a new button for each cmd -- i.e for open command and save command

            //builder.RegisterType<Editor>();

            //using (var container = builder.Build())
            //{
            //    var editor = container.Resolve<Editor>(); // editor calls for IEnumerable buttons -- buttons Save Button and Open Button are provided from the Registered components using RegisterAdapter<ICommand, IButton>();
            //    editor.ClickAll();
            //}



            ////Adding one more dependency (metadata here) for adapters
            //var builder = new ContainerBuilder();

            //builder.RegisterType<SaveCommand>().As<ICommand>()
            //    .WithMetadata("Name", "Save");
            //builder.RegisterType<OpenCommand>().As<ICommand>()
            //    .WithMetadata("Name", "Open");

            //builder.RegisterAdapter<Meta<ICommand>, Button>(cmd =>
            //    new Button(cmd.Value,(string)cmd.Metadata["Name"])
            //);

            //builder.RegisterType<Editor>();


            //using (var container = builder.Build())
            //{
            //    var editor = container.Resolve<Editor>();
            //    foreach(var button in editor.Buttons())
            //    {
            //        button.PrintInfo();
            //    }
            //}




            ////Decorators

            //var builder = new ContainerBuilder();
            //builder.RegisterType<ReportingService>().Named<IReportingService>("reporting");

            //builder.RegisterDecorator<IReportingService>(
            //    (context, service) => new ReportingServiceWithLogging(service), "reporting"
            //);

            //using(var container = builder.Build())
            //{
            //    var reportingServiceWithLogging = container.Resolve<IReportingService>();

            //    reportingServiceWithLogging.Report();
            //}





            ////Circular Dependencies


            //var builder = new ContainerBuilder();
            //builder.RegisterType<ParentWithProperty>()
            //    .InstancePerLifetimeScope()
            //    .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            //builder.RegisterType<ChildWithProperty>()
            //    .InstancePerLifetimeScope()
            //    .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            //using (var container = builder.Build())
            //{
            //    var child = container.Resolve<ChildWithProperty>();
            //    var parent = container.Resolve<ParentWithProperty>();

            //    Console.WriteLine(child);
            //    Console.WriteLine(child.Parent);
            //    Console.WriteLine();
            //    Console.WriteLine(parent);
            //    Console.WriteLine(parent.Child);
            //}

            ////Circular Dependencies when component has constructor
            //var builder = new ContainerBuilder();
            //builder.RegisterType<ParentWithConstructor1>().InstancePerLifetimeScope();


            //builder.RegisterType<ChildWithProperty1>()
            //    .InstancePerLifetimeScope()
            //    .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            //using(var container = builder.Build())
            //{
            //    var parentWithConstructor1 = container.Resolve<ParentWithConstructor1>();

            //    Console.WriteLine(parentWithConstructor1._child.Parent);
            //}   



            ////Attribute Based Metadata
            {
                //var builder = new ContainerBuilder();
                //builder.RegisterModule<AttributedMetadataModule>();
                //builder.RegisterType<CenturyArtWork>().As<IArtwork>();
                //builder.RegisterType<MillenialArtwork>().As<IArtwork>();

                //builder.RegisterType<ArtDisplay>().WithAttributeFiltering();

                //using (var container = builder.Build())
                //{
                //    container.Resolve<ArtDisplay>().Display();
                //}
            }

            ////Aggregate Services
            {
                //var builder = new ContainerBuilder();
                //builder.RegisterAggregateService<IMyAggregateService>();
                //builder.RegisterAssemblyTypes(typeof(Program).Assembly)
                //    .Where(t => t.Name.StartsWith("Class"))
                //    .AsImplementedInterfaces();

                //builder.RegisterType<Consumer>();   

                //using(var container = builder.Build())
                //{
                //    var consumer = container.Resolve<Consumer>();

                //    Console.WriteLine(consumer._allServices.Service1.GetType().Name);

                //    Console.WriteLine(consumer._allServices.GetService4("four").GetType().Name);
                //}

            }

            //Type Interceptors
            var builder = new ContainerBuilder();
            builder.Register(c => new CallLogger(Console.Out))
                .As<IInterceptor>()
                .AsSelf();

            builder.RegisterType<Audit>().As<IAudit>()
                .EnableInterfaceInterceptors();

            using (var container = builder.Build())
            {
                var audit = container.Resolve<IAudit>();
                audit.Start(DateTime.Now);
            }
        }
    }
}

