using Autofac;
using System;

namespace Advanced_Registration_Concepts
{
    public interface ILog
    {
        void Write(string message);
    }


    public class ConsoleLog : ILog
    {
        public void Write(string message)
        {
            Console.WriteLine($"Console Logged: {message} ");
        }
    }

    public class EmailLog : ILog
    {
        public void Write(string message)
        {
            Console.WriteLine($"Email sent: {message}");
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////

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
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////

    public class Engine
    {
        private ILog _log;
        private int _id;
        public Engine(ILog log)
        {
            _log = log;
            _id = new Random().Next();
        }

        public Engine(ILog log, int id)
        {
            _log = log;
            _id = id;
        }

        public void Ahead(int power)
        {
            _log.Write($"Engine [{_id}] ahead {power}");
        }
    }
    public class Car
    {
        private Engine _engine;
        private ILog _log;

        public Car(Engine engine)
        {
            _engine = engine;
            _log = new EmailLog();
        }

        public Car(Engine engine, ILog log)
        {
            _engine = engine;
            _log = log;
        }

        public void Go()
        {
            _engine.Ahead(100);
            _log.Write("Car is moving");
        }
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////

    // Delegate Factories
    public class Service
    {
        public string DoSomething(int value)
        {
            return $"I have a value {value}";
        }
    }

    public class DomainObject
    {
        private readonly Service _service;
        private readonly int _value;

        public delegate DomainObject Factory(int value);
        public DomainObject(Service service, int value)
        {
            _service = service;
            _value = value;
        }

        public override string ToString()
        {
            return _service.DoSomething(_value);
        }
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////


    //Objects on demand
    public class Entity
    {
        public delegate Entity Factory(int value);

        private readonly int _number;

        public Entity(int value)
        {
            _number = value;
        }

        public override string ToString()
        {
            return $"Returning the number passed: {_number}";
        }
    }

    public class ViewModel
    {
        private Entity.Factory _entityFactory;
        public ViewModel(Entity.Factory entityFactory)
        {
            _entityFactory = entityFactory;
        }

        public void Method(int value)
        {
            var entity = _entityFactory(value);
            Console.WriteLine(entity);
        }
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////

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

        public void SetParent(Parent parent)
        {
            Parent = parent;
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

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ContainerBuilder();



            // Passing a parameter

            //// named parameter
            //builder.RegisterType<SMSLog>().As<ILog>()
            //    .WithParameter("phoneNumber", "+12345678");

            //// typed parameter
            //builder.RegisterType<SMSLog>().As<ILog>()
            //    .WithParameter(new TypedParameter(typeof(string), "+12345678"));

            //// resolved parameter
            //builder.RegisterType<SMSLog>().As<ILog>()
            //    .WithParameter(
            //    new ResolvedParameter(
            //        //predicate
            //        (pi, ctx) => pi.ParameterType == typeof(string) && pi.Name == "phoneNumber",
            //        //value accessor
            //        (pi, ctx) => "+12345678"
            //        )
            //    );

            //var container = builder.Build();

            //var log = container.Resolve<ILog>();

            //log.Write("Paramenter passed");



            ////delaying parameter passing until container.Resolve
            //Random random = new Random();
            //builder.Register((c, p) => new SMSLog(p.Named<string>("phoneNumber"))).As<ILog>();

            //var container = builder.Build();

            //var log = container.Resolve<ILog>(new NamedParameter("phoneNumber", random.Next().ToString()));

            //log.Write("Paramenter passed");



            //// Delegate Factories
            //builder.RegisterType<Service>();
            //builder.RegisterType<DomainObject>();

            //IContainer container = builder.Build();

            ////// naive method
            //var dobj = container.Resolve<DomainObject>(new PositionalParameter(1, 42));
            //Console.WriteLine(dobj);

            //// using delegates
            //var factory = container.Resolve<DomainObject.Factory>(); // object instance of DomainObject created automatically along with other dependencies required and remaining input parameters can be specified as below
            //var dobj2 = factory(42);
            //Console.WriteLine(dobj2);




            //// Objects on Demand
            //builder.RegisterType<Entity>().InstancePerDependency();
            //builder.RegisterType<ViewModel>();

            //IContainer container = builder.Build();

            //var viewModel = container.Resolve<ViewModel>();

            //viewModel.Method(12);
            //viewModel.Method(14);




            // Property and Method Injection

            ////property injection
            //builder.RegisterType<Parent>();
            ////builder.RegisterType<Child>().PropertiesAutowired();

            ////OR
            //builder.RegisterType<Child>().WithParameter("Parent", new Parent());

            //IContainer container = builder.Build();
            //var child = container.Resolve<Child>();
            //Console.WriteLine(child.Parent.ToString());


            ////method injection
            //builder.RegisterType<Parent>();

            //builder.Register(c =>
            //{
            //    var child = new Child();
            //    child.SetParent(c.Resolve<Parent>());
            //    return child;
            //});

            //var container = builder.Build();

            //var child2 = container.Resolve<Child>();
            //Console.WriteLine(child2.Parent);

            ////OR
            //builder.RegisterType<Parent>();
            //builder.RegisterType<Child>()
            //    .OnActivated((IActivatedEventArgs<Child> e) =>
            //    {
            //        var p = e.Context.Resolve<Parent>();
            //        e.Instance.SetParent(p);
            //    });

            //IContainer container = builder.Build();
            //var child = container.Resolve<Child>();
            //Console.WriteLine(child.Parent);



            //// scanning for types 

            //// instead of registering all dependencies one by one,
            //// we can scan for all dependencies using Assembly.GetExecutingAssembly() and then register all of them using builder.RegisterAssemblyTypes()  

            ////var assembly = Assembly.GetExecutingAssembly();
            ////builder.RegisterAssemblyTypes(assembly);

            ////OR

            //// we can use filter using various techniques as below and take only required dependencies from assembly

            //// get all the types that end with "Log" except SMSLog and <ConsoleLog>().As<ILog>.SingleInstance
            //// every type .AsSelf() as well
            //var assembly = Assembly.GetExecutingAssembly();
            //builder.RegisterAssemblyTypes(assembly)
            //    .Where(t => t.Name.EndsWith("Log"))
            //    .Except<SMSLog>()
            //    .Except<ConsoleLog>(c => c.As<ILog>().SingleInstance())
            //    .AsSelf();

            ////get all the types except SMSLog that end with "Log" and take them as implementation of their first Interface
            //builder.RegisterAssemblyTypes(assembly)
            //    .Except<SMSLog>()
            //    .Where(t => t.Name.EndsWith("Log"))
            //    .As(t => t.GetInterfaces()[0]); 

            //var container = builder.Build();





            //scan for modules

            // define collection of registrations inside Module Classes and scan for them in main program using 
            // the .RegisterAssemblyModules < module_name_here >(typeof(Program).Assembly);

            //builder.RegisterAssemblyModules(typeof(Program).Assembly); // -- for including all modules in the assembly
            builder.RegisterAssemblyModules<ParentChildModule>(typeof(Program).Assembly); // -- for including only ParentChildModule

            var container = builder.Build();

            Console.WriteLine(container.Resolve<Child>().Parent);
        }
    }
}
