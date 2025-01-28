namespace OctoMessager.Client.Models.Content
{
    public class PollContent
    {
        public required string[] Options { get; set; }
        public bool MultiSelectable { get; set; }
    }
}