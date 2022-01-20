using Revit.DependencyInjection.Templates.Unity.Commands.SampleWindows.ViewModels;
using Revit.DependencyInjection.Templates.Unity.Commands.SampleWindows.Views;
using Revit.DependencyInjection.Templates.Unity.Interfaces;
using Unity;

namespace Revit.DependencyInjection.Templates.Unity
{
    public static class SamplePipeline
    {
        public static IUnityContainer RegisterSampleServices(this IUnityContainer container)
        {
            container.RegisterType<ISampleSelector, SampleSelector>();
            return container;
        }
        
        public static IUnityContainer RegisterViews(this IUnityContainer container)
        {
            container.RegisterType<SampleWindow>();
            return container;
        }
        
        public static IUnityContainer RegisterViewModels(this IUnityContainer container)
        {
            container.RegisterType<SampleWindowViewModel>();
            return container;
        }
    }
}