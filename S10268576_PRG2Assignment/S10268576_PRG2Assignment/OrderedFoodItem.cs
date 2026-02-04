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
    internal class OrderedFoodItem
    {
        public string ItemName { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        public OrderedFoodItem(string itemName, double price, int quantity)
        {
            ItemName = itemName;
            Price = price;
            Quantity = quantity;
        }

        public double GetSubtotal()
        {
            return Price * Quantity;
        }

        public override string ToString()
        {
            return $"{ItemName} - {Quantity} x ${Price:F2}";
        }
    }
}
