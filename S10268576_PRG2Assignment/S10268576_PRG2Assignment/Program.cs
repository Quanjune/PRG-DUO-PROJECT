using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static List<Restaurant> restaurants = new List<Restaurant>();
    static void Main()
    {
        LoadRestaurants();
        LoadFoodItems();

        Console.WriteLine($"{restaurants.Count} restaurants loaded!");
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
}
