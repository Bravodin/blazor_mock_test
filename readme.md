# Blazor Data Validator

A small Blazor Server tool that lets customers validate a CSV file **before go-live**, so they can find and fix data issues on their own. Inspired by implementation tooling for SaaS platforms.

## What it does

- Upload a CSV with `id,name,email,birthDate`
- Shows a summary: total rows, valid rows, rows with issues
- Lists every issue by line and field: required fields, duplicate ids, invalid or duplicate emails, invalid or future dates

A sample file with intentional errors is available on the page (`wwwroot/sample-customers.csv`).

## Run it

Requires the .NET 10 SDK.

```bash
cd BlazorApp1/BlazorApp1
dotnet run
```

Then open the URL shown in the terminal and go to **Data Validator**.

## Design decisions

- **Streaming, not loading:** the file is read line by line with `StreamReader`, so memory stays flat even for very large files.
- **Bounded memory for results:** only the first 500 issues are kept for display, while all of them are counted.
- **Dependency injection:** `RecordValidator` is registered as a **Singleton** because it's stateless. If it used a `DbContext`, it would need to be **Scoped**.
- **Separation of concerns:** the validation logic lives in a service behind an interface (`IRecordValidator`), so it can be unit tested and reused outside the UI.

## Next steps

- Unit tests for `RecordValidator` (xUnit)
- A proper CSV parser for quoted fields with commas
- Configurable validation rules per customer
- Export the issues as a CSV the customer can fix and re-upload

## AI usage

Built with Claude as a coding assistant. I wrote the spec, made the design decisions and reviewed every line.