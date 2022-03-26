using System.Collections.Generic;
using Newtonsoft.Json;

namespace CarDealer.DTO.ExportDtos
{
    public class ExportCarPartsDto
    {
        [JsonProperty("car")]
        public ExportOnlyCarDto Car { get; set; }

        [JsonProperty("parts")]
        public ICollection<ExportPartDto> Parts { get; set; }
    }
}
