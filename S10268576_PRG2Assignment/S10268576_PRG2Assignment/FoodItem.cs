public class FoodItem
{
    public string ItemName { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }

    public FoodItem(string itemName, string description, double price)
    {
        ItemName = itemName;
        Description = description;
        Price = price;
    }

    public override string ToString()
    {
        return $"{ItemName}: {Description} - ${Price:F2}";
    }
}
