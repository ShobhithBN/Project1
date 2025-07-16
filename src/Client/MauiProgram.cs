using AIProctoring.Client.Services;
using AIProctoring.Client.ViewModels;
using AIProctoring.Client.Views;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace AIProctoring.Client;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Configure services
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
        builder.Services.AddSingleton<IProctoringService, ProctoringService>();
        builder.Services.AddSingleton<IFaceDetectionService, FaceDetectionService>();
        builder.Services.AddSingleton<IAudioMonitoringService, AudioMonitoringService>();
        builder.Services.AddSingleton<IScreenMonitoringService, ScreenMonitoringService>();
        builder.Services.AddSingleton<IBehaviorAnalysisService, BehaviorAnalysisService>();
        builder.Services.AddSingleton<INotificationService, NotificationService>();

        // Configure ViewModels
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<ExamListViewModel>();
        builder.Services.AddTransient<ExamSetupViewModel>();
        builder.Services.AddTransient<ExamViewModel>();
        builder.Services.AddTransient<CalibrationViewModel>();
        builder.Services.AddTransient<ResultsViewModel>();

        // Configure Views
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ExamListPage>();
        builder.Services.AddTransient<ExamSetupPage>();
        builder.Services.AddTransient<ExamPage>();
        builder.Services.AddTransient<CalibrationPage>();
        builder.Services.AddTransient<ResultsPage>();

        // Configure HTTP client
        builder.Services.AddHttpClient("AIProctoring", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7001/api/");
            client.DefaultRequestHeaders.Add("User-Agent", "AI-Proctoring-Client/1.0");
        });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}