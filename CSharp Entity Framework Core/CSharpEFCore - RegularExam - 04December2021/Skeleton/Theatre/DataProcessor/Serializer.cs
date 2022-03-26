namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var topTheatres = context.Theatres
                .ToList()
                .Where(x => x.NumberOfHalls >= numbersOfHalls && x.Tickets.Count >= 20)
                .OrderByDescending(x => x.NumberOfHalls)
                .ThenBy(x => x.Name)
                .Select(x => new
                {
                    Name = x.Name,
                    Halls = x.NumberOfHalls,
                    TotalIncome = x.Tickets
                    .Where(x => x.RowNumber >= 1 && x.RowNumber <= 5)
                    .Sum(x => x.Price),
                    Tickets = x.Tickets
                    .Where(x => x.RowNumber >= 1 && x.RowNumber <= 5)
                    .Select(x => new
                    {
                        Price = x.Price,
                        RowNumber = x.RowNumber
                    })
                    .OrderByDescending(x => x.Price)
                    .ToList()
                })
                .ToList();

            string jsonTopTheatres = JsonConvert.SerializeObject(topTheatres, Formatting.Indented);

            return jsonTopTheatres;
        }

        public static string ExportPlays(TheatreContext context, double rating)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Plays");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportPlayDto[]), xmlRoot);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            ExportPlayDto[] dtoActorPlays = context.Plays
                .Where(x => x.Rating <= rating)
                .OrderBy(x => x.Title)
                .ThenByDescending(x => x.Genre)
                .Select(x => new ExportPlayDto
                {
                    Title = x.Title,
                    Duration = x.Duration.ToString("c", CultureInfo.InvariantCulture),
                    Rating = x.Rating.ToString(),
                    Genre = x.Genre,
                    Actors = x.Casts
                    .Where(c => c.IsMainCharacter == true)
                    .Select(y => new ExportPlayActorsDto
                    {
                        FullName = y.FullName,
                        MainCharacter = $"Plays main character in '{y.Play.Title}'."
                    })
                    .OrderByDescending(x => x.FullName)
                    .ToArray()
                })
                .ToArray();

            foreach (ExportPlayDto dtoPlay in dtoActorPlays)
            {
                if (dtoPlay.Rating == "0")
                {
                    dtoPlay.Rating = "Premier";
                }
            }

            xmlSerializer.Serialize(writer, dtoActorPlays, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
