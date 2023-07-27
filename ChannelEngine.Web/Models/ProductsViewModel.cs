using ChannelEngine.BusinessLogic.Models;

namespace ChannelEngine.Web.Models;

public class ProductsViewModel : BaseViewModel
{
    public List<MerchantProductResponse> Products { get; set; } = new();
}