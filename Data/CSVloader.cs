using System.Globalization;
using DailyProduction.Models;

public static class CsvLoader
{
    public static List<DailyProductionDTO> LoadDailyProduction(string filePath)
    {
        var result = new List<DailyProductionDTO>();

        var lines = File.ReadAllLines(filePath);
        if (lines.Length <= 1) return result; // ingen data

        foreach (var line in lines.Skip(1)) // spring header over
        {
            var parts = line.Split(',');

            if (parts.Length < 5) continue;

            int partitionKey = int.Parse(parts[0]);
            DateTime date = DateTime.Parse(parts[1], CultureInfo.InvariantCulture);
            int itemsProduced = int.Parse(parts[3]);

            BikeModel model = partitionKey switch
            {
                1 => BikeModel.IBv1,
                2 => BikeModel.evIB100,
                3 => BikeModel.evIB200,
                _ => BikeModel.undefined
            };

            result.Add(new DailyProductionDTO
            {
                Date = date,
                Model = model,
                ItemsProduced = itemsProduced
            });
        }

        return result;
    }
}