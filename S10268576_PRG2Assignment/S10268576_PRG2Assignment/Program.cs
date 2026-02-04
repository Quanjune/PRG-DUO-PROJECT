using System;
using System.Collections.Generic;
using System.IO;

//==========================================================
// Student Number : S10268576
// Student Name : Tang Quan Jun
// Partner Name : Rajakumar Kishore
//==========================================================


// Feature 1: Load files (restaurants and food items)
// Implemented by: S10268576 - QUAN JUN
class Program
{
    static List<Restaurant> restaurants = new List<Restaurant>();
    static void Main()
    {
        LoadRestaurants();
        LoadFoodItems();

        Console.WriteLine($"{restaurants.Count} restaurants loaded!");
        Console.WriteLine();

        ListAllRestaurantsAndMenuItems();
    }
    static void LoadRestaurants()
    {
        foreach (var line in File.ReadAllLines("restaurants.csv")[1..])
        {
            var parts = line.Split(',');
            restaurants.Add(new Restaurant(parts[0], parts[1], parts[2]));
        }
    }

    static void LoadFoodItems()
    {
        foreach (var line in File.ReadAllLines("fooditems.csv")[1..])
        {
            var parts = line.Split(',');
            var restaurant = restaurants.Find(r => r.RestaurantId == parts[0]);
            if (restaurant != null)
            {
                restaurant.Menu.AddFoodItem(
                    new FoodItem(parts[1], parts[2], double.Parse(parts[3]))
                );
            }
        }
    }

    static void ListAllRestaurantsAndMenuItems()
    {
        Console.WriteLine("All Restaurants and Menu Items");
        Console.WriteLine("==============================");

        foreach (var restaurant in restaurants)
        {
            Console.WriteLine($"Restaurant: {restaurant.RestaurantName} ({restaurant.RestaurantId})");
            restaurant.Menu.DisplayFoodItems();
        }
    }


}
