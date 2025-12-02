using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentProcessor.Web.Models;

public enum DocumentStatus { Pending, Processing, Processed, Failed }

[Table("documents", Schema = "dps_dbo")]
public class Document
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("filename")]
    public string FileName { get; set; } = string.Empty;
    
    [Column("originalfilename")]
    public string OriginalFileName { get; set; } = string.Empty;
    
    [Column("fileextension")]
    public string FileExtension { get; set; } = string.Empty;
    
    [Column("filesize")]
    public long FileSize { get; set; }
    
    [Column("contenttype")]
    public string ContentType { get; set; } = string.Empty;
    
    [Column("storagepath")]
    public string StoragePath { get; set; } = string.Empty;
    
    [Column("status")]
    public DocumentStatus Status { get; set; }
    
    [Column("summary")]
    public string? Summary { get; set; }
    
    [Column("uploadedby")]
    public string UploadedBy { get; set; } = string.Empty;
    
    [Column("isdeleted")]
    public bool IsDeleted { get; set; }
}
