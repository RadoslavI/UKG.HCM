namespace UKG.HCM.Shared.Utilities;

public class OperationResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }

    public OperationResult(bool success, string? errorMessage = null)
    {
        Success = success;
        ErrorMessage = errorMessage;
    }

    public static OperationResult SuccessResult() =>
        new OperationResult(true);

    public static OperationResult FailureResult(string? message = null) =>
        new OperationResult(false, message);
}