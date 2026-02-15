using Microsoft.AspNetCore.Http;

namespace UniversiteRestApi.Controllers;

public class UploadUeNotesCsvRequest
{
    public long UeId { get; set; }
    public IFormFile File { get; set; } = null!;
}