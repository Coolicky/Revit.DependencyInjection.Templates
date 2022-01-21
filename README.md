
# Revit Dependency Injection Templates

This repository contains a project template for use with [Revit Dependency Injection with Unity](https://github.com/Coolicky/Revit.DependencyInjection). 

The project templates are available on [Nuget](https://www.nuget.org/packages/Revit.DependencyInjection.Templates/)

## Installation

1.  Install the latest  [.Net SDK](https://dotnet.microsoft.com/download)
2.  Run  `dotnet new -i Revit.DependencyInjection.Templates`  to install the project templates

## Usage 
Just run [dotnet new](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-new) in a required folder.
`dotnet new RevitDiUnity --name ProjectName`

## Template
For dependency Injection refer to [Revit Dependency Injection with Unity](https://github.com/Coolicky/Revit.DependencyInjection) repo or [Unity Documentation](http://unitycontainer.org/articles/introduction.html).

The template contains sample `IExternalApplication` Class wrapped in `RevitApp`

### UI Creation
The following method is used to register Tabs, Panels & Buttons
```c#
// This attribute is crucial and should have a unique GUID (created by tamplate by default)
[ContainerProvider("{{GUID}}")]
public class App : RevitApp
{
	public override void OnCreateRibbon(IRibbonManager ribbonManager)
	{
		var br = ribbonManager.GetLineBreak();

		var sampleTab = ribbonManager.CreateTab("TabName");
		var samplePanel = ribbonManager.CreatePanel(sampleTab.GetTabName(), "PanelName");

		samplePanel.AddPushButton<ButtonClass, ButtonAvailability>($"Button{br}Name", "Image");
	}
	[...]
}
```
```
samplePanel.AddPushButton<ButtonClass, ButtonAvailability>($"Button{br}Name", "image");
```
*ButtonClass is a class implementing RevitAppCommand\<App\> where\<App\> is a name of the application class
		
*ButtonAvailability is class implementing IExternalCommandAvailability

*Image is a name of image file used for button icon. You should create two files for each button. One 32x32px & one 16x16px. Place them somewhere in Resources folder. Remember to follow the convention for naming the files "Image32" & "Image16".

### Container Registration
```c#
public class App : RevitApp
{
	[...]
	public override Result OnStartup(IUnityContainer container, UIControlledApplication application)
	{
		// Register Your services here
		container.RegisterType<ISampleService, SampleService>();
		// Or use Extension Methods
		container.RegisterSampleServies();

		// This registers Revit Async Handler (ref. below)
		container.AddRevitAsync(GetAsyncSettings);
	}
	[...]
}
```

### RevitAsync
Registering RevitAsync `container.AddRevitAsync(GetAsyncSettings);` you will register an instance of `IRevitEventHandler` that will allow you to execute methods within Revit Context. 
As the rest of the functions here it's been created by the guys at the excellent [Onbox Framework](https://github.com/engthiago/Onboxframework)

You can inject it into any registered class by 

```c#
public class SampleClass
{
	private readonly IRevitEventHandler _eventHandler;

	public SampleClass(IRevitEventHandler eventHandler)
	{
		_eventHandler = eventHandler;
	}
}
```

Then You can run it by 
```c#
// With return value
public async Task<T> GetSomethingFromRevit
{
	var returnValue = await _eventHandler.RunAsync(uiApp =>
	{
		// Access Revit Application Classes as such
		var doc = uiApp.ActiveUIDocument.Document;

		return something;
	});
}

// As void
public async Task DoSomethingInRevit
{
	await _eventHandler.RunAsync(uiApp =>
	{
		// Access Revit Application Classes as such
		var doc = uiApp.ActiveUIDocument.Document;
		
		// Your Logic Here
	}
}
```

If you prefer different implementation you could use the very popular [Revit.Async](https://github.com/KennanChan/Revit.Async),
Register an instance of [RevitTask](https://github.com/WhiteSharq/RevitTask) where required,
Create Your own implementation of `IExternalEventHandler`
or any other.

### Commands
Commands should inherit from `RevitAppCommand<App>` where \<App\> is the name of the application class.
```c#
//Remember to decorate the class with the Transaction Attribute
[Transaction(TransactionMode.Manual)]
public class SampleWindowCommand : RevitAppCommand<App>
{
	public override Result Execute(IUnityContainer container, ExternalCommandData commandData, ref string message, ElementSet elements)
	{
		// Your Logic Here
	}
}
```

### WPF Views
The template uses [Prism.Core](https://prismlibrary.com/docs/) library to handle MVVM implementation.
You can follow Prism or any other MVVM convention.

Create a standard XAML Window & a ViewModel

in Window class inject ViewModel class
```c#
public partial class SampleWindow : Window
{
	public SampleWindowViewModel ViewModel { get; }

	public SampleWindow(SampleWindowViewModel viewModel)
	{
		ViewModel = viewModel;
		InitializeComponent();
	}
}
```
In XAML you can set data context as such 
`DataContext="{Binding RelativeSource={RelativeSource Self}, Path=ViewModel}"`

Then register both View & ViewModel as transient

```c#
container.RegisterType<SampleWindow>();
container.RegisterType<SampleWindowViewModel>();
```

Opening a windows is as simple as
```c#
var window = container.Resolve<SampleWindow>();
window.Show();
```
## ViewModel
By Default Prism View Model should inherit from `BindableBase`
Inject any Dependencies You may need
```c#
public class SampleWindowViewModel : BindableBase
{
	private readonly ISampleService _sampleService;
	
	public SampleWindowViewModel(ISampleService sampleService)
	{
		_sampleService = sampleService;
	}
```
### Properties 
Properties have `SetProperty` from `BindableBase` implementing `INotifyPropertyChanged`
```c#
// Example Collection property to be bound
private ObservableCollection<Element> _elements;

public ObservableCollection<Element> Elements
{
get => _elements;
set => SetProperty(ref _elements, value);
}

// Remember to Instantiate the collection!
public SampleWindowViewModel(ISampleService sampleService)
{
	[...]
	Elements = new ObservableCollection<Element>();
}

// Example Element property to be bound
private Element _selectedElement;

public Element SelectedElement
{
get => _selectedElement;
set => SetProperty(ref _selectedElement, value);
}
}
```
You can bind them as such
```c#
<ListView ItemsSource="{Binding Elements}"
          SelectedItem="{Binding SelectedElement}">
	<ListView.ItemTemplate>
		<DataTemplate>
			// You can access properties of a bound class in following way
			<TextBlock Text="{Binding Name, Mode=OneWay}"/>
		</DataTemplate>
	</ListView.ItemTemplate>
</ListView>
```
### Commands
Delegate Commands are [Prism Standard](https://prismlibrary.com/docs/commanding.html) for `ICommand` implementation
```c#
// Create Delegate Command and define Function
public DelegateCommand DoSomethingCommand { get; set; }
private async void DoSomething()
{
// Your Logic
}

public SampleWindowViewModel(ISampleService sampleService)
{
	[...]
	// Register Command
	DoSomethingCommand = new DelegateCommand(DoSomething);
}
```
Binding Commands
```c#
<Button Command="{Binding DoSomethingCommand}" />
```

For more advanced documentation refer to [https://prismlibrary.com/docs/](https://prismlibrary.com/docs/) or [Brian Lagunas Pluralsight course](https://www.pluralsight.com/courses/prism-wpf-introduction) that goes way beyond the scope here.

### TODO: 
Create a way to implement Prism Application within this template and use Revit Dockpanel as Prism Shell. 
If someone knows how to approach it please drop a comment.
