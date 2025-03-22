using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Model
{
    [Table("Order")]
    public class CustomerOrder
    {
        [Column("Id")]
        [Key]
        public int OrderId { get; set; }
      
        [Column("Price")]
        public decimal TotalAmount { get; set; }

        public int Quantity { get; set; }

        public string ProductId { get; set; }

        public string Phone { get; set; }
    }
}

