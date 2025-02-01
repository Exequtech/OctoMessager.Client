using OctoMessager.Client.Models.Responses;

namespace OctoMessager.Client.Interfaces
{
    public interface IOctoClient
    {
        Task<MessageResponse> SendTextMessageAsync(string to, string message);
        Task<MessageResponse> SendImageMessageAsync(string to, string data, string fileName, string caption = "", int delay = 0, bool previewLinks = false);
        Task<MessageResponse> SendVideoMessageAsync(string to, string data, string fileName, string caption = "", int delay = 0, bool previewLinks = false);
        Task<MessageResponse> SendDocumentMessageAsync(string to, string data, string fileName, string caption = "", int delay = 0, bool previewLinks = false);
        Task<MessageResponse> SendPollMessageAsync(string to, string[] options, bool multiSelectable, string caption = "", int delay = 0, bool previewLinks = false);
        Task<MessageResponse> SendReferencedMediaAsync(string to, string messageId, string caption = "", int delay = 0, bool previewLinks = false);
    }
}
