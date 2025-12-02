using DocumentProcessor.Web.Models;
using DocumentProcessor.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace DocumentProcessor.Web.Services;

public class DocumentProcessingService(IServiceScopeFactory scopeFactory)
{
    public async Task ProcessDocumentAsync(Guid documentId)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var storage = scope.ServiceProvider.GetRequiredService<FileStorageService>();
        var ai = scope.ServiceProvider.GetRequiredService<AIService>();

        var doc = await db.Documents.FindAsync(documentId);
        if (doc == null) return;

        try
        {
            doc.Status = DocumentStatus.Processing;
            await db.SaveChangesAsync();

            await using var stream1 = await storage.GetDocumentAsync(doc.StoragePath);
            await using var stream2 = await storage.GetDocumentAsync(doc.StoragePath);

            var classification = await ai.ClassifyDocumentAsync(doc, stream1);
            var summary = await ai.SummarizeDocumentAsync(doc, stream2);

            doc.Status = DocumentStatus.Processed;
            doc.Summary = summary.Summary;
            await db.SaveChangesAsync();
        }
        catch
        {
            doc.Status = DocumentStatus.Failed;
            await db.SaveChangesAsync();
        }
    }
}
