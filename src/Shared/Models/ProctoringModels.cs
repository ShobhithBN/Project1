namespace AIProctoring.Shared.Models;

public class ViolationEvent
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public ViolationType Type { get; set; }
    public DateTime Timestamp { get; set; }
    public double Confidence { get; set; }
    public string Description { get; set; } = string.Empty;
    public ViolationSeverity Severity { get; set; }
    public string Evidence { get; set; } = string.Empty; // Base64 image or JSON data
    public bool IsResolved { get; set; }
    public string Resolution { get; set; } = string.Empty;
    public string ReviewedBy { get; set; } = string.Empty;
}

public class MonitoringData
{
    public Guid SessionId { get; set; }
    public DateTime Timestamp { get; set; }
    public FaceDetectionData? FaceData { get; set; }
    public AudioData? AudioData { get; set; }
    public ScreenData? ScreenData { get; set; }
    public BehaviorData? BehaviorData { get; set; }
    public EnvironmentData? EnvironmentData { get; set; }
}

public class FaceDetectionData
{
    public bool FaceDetected { get; set; }
    public int FaceCount { get; set; }
    public double Confidence { get; set; }
    public FacePosition Position { get; set; } = new();
    public bool IsLookingAtScreen { get; set; }
    public double EyeGazeConfidence { get; set; }
    public bool LivenessDetected { get; set; }
    public EmotionData? Emotions { get; set; }
    public string? FaceEncodingHash { get; set; } // For identity verification
}

public class FacePosition
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double CenterX => X + Width / 2;
    public double CenterY => Y + Height / 2;
}

public class EmotionData
{
    public double Anger { get; set; }
    public double Disgust { get; set; }
    public double Fear { get; set; }
    public double Happiness { get; set; }
    public double Sadness { get; set; }
    public double Surprise { get; set; }
    public double Neutral { get; set; }
    public string DominantEmotion => GetDominantEmotion();

    private string GetDominantEmotion()
    {
        var emotions = new Dictionary<string, double>
        {
            { "Anger", Anger },
            { "Disgust", Disgust },
            { "Fear", Fear },
            { "Happiness", Happiness },
            { "Sadness", Sadness },
            { "Surprise", Surprise },
            { "Neutral", Neutral }
        };
        return emotions.OrderByDescending(x => x.Value).First().Key;
    }
}

public class AudioData
{
    public double VoiceLevel { get; set; }
    public double BackgroundNoise { get; set; }
    public int SpeakerCount { get; set; }
    public bool VoiceDetected { get; set; }
    public bool MultipleVoicesDetected { get; set; }
    public double Confidence { get; set; }
    public List<string> DetectedWords { get; set; } = new();
}

public class ScreenData
{
    public bool IsFullScreen { get; set; }
    public bool TabSwitchDetected { get; set; }
    public bool WindowChangeDetected { get; set; }
    public bool ScreenshotAttempted { get; set; }
    public bool PrintScreenDetected { get; set; }
    public bool VirtualMachineDetected { get; set; }
    public bool RemoteDesktopDetected { get; set; }
    public List<string> RunningProcesses { get; set; } = new();
    public List<string> SuspiciousApplications { get; set; } = new();
}

public class BehaviorData
{
    public int KeystrokeCount { get; set; }
    public int MouseClickCount { get; set; }
    public int MouseMoveDistance { get; set; }
    public TimeSpan IdleTime { get; set; }
    public double TypingSpeed { get; set; }
    public bool CopyPasteDetected { get; set; }
    public bool SuspiciousPattern { get; set; }
    public double FocusScore { get; set; } // How focused the student appears
}

public class EnvironmentData
{
    public int BackgroundPersonCount { get; set; }
    public bool ObjectsOnDesk { get; set; }
    public double LightingQuality { get; set; }
    public bool PhoneDetected { get; set; }
    public bool BookDetected { get; set; }
    public bool PaperDetected { get; set; }
    public string RoomDescription { get; set; } = string.Empty;
}

public class ProctorAlert
{
    public Guid Id { get; set; }
    public Guid SessionId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public AlertType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public AlertPriority Priority { get; set; }
    public bool IsAcknowledged { get; set; }
    public string AcknowledgedBy { get; set; } = string.Empty;
    public DateTime? AcknowledgedAt { get; set; }
}

public class CalibrationData
{
    public Guid SessionId { get; set; }
    public FacePosition OptimalFacePosition { get; set; } = new();
    public double BaselineEyeGaze { get; set; }
    public double BackgroundNoiseLevel { get; set; }
    public double OptimalLighting { get; set; }
    public bool CalibrationCompleted { get; set; }
    public DateTime CalibrationTime { get; set; }
    public string CalibrationNotes { get; set; } = string.Empty;
}

public enum ViolationType
{
    FaceNotDetected,
    MultipleFaces,
    LookingAway,
    SuspiciousMovement,
    VoiceDetected,
    BackgroundNoise,
    TabSwitching,
    WindowChange,
    FullScreenExit,
    ScreenshotAttempt,
    CopyPaste,
    SuspiciousApplication,
    NetworkAnomaly,
    UnauthorizedDevice,
    IdentityMismatch,
    LivenessCheckFailed,
    EnvironmentChange,
    SystemTampering
}

public enum ViolationSeverity
{
    Low,
    Medium,
    High,
    Critical
}

public enum AlertType
{
    ViolationDetected,
    SystemIssue,
    TechnicalProblem,
    SessionTimeout,
    IdentityVerificationRequired,
    CalibrationNeeded
}

public enum AlertPriority
{
    Low,
    Normal,
    High,
    Urgent
}