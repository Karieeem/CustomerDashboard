using CustomerDashboard.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDashboard.Models.Dtos
{
    public class FilterModelService<T>
    {
        public Expression<Func<T, bool>> Filters { get; set; }
        public bool IsTotalAmount = false;
        public decimal TotalAmount { get; set; }
        public Operator Operator { get; set; }
    }
}
