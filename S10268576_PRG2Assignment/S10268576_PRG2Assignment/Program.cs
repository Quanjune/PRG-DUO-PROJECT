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
        int totalFoodItems = 0;
        foreach (var restaurant in restaurants)
        {
            totalFoodItems += restaurant.Menu.FoodItems.Count;
        }
        Console.WriteLine($"{totalFoodItems} food items loaded!"); // or count total food items across all restaurants
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
                    ProcessOrder();
                    break;
                case 5:
                    ModifyOrder();
                    break;

                case 6:
                    DeleteOrder();
                    break;
                case 7:
                    DisplayTotalOrderAmount();
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
        Console.WriteLine("7. Display total order amount");
        Console.WriteLine("0. Exit");
        Console.Write("Enter your choice: ");
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
        int newOrderId = 1;
        if (orders.Count > 0)
        {
            int maxId = 0;
            foreach (var order in orders)
            {
                if (order.OrderId > maxId)
                {
                    maxId = order.OrderId;
                }
            }
            newOrderId = maxId + 1;
        }


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
        Console.WriteLine("\nPayment method:");
        Console.Write("[CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");
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
        List<Order> customerPendingOrders = new List<Order>();
        foreach (var order in selectedCustomer.Orders)
        {
            if (order.OrderStatus == "Pending")
            {
                customerPendingOrders.Add(order);
            }
        }

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
        string[] lines = File.ReadAllLines("customers.csv");
        for (int i = 1; i < lines.Length; i++) // Start from index 1 to skip header
        {
            var parts = lines[i].Split(',');

            customers.Add(new Customer(
                parts[1],  // email
                parts[0]   // name
            ));
        }
    }


    static void LoadOrders()
    {
        string[] lines = File.ReadAllLines("orders.csv");

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;

            List<string> parts = new List<string>();
            bool inQuotes = false;
            string field = "";

            foreach (char ch in lines[i])
            {
                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (ch == ',' && !inQuotes)
                {
                    parts.Add(field);
                    field = "";
                }
                else
                {
                    field += ch;
                }
            }
            parts.Add(field); // last field

            if (parts.Count < 10)
                continue;

            if (!int.TryParse(parts[0], out int orderId))
                continue;

            string customerEmail = parts[1];
            string restaurantId = parts[2];

            // Parse delivery date and time
            if (!DateTime.TryParse($"{parts[3]} {parts[4]}", out DateTime deliveryDateTime))
                continue;

            string address = parts[5];

            // Parse order date/time (this is in ONE field with both date and time)
            if (!DateTime.TryParse(parts[6], out DateTime orderDateTime))
                continue;

            // Parse total amount
            string amtText = parts[7].Trim().Replace("$", "");
            if (!double.TryParse(amtText, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double totalAmount))
                continue;

            string status = parts[8];

            Order order = new Order(orderId)
            {
                DeliveryDateTime = deliveryDateTime,
                DeliveryAddress = address,
                OrderDateTime = orderDateTime,
                TotalAmount = totalAmount,
                OrderStatus = status,
                RestaurantId = restaurantId
            };

            // Load items
            if (!string.IsNullOrWhiteSpace(parts[9]))
            {
                Restaurant restaurant = null;
                for (int j = 0; j < restaurants.Count; j++)
                {
                    if (restaurants[j].RestaurantId == restaurantId)
                    {
                        restaurant = restaurants[j];
                        break;
                    }
                }

                if (restaurant != null)
                {
                    string itemsString = parts[9];
                    string[] itemPairs = itemsString.Split('|');

                    foreach (string pair in itemPairs)
                    {
                        string[] itemData = pair.Split(',');

                        if (itemData.Length >= 2)
                        {
                            string itemName = itemData[0].Trim();

                            // Get the last element as quantity
                            if (int.TryParse(itemData[itemData.Length - 1].Trim(), out int qty))
                            {
                                FoodItem food = null;
                                for (int m = 0; m < restaurant.Menu.FoodItems.Count; m++)
                                {
                                    if (restaurant.Menu.FoodItems[m].ItemName == itemName)
                                    {
                                        food = restaurant.Menu.FoodItems[m];
                                        break;
                                    }
                                }

                                if (food != null)
                                {
                                    order.AddOrderedFoodItem(
                                        new OrderedFoodItem(itemName, food.Price, qty)
                                    );
                                }
                            }
                        }
                    }
                }
            }

            Customer customer = null;
            for (int j = 0; j < customers.Count; j++)
            {
                if (customers[j].EmailAddress == customerEmail)
                {
                    customer = customers[j];
                    break;
                }
            }

            if (customer != null)
            {
                customer.AddOrder(order);
                order.Customer = customer;
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
        string[] lines = File.ReadAllLines("restaurants.csv");
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');
            restaurants.Add(new Restaurant(parts[0], parts[1], parts[2]));
        }
    }




    static void SaveOrdersToCSV()
    {
        using (StreamWriter writer = new StreamWriter("orders.csv"))
        {
            // Write header
            writer.WriteLine("OrderId,CustomerEmail,RestaurantId,DeliveryDate,DeliveryTime,DeliveryAddress,CreatedDateTime,TotalAmount,Status,Items");

            // Write each order
            foreach (var order in orders)
            {
                string customerEmail = order.Customer?.EmailAddress ?? "";
                string deliveryDate = order.DeliveryDateTime.ToString("dd/MM/yyyy");
                string deliveryTime = order.DeliveryDateTime.ToString("HH:mm");
                string orderDateTime = order.OrderDateTime.ToString("dd/MM/yyyy HH:mm");

                // Format items as: "ItemName1,Quantity1|ItemName2,Quantity2"
                string itemsString = "";
                for (int i = 0; i < order.OrderedItems.Count; i++)
                {
                    var item = order.OrderedItems[i];
                    itemsString += $"{item.ItemName},{item.Quantity}";  // ← CHANGED : to ,
                    if (i < order.OrderedItems.Count - 1)
                    {
                        itemsString += "|";
                    }
                }

                // Wrap items in quotes since they contain commas
                writer.WriteLine($"{order.OrderId},{customerEmail},{order.RestaurantId},{deliveryDate},{deliveryTime},{order.DeliveryAddress},{orderDateTime},{order.TotalAmount:F2},{order.OrderStatus},\"{itemsString}\"");  // ← Added quotes
            }
        }
    }

    static void LoadFoodItems()
    {
        string[] lines = File.ReadAllLines("fooditems.csv");
        for (int i = 1; i < lines.Length; i++)
        {
            var parts = lines[i].Split(',');
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

            Console.WriteLine(); // ⭐ ADD THIS LINE (blank spacing between restaurants)
        }
    }


    static Stack<Order> refundStack = new Stack<Order>();

    static void ProcessOrder()
    {
        Console.WriteLine("\nProcess Order");
        Console.WriteLine("=============");

        Console.Write("Enter Restaurant ID: ");
        string restaurantId = Console.ReadLine();

        List<Order> restaurantOrders = new List<Order>();
        foreach (var order in orders)
        {
            if (order.RestaurantId == restaurantId)
            {
                restaurantOrders.Add(order);
            }
        }

        if (restaurantOrders.Count == 0)
        {
            Console.WriteLine("No orders found for this restaurant.");
            return;
        }

        foreach (var order in restaurantOrders)
        {
            Console.WriteLine($"\nOrder {order.OrderId}:");
            Console.WriteLine($"Customer: {order.Customer?.CustomerName}");

            Console.WriteLine("Ordered Items:");
            for (int i = 0; i < order.OrderedItems.Count; i++)
            {
                var item = order.OrderedItems[i];
                Console.WriteLine($"{i + 1}. {item.ItemName} - {item.Quantity}");
            }

            Console.WriteLine($"Delivery date/time: {order.DeliveryDateTime:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Total Amount: ${order.TotalAmount:F2}");
            Console.WriteLine($"Order Status: {order.OrderStatus}");

            Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
            string action = Console.ReadLine()?.ToUpper();

            switch (action)
            {
                case "C":
                    if (order.OrderStatus == "Pending")
                    {
                        order.OrderStatus = "Preparing";
                        Console.WriteLine($"Order {order.OrderId} confirmed. Status: Preparing");
                    }
                    else
                    {
                        Console.WriteLine("Order cannot be confirmed.");
                    }
                    break;

                case "R":
                    if (order.OrderStatus == "Pending")
                    {
                        order.OrderStatus = "Rejected";
                        refundStack.Push(order);
                        Console.WriteLine($"Order {order.OrderId} rejected. Refund of ${order.TotalAmount:F2} processed.");
                    }
                    else
                    {
                        Console.WriteLine("Order cannot be rejected.");
                    }
                    break;

                case "S":
                    if (order.OrderStatus == "Cancelled")
                    {
                        Console.WriteLine("Order skipped.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Only cancelled orders can be skipped.");
                    }
                    break;

                case "D":
                    if (order.OrderStatus == "Preparing")
                    {
                        order.OrderStatus = "Delivered";
                        Console.WriteLine($"Order {order.OrderId} delivered.");
                    }
                    else
                    {
                        Console.WriteLine("Order cannot be delivered.");
                    }
                    break;

                default:
                    Console.WriteLine("Invalid action.");
                    break;
            }
        }

        SaveOrdersToCSV();
    }

    static void DisplayTotalOrderAmount()
    {
        double grandTotalOrders = 0;
        double grandTotalRefunds = 0;

        const double DELIVERY_FEE = 5.00;

        foreach (Restaurant r in restaurants)
        {
            double restaurantOrderTotal = 0;
            double restaurantRefundTotal = 0;

            foreach (Order o in orders)
            {
                if (o.RestaurantId != r.RestaurantId)
                    continue;

                if (o.OrderStatus == "Delivered")
                {
                    restaurantOrderTotal += (o.TotalAmount - DELIVERY_FEE);
                }
                else if (o.OrderStatus == "Rejected" || o.OrderStatus == "Cancelled")
                {
                    restaurantRefundTotal += o.TotalAmount;
                }
            }

            grandTotalOrders += restaurantOrderTotal;
            grandTotalRefunds += restaurantRefundTotal;

            Console.WriteLine($"Restaurant: {r.RestaurantName}");
            Console.WriteLine($"Total Delivered Orders (less delivery fee): ${restaurantOrderTotal:F2}");
            Console.WriteLine($"Total Refunds: ${restaurantRefundTotal:F2}");
            Console.WriteLine();
        }

        double finalEarnings = grandTotalOrders - grandTotalRefunds;

        Console.WriteLine("===== OVERALL TOTALS =====");
        Console.WriteLine($"Total Order Amount: ${grandTotalOrders:F2}");
        Console.WriteLine($"Total Refunds: ${grandTotalRefunds:F2}");
        Console.WriteLine($"Final Amount Gruberoo Earns: ${finalEarnings:F2}");
    }


    static void DeleteOrder()
    {
        Console.WriteLine("\nDelete Order");
        Console.WriteLine("============");

        Console.Write("Enter Customer Email: ");
        string email = Console.ReadLine();

        Customer customer = customers.Find(c => c.EmailAddress == email);
        if (customer == null)
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        List<Order> pendingOrders = new List<Order>();
        foreach (var order in customer.Orders)
        {
            if (order.OrderStatus == "Pending")
            {
                pendingOrders.Add(order);
            }
        }
        
        if (pendingOrders.Count == 0)
        {
            Console.WriteLine("No pending orders found.");
            return;
        }

        Console.WriteLine("Pending Orders:");
        foreach (var o in pendingOrders)
        {
            Console.WriteLine(o.OrderId);
        }

        Console.Write("Enter Order ID: ");
        if (!int.TryParse(Console.ReadLine(), out int orderId))
        {
            Console.WriteLine("Invalid Order ID.");
            return;
        }

        Order orderToDelete = pendingOrders.Find(o => o.OrderId == orderId);
        if (orderToDelete == null)
        {
            Console.WriteLine("Order not found.");
            return;
        }

        Console.WriteLine($"\nCustomer: {orderToDelete.Customer?.CustomerName}");

        Console.WriteLine("Ordered Items:");
        for (int i = 0; i < orderToDelete.OrderedItems.Count; i++)
        {
            var item = orderToDelete.OrderedItems[i];
            Console.WriteLine($"{i + 1}. {item.ItemName} - {item.Quantity}");
        }

        Console.WriteLine($"Delivery date/time: {orderToDelete.DeliveryDateTime:dd/MM/yyyy HH:mm}");
        Console.WriteLine($"Total Amount: ${orderToDelete.TotalAmount:F2}");
        Console.WriteLine($"Order Status: {orderToDelete.OrderStatus}");

        Console.Write("Confirm deletion? [Y/N]: ");
        string confirm = Console.ReadLine()?.ToUpper();

        if (confirm == "Y")
        {
            orderToDelete.OrderStatus = "Cancelled";
            refundStack.Push(orderToDelete);

            Console.WriteLine($"Order {orderToDelete.OrderId} cancelled. Refund of ${orderToDelete.TotalAmount:F2} processed.");

            SaveOrdersToCSV();
        }
        else
        {
            Console.WriteLine("Deletion cancelled.");
        }
    }
}
