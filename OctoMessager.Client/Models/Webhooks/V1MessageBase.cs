using System.Text.Json.Serialization;

namespace OctoMessager.Client.Models.Webhooks
{
    public class V1MessageBase
    {
        public required V1MessageInfo MessageInfo { get; set; }
        public required string MessageType { get; set; }
        public required string MimeType { get; set; }
        public V1MessageBase? Quoted { get; set; }
        public required string Text { get; set; }
        public V1ImageMessageData? Image { get; set; }
        public V1VideoMessageData? Video { get; set; }
        public V1AudioMessageData? Audio { get; set; }
        public V1DocumentMessageData? Document { get; set; }
        public V1Contact[]? Contacts { get; set; }
    }

    public struct V1MessageInfo
    {
        public bool FromSelf { get; set; }
        public required string MessageID { get; set; }
        public string? GroupID { get; set; }
        public long Timestamp { get; set; }
        public V1WhatsAppUserInfo? User { get; set; }
    }

    public struct V1WhatsAppUserInfo
    {
        public required string Id { get; set; }
        public string? ProfilePicUrl { get; set; }
        public string? PushName { get; set; }
    }

    public struct V1ImageMessageData
    {
        public required int Height { get; set; }
        public string? JpegThumbnail { get; set; }
        public required int Width { get; set; }
    }
    public struct V1VideoMessageData
    {
        public required int Height { get; set; }
        public string? JpegThumbnail { get; set; }
        public required int Width { get; set; }
    }
    public struct V1AudioMessageData
    {
        public required bool PushToTalk { get; set; }
        public required int Duration { get; set; }
        public float[]? WaveForm { get; set; }
    }
    public struct V1DocumentMessageData
    {
        public required string FileName { get; set; }
        public string? JpegThumbnail { get; set; }
        public required int PageCount { get; set; }
    }
    public struct V1PollMessageData
    {
        public required bool MultiSelectable { get; set; }
        public required string[] Options { get; set; }
    }
    public struct V1Contact
    {
        public required string ContactName { get; set; }
        public required string Email { get; set; }
        public required string[] Numbers { get; set; }
        public required string Organization { get; set; }
        public required string Url { get; set; }
    }
}
