using CustomerDashboard.Models.Dtos;
using CustomerDashboard.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerDashboard.DAL.Interfaces
{
    public interface ICustomerRepository
    {
        public Task<List<Customer>> SearchCustomers(FilterModelService<Customer> filter);

    }
}
