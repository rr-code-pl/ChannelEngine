using ChannelEngine.BusinessLogic.Models;
using System.Net.Http.Json;
using System.Text;

namespace ChannelEngine.BusinessLogic;

public interface IRestClient
{
    public Task<IReadOnlyCollection<MerchantOrderResponse>> GetOrdersInProgressAsync(CancellationToken token = default);
    public Task<IReadOnlyCollection<MerchantProductResponse>> GetTopProductsAsync(int number, CancellationToken token = default);
    public Task<SingleOfStockUpdateResponse> SetProductStockAsync(string merchantProductNo, int? stockLocationId, int stockSize, CancellationToken token = default);
}

public class RestClient : IRestClient
{
    public List<MerchantOrderResponse> OrderInProgressList { get; set; } = new();
    public List<MerchantProductResponse> TopProducts { get; set; } = new();

    private readonly HttpClient _httpClient;

    public RestClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<MerchantOrderResponse>> GetOrdersInProgressAsync(CancellationToken token = default)
    {
        using HttpResponseMessage apiResponse = await _httpClient.GetAsync(Data.OrdersApiUri, token);

        if (!apiResponse.IsSuccessStatusCode)
        {
            var badResult = await apiResponse.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: token) ?? new();
            throw new RestClientException(badResult.Message);
        }

        var response = await apiResponse.Content.ReadFromJsonAsync<CollectionOfMerchantOrderResponse>(cancellationToken: token);
        var result = response?.Content
            ?? Enumerable.Empty<MerchantOrderResponse>().ToList();

        OrderInProgressList = result;

        return result;
    }

    public async Task<IReadOnlyCollection<MerchantProductResponse>> GetTopProductsAsync(int number, CancellationToken token = default)
    {
        if (OrderInProgressList.Count == 0)
        {
            await GetOrdersInProgressAsync(token);
        }

        var productsInOrders = OrderInProgressList.SelectTopProductsFromOrderList(number);

        var url = $"{GetUrlWithListItemsQuery(Data.ProductsApiUri, "merchantProductNoList", productsInOrders.Select(x => x.MerchantProductNo).ToArray())}";

        using HttpResponseMessage apiResponse = await _httpClient.GetAsync(url, token);

        if (!apiResponse.IsSuccessStatusCode)
        {
            var badResult = await apiResponse.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: token) ?? new();
            throw new RestClientException(badResult.Message);
        }

        var productList = (await apiResponse.Content.ReadFromJsonAsync<CollectionOfMerchantProductResponse>(cancellationToken: token))?.Content
            ?? Enumerable.Empty<MerchantProductResponse>().ToList();

        var result = productsInOrders
            .MergeWithOtherProductList(productList)
            .ToList();

        TopProducts = result;

        return result;
    }

    public async Task<SingleOfStockUpdateResponse> SetProductStockAsync(string merchantProductNo, int? stockLocationId, int stock, CancellationToken token = default)
    {
        var request = new List<MerchantOfferStockUpdateRequest>
        {
            new MerchantOfferStockUpdateRequest
            {
                MerchantProductNo = merchantProductNo,
                StockLocations = new List<MerchantStockLocationUpdateRequest>
                {
                    new MerchantStockLocationUpdateRequest
                    {
                        StockLocationId = stockLocationId,
                        Stock = stock
                    }
                }
            }
        };

        using HttpResponseMessage apiResponse = await _httpClient.PutAsJsonAsync(Data.StockApiUri, request, token);

        if (!apiResponse.IsSuccessStatusCode)
        {
            var badResult = await apiResponse.Content.ReadFromJsonAsync<ApiResponse>(cancellationToken: token) ?? new();

            var exceptionMessage = new StringBuilder(badResult?.Message ?? string.Empty);
            
            if (badResult?.ValidationErrors.Count > 0)
            {
                exceptionMessage.AppendLine();
                exceptionMessage.AppendLine("VALIDATION ERRORS:");

                foreach (var field in badResult.ValidationErrors.Keys)
                {
                    exceptionMessage = exceptionMessage.AppendLine($"* {field}:");
                    foreach(var message in badResult.ValidationErrors[field])
                    {
                        exceptionMessage = exceptionMessage.AppendLine($"  - {message}");
                    }
                }
            }
            
            throw new RestClientException(exceptionMessage.ToString());
        }

        return await apiResponse.Content.ReadFromJsonAsync<SingleOfStockUpdateResponse>(cancellationToken: token) ?? new();
    }

    private static Uri GetUrlWithListItemsQuery(Uri url, string listName, params string[] listItems)
    {
        var uriBuilder = new UriBuilder(url);

        foreach (var item in listItems)
        {
            uriBuilder.Query = string.Join("&", uriBuilder.Query, $"{listName}={item}");
        }

        return uriBuilder.Uri;
    }
}