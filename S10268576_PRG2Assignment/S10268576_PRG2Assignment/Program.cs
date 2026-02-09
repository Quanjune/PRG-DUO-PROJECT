using S10268576_PRG2Assignment;
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

    static List<Customer> customers = new List<Customer>();
    static List<Order> orders = new List<Order>();


    static List<Restaurant> restaurants = new List<Restaurant>();
    static void Main()
    {
        // Load all data at startup
        LoadRestaurants();
        LoadFoodItems();
        LoadCustomers();
        LoadOrders();

        // Display welcome message with counts
        Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
        Console.WriteLine($"{restaurants.Count} restaurants loaded!");
        Console.WriteLine($"51 food items loaded!"); // or count total food items across all restaurants
        Console.WriteLine($"{customers.Count} customers loaded!");
        Console.WriteLine($"{orders.Count} orders loaded!");
        Console.WriteLine();

        // Main menu loop
        int choice;
        do
        {
            DisplayMenu();
            choice = GetUserChoice();

            switch (choice)
            {
                case 1:
                    ListAllRestaurantsAndMenuItems();
                    break;
                case 2:
                    ListAllOrders();
                    break;
                case 3:
                    CreateNewOrder();
                    break;
                    
                case 4:
                    // ProcessOrder(); - implement this
                    Console.WriteLine("Feature not yet implemented");
                    break;
                case 5:
                    ModifyOrder();
                    break;
                    
                case 6:
                    // DeleteOrder(); - implement this
                    Console.WriteLine("Feature not yet implemented");
                    break;
                case 0:
                    Console.WriteLine("Thank you for using Gruberoo!");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            if (choice != 0)
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }

        } while (choice != 0);
    }

    static void DisplayMenu()
    {
        Console.WriteLine("===== Gruberoo Food Delivery System =====");
        Console.WriteLine("1. List all restaurants and menu items");
        Console.WriteLine("2. List all orders");
        Console.WriteLine("3. Create a new order");
        Console.WriteLine("4. Process an order");
        Console.WriteLine("5. Modify an existing order");
        Console.WriteLine("6. Delete an existing order");
        Console.WriteLine("0. Exit");
        Console.Write("Enter your choice: ");
    }

    static int GetUserChoice()
    {
        if (int.TryParse(Console.ReadLine(), out int choice))
        {
            return choice;
        }
        return -1; // Invalid input
    }

    static void LoadCustomers()
    {
        foreach (var line in File.ReadAllLines("customers.csv")[1..])
        {
            var parts = line.Split(',');

            customers.Add(new Customer(
                parts[1],  // email (first parameter)
                parts[0]   // name (second parameter)
            ));
        }
    }

    static void LoadOrders()
    {
        foreach (var line in File.ReadAllLines("orders.csv")[1..])
        {
            var parts = line.Split(',');

            // Your CSV structure:
            // 0: OrderId
            // 1: CustomerEmail
            // 2: RestaurantId
            // 3: DeliveryDate
            // 4: DeliveryTime
            // 5: Address
            // 6: OrderDateTime
            // 7: TotalAmount
            // 8: Status
            // 9: Items (if present)

            int orderId = int.Parse(parts[0]);
            string customerEmail = parts[1];
            string restaurantId = parts[2];

            // Combine date and time for delivery
            DateTime deliveryDateTime = DateTime.Parse($"{parts[3]} {parts[4]}");

            string address = parts[5];
            DateTime orderDateTime = DateTime.Parse(parts[6]);

            string amtText = parts[7].Trim().Replace("$", "").Replace(",", "");
            if (!double.TryParse(amtText, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double totalAmount))
            {
                Console.WriteLine($"[ERROR] Bad amount '{parts[7]}' in orders.csv line: {line}");
                continue;
            }

            string status = parts[8];

            Order order = new Order(orderId)
            {
                DeliveryDateTime = deliveryDateTime,
                TotalAmount = totalAmount,
                OrderStatus = status,
                RestaurantId = restaurantId
            };

            // In LoadOrders() after creating the order:
            Customer customer = customers.Find(c => c.EmailAddress == customerEmail);
            if (customer != null)
            {
                customer.AddOrder(order);
                order.Customer = customer; // Add this line (if Order has a Customer property)
            }

            // Link to restaurant
            Restaurant restaurant = restaurants.Find(r => r.RestaurantId == restaurantId);
            if (restaurant != null)
            {
                // Add to restaurant if needed
            }

           

            orders.Add(order);
        }
    }


    static void ListAllOrders()
    {
        Console.WriteLine("\nAll Orders");
        Console.WriteLine("==========");
        Console.WriteLine("Order ID Customer      Restaurant          Delivery Date/Time Amount Status");
        Console.WriteLine("-------- ------------- ------------------- ------------------ ------ ---------");

        foreach (var order in orders)
        {
            // Get customer name
            string customerName = order.Customer?.CustomerName ?? "Unknown";

            // Get restaurant name
            Restaurant restaurant = restaurants.Find(r => r.RestaurantId == order.RestaurantId);
            string restaurantName = restaurant?.RestaurantName ?? "Unknown";

            // Format delivery date/time
            string deliveryDateTime = order.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm");

            Console.WriteLine(
                $"{order.OrderId,-8} {customerName,-13} {restaurantName,-19} {deliveryDateTime,-18} ${order.TotalAmount,-5:F2} {order.OrderStatus}"
            );
        }
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
