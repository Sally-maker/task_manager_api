namespace TaskManagerApi.Constants;

public static class TaskStatuses
{
    public const string Pending    = "pending";
    public const string InProgress = "in_progress";
    public const string Done       = "done";

    public static readonly IReadOnlyList<string> AllowedValues = new[]
    {
        Pending,
        InProgress,
        Done
    };

    public static bool IsValid(string? status) =>
        status is not null && AllowedValues.Contains(status);
}
