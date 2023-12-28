using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuickFoodServer.Models
{
    public class ProductCount
    {
        [JsonPropertyName("productId")]
        public int ProductId { get; set; }
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
