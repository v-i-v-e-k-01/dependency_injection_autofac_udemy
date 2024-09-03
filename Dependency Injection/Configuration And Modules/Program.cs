using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

using Autofac;
using Autofac.Configuration;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Primitives;

using System.IO;

namespace Configuration_And_Modules
{

    //// Using Modules
    //public interface IVehicle
    //{
    //    void Go();
    //}

    //public class Truck : IVehicle
    //{
    //    private readonly IDriver _driver;

    //    public Truck(IDriver driver)
    //    {
    //        _driver = driver;
    //    }
    //    public void Go()
    //    {
    //        _driver.Drive();
    //    }
    //}

    //public interface IDriver
    //{
    //    void Drive();
    //}

    //public class SaneDriver : IDriver
    //{
    //    public void Drive()
    //    {
    //        Console.WriteLine("Driving safely to destination");
    //    }
    //}

    //public class CrazyDriver : IDriver
    //{
    //    public void Drive()
    //    {
    //        Console.WriteLine("Going too fast and crashing into a tree");
    //    }
    //}

    //public class TransportModule : Autofac.Module
    //{
    //    public bool ObeySpeedLimit { get; set; }
    //    protected override void Load(ContainerBuilder builder)
    //    {
    //        if (ObeySpeedLimit)
    //        {
    //            builder.RegisterType<SaneDriver>().As<IDriver>();
    //        }
    //        else
    //        {
    //            builder.RegisterType<CrazyDriver>().As<IDriver>();
    //        }

    //        builder.RegisterType<Truck>().As<IVehicle>();
    //    }
    //}

    ///////////////////////////////////////////////////////////////////////////////////////////////////

    // JSON/XML Config using MS Config.

    public interface IOperation
    {
        float Calculate(float x, float y);
    }

    public class Addition : IOperation
    {
        public float Calculate(float x, float y)
        {
            return x + y;
        }
    }

    public class Multiplication : IOperation
    {
        public float Calculate(float x, float y)
        {
            return x * y;
        }
    }

    public class CalculationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Addition>().As<IOperation>();
            builder.RegisterType<Multiplication>().As<IOperation>();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            ////Using Modules

            //var builder = new ContainerBuilder();
            //builder.RegisterModule(new TransportModule { ObeySpeedLimit = true });

            //using (var c = builder.Build())
            //{
            //    c.Resolve<IVehicle>().Go();
            //}



            // JSON/XML Config using MS Config.

            //Microsoft Extension Configuration
            var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json");
            var configuration = configBuilder.Build();

            var configModule = new ConfigurationModule(configuration);

            //Autofac
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(configModule);

            using (var container = containerBuilder.Build())
            {
                float a = 3, b = 4;
                foreach (IOperation op in container.Resolve<IList<IOperation>>())
                {
                    Console.WriteLine($"{op.GetType().Name} of {a} and {b} = {op.Calculate(a,b)}");
                }
            }
        }
    }
}
