namespace ChannelEngine.BusinessLogic.Models;

public  class MerchantProductResponse
{
    public string Name { get; set; } = string.Empty;
    public string MerchantProductNo { get; set; } = string.Empty;
    public string? EAN { get; set; }
    public int StockLocationId { get; set; }
    public int Quantity { get; set; }
}
