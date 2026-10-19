# 📄 Document Processing System

A single-project .NET 10 Blazor Server application that summarizes uploaded documents with
Claude Sonnet 5 on Amazon Bedrock, storing results in SQL Server.

![Application Dashboard](screenshots/dashboard.png)

## 🌟 Key Features

- **🤖 AI Summaries**: Amazon Bedrock Converse API with Claude Sonnet 5
- **📄 Text Extraction**: PDF text extraction via PdfPig, plus plain-text and log files
- **📤 Upload**: Drag-and-drop or file picker, with size and file-type validation
- **🗂️ Document List**: Newest first, with upload timestamp, status, and summary preview
- **💾 SQL Server**: Entity Framework Core 10, with a local instance in Docker
- **🔐 Credentials**: Optional AWS Secrets Manager lookup for RDS credentials
- **🧭 Provider Indicator**: Header pill reports the database engine actually in use

## 🏗️ Architecture

One ASP.NET Core project. Interactive Server components render the UI; a small pipeline
handles storage, extraction, and summarization.

```
src/DocumentProcessor.Web/
├── Components/
│   ├── Documents/      # DocumentList, DocumentUploader, StatusBadge
│   ├── Layout/         # MainLayout, DatabaseIndicator, DatabaseFooter
│   ├── Pages/          # Home, Error
│   └── Shared/         # ModalDialog
├── Configuration/      # BedrockOptions, StorageOptions, DatabaseOptions
├── Data/               # AppDbContext, connection resolution
├── Extensions/         # DI registration
├── Models/             # Document, DocumentStatus, Notification
├── Services/           # Storage, text extraction, summarization, pipeline
└── wwwroot/            # app.css, Bootstrap
```

Upload flow:

```
Browser → Home.SaveAsync → IDocumentStorage (disk)
                         → AppDbContext (row, status Pending)
        → DocumentPipeline → DocumentTextExtractor (PdfPig)
                           → IDocumentSummarizer (Bedrock Converse)
                           → AppDbContext (summary, status Processed)
```

## 🚀 Getting Started

### Prerequisites

- .NET 10.0 SDK
- Docker (for local SQL Server) or an existing SQL Server instance
- An AWS account with Bedrock access to Claude Sonnet 5 in your chosen region
- AWS credentials available to the default SDK chain

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/aws-samples/sample-document-processing-system.git
   cd sample-document-processing-system
   ```

2. **Configure AWS credentials**
   ```bash
   aws configure
   ```
   Or export `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`, and `AWS_DEFAULT_REGION`.

   Confirm the model is reachable:
   ```bash
   aws bedrock-runtime converse \
     --region us-east-1 \
     --model-id global.anthropic.claude-sonnet-5 \
     --messages '[{"role":"user","content":[{"text":"Reply with: OK"}]}]' \
     --inference-config '{"maxTokens":20}'
   ```

3. **Start SQL Server**
   ```bash
   docker compose up -d
   ```

   Runs `mcr.microsoft.com/mssql/server:2022-latest` on port 1433 with the `sa` password
   `LocalDev!Passw0rd`, matching the default connection string. Override it by setting
   `MSSQL_SA_PASSWORD` before `docker compose up` and updating the connection string to
   match. Data persists in the `sqlserver-data` volume.

4. **Run the application**
   ```bash
   cd src/DocumentProcessor.Web
   dotnet run
   ```

   The `DPS` database and `Documents` table are created on first run.

5. **Open the app**

   Navigate to <https://localhost:7266>. The HTTP port redirects there.

## 📋 Features Overview

![Upload panel](screenshots/upload.png)

### Upload and processing

- Drag files onto the drop zone or click **Browse files**
- `.pdf`, `.txt`, and `.log` are accepted, up to 50 MB and 10 files per upload
- Rejected files report why, inline
- Each document is saved to disk, recorded, then summarized

Uploads are processed synchronously, so the request stays open until Bedrock responds.

### Document list

- Ordered by upload time, newest first
- Shows file type, name, status, upload timestamp, and a two-line summary preview
- The eye button opens the full summary; it is disabled until one exists
- Delete is a soft delete, so rows stay in the table behind a global query filter

## 🛠️ Technology Stack

- **Backend**: .NET 10, ASP.NET Core Blazor Server (Interactive Server), EF Core 10
- **Database**: Microsoft SQL Server (2022 in Docker for local development)
- **AI**: Amazon Bedrock Converse API, Claude Sonnet 5
- **Documents**: PdfPig 0.1.11
- **Frontend**: Bootstrap 5, Bootstrap Icons, custom CSS

## 🔧 Configuration

All settings live in `src/DocumentProcessor.Web/appsettings.json` and bind to option
classes validated at startup, so a bad value fails the build-up rather than the first
request.

### Bedrock

```json
"Bedrock": {
  "Region": "us-east-1",
  "SummarizationModelId": "global.anthropic.claude-sonnet-5",
  "MaxTokens": 2000,
  "MaxInputCharacters": 10000,
  "MaxPdfPages": 5
}
```

`MaxPdfPages` and `MaxInputCharacters` bound how much text is sent per document.

> **Note:** Claude Sonnet 5 rejects `temperature` and `topP`. The inference config
> intentionally sets only `maxTokens`.

### Storage

```json
"Storage": {
  "RootPath": "uploads",
  "MaxFileSizeMegabytes": 50,
  "MaxFilesPerUpload": 10,
  "AllowedExtensions": [ ".pdf", ".txt", ".log" ]
}
```

Files are written to `uploads/yyyy/MM/dd/`. Only extensions the extractor can read should
be listed here.

### Database

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost,1433;Database=DPS;User Id=sa;Password=LocalDev!Passw0rd;TrustServerCertificate=true;MultipleActiveResultSets=true"
},
"Database": {
  "UseSecretsManager": false
}
```

