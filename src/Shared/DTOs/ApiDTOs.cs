using System.ComponentModel.DataAnnotations;

namespace AIProctoring.Shared.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserProfile User { get; set; } = new();
}

public class UserProfile
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public List<string> Roles { get; set; } = new();
    public string Institution { get; set; } = string.Empty;
}

public class StartExamRequest
{
    [Required]
    public Guid ExamId { get; set; }

    [Required]
    public string StudentId { get; set; } = string.Empty;

    [Required]
    public SystemInfoDTO SystemInfo { get; set; } = new();

    public CalibrationDataDTO? CalibrationData { get; set; }
}

public class StartExamResponse
{
    public Guid SessionId { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public ExamDTO Exam { get; set; } = new();
    public DateTime ServerTime { get; set; }
    public List<string> RequiredPermissions { get; set; } = new();
}

public class SystemInfoDTO
{
    public string OperatingSystem { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string ScreenResolution { get; set; } = string.Empty;
    public int AvailableMonitors { get; set; }
    public string DeviceFingerprint { get; set; } = string.Empty;
}

public class CalibrationDataDTO
{
    public FacePositionDTO OptimalFacePosition { get; set; } = new();
    public double BaselineEyeGaze { get; set; }
    public double BackgroundNoiseLevel { get; set; }
    public double OptimalLighting { get; set; }
    public string CalibrationNotes { get; set; } = string.Empty;
}

public class FacePositionDTO
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}

public class ExamDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public ExamSettingsDTO Settings { get; set; } = new();
    public List<QuestionDTO> Questions { get; set; } = new();
}

public class ExamSettingsDTO
{
    public bool AllowMultipleMonitors { get; set; }
    public bool RequireFaceDetection { get; set; }
    public bool RequireAudioMonitoring { get; set; }
    public bool AllowTabSwitching { get; set; }
    public bool RequireFullScreen { get; set; }
    public double ViolationThreshold { get; set; }
    public bool EnableLivenessDetection { get; set; }
    public bool EnableBehaviorAnalysis { get; set; }
    public int MaxIdleTimeMinutes { get; set; }
}

public class QuestionDTO
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new();
    public int Points { get; set; }
    public int OrderIndex { get; set; }
}

public class SubmitAnswerRequest
{
    [Required]
    public Guid SessionId { get; set; }

    [Required]
    public Guid QuestionId { get; set; }

    [Required]
    public string Answer { get; set; } = string.Empty;
}

public class MonitoringDataDTO
{
    public Guid SessionId { get; set; }
    public DateTime Timestamp { get; set; }
    public FaceDetectionDataDTO? FaceData { get; set; }
    public AudioDataDTO? AudioData { get; set; }
    public ScreenDataDTO? ScreenData { get; set; }
    public BehaviorDataDTO? BehaviorData { get; set; }
    public EnvironmentDataDTO? EnvironmentData { get; set; }
}

public class FaceDetectionDataDTO
{
    public bool FaceDetected { get; set; }
    public int FaceCount { get; set; }
    public double Confidence { get; set; }
    public FacePositionDTO Position { get; set; } = new();
    public bool IsLookingAtScreen { get; set; }
    public double EyeGazeConfidence { get; set; }
    public bool LivenessDetected { get; set; }
    public EmotionDataDTO? Emotions { get; set; }
    public string? FaceEncodingHash { get; set; }
}

public class EmotionDataDTO
{
    public double Anger { get; set; }
    public double Disgust { get; set; }
    public double Fear { get; set; }
    public double Happiness { get; set; }
    public double Sadness { get; set; }
    public double Surprise { get; set; }
    public double Neutral { get; set; }
}

public class AudioDataDTO
{
    public double VoiceLevel { get; set; }
    public double BackgroundNoise { get; set; }
    public int SpeakerCount { get; set; }
    public bool VoiceDetected { get; set; }
    public bool MultipleVoicesDetected { get; set; }
    public double Confidence { get; set; }
    public List<string> DetectedWords { get; set; } = new();
}

public class ScreenDataDTO
{
    public bool IsFullScreen { get; set; }
    public bool TabSwitchDetected { get; set; }
    public bool WindowChangeDetected { get; set; }
    public bool ScreenshotAttempted { get; set; }
    public bool PrintScreenDetected { get; set; }
    public bool VirtualMachineDetected { get; set; }
    public bool RemoteDesktopDetected { get; set; }
    public List<string> SuspiciousApplications { get; set; } = new();
}

public class BehaviorDataDTO
{
    public int KeystrokeCount { get; set; }
    public int MouseClickCount { get; set; }
    public int MouseMoveDistance { get; set; }
    public int IdleTimeSeconds { get; set; }
    public double TypingSpeed { get; set; }
    public bool CopyPasteDetected { get; set; }
    public bool SuspiciousPattern { get; set; }
    public double FocusScore { get; set; }
}

public class EnvironmentDataDTO
{
    public int BackgroundPersonCount { get; set; }
    public bool ObjectsOnDesk { get; set; }
    public double LightingQuality { get; set; }
    public bool PhoneDetected { get; set; }
    public bool BookDetected { get; set; }
    public bool PaperDetected { get; set; }
    public string RoomDescription { get; set; } = string.Empty;
}

public class ViolationEventDTO
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public double Confidence { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
}

public class ProctorAlertDTO
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Priority { get; set; } = string.Empty;
    public bool IsAcknowledged { get; set; }
}

public class ExamResultDTO
{
    public Guid SessionId { get; set; }
    public Guid ExamId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int TotalPoints { get; set; }
    public int PointsEarned { get; set; }
    public double PercentageScore { get; set; }
    public string Grade { get; set; } = string.Empty;
    public int ViolationCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<ViolationEventDTO> Violations { get; set; } = new();
}

public class EndExamRequest
{
    [Required]
    public Guid SessionId { get; set; }

    public string Reason { get; set; } = string.Empty;
    public bool IsVoluntary { get; set; } = true;
}