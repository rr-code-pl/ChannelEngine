using ChannelEngine.BusinessLogic.Models;

namespace ChannelEngine.BusinessLogic
{
    public static class Data
    {

        public const string ApiKey = "541b989ef78ccb1bad630ea5b85c6ebff9ca3322";
        public const string InProgressStatus = "IN_PROGRESS";
        public const string BaseApiUrl = "https://api-dev.channelengine.net/api/v2/";
        public static Uri OrdersApiUri = new Uri($"{BaseApiUrl}orders?statuses={InProgressStatus}&apikey={ApiKey}");
        public static Uri ProductsApiUri = new Uri($"{BaseApiUrl}products?apikey={ApiKey}");
        public static Uri StockApiUri = new Uri($"{BaseApiUrl}offer/stock?apikey={ApiKey}");
    }
}
