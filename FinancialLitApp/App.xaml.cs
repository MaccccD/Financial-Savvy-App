namespace FinancialLitApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();// initializes the XAML bindings on the xaml file and loads the xaml
							
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}