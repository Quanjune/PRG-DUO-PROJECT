using System;
using System.Collections.Generic;
using System.IO;

class Program
{
    static List<Restaurant> restaurants = new List<Restaurant>();
    static void Main()
    {
        
    }
    static void LoadRestaurants()
    {
        foreach (var line in File.ReadAllLines("restaurants.csv")[1..])
        {
            var parts = line.Split(',');
            restaurants.Add(new Restaurant(parts[0], parts[1], parts[2]));
        }
    }
}
