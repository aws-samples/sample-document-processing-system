using System.Text;
using System.Text.Json;
using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using DocumentProcessor.Web.Models;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace DocumentProcessor.Web.Services;

public class AIService(IConfiguration configuration)
{
    private readonly IAmazonBedrockRuntime _bedrock = new AmazonBedrockRuntimeClient(new AmazonBedrockRuntimeConfig { RegionEndpoint = RegionEndpoint.GetBySystemName(configuration["Bedrock:Region"] ?? "us-west-2") });

    public async Task<ClassificationResult> ClassifyDocumentAsync(Document document, Stream content)
    {
        var text = await ExtractTextAsync(document, content);
        var prompt = $"Classify this document. Respond with JSON: {{\"category\": \"Invoice\", \"confidence\": 0.95}}\n\nFile: {document.FileName}\n{text}";
        var response = await CallBedrockAsync(prompt);
        return ParseClassification(response);
    }

    public async Task<SummaryResult> SummarizeDocumentAsync(Document document, Stream content)
    {
        var text = await ExtractTextAsync(document, content);
        var prompt = $"Summarize in 500 characters:\n\nFile: {document.FileName}\n{text}";
        var summary = await CallBedrockAsync(prompt);
        return new SummaryResult { Summary = summary.Trim() };
    }

    private async Task<string> CallBedrockAsync(string prompt)
    {
        var request = new ConverseRequest
        {
            ModelId = configuration["Bedrock:ClassificationModelId"] ?? "anthropic.claude-3-haiku-20240307-v1:0",
            Messages = [new Message { Role = ConversationRole.User, Content = [new ContentBlock { Text = prompt }] }],
            InferenceConfig = new InferenceConfiguration { MaxTokens = 2000, Temperature = 0.3f }
        };
        var response = await _bedrock.ConverseAsync(request);
        return response.Output?.Message?.Content?.FirstOrDefault()?.Text ?? "";
    }

    private async Task<string> ExtractTextAsync(Document doc, Stream stream)
    {
        var ext = Path.GetExtension(doc.FileName)?.ToLower();
        if (ext == ".pdf")
        {
            var sb = new StringBuilder();
            await Task.Run(() =>
            {
                using var pdf = PdfDocument.Open(stream);
                foreach (var page in pdf.GetPages().Take(5))
                {
                    sb.AppendLine(ContentOrderTextExtractor.GetText(page));
                    if (sb.Length > 10000) break;
                }
            });
            return sb.ToString();
        }
        if (ext == ".txt" || ext == ".log")
        {
            using var reader = new StreamReader(stream);
            var text = await reader.ReadToEndAsync();
            return text.Length > 10000 ? text[..10000] : text;
        }
        return $"[Unsupported: {ext}]";
    }

    private ClassificationResult ParseClassification(string response)
    {
        try
        {
            var cleaned = response.Replace("```json", "").Replace("```", "").Trim();
            var start = cleaned.IndexOf('{');
            var end = cleaned.LastIndexOf('}');
            if (start >= 0 && end > start) cleaned = cleaned[start..(end + 1)];
            var json = JsonDocument.Parse(cleaned);
            return new ClassificationResult { PrimaryCategory = json.RootElement.GetProperty("category").GetString() ?? "Unknown" };
        }
        catch { return new ClassificationResult { PrimaryCategory = "Unknown" }; }
    }
}

public class ClassificationResult
{
    public string PrimaryCategory { get; set; } = "";
}

public class SummaryResult
{
    public string Summary { get; set; } = "";
}
