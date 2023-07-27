namespace ChannelEngine.BusinessLogic.Models;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Dictionary<string, List<string>> ValidationErrors { get; set; } = new();

}
