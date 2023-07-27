namespace ChannelEngine.BusinessLogic.Models;

public class CollectionOfMerchantOrderResponse : ApiResponse
{
    public List<MerchantOrderResponse> Content { get; set; } = new();
}
