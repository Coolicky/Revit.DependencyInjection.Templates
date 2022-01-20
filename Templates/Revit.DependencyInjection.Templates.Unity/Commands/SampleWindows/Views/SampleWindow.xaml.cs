using System.Windows;
using Revit.DependencyInjection.Templates.Unity.Commands.SampleWindows.ViewModels;

namespace Revit.DependencyInjection.Templates.Unity.Commands.SampleWindows.Views
{
    public partial class SampleWindow : Window
    {
        public SampleWindowViewModel ViewModel { get; }

        public SampleWindow(SampleWindowViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}