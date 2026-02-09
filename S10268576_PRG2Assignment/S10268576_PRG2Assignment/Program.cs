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
    static void CreateNewOrder()
    {
        Console.WriteLine("\nCreate New Order");
        Console.WriteLine("================");

        // Step 1: Get Customer Email
        Console.Write("Enter Customer Email: ");
        string customerEmail = Console.ReadLine();

        Customer selectedCustomer = customers.Find(c => c.EmailAddress == customerEmail);
        if (selectedCustomer == null)
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        // Step 2: Get Restaurant ID
        Console.Write("Enter Restaurant ID: ");
        string restaurantId = Console.ReadLine();

        Restaurant selectedRestaurant = restaurants.Find(r => r.RestaurantId == restaurantId);
        if (selectedRestaurant == null)
        {
            Console.WriteLine("Restaurant not found.");
            return;
        }

        // Step 3: Get Delivery Date
        Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
        if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime deliveryDate))
        {
            Console.WriteLine("Invalid date format.");
            return;
        }

        // Step 4: Get Delivery Time
        Console.Write("Enter Delivery Time (hh:mm): ");
        if (!DateTime.TryParseExact(Console.ReadLine(), "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime deliveryTime))
        {
            Console.WriteLine("Invalid time format.");
            return;
        }

        // Step 5: Get Delivery Address
        Console.Write("Enter Delivery Address: ");
        string deliveryAddress = Console.ReadLine();

        // Step 6: Display menu and add items
        Console.WriteLine("\nAvailable Food Items:");
        var menuItems = selectedRestaurant.Menu.FoodItems;
        for (int i = 0; i < menuItems.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {menuItems[i].ItemName} - ${menuItems[i].Price:F2}");
        }

        // Generate new order ID
        int newOrderId = orders.Count > 0 ? orders.Max(o => o.OrderId) + 1 : 1;
        Order newOrder = new Order(newOrderId)
        {
            Customer = selectedCustomer,
            RestaurantId = selectedRestaurant.RestaurantId,
            OrderStatus = "Pending",
            DeliveryAddress = deliveryAddress,
            DeliveryDateTime = new DateTime(
                deliveryDate.Year, deliveryDate.Month, deliveryDate.Day,
                deliveryTime.Hour, deliveryTime.Minute, 0
            )
        };

        // Add food items to order
        while (true)
        {
            Console.Write("Enter item number (0 to finish): ");
            if (!int.TryParse(Console.ReadLine(), out int itemChoice))
            {
                Console.WriteLine("Invalid input.");
                continue;
            }

            if (itemChoice == 0)
            {
                if (newOrder.OrderedItems.Count == 0)
                {
                    Console.WriteLine("You must add at least one item to the order.");
                    continue;
                }
                break;
            }

            if (itemChoice < 1 || itemChoice > menuItems.Count)
            {
                Console.WriteLine("Invalid item number.");
                continue;
            }

            FoodItem selectedFood = menuItems[itemChoice - 1];

            Console.Write("Enter quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 1)
            {
                Console.WriteLine("Invalid quantity.");
                continue;
            }

            // Check if item already in order, if so update quantity
            var existingItem = newOrder.OrderedItems.Find(oi => oi.ItemName == selectedFood.ItemName);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                OrderedFoodItem orderedItem = new OrderedFoodItem(
                    selectedFood.ItemName,
                    selectedFood.Price,
                    quantity
                );
                newOrder.AddOrderedFoodItem(orderedItem);
            }
        }

        // Step 7: Special request
        Console.Write("Add special request? [Y/N]: ");
        string specialRequestChoice = Console.ReadLine()?.ToUpper();

        string specialRequest = "";
        if (specialRequestChoice == "Y")
        {
            Console.Write("Enter special request: ");
            specialRequest = Console.ReadLine();

        }

        newOrder.SpecialRequest = specialRequest;

        // Note: If you need to store the special request, you may need to add a property to the Order class
        // For example: newOrder.SpecialRequest = specialRequest;

        // Step 8: Calculate total
        double subtotal = newOrder.CalculateOrderTotal();
        double deliveryFee = 5.00;
        double totalAmount = subtotal + deliveryFee;

        Console.WriteLine($"\nOrder Total: ${subtotal:F2} + ${deliveryFee:F2} (delivery) = ${totalAmount:F2}");

        // Step 9: Proceed to payment
        Console.Write("Proceed to payment? [Y/N]: ");
        string proceedChoice = Console.ReadLine()?.ToUpper();

        if (proceedChoice != "Y")
        {
            Console.WriteLine("Order cancelled.");
            return;
        }

        // Step 10: Payment method
        Console.Write("Payment method: [CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
        string paymentMethod = Console.ReadLine()?.ToUpper();

        // Validate payment method
        if (paymentMethod != "CC" && paymentMethod != "PP" && paymentMethod != "CD")
        {
            Console.WriteLine("Invalid payment method.");
            return;
        }

        // Map payment method codes to full names
        switch (paymentMethod)
        {
            case "CC":
                newOrder.PaymentMethod = "Credit Card";
                break;
            case "PP":
                newOrder.PaymentMethod = "PayPal";
                break;
            case "CD":
                newOrder.PaymentMethod = "Cash on Delivery";
                break;
        }

        // Set the total amount (including delivery)
        newOrder.TotalAmount = totalAmount;

        // Add to lists
        orders.Add(newOrder);
        selectedCustomer.AddOrder(newOrder);

        // Save to CSV
        SaveOrdersToCSV();

        Console.WriteLine($"\nOrder {newOrder.OrderId} created successfully!");
        Console.WriteLine($"Status: {newOrder.OrderStatus}");
    }

    static void ModifyOrder()
    {
        Console.WriteLine("\nModify Order");
        Console.WriteLine("============");

        // Step 1: Get Customer Email
        Console.Write("Enter Customer Email: ");
        string customerEmail = Console.ReadLine();

        Customer selectedCustomer = customers.Find(c => c.EmailAddress == customerEmail);
        if (selectedCustomer == null)
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        // Step 2: Display pending orders for this customer
        var customerPendingOrders = selectedCustomer.Orders.Where(o => o.OrderStatus == "Pending").ToList();

        if (customerPendingOrders.Count == 0)
        {
            Console.WriteLine("No pending orders found for this customer.");
            return;
        }

        Console.WriteLine("Pending Orders:");
        foreach (var order in customerPendingOrders)
        {
            Console.Write($"{order.OrderId} ");
        }
        Console.WriteLine(); // New line after order IDs

        // Step 3: Get Order ID to modify
        Console.Write("Enter Order ID: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid Order ID.");
            return;
        }

        Order orderToModify = customerPendingOrders.Find(o => o.OrderId == orderId);
        if (orderToModify == null)
        {
            Console.WriteLine("Order not found.");
            return;
        }

        // Step 4: Display order details
        Console.WriteLine("Order Items:");
        for (int i = 0; i < orderToModify.OrderedItems.Count; i++)
        {
            var item = orderToModify.OrderedItems[i];
            Console.WriteLine($"{i + 1}. {item.ItemName} - {item.Quantity}");
        }

        Console.WriteLine($"Address: {orderToModify.DeliveryAddress}");
        Console.WriteLine($"Delivery Date/Time: {orderToModify.DeliveryDateTime:d/M/yyyy, HH:mm}");

        // Step 5: Choose what to modify
        Console.Write("Modify: [1] Items [2] Address [3] Delivery Time: ");
        if (!int.TryParse(Console.ReadLine(), out int modifyChoice))
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        Restaurant orderRestaurant = restaurants.Find(r => r.RestaurantId == orderToModify.RestaurantId);
        double deliveryFee = 5.00;

        switch (modifyChoice)
        {
            case 1: // Modify Items
                bool modifyingItems = true;
                while (modifyingItems)
                {
                    Console.WriteLine("\nItem Modification:");
                    Console.WriteLine("[1] Add item");
                    Console.WriteLine("[2] Remove item");
                    Console.WriteLine("[3] Change quantity");
                    Console.WriteLine("[0] Done");
                    Console.Write("Choose option: ");

                    if (!int.TryParse(Console.ReadLine(), out int itemChoice))
                    {
                        Console.WriteLine("Invalid choice.");
                        continue;
                    }

                    switch (itemChoice)
                    {
                        case 1: // Add item
                            Console.WriteLine("\nAvailable Food Items:");
                            var menuItems = orderRestaurant.Menu.FoodItems;
                            for (int i = 0; i < menuItems.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {menuItems[i].ItemName} - ${menuItems[i].Price:F2}");
                            }

                            Console.Write("Enter item number: ");
                            if (!int.TryParse(Console.ReadLine(), out int addItemNum) || addItemNum < 1 || addItemNum > menuItems.Count)
                            {
                                Console.WriteLine("Invalid item number.");
                                break;
                            }

                            FoodItem selectedFood = menuItems[addItemNum - 1];

                            Console.Write("Enter quantity: ");
                            if (!int.TryParse(Console.ReadLine(), out int qty) || qty < 1)
                            {
                                Console.WriteLine("Invalid quantity.");
                                break;
                            }

                            var existingItem = orderToModify.OrderedItems.Find(oi => oi.ItemName == selectedFood.ItemName);
                            if (existingItem != null)
                            {
                                existingItem.Quantity += qty;
                                Console.WriteLine($"Updated {selectedFood.ItemName} quantity to {existingItem.Quantity}");
                            }
                            else
                            {
                                OrderedFoodItem orderedItem = new OrderedFoodItem(
                                    selectedFood.ItemName,
                                    selectedFood.Price,
                                    qty
                                );
                                orderToModify.AddOrderedFoodItem(orderedItem);
                                Console.WriteLine($"Added {qty} x {selectedFood.ItemName}");
                            }

                            orderToModify.TotalAmount = orderToModify.CalculateOrderTotal() + deliveryFee;
                            break;

                        case 2: // Remove item
                            if (orderToModify.OrderedItems.Count == 1)
                            {
                                Console.WriteLine("Cannot remove the last item. Order must have at least one item.");
                                break;
                            }

                            Console.WriteLine("\nCurrent Items:");
                            for (int i = 0; i < orderToModify.OrderedItems.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {orderToModify.OrderedItems[i].ItemName} - {orderToModify.OrderedItems[i].Quantity}");
                            }

                            Console.Write("Enter item number to remove: ");
                            if (!int.TryParse(Console.ReadLine(), out int removeNum) || removeNum < 1 || removeNum > orderToModify.OrderedItems.Count)
                            {
                                Console.WriteLine("Invalid item number.");
                                break;
                            }

                            var removedItem = orderToModify.OrderedItems[removeNum - 1];
                            orderToModify.OrderedItems.RemoveAt(removeNum - 1);
                            Console.WriteLine($"Removed {removedItem.ItemName}");

                            orderToModify.TotalAmount = orderToModify.CalculateOrderTotal() + deliveryFee;
                            break;

                        case 3: // Change quantity
                            Console.WriteLine("\nCurrent Items:");
                            for (int i = 0; i < orderToModify.OrderedItems.Count; i++)
                            {
                                Console.WriteLine($"{i + 1}. {orderToModify.OrderedItems[i].ItemName} - {orderToModify.OrderedItems[i].Quantity}");
                            }

                            Console.Write("Enter item number: ");
                            if (!int.TryParse(Console.ReadLine(), out int changeNum) || changeNum < 1 || changeNum > orderToModify.OrderedItems.Count)
                            {
                                Console.WriteLine("Invalid item number.");
                                break;
                            }

                            var itemToChange = orderToModify.OrderedItems[changeNum - 1];
                            Console.Write($"Enter new quantity for {itemToChange.ItemName}: ");
                            if (!int.TryParse(Console.ReadLine(), out int newQty) || newQty < 1)
                            {
                                Console.WriteLine("Invalid quantity.");
                                break;
                            }

                            itemToChange.Quantity = newQty;
                            Console.WriteLine($"Updated {itemToChange.ItemName} quantity to {newQty}");

                            orderToModify.TotalAmount = orderToModify.CalculateOrderTotal() + deliveryFee;
                            break;

                        case 0: // Done
                            modifyingItems = false;
                            break;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }
                }

                SaveOrdersToCSV();
                Console.WriteLine($"\nOrder {orderToModify.OrderId} updated.");
                Console.WriteLine("Updated Items:");
                for (int i = 0; i < orderToModify.OrderedItems.Count; i++)
                {
                    var item = orderToModify.OrderedItems[i];
                    Console.WriteLine($"{i + 1}. {item.ItemName} - {item.Quantity}");
                }
                break;

            case 2: // Modify Address
                Console.Write("Enter new Address: ");
                string newAddress = Console.ReadLine();
                orderToModify.DeliveryAddress = newAddress;

                SaveOrdersToCSV();
                Console.WriteLine($"\nOrder {orderToModify.OrderId} updated.");
                Console.WriteLine($"New Address: {newAddress}");
                break;

            case 3: // Modify Delivery Time
                Console.Write("Enter new Delivery Time (hh:mm): ");
                if (!DateTime.TryParseExact(Console.ReadLine(), "HH:mm", null, System.Globalization.DateTimeStyles.None, out DateTime newTime))
                {
                    Console.WriteLine("Invalid time format.");
                    return;
                }

                // Keep the same date, just change the time
                orderToModify.DeliveryDateTime = new DateTime(
                    orderToModify.DeliveryDateTime.Year,
                    orderToModify.DeliveryDateTime.Month,
                    orderToModify.DeliveryDateTime.Day,
                    newTime.Hour,
                    newTime.Minute,
                    0
                );

                SaveOrdersToCSV();
                Console.WriteLine($"\nOrder {orderToModify.OrderId} updated.");
                Console.WriteLine($"New Delivery Time: {newTime:HH:mm}");
                break;

            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }

    static void SaveOrdersToCSV()
    {
        using (StreamWriter writer = new StreamWriter("orders.csv"))
        {
            // Write header
            writer.WriteLine("OrderId,CustomerEmail,RestaurantId,DeliveryDate,DeliveryTime,Address,OrderDateTime,TotalAmount,Status");

            // Write each order
            foreach (var order in orders)
            {
                string customerEmail = order.Customer?.EmailAddress ?? "";
                string deliveryDate = order.DeliveryDateTime.ToString("dd/MM/yyyy");
                string deliveryTime = order.DeliveryDateTime.ToString("HH:mm");
                string orderDateTime = order.OrderDateTime.ToString("dd/MM/yyyy HH:mm");

                writer.WriteLine($"{order.OrderId},{customerEmail},{order.RestaurantId},{deliveryDate},{deliveryTime},{order.DeliveryAddress},{orderDateTime},{order.TotalAmount:F2},{order.OrderStatus}");
            }
        }
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
