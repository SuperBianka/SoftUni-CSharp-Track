using Newtonsoft.Json;

namespace CarDealer.DTO.ExportDtos
{
    public class ExportSaleInformationDto
    {
        [JsonProperty("car")]
        public ExportOnlyCarDto Car { get; set; }

        [JsonProperty("customerName")]
        public string CustomerName { get; set; }

        [JsonProperty("Discount")]
        public string Discount { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("priceWithDiscount")]
        public string PriceWithDiscount { get; set; }
    }
}
