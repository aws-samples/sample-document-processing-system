# 📄 Document Processing System

A modern, enterprise-grade document processing platform built with .NET 8 and Blazor that leverages AWS Bedrock AI for intelligent document analysis, classification, and metadata extraction.

![Dashboard Overview](screenshots/Screenshot%202025-08-13%20113417.png)

## 🌟 Key Features

- **🤖 AI-Powered Processing**: Integrates with AWS Bedrock (Claude 3 models) for intelligent document analysis
- **📊 Real-time Dashboard**: Monitor document processing statistics, queue status, and system health
- **🔍 Smart Classification**: Automatically categorize documents using AI-driven classification
- **📝 Metadata Extraction**: Extract and store structured metadata from unstructured documents  
- **⚡ Background Processing**: Asynchronous document processing with queue management
- **🔐 Security-First**: Built-in authentication with ASP.NET Core Identity
- **📱 Responsive UI**: Modern Blazor Server-Side Rendering with Bootstrap 5
- **🔄 Real-time Updates**: SignalR integration for live processing status updates
- **📈 Analytics & Charts**: Visual insights with Chart.js integration

## 🏗️ Architecture

The application follows Clean Architecture principles with clear separation of concerns:

```
DocumentProcessor/
├── src/
│   ├── DocumentProcessor.Core/          # Domain entities and interfaces
│   ├── DocumentProcessor.Infrastructure/ # Data access, AI services, external integrations
│   ├── DocumentProcessor.Application/   # Business logic and services
│   └── DocumentProcessor.Web/          # Blazor UI and API endpoints
└── tests/
    └── DocumentProcessor.Tests/        # Unit and integration tests
```

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server (LocalDB or full instance)
- AWS Account with Bedrock access (for AI features)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/document-processor.git
   cd document-processor
   ```

2. **Configure AWS Credentials**
   
   Set up your AWS credentials using one of these methods:
   - AWS CLI: `aws configure`
   - Environment variables: `AWS_ACCESS_KEY_ID` and `AWS_SECRET_ACCESS_KEY`
   - IAM roles (for EC2 deployment)

3. **Configure Application Settings**
   
   Update `src/DocumentProcessor.Web/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DocumentProcessorDB;Trusted_Connection=True;"
     },
     "BedrockOptions": {
       "Region": "us-west-2",
       "ClassificationModelId": "anthropic.claude-3-haiku-20240307-v1:0",
       "ExtractionModelId": "anthropic.claude-3-sonnet-20240229-v1:0",
       "MaxTokens": 2000,
       "Temperature": 0.3
     }
   }
   ```

4. **Set up the database**
   ```bash
   dotnet ef database update -p src/DocumentProcessor.Infrastructure -s src/DocumentProcessor.Web
   ```

5. **Run the application**
   ```bash
   dotnet run --project src/DocumentProcessor.Web
   ```

6. **Access the application**
   
   Navigate to `https://localhost:7266` or `http://localhost:5197`

## 📋 Features Overview

### Document Management
- **Multi-format Support**: PDF, DOCX, TXT, RTF, ODT, JPG, PNG, XLSX
- **Drag-and-drop Upload**: Intuitive file upload interface
- **Batch Processing**: Queue multiple documents for processing
- **Document Viewer**: Preview documents directly in the browser
- **Search & Filter**: Find documents by metadata, type, or content

![Document Upload Interface](screenshots/Screenshot%202025-08-13%20113424.png)

### AI Processing Capabilities
- **Intelligent Classification**: Automatically categorize documents into predefined types
- **Content Extraction**: Extract text from various document formats including PDFs
- **Metadata Generation**: Create structured metadata from unstructured content
- **Multi-model Support**: Configurable AI models for different tasks:
  - Classification: Claude 3 Haiku for fast categorization
  - Extraction: Claude 3 Sonnet for detailed content analysis
  - Summarization: Claude 3 Haiku for quick summaries

### Real-time Dashboard
- **Processing Statistics**: Total documents, processed, queued, and failed counts
- **Activity Charts**: 7-day processing activity visualization
- **Document Type Distribution**: Doughnut chart showing document categories
- **Queue Monitoring**: Real-time processing queue status
- **System Health**: Monitor database, storage, and AI processor status
- **Storage Usage**: Track storage consumption with visual indicators

![Document List View](screenshots/Screenshot%202025-08-13%20113444.png)

