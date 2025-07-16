using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AIProctoring.Shared.DTOs;
using Microsoft.Extensions.Logging;

namespace AIProctoring.Client.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiService(IHttpClientFactory httpClientFactory, ILogger<ApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("AIProctoring");
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public void SetAuthToken(string token)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("auth/login", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseContent, _jsonOptions);
                return new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Data = loginResponse,
                    Message = "Login successful"
                };
            }
            else
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, _jsonOptions);
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = errorResponse?.Message ?? "Login failed",
                    Errors = errorResponse?.Errors ?? new List<string> { "Unknown error" }
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "Network error",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<StartExamResponse>> StartExamAsync(StartExamRequest request)
    {
        return await PostAsync<StartExamRequest, StartExamResponse>("exam/start", request);
    }

    public async Task<ApiResponse<bool>> SubmitAnswerAsync(SubmitAnswerRequest request)
    {
        return await PostAsync<SubmitAnswerRequest, bool>("exam/answer", request);
    }

    public async Task<ApiResponse<bool>> SubmitMonitoringDataAsync(MonitoringDataDTO data)
    {
        return await PostAsync<MonitoringDataDTO, bool>("proctoring/data", data);
    }

    public async Task<ApiResponse<ExamResultDTO>> EndExamAsync(EndExamRequest request)
    {
        return await PostAsync<EndExamRequest, ExamResultDTO>("exam/end", request);
    }

    public async Task<ApiResponse<List<ExamDTO>>> GetAvailableExamsAsync(string studentId)
    {
        return await GetAsync<List<ExamDTO>>($"exam/available/{studentId}");
    }

    public async Task<ApiResponse<ExamDTO>> GetExamAsync(Guid examId)
    {
        return await GetAsync<ExamDTO>($"exam/{examId}");
    }

    private async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string endpoint)
    {
        try
        {
            var response = await _httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
                return new ApiResponse<TResponse>
                {
                    Success = true,
                    Data = data,
                    Message = "Success"
                };
            }
            else
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, _jsonOptions);
                return new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = errorResponse?.Message ?? "Request failed",
                    Errors = errorResponse?.Errors ?? new List<string> { "Unknown error" }
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during GET request to {Endpoint}", endpoint);
            return new ApiResponse<TResponse>
            {
                Success = false,
                Message = "Network error",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    private async Task<ApiResponse<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest request)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync(endpoint, content);
            var responseContent = await response.Content.ReadAsStringAsync();
            
            if (response.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<TResponse>(responseContent, _jsonOptions);
                return new ApiResponse<TResponse>
                {
                    Success = true,
                    Data = data,
                    Message = "Success"
                };
            }
            else
            {
                var errorResponse = JsonSerializer.Deserialize<ApiResponse<object>>(responseContent, _jsonOptions);
                return new ApiResponse<TResponse>
                {
                    Success = false,
                    Message = errorResponse?.Message ?? "Request failed",
                    Errors = errorResponse?.Errors ?? new List<string> { "Unknown error" }
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during POST request to {Endpoint}", endpoint);
            return new ApiResponse<TResponse>
            {
                Success = false,
                Message = "Network error",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}