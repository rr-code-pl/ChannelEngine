namespace ChannelEngine.BusinessLogic.Models;

public class MerchantOrderResponse
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<MerchantOrderLineResponse> Lines { get; set; } = new();
}