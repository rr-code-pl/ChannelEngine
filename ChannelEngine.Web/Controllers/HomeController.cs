using ChannelEngine.BusinessLogic;
using ChannelEngine.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ChannelEngine.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly RestClient _restClient;

    public HomeController(
        ILogger<HomeController> logger,
        RestClient restClient)
    {
        _logger = logger;
        _restClient = restClient;
    }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Orders()
    {
        var model = new OrdersViewModel();

        try
        {
            await _restClient.GetOrdersInProgressAsync();
            model.Orders = _restClient.OrderInProgressList;
        }
        catch (RestClientException ex)
        {
            model.Messages.Add(new Message(ex.Message, MessageType.Error));
        }

        return View(model);
    }

    public async Task<IActionResult> Products()
    {
        var model = new ProductsViewModel();

        try
        {
            await _restClient.GetTopProductsAsync(5);
            model.Products = _restClient.TopProducts;
        }
        catch (RestClientException ex)
        {
            model.Messages.Add(new Message(ex.Message, MessageType.Error));
        }

        return View(model);
    }

    public async Task<IActionResult> Stock(string product, int stock, int size)
    {
        var model = new ProductsViewModel();

        try
        {
            var response = await _restClient.SetProductStockAsync(product, stock, size);
            model.Products = _restClient.TopProducts;

            if (response?.Content?.Results?.ContainsKey(product) == true)
            {
                foreach (var message in response.Content.Results[product])
                {
                    model.Messages.Add(new Message($"WARNING! {message}", MessageType.Warning));
                }
            }
            else
            {
                model.Messages.Add(new Message($"Stock size of \"{model.Products.First(x => x.MerchantProductNo == product).Name}\" updated successfully", MessageType.Information));
            }
        }
        catch (RestClientException ex)
        {
            model.Messages.Add(new Message($"{ex.Message}", MessageType.Error));
        }

        return View("Products", model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}