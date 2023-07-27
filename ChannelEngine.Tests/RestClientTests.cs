using ChannelEngine.BusinessLogic;
using ChannelEngine.BusinessLogic.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ChannelEngine.Tests;

public class RestClientTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public async Task GetTopProductsAsync_ReturnsCorrectData_OnSuccess()
    {
        var topNumber = 5;

        var mockHttpMessageHandler = Substitute.ForPartsOf<MockHttpMessageHandler>();
        var httpClient = new HttpClient(mockHttpMessageHandler);

        var fakeOrdersInProgress = new List<MerchantOrderResponse>
        {
            new MerchantOrderResponse
            {
                Id = 1,
                Status = Data.InProgressStatus,
                Lines = new List<MerchantOrderLineResponse>
                {
                    new MerchantOrderLineResponse
                    {
                        MerchantProductNo = "1",
                        Description = "prod1",
                        Quantity = 1,
                        StockLocation = new MerchantStockLocationResponse
                        {
                            Id = 1
                        }
                    }
                }
            },
            new MerchantOrderResponse
            {
                Id = 2,
                Status = Data.InProgressStatus,
                Lines = new List<MerchantOrderLineResponse>
                {
                    new MerchantOrderLineResponse
                    {
                        MerchantProductNo = "2",
                        Description = "prod2",
                        Quantity = 11,
                        StockLocation = new MerchantStockLocationResponse
                        {
                            Id = 1
                        }
                    }
                }
            },
            new MerchantOrderResponse
            {
                Id = 3,
                Status = Data.InProgressStatus,
                Lines = new List<MerchantOrderLineResponse>
                {
                    new MerchantOrderLineResponse
                    {
                        MerchantProductNo = "2",
                        Description = "prod2",
                        Quantity = 11,
                        StockLocation = new MerchantStockLocationResponse
                        {
                            Id = 1
                        }
                    },
                    new MerchantOrderLineResponse
                    {
                        MerchantProductNo = "3",
                        Description = "prod3",
                        Quantity = 3,
                        StockLocation = new MerchantStockLocationResponse
                        {
                            Id = 1
                        }
                    }
                }
            },
            new MerchantOrderResponse
            {
                Id = 4,
                Status = Data.InProgressStatus,
                Lines = new List<MerchantOrderLineResponse>
                {
                    new MerchantOrderLineResponse
                    {
                        MerchantProductNo = "4",
                        Description = "prod4",
                        Quantity = 4,
                        StockLocation = new MerchantStockLocationResponse
                        {
                            Id = 1
                        }
                    }
                }
            },
            new MerchantOrderResponse
            {
                Id = 5,
                Status = Data.InProgressStatus,
                Lines = new List<MerchantOrderLineResponse>
                {
                    new MerchantOrderLineResponse
                    {
                        MerchantProductNo = "5",
                        Description = "prod5",
                        Quantity = 5,
                        StockLocation = new MerchantStockLocationResponse
                        {
                            Id = 1
                        }
                    }
                }
            }
        };

        var productsInOrders = fakeOrdersInProgress
            .SelectTopProductsFromOrderList(topNumber);

        var fakeProductsResponse = new CollectionOfMerchantProductResponse
        {
            Content = new List<MerchantProductResponse>
            {
                new MerchantProductResponse
                {
                    MerchantProductNo = "1",
                    Name = "prod1",
                    EAN = "p1"
                },
                new MerchantProductResponse
                {
                    MerchantProductNo = "1",
                    Name = "prod1",
                    EAN = "p1"
                },
                new MerchantProductResponse
                {
                    MerchantProductNo = "1",
                    Name = "prod1",
                    EAN = "p1"
                },
                new MerchantProductResponse
                {
                    MerchantProductNo = "1",
                    Name = "prod1",
                    EAN = "p1"
                },
                new MerchantProductResponse
                {
                    MerchantProductNo = "1",
                    Name = "prod1",
                    EAN = "p1"
                },
            },
            Success = true,
            StatusCode = 200
        };

        var fakeContent = JsonContent.Create(fakeProductsResponse);

        var fakeResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = fakeContent
        };

        mockHttpMessageHandler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(fakeResponse);

        var restClient = new RestClient(httpClient);
        restClient.OrderInProgressList = fakeOrdersInProgress;

        var expectedResult = productsInOrders
            .MergeWithOtherProductList(fakeProductsResponse.Content);

        var task = await restClient.GetTopProductsAsync(topNumber);
        
        task.Should().BeEquivalentTo(expectedResult);
    }

    [Test]
    public async Task GetTopProductsAsync_ThrowsException_OnError()
    {
        var errorMessage = "An error occured";

        var mockHttpMessageHandler = Substitute.ForPartsOf<MockHttpMessageHandler>();
        var httpClient = new HttpClient(mockHttpMessageHandler);

        var fakeApiResponse = new ApiResponse
        {
            StatusCode = 400,
            Success = false,
            Message = errorMessage
        };

        var fakeContent = JsonContent.Create(fakeApiResponse);

        var fakeResponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = fakeContent
        };

        mockHttpMessageHandler.MockSend(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
            .Returns(fakeResponse);

        var restClient = new RestClient(httpClient);

        await Invoking(() => restClient.GetTopProductsAsync(5)).Should().ThrowAsync<RestClientException>().WithMessage(errorMessage);
    }
}