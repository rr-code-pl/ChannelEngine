namespace ChannelEngine.BusinessLogic.Models;

public class StockUpdateResponse
{
    public Dictionary<string, List<string>> Results { get; set; } = new();
}