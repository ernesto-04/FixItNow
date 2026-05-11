namespace FixItNow.Web.Models
{
    public class UploadedImage
    {
        public byte[] Data { get; set; } = [];

        public string Name { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public string PreviewUrl { get; set; } = string.Empty;
    }
}
