namespace ChannelEngine.BusinessLogic.Models;

public class SingleOfStockUpdateResponse : ApiResponse
{
    public StockUpdateResponse Content { get; set; } = new();
}
