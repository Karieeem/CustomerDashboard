using CustomerDashboard.DAL.Interfaces;
using CustomerDashboard.Models.Dtos;
using CustomerDashboard.Models.Entities;
using CustomerDashboard.Services.Helpers;
using CustomerDashboard.Services.Interfaces;
using Microsoft.Extensions.Logging;


namespace CustomerDashboard.Services.Implementation
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository customerRepository, ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }
        public async Task<ResponseDto<List<Customer>>> SearchCustomers(FilterModelDto filterModel)
        {
            try
            {
                var totalAmountFilter = filterModel.Filters.FirstOrDefault(fm => fm.PropertyName.ToLower() == "orders.totalamount");
                var checkResult = CheckFilters(filterModel.Filters.Count, totalAmountFilter == null);
                if (!checkResult.IsSuccess)
                    return checkResult;

                FilterModelService<Customer> filterModelServie = null;
                List<Customer> customerList = null;

                if (totalAmountFilter != null && filterModel.Filters.Count == 1)
                {
                    decimal result;
                    decimal.TryParse(totalAmountFilter.PropertyValue, out result);
                    filterModelServie = new FilterModelService<Customer> { IsTotalAmount=true, Operator = totalAmountFilter.Operator, TotalAmount = result };
                    customerList = await _customerRepository.SearchCustomers(filterModelServie);
                }
                else
                {
                    var filterModels = filterModel.Filters;

                    var simpleFilterExpression = ExpressionBuilder.GetFilterExpression<Customer>(filterModel.Filters);

                    filterModelServie = new FilterModelService<Customer> { Filters = simpleFilterExpression };

                    customerList = await _customerRepository.SearchCustomers(filterModelServie);
                }

                return new ResponseDto<List<Customer>>(customerList);
            }

            catch (FormatException ex)
            {
                //We can log in Db instead
                var msg = "Format error occurred while searching customers , check the filter values!.";
                _logger.LogError(ex, msg);
                return new ResponseDto<List<Customer>>(msg);
            }
            catch (NotSupportedException ex)
            {
                var msg = "Unsupported operation error occurred while searching customers!.";
                _logger.LogError(ex, msg);
                return new ResponseDto<List<Customer>>(msg);
            }
            catch (Exception ex)
            {
                var msg = "An error occurred while searching customers!.";
                _logger.LogError(ex, msg);
                return new ResponseDto<List<Customer>>(msg);
            }

        }

        private ResponseDto<List<Customer>> CheckFilters(int count, bool isTotalAmountFilterNull = false)
        {
            string msg = null;
            if (count == 0)
                msg = "No filters were found!.";

            else if (count > 2)
                msg = "Can't filter with more than 2 filters.";

            else if (!isTotalAmountFilterNull && count > 1)
                msg = "Can't send another filter with this property orders.totalamount.";


            if (msg == null)
                return new ResponseDto<List<Customer>>();

            else
                return new ResponseDto<List<Customer>>(msg);


        }
    
    }
}
