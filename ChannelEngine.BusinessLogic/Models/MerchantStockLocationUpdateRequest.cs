using System.ComponentModel.DataAnnotations;

namespace ChannelEngine.BusinessLogic.Models;

public class MerchantStockLocationUpdateRequest
{
    [Range(0, int.MaxValue, ErrorMessage = "Stock can't have negative value")]
    public int Stock { get; set; }
    public int? StockLocationId { get; set; }
}