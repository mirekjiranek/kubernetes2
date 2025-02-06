using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NorthwindStore.BL;
using NorthwindStore.BL.Mappings;

namespace NorthwindStore.App.Installers
{
    public class AutoMapperInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                Component.For<IMapper>()
                    .UsingFactoryMethod(() => new Mapper(new MapperConfiguration(config =>
                    {
                        config.AddMaps(typeof(BusinessLayer).Assembly);
                    })))
                    .LifestyleSingleton()
                        
            );
        }
    }
}
