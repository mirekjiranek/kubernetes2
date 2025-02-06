using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NorthwindStore.BL;
using NorthwindStore.BL.Services;
using Riganti.Utils.Infrastructure.Services.Facades;

namespace NorthwindStore.App.Installers
{
    public class FacadeInstaller : IWindsorInstaller
    {
        private readonly string applicationPath;

        public FacadeInstaller(string applicationPath)
        {
            this.applicationPath = applicationPath;
        }

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(

                Classes.FromAssemblyContaining<BusinessLayer>()
                    .BasedOn<FacadeBase>()
                    .LifestyleTransient(),

                Component.For<ImageService>()
                    .UsingFactoryMethod(() => new ImageService(applicationPath))
                    .LifestyleSingleton()

            );
        }
    }
}