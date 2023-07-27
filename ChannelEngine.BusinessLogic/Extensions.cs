using ChannelEngine.BusinessLogic.Models;

namespace ChannelEngine.BusinessLogic;

public static class Extensions
{
    public static IEnumerable<MerchantProductResponse> SelectTopProductsFromOrderList(this IEnumerable<MerchantOrderResponse> orderList, int topNumber)
    {
        return orderList.SelectMany(o => o.Lines)
            .GroupBy(x => x.MerchantProductNo)
            .Select(x => new MerchantProductResponse
            {
                MerchantProductNo = x.Key,
                StockLocationId = x.FirstOrDefault()?.StockLocation.Id ?? 0,
                Quantity = x.Sum(y => y.Quantity)
            })
            .OrderByDescending(x => x.Quantity)
            .Take(topNumber);
    }

    public static IEnumerable<MerchantProductResponse> MergeWithOtherProductList(this IEnumerable<MerchantProductResponse> productList, IEnumerable<MerchantProductResponse> productListToBeMerged)
    {
        return from pl in productList
               join plm in productListToBeMerged on pl.MerchantProductNo equals plm.MerchantProductNo
               select new MerchantProductResponse
               {
                   MerchantProductNo = pl.MerchantProductNo,
                   StockLocationId = pl.StockLocationId,
                   Quantity = pl.Quantity,
                   Name = plm.Name,
                   EAN = plm.EAN
               };
    }


}