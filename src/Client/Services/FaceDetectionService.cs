using OpenCvSharp;
using AIProctoring.Shared.DTOs;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace AIProctoring.Client.Services;

public class FaceDetectionService : IFaceDetectionService
{
    private readonly ILogger<FaceDetectionService> _logger;
    private CascadeClassifier? _faceCascade;
    private bool _isInitialized;

    public FaceDetectionService(ILogger<FaceDetectionService> logger)
    {
        _logger = logger;
    }

    public bool IsInitialized => _isInitialized;

    public async Task InitializeAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                // Initialize OpenCV face cascade classifier
                // Note: You'll need to include the haarcascade_frontalface_alt.xml file in your resources
                _faceCascade = new CascadeClassifier("Resources/Raw/haarcascade_frontalface_alt.xml");
                _isInitialized = true;
            });

            _logger.LogInformation("Face detection service initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize face detection service");
            _isInitialized = false;
        }
    }

    public async Task<FaceDetectionDataDTO?> DetectFaceAsync(byte[] imageData)
    {
        if (!_isInitialized || _faceCascade == null)
        {
            _logger.LogWarning("Face detection service not initialized");
            return null;
        }

        try
        {
            return await Task.Run(() =>
            {
                using var mat = Mat.FromImageData(imageData, ImreadModes.Color);
                using var grayMat = new Mat();
                Cv2.CvtColor(mat, grayMat, ColorConversionCodes.BGR2GRAY);

                var faces = _faceCascade.DetectMultiScale(
                    grayMat,
                    scaleFactor: 1.1,
                    minNeighbors: 3,
                    flags: HaarDetectionTypes.ScaleImage,
                    minSize: new Size(30, 30)
                );

                var faceData = new FaceDetectionDataDTO
                {
                    FaceDetected = faces.Length > 0,
                    FaceCount = faces.Length,
                    Confidence = faces.Length > 0 ? 0.85 : 0.0, // Mock confidence
                    IsLookingAtScreen = true, // Mock - would need eye tracking
                    EyeGazeConfidence = 0.8, // Mock
                    LivenessDetected = true // Mock - would need liveness detection
                };

                if (faces.Length > 0)
                {
                    var face = faces[0]; // Use the first detected face
                    faceData.Position = new FacePositionDTO
                    {
                        X = face.X,
                        Y = face.Y,
                        Width = face.Width,
                        Height = face.Height
                    };

                    // Mock emotion data - in a real implementation, you'd use an emotion recognition model
                    faceData.Emotions = new EmotionDataDTO
                    {
                        Neutral = 0.7,
                        Happiness = 0.15,
                        Sadness = 0.05,
                        Anger = 0.03,
                        Fear = 0.02,
                        Surprise = 0.03,
                        Disgust = 0.02
                    };

                    // Generate face encoding hash for identity verification
                    var faceRegion = new Mat(mat, face);
                    using var faceBytes = faceRegion.ToBytes();
                    faceData.FaceEncodingHash = GenerateHash(faceBytes);
                }

                return faceData;
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during face detection");
            return null;
        }
    }

    public async Task<bool> VerifyIdentityAsync(byte[] imageData, string referenceHash)
    {
        try
        {
            var faceData = await DetectFaceAsync(imageData);
            if (faceData?.FaceEncodingHash == null)
                return false;

            // Simple hash comparison - in production, use proper face recognition
            return faceData.FaceEncodingHash == referenceHash;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during identity verification");
            return false;
        }
    }

    public async Task<string> GenerateFaceHashAsync(byte[] imageData)
    {
        try
        {
            var faceData = await DetectFaceAsync(imageData);
            return faceData?.FaceEncodingHash ?? string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating face hash");
            return string.Empty;
        }
    }

    private static string GenerateHash(byte[] data)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(data);
        return Convert.ToBase64String(hashBytes);
    }

    public void Dispose()
    {
        _faceCascade?.Dispose();
        _isInitialized = false;
    }
}