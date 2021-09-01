using System;
using System.Collections.Generic;

namespace Microsoft.Models
{
    public class Order
    {
        public string id { get; set; }
        public string BuyerId { get; set; }
        public DateTimeOffset OrderDate { get; set; }
        public Address ShipToAddress { get; set; }

        // DDD Patterns comment
        // Using a private collection field, better for DDD Aggregate's encapsulation
        // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
        // but only through the method Order.AddOrderItem() which includes behavior.
        public List<OrderItem> OrderItems = new List<OrderItem>();
    }
}
