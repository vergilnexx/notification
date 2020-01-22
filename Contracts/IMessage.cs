namespace Contracts
{
    public interface IMessage
    {
        string Content { get; set; }

        string Subject { get; set; }

        string From { get; set; }
    }
}
