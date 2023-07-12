namespace rsj;

public class Property
{
    public int Id { get; set; }
    public int Area { get; set; }
    public double AreaPercentage { get; set; }

    public Property(int id, int area, double areaPercentage)
    {
        Id = id;
        Area = area;
        AreaPercentage = areaPercentage;
    }
}
