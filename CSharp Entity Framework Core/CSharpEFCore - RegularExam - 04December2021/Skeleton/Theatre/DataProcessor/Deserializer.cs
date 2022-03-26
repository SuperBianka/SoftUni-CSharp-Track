namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";

        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            StringReader reader = new StringReader(xmlString);

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Plays");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPlayDto[]), xmlRoot);

            ImportPlayDto[] dtoPlays = (ImportPlayDto[])xmlSerializer.Deserialize(reader);

            List<Play> validPlays = new List<Play>();

            foreach (ImportPlayDto dtoPlay in dtoPlays)
            {
                if (!IsValid(dtoPlay))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isDurationValid = TimeSpan.TryParseExact(dtoPlay.Duration, "c", CultureInfo.InvariantCulture, TimeSpanStyles.None, out TimeSpan duration);

                if (!isDurationValid || duration.TotalHours < 1)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Play play = new Play()
                {
                    Title = dtoPlay.Title,
                    Duration = duration,
                    Rating = dtoPlay.Rating,
                    Genre = Enum.Parse<Genre>(dtoPlay.Genre),
                    Description = dtoPlay.Description,
                    Screenwriter = dtoPlay.Screenwriter
                };

                validPlays.Add(play);

                sb.AppendLine(string.Format(SuccessfulImportPlay, dtoPlay.Title, dtoPlay.Genre, dtoPlay.Rating));
            }

            context.Plays.AddRange(validPlays);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();
            StringReader reader = new StringReader(xmlString);

            XmlRootAttribute xmlRoot = new XmlRootAttribute("Casts");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCastDto[]), xmlRoot);

            ImportCastDto[] dtoCasts = (ImportCastDto[])xmlSerializer.Deserialize(reader);

            List<Cast> validCasts = new List<Cast>();

            foreach (ImportCastDto dtoCast in dtoCasts)
            {
                if (!IsValid(dtoCast))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Cast cast = new Cast()
                {
                    FullName = dtoCast.FullName,
                    IsMainCharacter = dtoCast.IsMainCharacter,
                    PhoneNumber = dtoCast.PhoneNumber,
                    PlayId = dtoCast.PlayId
                };

                validCasts.Add(cast);

                string characterType = dtoCast.IsMainCharacter ? "main" : "lesser";

                sb.AppendLine(string.Format(SuccessfulImportActor, dtoCast.FullName,
                    characterType));
            }

            context.Casts.AddRange(validCasts);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ICollection<ImportProjectionDto> dtoProjections = JsonConvert.DeserializeObject<ICollection<ImportProjectionDto>>(jsonString);

            List<Theatre> validTheatres = new List<Theatre>();

            foreach (ImportProjectionDto dtoProjection in dtoProjections)
            {
                if (!IsValid(dtoProjection) || dtoProjection.Name == "")
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }               

                Theatre theatre = new Theatre()
                {
                    Name = dtoProjection.Name,
                    NumberOfHalls = dtoProjection.NumberOfHalls,
                    Director = dtoProjection.Director                 
                };

                foreach (ImportProjectionTicketsDto dtoTicket in dtoProjection.Tickets)
                {
                    if (!IsValid(dtoTicket))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Ticket ticket = new Ticket()
                    {
                        Price = dtoTicket.Price,
                        RowNumber = dtoTicket.RowNumber,
                        PlayId = dtoTicket.PlayId
                    };

                    theatre.Tickets.Add(ticket);
                }

                validTheatres.Add(theatre);

                sb.AppendLine(string.Format(SuccessfulImportTheatre, theatre.Name,
                    theatre.Tickets.Count));
            }

            context.Theatres.AddRange(validTheatres);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
