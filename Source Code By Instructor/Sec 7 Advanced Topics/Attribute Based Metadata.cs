using System;
using System.ComponentModel.Composition;
using System.Security.Cryptography.X509Certificates;
using Autofac;
using Autofac.Extras.AttributeMetadata;
using Autofac.Features.AttributeFilters;
using Autofac.Integration.Mef;

namespace AutofacDemos
{
  [MetadataAttribute]
  public class AgeMetadataAttribute : Attribute
  {
    public int Age { get; private set; }

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
  public class CenturyArtwork: IArtwork
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
      Console.WriteLine("Displaying a REALLY old piece of art");
    }
  }

  public class ArtDisplay
  {
    private readonly IArtwork art;

    public ArtDisplay([MetadataFilter("Age", 1000)] IArtwork art)
    {
      this.art = art;
    }

    public void Display() { art.Display(); }
  }

  public class AttributeBasedMetadata
  {
    static void Main_(string[] args)
    {
      var b = new ContainerBuilder();
      b.RegisterModule<AttributedMetadataModule>();
      b.RegisterType<CenturyArtwork>().As<IArtwork>();
      b.RegisterType<MillenialArtwork>().As<IArtwork>();
      b.RegisterType<ArtDisplay>().WithAttributeFiltering();
      using (var c = b.Build())
      {
        c.Resolve<ArtDisplay>().Display();
      }
    }
  }
}