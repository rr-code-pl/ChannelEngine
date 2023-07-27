using System.ComponentModel.DataAnnotations;

namespace ChannelEngine.BusinessLogic.Models
{
    public class MerchantOfferStockUpdateRequest
    {
        [Required(ErrorMessage = "Merchant product no. is required")]
        public string MerchantProductNo { get; set; } = string.Empty;
        [MinLength(1, ErrorMessage = "At least one stock location is required")]
        public List<MerchantStockLocationUpdateRequest> StockLocations { get; set; } = new();
    }
}
