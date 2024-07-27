using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDashboard.Models.Entities
{
    [Table("OrderDetails", Schema = "Customer")]
    public class OrderDetails
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string Item { get; set; }
        public decimal Amount { get; set; }
        public Order Order { get; set; }
    }
}
