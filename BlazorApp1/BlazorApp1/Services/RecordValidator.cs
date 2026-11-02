using System.Globalization;
using System.Net.Mail;
using BlazorApp1.Models;

namespace BlazorApp1.Services;

public interface IRecordValidator
{
    Task<FileValidationReport> ValidateAsync(Stream csv, CancellationToken ct = default);
}

public sealed class RecordValidator : IRecordValidator
{
    // Expected header: id,name,email,birthDate
    public async Task<FileValidationReport> ValidateAsync(Stream csv, CancellationToken ct = default)
    {
        var report = new FileValidationReport();
        var seenIds = new HashSet<string>();
        var seenEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // Streaming: lê linha a linha, nunca o arquivo inteiro na memória
        using var reader = new StreamReader(csv);
        var header = await reader.ReadLineAsync(ct);
        if (header is null)
        {
            report.AddIssue(0, "file", "The file is empty.");
            return report;
        }

        var line = 1;
        string? row;
        while ((row = await reader.ReadLineAsync(ct)) is not null)
        {
            line++;
            if (string.IsNullOrWhiteSpace(row)) continue;
            report.TotalRows++;
            var issuesBefore = report.IssueCount;

            var cols = row.Split(',');
            if (cols.Length != 4)
            {
                report.AddIssue(line, "row", $"Expected 4 columns, found {cols.Length}.");
                continue;
            }

            var id = cols[0].Trim();
            var name = cols[1].Trim();
            var email = cols[2].Trim();
            var birth = cols[3].Trim();

            if (id.Length == 0) report.AddIssue(line, "id", "Required.");
            else if (!seenIds.Add(id)) report.AddIssue(line, "id", $"Duplicate id '{id}'.");

            if (name.Length == 0) report.AddIssue(line, "name", "Required.");

            if (!MailAddress.TryCreate(email, out _)) report.AddIssue(line, "email", $"Invalid email '{email}'.");
            else if (!seenEmails.Add(email)) report.AddIssue(line, "email", $"Duplicate email '{email}'.");

            if (!DateOnly.TryParseExact(birth, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out var birthDate))
                report.AddIssue(line, "birthDate", $"Invalid date '{birth}', expected yyyy-MM-dd.");
            else if (birthDate > DateOnly.FromDateTime(DateTime.Today))
                report.AddIssue(line, "birthDate", "Date is in the future.");

            if (report.IssueCount == issuesBefore)
            {
                report.ValidRows++;
                report.ValidCustomers.Add(new Customer
                {
                    ExternalId = id,
                    Name = name,
                    Email = email,
                    BirthDate = birthDate,
                    ImportedAt = DateTime.UtcNow
                });
            }

        }

        return report;
    }
}