using System.ComponentModel.DataAnnotations;

namespace AIProctoring.Shared.Models;

public class Exam
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public ExamType Type { get; set; }
    public ExamSettings Settings { get; set; } = new();
    public List<Question> Questions { get; set; } = new();
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ExamSettings
{
    public bool AllowMultipleMonitors { get; set; } = false;
    public bool RequireFaceDetection { get; set; } = true;
    public bool RequireAudioMonitoring { get; set; } = true;
    public bool AllowTabSwitching { get; set; } = false;
    public bool RequireFullScreen { get; set; } = true;
    public double ViolationThreshold { get; set; } = 0.8;
    public bool EnableLivenessDetection { get; set; } = true;
    public bool EnableBehaviorAnalysis { get; set; } = true;
    public TimeSpan MaxIdleTime { get; set; } = TimeSpan.FromMinutes(5);
}

public class Question
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public QuestionType Type { get; set; }
    public List<string> Options { get; set; } = new();
    public string CorrectAnswer { get; set; } = string.Empty;
    public int Points { get; set; }
    public int OrderIndex { get; set; }
}

public class ExamSession
{
    public Guid Id { get; set; }
    public Guid ExamId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public ExamSessionStatus Status { get; set; }
    public List<Answer> Answers { get; set; } = new();
    public List<ViolationEvent> Violations { get; set; } = new();
    public SystemInfo SystemInfo { get; set; } = new();
    public string AccessToken { get; set; } = string.Empty;
}

public class Answer
{
    public Guid QuestionId { get; set; }
    public string StudentAnswer { get; set; } = string.Empty;
    public DateTime AnsweredAt { get; set; }
    public bool IsCorrect { get; set; }
    public int PointsAwarded { get; set; }
}

public class SystemInfo
{
    public string OperatingSystem { get; set; } = string.Empty;
    public string Browser { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public string ScreenResolution { get; set; } = string.Empty;
    public int AvailableMonitors { get; set; }
    public string DeviceFingerprint { get; set; } = string.Empty;
}

public enum ExamType
{
    Practice,
    Graded,
    Final,
    Certification
}

public enum QuestionType
{
    MultipleChoice,
    TrueFalse,
    ShortAnswer,
    Essay,
    Coding
}

public enum ExamSessionStatus
{
    Created,
    InProgress,
    Paused,
    Completed,
    Terminated,
    UnderReview
}