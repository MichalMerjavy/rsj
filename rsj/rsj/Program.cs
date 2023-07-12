// See https://aka.ms/new-console-template for more information
using rsj;
using System.Globalization;

List<Transaction> transactions =  Read("Smlouvy.txt");
var properties = GetPropertiesDate(transactions,
    DateTime.ParseExact("2023-08-14", "yyyy-MM-dd", CultureInfo.InvariantCulture), out double area);
Console.WriteLine($"Number of properties: {properties.Count}, area: {area}");
Console.ReadLine();



static List<Transaction> Read(string filePath)
{
    List<Transaction> transactions = new List<Transaction>();
    Transaction? currentTransaction = null;
    string[] lines = File.ReadAllLines(filePath);

    foreach (string line in lines)
    {
        if (line.StartsWith("Nakup") || line.StartsWith("Prodej"))
        {
            string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string type = parts[0];
            string date = parts[1];
            currentTransaction = new Transaction(type == "Nakup" ? true : false,
                DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture));
            transactions.Add(currentTransaction);
        }
        else
        {
            string[] entryParts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (entryParts.Length == 0)
            {
                continue;
            }

            int id = int.Parse(entryParts[0]);
            int area = int.Parse(entryParts[1]);
            double areaPercentage = double.Parse(entryParts[2]);
            Property entry = new Property(id, area, areaPercentage);

            if (currentTransaction != null)
            {
                currentTransaction.Properties.Add(entry);
            }
        }
    }

    return transactions;
}

static List<Property> GetPropertiesDate(List<Transaction> transactions, DateTime date, out double area)
{
    transactions.Sort((x, y) => DateTime.Compare(x.Date, y.Date));
    Dictionary<int, Property> properties = new Dictionary<int, Property>();
    area = 0;

    foreach (Transaction transaction in transactions)
    {
        if (transaction.Date > date)
        {
            return properties.Values.ToList();
        }

        if (transaction.Bought)
        {
            foreach (Property property in transaction.Properties)
            {
                if (property.AreaPercentage < 0)
                {
                    continue;
                }

                if (property.AreaPercentage > 1) 
                {
                    property.AreaPercentage = 1;
                }

                if (!properties.TryAdd(property.Id, property))
                {
                    if (properties[property.Id].AreaPercentage + property.AreaPercentage > 1)
                    {
                        // kupene viac ako 100%
                        area += (1 - properties[property.Id].AreaPercentage) * properties[property.Id].Area;
                        properties[property.Id].AreaPercentage = 1;
                        continue;
                    }

                    properties[property.Id].AreaPercentage += property.AreaPercentage;
                }

                area += property.Area * property.AreaPercentage;
            }
        }
        else
        {
            foreach (Property property in transaction.Properties)
            {
                if (property.AreaPercentage < 0)
                {
                    continue;
                }

                if (property.AreaPercentage > 1)
                {
                    property.AreaPercentage = 1;
                }

                if (!properties.ContainsKey(property.Id))
                {
                    // predava sa property ktore sa nekupilo
                    continue;
                }

                if ((properties[property.Id].AreaPercentage - property.AreaPercentage) < 0)
                {
                    // predava sa viac ako vlastnim
                    area -= property.Area * properties[property.Id].AreaPercentage;
                    properties.Remove(property.Id);
                    continue;
                }

                if ((properties[property.Id].AreaPercentage - property.AreaPercentage) == 0)
                {
                    area -= property.Area * property.AreaPercentage;
                    properties.Remove(property.Id);
                }
                else
                {
                    properties[property.Id].AreaPercentage -= property.AreaPercentage;
                    area -= property.Area * property.AreaPercentage;
                }
            }
        }
    }

    return properties.Values.ToList();
}

