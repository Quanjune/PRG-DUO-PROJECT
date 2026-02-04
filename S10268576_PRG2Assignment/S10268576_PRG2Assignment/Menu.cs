public class Menu
{
    public string MenuId { get; set; }
    public string MenuName { get; set; }
    public List<FoodItem> FoodItems { get; set; }

    public Menu(string menuId, string menuName)
    {
        MenuId = menuId;
        MenuName = menuName;
        FoodItems = new List<FoodItem>();
    }

    public void AddFoodItem(FoodItem item)
    {
        FoodItems.Add(item);
    }
}