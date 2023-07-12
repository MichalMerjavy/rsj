namespace rsj;

public class Transaction
{
    public bool Bought { get; set; }
    public DateTime Date { get; set; }
    public List<Property> Properties { get; set; } = new List<Property>();

    public Transaction(bool bought, DateTime date)
    {
        Bought = bought;
        Date = date;
    }
}
