using OctoMessager.client.Models.Content;

namespace OctoMessager.client.Models.Requests
{
    public class MessageRequest
    {
        public required string Id { get; set; }
        public required string Text { get; set; }
        public MediaContent? Image { get; set; }
        public MediaContent? Video { get; set; }
        public MediaContent? Document { get; set; }
        public PollContent? Poll { get; set; }
        public AttachmentReference? AttachmentRef { get; set; }
        public int Delay { get; set; }
        public bool PreviewLinks { get; set; }
    }
}