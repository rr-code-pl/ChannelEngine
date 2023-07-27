using System.Text.Json.Serialization;

namespace ChannelEngine.BusinessLogic.Models;

public  class MerchantOrderLineResponse
{
    public string Description { get; set; } = string.Empty;
    public string MerchantProductNo { get; set; } = string.Empty;
    public MerchantStockLocationResponse StockLocation { get; set; } = default!;
    public int Quantity { get; set; }
}
