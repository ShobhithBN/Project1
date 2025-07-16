using AIProctoring.Shared.DTOs;

namespace AIProctoring.Client.Services;

public interface IApiService
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);
    Task<ApiResponse<StartExamResponse>> StartExamAsync(StartExamRequest request);
    Task<ApiResponse<bool>> SubmitAnswerAsync(SubmitAnswerRequest request);
    Task<ApiResponse<bool>> SubmitMonitoringDataAsync(MonitoringDataDTO data);
    Task<ApiResponse<ExamResultDTO>> EndExamAsync(EndExamRequest request);
    Task<ApiResponse<List<ExamDTO>>> GetAvailableExamsAsync(string studentId);
    Task<ApiResponse<ExamDTO>> GetExamAsync(Guid examId);
    void SetAuthToken(string token);
}