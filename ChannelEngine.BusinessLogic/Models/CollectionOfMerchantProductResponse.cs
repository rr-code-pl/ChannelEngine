namespace ChannelEngine.BusinessLogic.Models;

public class CollectionOfMerchantProductResponse : ApiResponse
{
    public List<MerchantProductResponse> Content { get; set; } = new();
}
