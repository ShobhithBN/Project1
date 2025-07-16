using AIProctoring.Client.Views;

namespace AIProctoring.Client;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        RegisterRoutes();
    }

    private void RegisterRoutes()
    {
        Routing.RegisterRoute("examlist", typeof(ExamListPage));
        Routing.RegisterRoute("examsetup", typeof(ExamSetupPage));
        Routing.RegisterRoute("calibration", typeof(CalibrationPage));
        Routing.RegisterRoute("exam", typeof(ExamPage));
        Routing.RegisterRoute("results", typeof(ResultsPage));
    }
}