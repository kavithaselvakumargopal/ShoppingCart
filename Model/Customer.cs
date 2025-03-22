using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
//using System.Data.Entity;

namespace ShoppingCart.Model
{

    [Table("Customer")]
    public class Customer
    {
    
        public int Id { get; set; }

        [Column("CustomerName")]
        public string Name { get; set; }
        public string Phone { get; set; }
    }

}
