

// DONE BY KISHORE

//==========================================================
// Student Number : S10268576
// Student Name : Tang Quan Jun
// Partner Name : Rajakumar Kishore
//==========================================================

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

    public void DisplayFoodItems()
    {
        foreach (var item in FoodItems)
        {
            Console.WriteLine($" - {item.ItemName}: {item.Description} - ${item.Price:F2}");
        }
    }
}