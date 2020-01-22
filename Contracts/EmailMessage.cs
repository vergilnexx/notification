namespace Contracts
{
    public class EmailMessage : IMessage
    {
        public string Content { get; set; }

        public string Subject { get; set; }

        public string From { get; set; }
    }
}
