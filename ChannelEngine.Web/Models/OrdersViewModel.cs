using ChannelEngine.BusinessLogic;
using ChannelEngine.BusinessLogic.Models;

namespace ChannelEngine.Web.Models;

public class OrdersViewModel : BaseViewModel
{
    public List<MerchantOrderResponse> Orders { get; set; } = new();
}
