using S10268576_PRG2Assignment;
using System;
using System.Collections.Generic;
using System.IO;

//==========================================================
// Student Number : S10268576
// Student Name : Tang Quan Jun
// Partner Name : Rajakumar Kishore
//==========================================================


// ==========================================================
// Feature 1: Load Data from CSV Files
// Implemented by: S10268576 – Tang Quan Jun
//
// Description:
// This feature loads all required system data at program startup,
// including restaurants, food items, customers, and orders from
// their respective CSV files.
//
// Purpose:
// Ensures the Gruberoo system is fully initialised with persistent
// data before any operations are performed.
// ==========================================================
class Program
{

    static List<Customer> customers = new List<Customer>();
    static List<Order> orders = new List<Order>();
    static Stack<Order> refundStack = new Stack<Order>();

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
        int choice = -1;   // give initial value so it’s not unassigned

        while (choice != 0)
        {
            DisplayMenu();
            choice = GetUserChoice();

            if (choice == 1)
            {
                ListAllRestaurantsAndMenuItems();
            }
            else if (choice == 2)
            {
                ListAllOrders();
            }
            else if (choice == 3)
            {
                CreateNewOrder();
            }
            else if (choice == 4)
            {
                ProcessOrder();
            }
            else if (choice == 5)
            {
                ModifyOrder();
            }
            else if (choice == 6)
            {
                DeleteOrder();
            }
            else if (choice == 7)
            {
                DisplayTotalOrderAmount();
            }
            else if (choice == 8)
            {
                BulkProcessTodayOrders();
            }
            else if (choice == 9)
            {
                DisplayFavouriteRestaurant();
            }
            else if (choice == 10)
            {
                ReorderFavouriteOrder();
            }
            else if (choice == 0)
            {
                Console.WriteLine("Thank you for using Gruberoo!");
            }
            else
            {
                Console.WriteLine("Invalid choice. Please try again.");
            }
            if (choice != 0)
            {
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    // ==========================================================
    // Feature 8: Bulk Process Pending Orders
    // Implemented by: S10268576 – Tang Quan Jun
    //
    // Description:
    // Automatically processes all pending orders based on delivery
    // time rules. Orders within 60 minutes are rejected and refunded,
    // while others move to "Preparing" status. Processing statistics
    // are displayed and saved.
    //
    // Purpose:
    // Demonstrates automated operational decision-making similar
    // to real food delivery management systems.
    // ==========================================================

    static void BulkProcessTodayOrders()
    {
        Console.WriteLine("\nBulk Processing Pending Orders");
        Console.WriteLine("==============================");

        DateTime now = DateTime.Now;

        List<Order> pendingOrders = new List<Order>();

        foreach (Order o in orders)
        {
            if (o.OrderStatus == "Pending")
            {
                pendingOrders.Add(o);
            }
        }

        int totalPending = pendingOrders.Count;

        if (totalPending == 0)
        {
            Console.WriteLine("No pending orders to process.");
            return;
        }

        int preparingCount = 0;
        int rejectedCount = 0;

        foreach (Order o in pendingOrders)
        {
            TimeSpan timeToDelivery = o.DeliveryDateTime - now;

            if (timeToDelivery.TotalMinutes < 60)
            {
                o.OrderStatus = "Rejected";
                refundStack.Push(o);
                rejectedCount++;
            }
            else
            {
                o.OrderStatus = "Preparing";
                preparingCount++;
            }
        }

        int processedCount = preparingCount + rejectedCount;
        double percentageProcessed =
            (double)processedCount / orders.Count * 100;

        Console.WriteLine($"Total pending orders   : {totalPending}");
        Console.WriteLine($"Orders processed       : {processedCount}");
        Console.WriteLine($"Preparing orders       : {preparingCount}");
        Console.WriteLine($"Rejected orders        : {rejectedCount}");
        Console.WriteLine($"% auto-processed       : {percentageProcessed:F2}%");

        SaveOrdersToCSV();
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
        Console.WriteLine("8. Bulk process today's pending orders");
        Console.WriteLine("9. Display favourite restaurant");
        Console.WriteLine("10. Reorder favourite order");
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
        if (paymentMethod == "CC")
        {
            newOrder.PaymentMethod = "Credit Card";
        }
        else if (paymentMethod == "PP")
        {
            newOrder.PaymentMethod = "PayPal";
        }
        else if (paymentMethod == "CD")
        {
            newOrder.PaymentMethod = "Cash on Delivery";
        }


        // Set the total amount (including delivery)
        newOrder.TotalAmount = totalAmount;

        // ⭐ Ask if favourite
        Console.Write("Mark this order as favourite? [Y/N]: ");
        string favInput = Console.ReadLine()?.ToUpper();

        newOrder.IsFavourite = favInput == "Y";

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

        // ================= MODIFY ORDER =================
        if (modifyChoice == 1) // Modify Items
        {
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



                // ---------- ADD ITEM ----------
                if (itemChoice == 1)
                {
                    var menuItems = orderRestaurant.Menu.FoodItems;

                    Console.WriteLine("\nAvailable Food Items:");
                    for (int i = 0; i < menuItems.Count; i++)
                        Console.WriteLine($"{i + 1}. {menuItems[i].ItemName} - ${menuItems[i].Price:F2}");

                    Console.Write("Enter item number: ");
                    if (!int.TryParse(Console.ReadLine(), out int addNum) ||
                        addNum < 1 || addNum > menuItems.Count)
                    {
                        Console.WriteLine("Invalid item number.");
                        continue;
                    }

                    Console.Write("Enter quantity: ");
                    if (!int.TryParse(Console.ReadLine(), out int qty) || qty < 1)
                    {
                        Console.WriteLine("Invalid quantity.");
                        continue;
                    }

                    var selectedFood = menuItems[addNum - 1];
                    var existing = orderToModify.OrderedItems
                        .Find(x => x.ItemName == selectedFood.ItemName);

                    if (existing != null)
                    {
                        existing.Quantity += qty;
                        Console.WriteLine($"Updated {selectedFood.ItemName} → {existing.Quantity}");
                    }
                    else
                    {
                        orderToModify.AddOrderedFoodItem(
                            new OrderedFoodItem(selectedFood.ItemName, selectedFood.Price, qty)
                        );
                        Console.WriteLine($"Added {qty} × {selectedFood.ItemName}");
                    }
                }

                // ---------- REMOVE ITEM ----------
                else if (itemChoice == 2)
                {
                    if (orderToModify.OrderedItems.Count <= 1)
                    {
                        Console.WriteLine("Order must contain at least ONE item.");
                        continue;
                    }

                    Console.WriteLine("\nCurrent Items:");
                    for (int i = 0; i < orderToModify.OrderedItems.Count; i++)
                        Console.WriteLine($"{i + 1}. {orderToModify.OrderedItems[i].ItemName} - {orderToModify.OrderedItems[i].Quantity}");

                    Console.Write("Enter item number to remove: ");
                    if (!int.TryParse(Console.ReadLine(), out int removeNum) ||
                        removeNum < 1 || removeNum > orderToModify.OrderedItems.Count)
                    {
                        Console.WriteLine("Invalid item number.");
                        continue;
                    }

                    var removed = orderToModify.OrderedItems[removeNum - 1];
                    orderToModify.OrderedItems.RemoveAt(removeNum - 1);
                    Console.WriteLine($"Removed {removed.ItemName}");
                }

                // ---------- CHANGE QUANTITY ----------
                else if (itemChoice == 3)
                {
                    Console.WriteLine("\nCurrent Items:");
                    for (int i = 0; i < orderToModify.OrderedItems.Count; i++)
                        Console.WriteLine($"{i + 1}. {orderToModify.OrderedItems[i].ItemName} - {orderToModify.OrderedItems[i].Quantity}");

                    Console.Write("Enter item number: ");
                    if (!int.TryParse(Console.ReadLine(), out int changeNum) ||
                        changeNum < 1 || changeNum > orderToModify.OrderedItems.Count)
                    {
                        Console.WriteLine("Invalid item number.");
                        continue;
                    }

                    Console.Write("Enter new quantity: ");
                    if (!int.TryParse(Console.ReadLine(), out int newQty) || newQty < 1)
                    {
                        Console.WriteLine("Invalid quantity.");
                        continue;
                    }

                    orderToModify.OrderedItems[changeNum - 1].Quantity = newQty;
                    Console.WriteLine("Quantity updated.");
                }

                // ---------- DONE ----------
                else if (itemChoice == 0)
                {
                    modifyingItems = false;
                }

                else
                {
                    Console.WriteLine("Invalid choice.");
                    continue;
                }



                // 🔁 Recalculate total after ANY change
                orderToModify.TotalAmount =
                    orderToModify.CalculateOrderTotal() + deliveryFee;
            }


            // 💾 Save once after finishing edits
            SaveOrdersToCSV();

            Console.WriteLine($"\nOrder {orderToModify.OrderId} updated.");
        }

        else if (modifyChoice == 2) // Modify Address
        {
            Console.Write("Enter new address: ");
            string newAddress = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(newAddress))
            {
                Console.WriteLine("Address cannot be empty.");
            }
            else
            {
                orderToModify.DeliveryAddress = newAddress;
                SaveOrdersToCSV();
                Console.WriteLine("Address updated.");
            }
        }

        else if (modifyChoice == 3) // Modify Delivery Time
        {
            Console.Write("Enter new delivery time (HH:mm): ");

            if (!DateTime.TryParseExact(
                    Console.ReadLine(),
                    "HH:mm",
                    null,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime newTime))
            {
                Console.WriteLine("Invalid time format.");
            }
            else
            {
                orderToModify.DeliveryDateTime = new DateTime(
                    orderToModify.DeliveryDateTime.Year,
                    orderToModify.DeliveryDateTime.Month,
                    orderToModify.DeliveryDateTime.Day,
                    newTime.Hour,
                    newTime.Minute,
                    0
                );

                SaveOrdersToCSV();
                Console.WriteLine("Delivery time updated.");
            }
        }

        else
        {
            Console.WriteLine("Invalid choice.");
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

            string[] parts = lines[i].Split(',');

            // Need at least 10 columns now (because of IsFavourite)
            if (parts.Length < 10)
                continue;

            int orderId = int.Parse(parts[0]);
            string customerEmail = parts[1];
            string restaurantId = parts[2];

            DateTime deliveryDateTime = DateTime.Parse(parts[3] + " " + parts[4]);
            string address = parts[5];
            DateTime orderDateTime = DateTime.Parse(parts[6]);
            double totalAmount = double.Parse(parts[7]);
            string status = parts[8];

            // ⭐ NEW: load favourite flag
            bool isFavourite = false;
            bool.TryParse(parts[9], out isFavourite);

            Order order = new Order(orderId);
            order.DeliveryDateTime = deliveryDateTime;
            order.DeliveryAddress = address;
            order.OrderDateTime = orderDateTime;
            order.TotalAmount = totalAmount;
            order.OrderStatus = status;
            order.RestaurantId = restaurantId;
            order.IsFavourite = isFavourite;   // ⭐ IMPORTANT

            // link customer
            for (int j = 0; j < customers.Count; j++)
            {
                if (customers[j].EmailAddress == customerEmail)
                {
                    order.Customer = customers[j];
                    customers[j].AddOrder(order);
                    break;
                }
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
            writer.WriteLine("OrderId,CustomerEmail,RestaurantId,DeliveryDate,DeliveryTime,DeliveryAddress,CreatedDateTime,TotalAmount,Status,IsFavourite,Items");


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
                writer.WriteLine($"{order.OrderId},{customerEmail},{order.RestaurantId},{deliveryDate},{deliveryTime},{order.DeliveryAddress},{orderDateTime},{order.TotalAmount:F2},{order.OrderStatus},{order.IsFavourite},\"{itemsString}\"");
                // ← Added quotes
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

    // ==========================================================
    // Feature 4: Process Order Status
    // Implemented by: Rajakumar Kishore
    //
    // Description:
    // Allows restaurant staff to process customer orders by
    // confirming, rejecting, skipping, or delivering them.
    // The order status is updated accordingly and refunds are
    // handled using a stack data structure when orders are rejected.
    //
    // Purpose:
    // Simulates real-world restaurant order workflow management.
    // ==========================================================

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
                string action = Console.ReadLine();
                action = action.ToUpper();

                if (action == "C")
                {
                    if (order.OrderStatus == "Pending")
                    {
                        order.OrderStatus = "Preparing";
                        Console.WriteLine($"Order {order.OrderId} confirmed. Status: Preparing");
                    }
                    else
                    {
                        Console.WriteLine("Order cannot be confirmed.");
                    }
                }
                else if (action == "R")
                {
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
                }
                else if (action == "S")
                {
                    if (order.OrderStatus == "Cancelled")
                    {
                        Console.WriteLine("Order skipped.");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine("Only cancelled orders can be skipped.");
                    }
                }
                else if (action == "D")
                {
                    if (order.OrderStatus == "Preparing")
                    {
                        order.OrderStatus = "Delivered";
                        Console.WriteLine($"Order {order.OrderId} delivered.");
                    }
                    else
                    {
                        Console.WriteLine("Order cannot be delivered.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid action.");
                }
            }

            SaveOrdersToCSV();
        }

    // ==========================================================
    // Advanced Feature B: Financial Summary and Earnings Report
    // Implemented by: S10268576 – Tang Quan Jun
    //
    // Description:
    // Calculates total delivered order revenue per restaurant,
    // total refunds issued, and overall earnings for Gruberoo
    // based on commission percentage.
    //
    // Purpose:
    // Simulates real-world revenue analytics for platform operators.
    // ==========================================================

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

            double finalEarnings = grandTotalOrders * 0.30;


            Console.WriteLine("===== OVERALL TOTALS =====");
            Console.WriteLine($"Total Order Amount: ${grandTotalOrders:F2}");
            Console.WriteLine($"Total Refunds: ${grandTotalRefunds:F2}");
            Console.WriteLine($"Final Amount Gruberoo Earns: ${finalEarnings:F2}");
        }

    // ==========================================================
    // Feature 6: Delete (Cancel) Existing Order
    // Implemented by: S10268576 – Tang Quan Jun
    //
    // Description:
    // Enables a customer to cancel a pending order.
    // The system validates ownership, updates order status to
    // "Cancelled", processes refund via stack, and saves changes.
    //
    // Purpose:
    // Provides safe order cancellation while maintaining
    // transaction integrity.
    // ==========================================================

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

    static void DisplayFavouriteRestaurant()
    {
        Restaurant favouriteRestaurant = null;
        int maxDelivered = 0;

        for (int i = 0; i < restaurants.Count; i++)
        {
            Restaurant r = restaurants[i];
            int deliveredCount = 0;

            for (int j = 0; j < orders.Count; j++)
            {
                if (orders[j].RestaurantId == r.RestaurantId &&
                    orders[j].OrderStatus == "Delivered")
                {
                    deliveredCount++;
                }
            }

            if (deliveredCount > maxDelivered)
            {
                maxDelivered = deliveredCount;
                favouriteRestaurant = r;
            }
        }

        Console.WriteLine("\nFavourite Restaurant");
        Console.WriteLine("====================");

        if (favouriteRestaurant != null)
        {
            Console.WriteLine($"Restaurant: {favouriteRestaurant.RestaurantName}");
            Console.WriteLine($"Delivered Orders: {maxDelivered}");
        }
        else
        {
            Console.WriteLine("No delivered orders yet.");
        }
    }

    // ==========================================================
    // Bonus Feature: Favourite Orders & Reorder System
    // Implemented by: S10268576 – Tang Quan Jun
    //
    // Description:
    // Allows customers to mark orders as favourites and quickly
    // reorder them. The system performs a deep copy of the original
    // order’s items, generates a new Order ID and delivery time,
    // resets status to "Pending", and saves the new order.
    //
    // Purpose:
    // Enhances usability with real-world functionality similar to
    // GrabFood’s “Reorder” feature while demonstrating advanced
    // object-oriented programming design.
    // ==========================================================

    static void ReorderFavouriteOrder()
    {
        Console.WriteLine("\nReorder Favourite Order");
        Console.WriteLine("=======================");

        // Step 1 — Get customer
        Console.Write("Enter Customer Email: ");
        string email = Console.ReadLine();

        Customer customer = customers.Find(c => c.EmailAddress == email);
        if (customer == null)
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        // Step 2 — Find favourite orders
        List<Order> favouriteOrders = new List<Order>();

        foreach (var o in customer.Orders)
        {
            if (o.IsFavourite)
            {
                favouriteOrders.Add(o);
            }
        }

        if (favouriteOrders.Count == 0)
        {
            Console.WriteLine("No favourite orders found.");
            return;
        }

        // Step 3 — Display favourites
        Console.WriteLine("Favourite Orders:");
        foreach (var o in favouriteOrders)
        {
            Console.WriteLine($"Order {o.OrderId} | Restaurant {o.RestaurantId} | Total ${o.TotalAmount:F2}");
        }

        // Step 4 — Choose order
        Console.Write("Enter Order ID to reorder: ");
        if (!int.TryParse(Console.ReadLine(), out int oldId))
        {
            Console.WriteLine("Invalid Order ID.");
            return;
        }

        Order oldOrder = favouriteOrders.Find(o => o.OrderId == oldId);
        if (oldOrder == null)
        {
            Console.WriteLine("Favourite order not found.");
            return;
        }

        // Step 5 — Generate NEW Order ID
        int newId = 1;
        foreach (var o in orders)
        {
            if (o.OrderId >= newId)
                newId = o.OrderId + 1;
        }

        // Step 6 — Create NEW order (DEEP COPY)
        Order newOrder = new Order(newId)
        {
            Customer = customer,
            RestaurantId = oldOrder.RestaurantId,
            DeliveryAddress = oldOrder.DeliveryAddress,
            DeliveryDateTime = DateTime.Now.AddHours(1),
            OrderStatus = "Pending",
            PaymentMethod = oldOrder.PaymentMethod,
            SpecialRequest = oldOrder.SpecialRequest,
            IsFavourite = false
        };

        // ⭐ Deep copy items
        foreach (var item in oldOrder.OrderedItems)
        {
            newOrder.AddOrderedFoodItem(
                new OrderedFoodItem(item.ItemName, item.Price, item.Quantity)
            );
        }

        // Recalculate total
        double subtotal = newOrder.CalculateOrderTotal();
        newOrder.TotalAmount = subtotal + 5.00;

        // Step 7 — Add to system
        orders.Add(newOrder);
        customer.AddOrder(newOrder);

        SaveOrdersToCSV();

        Console.WriteLine($"\nReorder successful! New Order ID: {newOrder.OrderId}");
    }
}

