namespace VeriGit;

#pragma warning disable CA1032 // Implement standard exception constructors

/// <summary>
/// Exception thrown when validation against a snapshot fails.
/// </summary>
/// <param name="message">The message that describes the error.</param>
/// <param name="filePath">The path to the file that was validated.</param>
/// <param name="actualText">The actual text that was validated against the snapshot.</param>
/// <param name="diffText">The diff text showing the differences between the actual text and the snapshot.</param>
public sealed class ValidationFailedException(string message, string filePath, string? actualText, string? diffText) : Exception(message)
{
    /// <summary>
    /// The path to the file that was validated.
    /// </summary>
    public string FilePath { get; } = filePath;
    /// <summary>
    /// The actual text that was validated against the snapshot.
    /// </summary>
    public string? ActualText { get; } = actualText;
    /// <summary>
    /// The diff text showing the differences between the actual text and the snapshot.
    /// </summary>
    public string? DiffText { get; } = diffText;
}