### Background Processing
- **Async Queue Processing**: Non-blocking document processing
- **Priority Management**: Process documents based on priority levels
- **Retry Logic**: Automatic retry with exponential backoff
- **Status Tracking**: Real-time status updates via SignalR
- **Auto-refresh**: Dashboard updates every 10 seconds

![Document Search Interface](screenshots/Screenshot%202025-08-13%20113453.png)

![Document Metadata View](screenshots/Screenshot%202025-08-13%20113508.png)

## 🛠️ Technology Stack

- **Backend**: 
  - .NET 8 with C# 12
  - Entity Framework Core 8
  - ASP.NET Core Identity
- **Frontend**: 
  - Blazor Server-Side Rendering
  - Bootstrap 5
  - Chart.js
- **Database**: 
  - SQL Server 
  - Temporal tables for audit trails
- **AI/ML**: 
  - AWS Bedrock
  - Claude 3 Haiku & Sonnet models
- **Real-time**: 
  - SignalR for live updates
- **Document Processing**: 
  - PdfPig for PDF extraction
  - DocumentFormat.OpenXml for Office documents
- **Background Jobs**: 
  - IHostedService
  - Custom Background Task Queue

## 📁 Project Structure

```
src/
├── DocumentProcessor.Core/             # Domain layer
│   ├── Entities/                      # Domain models
│   │   ├── Document.cs               # Main document entity
│   │   ├── Classification.cs         # Classification results
│   │   ├── DocumentMetadata.cs       # Extracted metadata
│   │   └── ProcessingQueue.cs        # Queue management
│   └── Interfaces/                    # Core contracts
│       ├── IDocumentProcessor.cs      
│       ├── IAIProcessor.cs           
│       └── IDocumentRepository.cs    
│
├── DocumentProcessor.Infrastructure/   # Infrastructure layer
│   ├── AI/                            # AI processing services
│   │   ├── BedrockAIProcessor.cs     # AWS Bedrock integration
│   │   └── DocumentContentExtractor.cs # Content extraction
│   ├── Data/                          # EF Core context
│   │   └── ApplicationDbContext.cs   
│   ├── Repositories/                  # Data access
│   └── BackgroundTasks/               # Queue processing
│
├── DocumentProcessor.Application/      # Application layer
│   └── Services/                      # Business logic
│       ├── DocumentProcessingService.cs
│       └── BackgroundDocumentProcessingService.cs
│
└── DocumentProcessor.Web/             # Presentation layer
    ├── Components/                    # Blazor components
    │   ├── Pages/                    # Page components
    │   │   ├── Dashboard.razor       # Main dashboard
    │   │   ├── DocumentUpload.razor  # Upload interface
    │   │   └── DocumentList.razor    # Document management
    │   └── Layout/                   # Layout components
    ├── Hubs/                         # SignalR hubs
    └── wwwroot/                      # Static assets
```

## 🔧 Configuration

### Document Storage Options

Configure storage in `appsettings.json`:

```json
{
  "DocumentStorage": {
    "Provider": "LocalFileSystem",
    "LocalFileSystem": {
      "RootPath": "uploads",
      "MaxFileSizeInMB": 100,
      "AllowedExtensions": [".pdf", ".doc", ".docx", ".txt", ".rtf", ".odt"]
    },
    "S3": {
      "BucketName": "document-processor-bucket",
      "Region": "us-east-1",
      "UsePresignedUrls": true
    },
    "FileShare": {
      "NetworkPath": "\\\\fileserver\\documents",
      "MaxFileSizeInMB": 100
    }
  }
}
```

### AI Configuration

```json
{
  "BedrockOptions": {
    "Region": "us-west-2",
    "ClassificationModelId": "anthropic.claude-3-haiku-20240307-v1:0",
    "ExtractionModelId": "anthropic.claude-3-sonnet-20240229-v1:0",
    "SummarizationModelId": "anthropic.claude-3-haiku-20240307-v1:0",
    "MaxTokens": 2000,
    "Temperature": 0.3,
    "TopP": 0.9,
    "MaxRetries": 3,
    "RetryDelayMilliseconds": 1000,
    "EnableDetailedLogging": true,
    "UseSimulatedResponses": false
  }
}
```

## 🧪 Testing

