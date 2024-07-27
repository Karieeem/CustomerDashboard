using CustomerDashboard.Models.Dtos;
using CustomerDashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDashboard.Services.Interfaces
{
    public interface ICustomerService
    {
        public Task<ResponseDto<List<Customer>>> SearchCustomers(FilterModelDto filterModel);

    }
}
