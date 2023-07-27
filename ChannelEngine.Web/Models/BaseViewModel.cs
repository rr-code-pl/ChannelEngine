namespace ChannelEngine.Web.Models;

public abstract class BaseViewModel
{
    public List<Message> Messages { get; set; } = new();
}

public class Message
{
    public string Text { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.Information;

    public Message(string text, MessageType type)
    {
        Text = text;
        Type = type;
    }
}
public enum MessageType
{
    Error,
    Warning,
    Information
}