Run the test suite:
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# Run specific test project
dotnet test tests/DocumentProcessor.Tests
```

## 📈 Performance Features

- **Virtualization**: Efficient rendering of large document lists
- **Lazy Loading**: Load data on demand
- **Caching**: In-memory caching for frequently accessed data
- **Connection Pooling**: Optimized database connections
- **Async/Await**: Non-blocking I/O operations throughout
- **Batch Processing**: Process multiple documents efficiently
- **Optimized Queries**: EF Core query optimization

## 🔒 Security Features

- **Authentication**: ASP.NET Core Identity integration
- **Role-based Access**: Configurable user roles and permissions
- **Input Validation**: Comprehensive validation on all inputs
- **File Type Validation**: Whitelist-based file extension filtering
- **Secure File Storage**: Files stored outside web root
- **SQL Injection Prevention**: Parameterized queries via EF Core
- **XSS Protection**: Built-in Blazor security features
- **CSRF Protection**: Anti-forgery tokens

## 🚢 Deployment

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/DocumentProcessor.Web/DocumentProcessor.Web.csproj", "DocumentProcessor.Web/"]
RUN dotnet restore "DocumentProcessor.Web/DocumentProcessor.Web.csproj"
COPY . .
WORKDIR "/src/DocumentProcessor.Web"
RUN dotnet build "DocumentProcessor.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DocumentProcessor.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DocumentProcessor.Web.dll"]
```

### AWS Deployment

Deploy to AWS using Elastic Beanstalk or ECS:

```bash
# Using AWS CLI for Elastic Beanstalk
eb init -p docker document-processor
eb create production
eb deploy
```

## 📊 Monitoring

The application includes built-in monitoring capabilities:

- **Health Checks**: `/health` endpoint for monitoring
- **Logging**: Structured logging with configurable levels
- **Metrics**: Processing statistics and system metrics
- **Dashboard**: Real-time monitoring via the web interface

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Development Guidelines

- Follow C# coding conventions
- Write unit tests for new features
- Update documentation as needed
- Ensure all tests pass before submitting PR
- Add meaningful commit messages

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

For issues, questions, or suggestions:
- Open an issue on GitHub
- Check existing issues before creating new ones
- Provide detailed information for bug reports

## 🗺️ Roadmap

### Short-term (Q1 2025)
- [ ] Add support for additional AI providers (OpenAI, Azure OpenAI)
- [ ] Implement OCR for scanned documents
- [ ] Add batch export functionality
- [ ] Enhanced search with full-text search

### Medium-term (Q2-Q3 2025)
- [ ] Document versioning and change tracking
- [ ] Multi-tenant support
- [ ] REST API for external integrations
- [ ] Mobile-responsive design improvements

### Long-term (Q4 2025 and beyond)
- [ ] Workflow automation features
- [ ] Machine learning model training on classified documents
- [ ] Advanced analytics and reporting
- [ ] Plugin architecture for custom processors

## 📸 Screenshots

### Dashboard Overview
![Dashboard showing real-time processing statistics and system health](screenshots/Screenshot%202025-08-13%20113417.png)
*Real-time dashboard with processing statistics, activity charts, and system health monitoring*

### Document Upload
![Document upload interface with drag-and-drop support](screenshots/Screenshot%202025-08-13%20113424.png)
*Intuitive drag-and-drop interface for uploading documents with progress tracking*

### Document Management
![Document list view with filtering and actions](screenshots/Screenshot%202025-08-13%20113444.png)
*Comprehensive document list with status indicators and quick actions*

### Search Functionality
![Advanced document search interface](screenshots/Screenshot%202025-08-13%20113453.png)
*Powerful search capabilities to find documents by various criteria*

### Metadata Extraction
![AI-powered metadata extraction results](screenshots/Screenshot%202025-08-13%20113508.png)
*View and edit extracted metadata from processed documents*

## 📚 Documentation

Additional documentation can be found in the `/docs` directory:
- [API Documentation](docs/api.md)
- [Architecture Guide](docs/architecture.md)
- [Deployment Guide](docs/deployment.md)
- [Development Setup](docs/development.md)

## 🙏 Acknowledgments

- Built with [.NET 8](https://dotnet.microsoft.com/)
- AI powered by [AWS Bedrock](https://aws.amazon.com/bedrock/)
- UI components from [Bootstrap](https://getbootstrap.com/)
- Charts by [Chart.js](https://www.chartjs.org/)

---

**Built with ❤️ using .NET 8 and AWS Bedrock AI**

For more information, visit our [documentation](https://github.com/yourusername/document-processor/wiki) or contact the maintainers.