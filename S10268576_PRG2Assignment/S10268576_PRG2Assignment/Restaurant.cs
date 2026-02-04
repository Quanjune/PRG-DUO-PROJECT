

// DONE BY KISHORE

//==========================================================
// Student Number : S10268576
// Student Name : Tang Quan Jun
// Partner Name : Rajakumar Kishore
//==========================================================

public class Restaurant
{
    public string RestaurantId { get; set; }
    public string RestaurantName { get; set; }
    public string RestaurantEmail { get; set; }

    public Menu Menu { get; set; }
    public List<SpecialOffer> SpecialOffers { get; set; }

    public Restaurant(string id, string name, string email)
    {
        RestaurantId = id;
        RestaurantName = name;
        RestaurantEmail = email;
        Menu = new Menu(id, $"{name} Menu");
        SpecialOffers = new List<SpecialOffer>();
    }

    public override string ToString()
    {
        return $"Restaurant: {RestaurantName} ({RestaurantId})";
    }
}