Set `Database:UseSecretsManager` to `true` to build the connection string from an AWS
Secrets Manager secret whose description starts with
`Password for RDS MSSQL used for MAM417.`:

```json
{
  "username": "your_username",
  "password": "your_password",
  "host": "your-db-host.rds.amazonaws.com",
  "port": "1433",
  "dbname": "your_database_name"
}
```

If the lookup fails, the app logs a warning and falls back to `DefaultConnection`.

## 🐘 Migrating to Aurora PostgreSQL

The app targets SQL Server today and is structured so the engine can change in one place.

- `DatabaseConnectionResolver.DetectProvider` infers the engine from the connection string
  (`Host=`/`Username=` means Npgsql; `Server=`/`User Id=` means SQL Server)
- `DatabaseIndicator` and the footer report that provider, so a successful migration is
  visible in the UI rather than assumed
- `ServiceCollectionExtensions.AddDocumentProcessing` has the single provider switch and
  currently throws a clear error for PostgreSQL instead of passing Npgsql syntax to
  `SqlClient`

To complete a migration:

1. Add `Npgsql.EntityFrameworkCore.PostgreSQL`
2. Add the `DatabaseProvider.PostgreSql` arm calling `UseNpgsql`
3. Point `DefaultConnection` at the Aurora cluster

The entity configuration uses no SQL-Server-specific types, so the mapping ports unchanged.

## 📊 Document Status States

| Status | Meaning |
|---|---|
| `Pending` | Row created, not yet processed |
| `Processing` | Text extraction and summarization under way |
| `Processed` | Summary stored |
| `Failed` | Extraction or the Bedrock call threw; see logs |

## 🔒 Security Notes

- The `sa` password in `appsettings.json` exists only for the throwaway local container.
  Use Secrets Manager, environment variables, or user-secrets for anything real.
- Uploaded files are stored on local disk under `uploads/`, which is git-ignored.
- There is no authentication; add it before exposing the app.

## 🚢 Deployment

- **Database**: RDS for SQL Server, or Aurora PostgreSQL after the migration above
- **Compute**: ECS, EC2, or Elastic Beanstalk
- **Credentials**: Secrets Manager with `Database:UseSecretsManager` enabled
- **Schema**: `EnsureCreatedAsync()` creates the schema on first run but cannot evolve it.
  Adopt EF Core migrations before running more than one environment.

`docker-compose.yml` provisions SQL Server for development only; the app itself runs on the
host via `dotnet run`. Containerizing it needs a standard .NET 10 Dockerfile.

## 🆘 Troubleshooting

### `Microsoft.AspNetCore.App` version not found

The installed ASP.NET Core runtime does not match the target framework. Install the .NET 10
runtime, or check `dotnet --list-runtimes`.

### `_framework/blazor.web.js` returns 404

Framework assets resolve through the static-web-assets manifest, which loads in the
Development environment. Ensure `ASPNETCORE_ENVIRONMENT=Development` when running
`dotnet run`; a published build serves them from `wwwroot`.

### Database connection failures

1. `docker compose ps` — is the container healthy?
2. Confirm port 1433 is free and the password matches the connection string
3. With `UseSecretsManager` enabled, check the warning logged at startup

### Bedrock access errors

1. Confirm model access is granted for Claude Sonnet 5 in the configured region
2. `aws bedrock list-inference-profiles` — is `global.anthropic.claude-sonnet-5` active?
3. `AccessDeniedException` means the caller lacks `bedrock:InvokeModel`
4. A `temperature`/`topP` validation error means sampling parameters were reintroduced

### Upload issues

1. Only `.pdf`, `.txt`, and `.log` are accepted; others are rejected at selection
2. Files over `MaxFileSizeMegabytes` are rejected
3. A `Failed` status means extraction or the Bedrock call threw — check the logs
4. Scanned PDFs with no text layer yield little for the model to summarize

## 🗺️ Roadmap

- EF Core migrations in place of `EnsureCreatedAsync`
- Aurora PostgreSQL support
- Background processing so uploads return immediately
- Additional formats (DOCX, XLSX, images via OCR)
- Document classification persisted to a `Category` column
- Search, filtering, and pagination
- Authentication

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License.

## 🙏 Acknowledgments

- Built with [.NET 10](https://dotnet.microsoft.com/)
- AI powered by [Amazon Bedrock](https://aws.amazon.com/bedrock/)
- UI framework by [Bootstrap](https://getbootstrap.com/)
- PDF processing by [PdfPig](https://github.com/UglyToad/PdfPig)

---

**Built with ❤️ using .NET 10 and Claude Sonnet 5 on Amazon Bedrock**
