using FinancialLitApp.ViewModels;
using FinancialLitApp.Views;
using FinancialLitApp.Views.DetailPages;
using FinancialLitApp.Views.Pages;
using FinancialLitApp.Views.Pages.Challenges;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace FinancialLitApp;

public partial class AppShell : Shell
{
	public AppShellViewModel ViewModel { get; }
	public AppShell()
	{
		InitializeComponent(); //loads the XAML Files
		BindingContext = ViewModel; // connecting XAML bindings to view moodel properties


		RegisterRoutes();

	}
	//page life cycle method:

	private void RegisterRoutes()
	{
		//Detailing pages that are not directly in the shell hierarchy:
		Routing.RegisterRoute("accountSetUpPage1", typeof(AccountSetUpPage1));
		Routing.RegisterRoute("login", typeof(Login));
		Routing.RegisterRoute("homepage", typeof(HomePage));
		Routing.RegisterRoute("challengedetail", typeof(ChallengeDetailPage));
		Routing.RegisterRoute("quiz", typeof(QuizPage));
		Routing.RegisterRoute("lessondetail", typeof(LessonDetailPage));


		//nested routes for the specifc challenges that focus on each financial concept:
		Routing.RegisterRoute("challenges/budgeting", typeof(Budgeting));
		Routing.RegisterRoute("challenges/savings", typeof(Savings));



	}
}

public partial class AppShellViewModel : ObservableObject
{
	[RelayCommand]
	async Task NavigateToLogin()
	{
		await Shell.Current.GoToAsync("login");
	}

	[RelayCommand]
	async Task NavigateToHomePage()
	{
		await Shell.Current.GoToAsync("homepage");
	}

	[RelayCommand]
	async Task NavigatetoLessonDetailPage()
	{
		await Shell.Current.GoToAsync("lessondetail");
	}
}
