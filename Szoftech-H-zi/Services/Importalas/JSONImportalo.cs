using MeterologiaiAdatbazis.Interface;
using MeterologiaiAdatbazis.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MeterologiaiAdatbazis.Services.Importalas
{
    public class JSONImportalo : IImportalo
    {
        public string FajlNev => "JSON Importalo";

        // A belső DTO osztály
        private class JsonAdatDto
        {
            public string? timestamp { get; set; }

            // Itt használjuk a lenti konvertert:
            [JsonConverter(typeof(BiztonsagosDoubleKonverter))]
            public double? value { get; set; }

            public string? unit { get; set; }
            public string? source { get; set; }
            public string? sensor { get; set; }
            public string? category { get; set; }
        }

        public (int sikeres, int hibas, List<Adat> lista) Importal(string fajl)
        {
            var lista = new List<Adat>();
            int sikeres = 0;
            int hibas = 0;

            if (!File.Exists(fajl))
                throw new FileNotFoundException("A JSON fájl nem található.", fajl);

            try
            {
                string jsonContent = File.ReadAllText(fajl);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

                // A konverter itt dolgozik
                var dtos = JsonSerializer.Deserialize<List<JsonAdatDto>>(jsonContent, options);

                if (dtos != null)
                {
                    foreach (var elem in dtos)
                    {
                        if (DateTime.TryParse(elem.timestamp, out DateTime ido) &&
                            elem.value.HasValue &&
                            !string.IsNullOrWhiteSpace(elem.unit))
                        {
                            var adat = new Adat(ido, elem.value.Value, elem.unit, AdatEredet.Imported)
                            {
                                SzenzorNev = elem.sensor,
                                Kategoria = elem.category
                            };
                            lista.Add(adat);
                            sikeres++;
                        }
                        else
                        {
                            hibas++;
                        }
                    }
                }
            }
            catch (JsonException)
            {
                hibas++;
            }

            return (sikeres, hibas, lista);
        }
    }

    // 2. A Segéd Konverter Osztály
    public class BiztonsagosDoubleKonverter : JsonConverter<double?>
    {
        public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Ha szám jön
            if (reader.TokenType == JsonTokenType.Number)
                return reader.GetDouble();

            // Ha szöveg jön (pl. "12.5" vagy "EZ_HIBAS")
            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                // Megpróbáljuk számmá alakítani, ha nem megy, null lesz
                if (double.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double val))
                    return val;
            }

            // Minden más esetben null (hiba elnyelése)
            return null;
        }

        public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
        {
            if (value.HasValue) writer.WriteNumberValue(value.Value);
            else writer.WriteNullValue();
        }
    }
}