using Microsoft.AspNetCore.Http;

namespace ServiceReceiverMedia.Models
{
    public class VideoUploadModel
    {
        public IFormFile VideoFile { get; set; }
        public string token { get; set; }
    }
}
