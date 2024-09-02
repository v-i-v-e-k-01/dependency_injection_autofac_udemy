using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;

namespace Registration_Concepts
{
    //// Scenario (Without DI)
    //public interface ILog
    //{
    //    void Write(string message);
    //}

    //public class ConsoleLog: ILog
    //{
    //    public void Write(string message)
    //    {
    //        Console.WriteLine(message);
    //    }
    //}

    //public class Engine
    //{
    //    private ILog _log;
    //    private int _id;
    //    public Engine(ILog log)
    //    {
    //        _log = log;
    //        _id = new Random().Next();
    //    }

    //    public void Ahead(int power)
    //    {
    //        _log.Write($"Engine [{_id}] ahead {power}");
    //    }
    //}
    //public class Car
    //{
    //    private Engine _engine;
    //    private ILog _log;

    //    public Car(Engine engine, ILog log)
    //    {
    //        _engine = engine;
    //        _log = log;
    //    }

    //    public void Go()
    //    {
    //        _engine.Ahead(100);
    //    }
    //}
    //public class Program
    //{
    //    public static void Main(string[] args)
    //    {
    //        var log = new ConsoleLog();
    //        var engine = new Engine(log);
    //        var car =  new Car(engine, log);

    //        car.Go();
    //    }
    //}
    



    // Registering Types (Reflection Components)
    public interface ILog
    {
        void Write(string message);
    }


    //this interface is used at last
    public interface IConsole
    {
        void Write(string message);
    }

    public class ConsoleLog : ILog, IConsole
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

    public class Engine
    {
        private ILog _log;
        private int _id;
        public Engine(ILog log)
        {
            _log = log;
            _id = new Random().Next();
        }

        // below constructor used at line 230
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
    public class Program
    {
        public static void Main(string[] args)
        {
            ////Steps to define and Initialise container

            ////1. creating a builder for container
            //var builder = new ContainerBuilder();

            ////2. Register components in container
            //builder.RegisterType<ConsoleLog>().As<ILog>(); // -- Whenever some class asks for instance of ILog -- return a ConsoleLog
            //builder.RegisterType<Engine>();
            //builder.RegisterType<Car>();

            ////3. build the container
            //IContainer container = builder.Build();

            ////4. initiate car class with all dependencies in container (last step)
            //var car = container.Resolve<Car>(); 

            //car.Go();




            //// to make the ConsoleLog instance available as itself on request along with availability as instance of Interface ILog
            //var builder = new ContainerBuilder();
            //builder.RegisterType<ConsoleLog>().As<ILog>().AsSelf(); 
            //IContainer container = builder.Build();

            //var consoleLogger = container.Resolve<ConsoleLog>();




            //// Catering to requests for interface components whose implementation is a single class 
            //var builder = new ContainerBuilder();
            //builder.RegisterType<ConsoleLog>().As<ILog>().As<IConsole>(); // -- ConsoleLog is implementation of both ILog and IConsole so whenever either of ILog or IConsole is requested, ConsoleLog object will be sent

            //builder.RegisterType<Engine>();
            //builder.RegisterType<Car>();

            //IContainer container = builder.Build();

            //var car = container.Resolve<Car>();

            //car.Go();


            //// When there are more than one constructors in a class, by default, the container chooses largest one. We can specify which constructor to use explicitly using -- .UsingConstructor(typeOf( input_type_here ), typeOf( input_type_here ) ....) -- this will include the constructor with given types inside the brackets
            //var builder = new ContainerBuilder();
            //builder.RegisterType<ConsoleLog>().As<ILog>();
            //builder.RegisterType<Engine>();
            //builder.RegisterType<Car>().UsingConstructor(typeof(Engine));

            //IContainer container = builder.Build();

            //var car = container.Resolve<Car>();

            //car.Go(); // - will email as we're using the constructor in car class which initialises email log




            //// When we need a component again and again for different containers we create an instance first and then assign this instance to the container builder of the required containers  (with .RegisterInstance(instance_name))

            ////creating instance
            //var log = new ConsoleLog();

            ////using above instance for Car container builder
            //var builder = new ContainerBuilder();
            //builder.RegisterInstance(log).As<ILog>();
            //builder.RegisterType<Engine>();
            //builder.RegisterType<Car>();

            //IContainer container = builder.Build();

            //var car = container.Resolve<Car>();

            //car.Go();





            //// Explicitly initialising a dependency which also has other dependencies which are in builder
            //var builder = new ContainerBuilder();
            //builder.RegisterType<ConsoleLog>().As<ILog>();

            //builder.Register((IComponentContext c) => new Engine(c.Resolve<ILog>(), 123)); // here Engine is explicitly initialised with dependency including a container component ILog (using c.Resolve<ILog>())

            //builder.RegisterType<Car>();

            //IContainer container = builder.Build();

            //var car = container.Resolve<Car>();

            //car.Go();




            // using generic components
            var builder = new ContainerBuilder();

            //IList<T> -- List<T>
            //IList<int> -- List<int>
            builder.RegisterGeneric(typeof(List<>)).As(typeof(IList<>));
            IContainer container = builder.Build();

            var myList = container.Resolve<IList<int>>();

            Console.WriteLine(myList.GetType());

        }
    }

}
