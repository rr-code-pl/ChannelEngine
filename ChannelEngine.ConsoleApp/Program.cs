using ChannelEngine.BusinessLogic;
using Microsoft.Extensions.DependencyInjection;

IServiceCollection services = new ServiceCollection();

var serviceProvider = services
    .AddHttpClient()
    .AddSingleton<RestClient>()
    .BuildServiceProvider();

RestClient restClient = serviceProvider.GetRequiredService<RestClient>();

while (true)
{
    try
    {
        Console.WriteLine("Please select an action from a list below:");
        Console.WriteLine("O - Get orders in progress");
        Console.WriteLine("T - Get top products from orders in progress");
        Console.WriteLine("S - Set a product stock size");
        Console.WriteLine("X - Close app");
        Console.WriteLine();
        Console.Write("Selected action: ");

        var option = Console.ReadKey();

        if (option.Key == ConsoleKey.O)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("ORDERS IN PROGRESS");

            await HandleGetOrdersInProgress();
        }
        else if (option.Key == ConsoleKey.T)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("TOP PRODUCTS FROM ORDERS IN PROGRESS");
            
            await HandleGetTopProductsFromOrdersInProgress();
        }
        else if (option.Key == ConsoleKey.S)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("CHANGE PRODUCT STOCK SIZE");

            await HandleChangeProductStockSizeAsync();
        }
        else if (option.Key == ConsoleKey.X)
        {
            break;
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex.Message);
        Console.ResetColor();
    }

    Console.WriteLine();
    Console.WriteLine();
}

async Task HandleGetOrdersInProgress()
{
    Console.WriteLine();
    var result = await restClient.GetOrdersInProgressAsync();

    var maxLength = result.Count == 0 ? 6 : result.Select(x => x.Lines.Select(p => p.Description.Length).Max()).Max() + 1;
    var separator = new string('-', maxLength + 24);

    Console.WriteLine(separator);
    Console.WriteLine(string.Format($"|{{0,-10}}|{{1,{-maxLength}}}|{{2,-10}}|", " Order ID", " Product", " Quantity"));
    Console.WriteLine(separator);

    if (result.Count == 0)
    {
        Console.WriteLine(string.Format($"|{{0,{-maxLength - 24}}}|", " The list is empty"));
        Console.WriteLine(separator);
    }

    foreach (var order in result)
    {
        var id = $"{order.Id}";
        foreach (var product in order.Lines)
        {
            Console.WriteLine(string.Format($"|{{0,-10}}|{{1,{-maxLength}}}|{{2,-10}}|", $" {id}", $" {product.Description}", $" {product.Quantity}"));
            id = string.Empty;
        }
        Console.WriteLine(separator);
    }
}

async Task HandleGetTopProductsFromOrdersInProgress(bool showList = true)
{
    Console.WriteLine();
    Console.Write("Top number: ");

    var selectedNumber = Convert.ToInt32(Console.ReadLine());

    var result = await restClient.GetTopProductsAsync(selectedNumber);

    if (showList)
    {
        var maxNameLength = result.Count == 0 ? 2 : result.Select(x => x.Name.Length).Max() + 2;
        var maxEanLength = result.Count == 0 ? 2 : result.Select(x => x.EAN?.Length ?? 0).Max() + 2;
        var maxLength = maxNameLength + maxEanLength + 14;
        var separator = new string('-', maxLength);

        Console.WriteLine(separator);
        Console.WriteLine(string.Format($"|{{0,{-maxNameLength}}}|{{1,{-maxEanLength}}}|{{2,-10}}|", " Name", " EAN", " Quantity"));
        Console.WriteLine(separator);

        if (result.Count == 0)
        {
            Console.WriteLine(string.Format($"|{{0,{-maxLength}}}|", " The list is empty"));
            Console.WriteLine(separator);
        }

        foreach (var product in result)
        {
            Console.WriteLine(string.Format($"|{{0,{-maxNameLength}}}|{{1,{-maxEanLength}}}|{{2,-10}}|", $" {product.Name}", $" {product.EAN}", $" {product.Quantity}"));
            Console.WriteLine(separator);
        }
    }
}

async Task HandleChangeProductStockSizeAsync()
{
    Console.WriteLine();

    if (restClient.TopProducts.Count == 0)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"WARNING: List of top products is empty. Please provide the top number.");
        Console.ResetColor();
        
        await HandleGetTopProductsFromOrdersInProgress(false);
    }

    Console.WriteLine();
    Console.WriteLine("Select a product from a list below and provide its new stock size: ");

    for (int i = 0; i < restClient.TopProducts.Count; i++)
    {
        var product = restClient.TopProducts.ElementAt(i);
        Console.WriteLine($"{i} - {product.Name}, EAN: {product.EAN}, Quantity: {product.Quantity}");
    }

    Console.WriteLine();
    Console.Write("Selected product: ");

    var selectedNumber = Convert.ToInt32(Console.ReadLine());

    var selectedProduct = restClient.TopProducts.ElementAt(selectedNumber);

    Console.WriteLine();
    Console.Write("New stock size: ");

    var newStockSize = Convert.ToInt32(Console.ReadLine());

    var result = await restClient.SetProductStockAsync(selectedProduct.MerchantProductNo, selectedProduct.StockLocationId, newStockSize);

    if (result?.Content?.Results?.ContainsKey(selectedProduct.MerchantProductNo) == true)
    {
        foreach (var message in result.Content.Results[selectedProduct.MerchantProductNo])
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"WARNING! {message}");
            Console.ResetColor();
        }
    }
    else
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Stock size of \"{selectedProduct.Name}\" updated successfully");
        Console.ResetColor();
    }
}