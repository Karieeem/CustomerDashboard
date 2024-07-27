using CustomerDashboard.DAL.Interfaces;
using CustomerDashboard.Models.Dtos;
using CustomerDashboard.Models.Entities;
using CustomerDashboard.Models.Enums;
using Microsoft.EntityFrameworkCore;


namespace CustomerDashboard.DAL.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _context;

        public CustomerRepository(CustomerDbContext context)
        {
            _context = context;
        }
        public async Task<List<Customer>> SearchCustomers(FilterModelService<Customer> filter)
        {
            IQueryable<Customer> result =null;
            IQueryable<Customer> query = _context.Customers.AsNoTracking().Include(c => c.Orders).ThenInclude(o => o.OrderDetails);

            if (filter.IsTotalAmount)
            {
                switch (filter.Operator)
                {
                    case Operator.Equals:
                        result = query.Where(c => c.Orders.Sum(o => o.TotalAmount) == filter.TotalAmount);
                        break;
                    case Operator.GreaterThan:
                        result = query.Where(c => c.Orders.Sum(o => o.TotalAmount) > filter.TotalAmount);
                        break;
                    case Operator.LessThan:
                        result = query.Where(c => c.Orders.Sum(o => o.TotalAmount) < filter.TotalAmount);
                        break;
                }
            }

            else if (filter.Filters != null)
                result = query.Where(filter.Filters);
          
            return result?.ToList() ?? new List<Customer>(); 
        }

    }

}
