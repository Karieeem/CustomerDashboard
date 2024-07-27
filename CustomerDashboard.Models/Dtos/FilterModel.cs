using CustomerDashboard.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDashboard.Models.Dtos
{
    public class FilterModel
    {
        public string PropertyName { get; set; }
        public Operator Operator { get; set; }
        public string PropertyValue { get; set; }
        public LogicalOperator? NextLogicalOperator { get; set; } 
    }

}
