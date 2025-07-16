using AIProctoring.Shared.DTOs;
using AIProctoring.Shared.Models;

namespace AIProctoring.Client.Services;

public interface IProctoringService
{
    Task StartMonitoringAsync(Guid sessionId, ExamSettingsDTO settings);
    Task StopMonitoringAsync();
    Task<CalibrationDataDTO> CalibrateAsync();
    bool IsMonitoring { get; }
    event EventHandler<ViolationEventDTO> ViolationDetected;
    event EventHandler<MonitoringDataDTO> MonitoringDataCaptured;
}

public interface IFaceDetectionService
{
    Task<FaceDetectionDataDTO?> DetectFaceAsync(byte[] imageData);
    Task<bool> VerifyIdentityAsync(byte[] imageData, string referenceHash);
    Task<string> GenerateFaceHashAsync(byte[] imageData);
    Task InitializeAsync();
    bool IsInitialized { get; }
}

public interface IAudioMonitoringService
{
    Task StartMonitoringAsync();
    Task StopMonitoringAsync();
    AudioDataDTO? GetCurrentAudioData();
    bool IsMonitoring { get; }
    event EventHandler<AudioDataDTO> AudioDataCaptured;
}

public interface IScreenMonitoringService
{
    Task StartMonitoringAsync();
    Task StopMonitoringAsync();
    ScreenDataDTO GetCurrentScreenData();
    bool IsMonitoring { get; }
    event EventHandler<ScreenDataDTO> ScreenViolationDetected;
}

public interface IBehaviorAnalysisService
{
    Task StartMonitoringAsync();
    Task StopMonitoringAsync();
    BehaviorDataDTO GetCurrentBehaviorData();
    void RecordKeyStroke();
    void RecordMouseClick();
    void RecordMouseMove(int distance);
    void RecordCopyPaste();
    bool IsMonitoring { get; }
}

public interface INotificationService
{
    Task ShowWarningAsync(string message);
    Task ShowErrorAsync(string message);
    Task ShowInfoAsync(string message);
    Task<bool> ShowConfirmationAsync(string title, string message);
}