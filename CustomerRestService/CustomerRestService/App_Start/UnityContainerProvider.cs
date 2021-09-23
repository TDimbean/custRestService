using Bootstrap;
using Bootstrap.AutoMapper;
using Bootstrap.Unity;
using Unity;

namespace CustomerRestService.App_Start
{
    public class UnityContainerProvider
    {
        public static IUnityContainer CreateContainer()
        {
            Bootstrapper.With.AutoMapper().Unity().UsingAutoRegistration().Start();

            var container = (IUnityContainer)Bootstrapper.Container;

            return container;
        }
    }
}