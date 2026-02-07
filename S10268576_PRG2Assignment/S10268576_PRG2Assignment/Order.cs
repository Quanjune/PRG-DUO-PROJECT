using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// DONE BY QUAN JUN
//==========================================================
// Student Number : S10268576
// Student Name : Tang Quan Jun
// Partner Name : Rajakumar Kishore
//==========================================================
namespace S10268576_PRG2Assignment
{
    internal class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDateTime { get; set; }
        public string OrderStatus { get; set; }
        public double TotalAmount { get; set; }
        public string DeliveryAddress { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public string PaymentMethod { get; set; }
        public List<OrderedFoodItem> OrderedItems { get; set; }

        public Customer Customer { get; set; }  // ✅ ADD THIS LINE

        public Order(int orderId)
        {
            OrderId = orderId;
            OrderDateTime = DateTime.Now;
            OrderStatus = "Pending";
            OrderedItems = new List<OrderedFoodItem>();
        }
        public void AddOrderedFoodItem(OrderedFoodItem item)
        {
            OrderedItems.Add(item);
        }
        public double CalculateOrderTotal()
        {
            double total = 0;
            foreach (var item in OrderedItems)
            {
                total += item.GetSubtotal();
            }
            return total;
        }
        public override string ToString()
        {
            return $"Order {OrderId} - {OrderStatus} - ${TotalAmount:F2}";
        }
    }
}