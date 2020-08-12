using System;
using System.Collections.Generic;
using System.Text;

namespace BooksZone.Models.ViewModels
{
    public class OrderDetailsVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
