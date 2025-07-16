using AIProctoring.Client.Views;

namespace AIProctoring.Client;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Set the main page to the login page initially
        MainPage = new AppShell();
    }

    protected override void OnStart()
    {
        base.OnStart();
        // Handle when your app starts
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        // Handle when your app sleeps
    }

    protected override void OnResume()
    {
        base.OnResume();
        // Handle when your app resumes
    }
}