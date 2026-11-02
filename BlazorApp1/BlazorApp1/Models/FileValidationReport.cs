namespace BlazorApp1.Models;

public sealed record ValidationIssue(int Line, string Field, string Message);

public sealed class FileValidationReport
{
    public const int MaxIssuesShown = 500;
    public List<Customer> ValidCustomers { get; } = new();
    public int TotalRows { get; set; }
    public int ValidRows { get; set; }
    public int InvalidRows => TotalRows - ValidRows;
    public int IssueCount { get; set; }
    public List<ValidationIssue> Issues { get; } = new();

    public void AddIssue(int line, string field, string message)
    {
        IssueCount++;
        // Guarda só os primeiros 500 na memória: um arquivo enorme não derruba o servidor
        if (Issues.Count < MaxIssuesShown) Issues.Add(new(line, field, message));
    }
}