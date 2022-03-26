using Newtonsoft.Json;

namespace CarDealer.DTO.ExportDtos
{
    public class ExportCustomerSalesDto
    {
        [JsonProperty("fullName")]
        public string FullName { get; set; }

        [JsonProperty("boughtCars")]
        public int BoughtCars { get; set; }

        [JsonProperty("spentMoney")]
        public decimal SpentMoney { get; set; }
    }
}
