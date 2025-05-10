using System.Text.Json.Serialization;

namespace WebSale.Dto.Orders
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OrderPaymentStatus
    {
        COD,
        VnPay
    }
}
