using Moq;
using CustomerDashboard.DAL.Interfaces;
using Microsoft.Extensions.Logging;
using CustomerDashboard.Services.Implementation;
using CustomerDashboard.Models.Entities;
using CustomerDashboard.Models.Dtos;
using CustomerDashboard.Models.Enums;

namespace CustomerDashboard.Tests
{
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;
        private readonly Mock<ILogger<CustomerService>> _mockLogger;
        private readonly CustomerService _customerService;
        private readonly List<Customer> _customers;

        public CustomerServiceTests()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _mockLogger = new Mock<ILogger<CustomerService>>();
            _customerService = new CustomerService(_mockCustomerRepository.Object, _mockLogger.Object);

            // Initialize the readonly list of customers
            _customers = new List<Customer>
        {
            new Customer
            {
                Id = 1,
                Name = "John",
                DateOfBirth = new DateTime(1985, 5, 20),
                Job = "Developer",
                Address = "123 Main St",
                Phone = "555-1234",
                Email = "john@example.com",
                Orders = new List<Order>
                {
                    new Order
                    {
                        Id = 1,
                        CustomerId = 1,
                        TotalAmount = 150,
                        Date = new DateTime(2023, 7, 1),
                        OrderDetails = new List<OrderDetails>()
                    }
                }
            },
            new Customer
            {
                Id = 2,
                Name = "Jane",
                DateOfBirth = new DateTime(1990, 8, 15),
                Job = "Designer",
                Address = "456 Elm St",
                Phone = "555-5678",
                Email = "jane@example.com",
                Orders = new List<Order>
                {
                    new Order
                    {
                        Id = 2,
                        CustomerId = 2,
                        TotalAmount = 200,
                        Date = new DateTime(2023, 7, 2),
                        OrderDetails = new List<OrderDetails>()
                    }
                }
            }
        };
        }

        [Fact]
        public async Task SearchCustomers_NoFilters_ReturnsError()
        {
            // Arrange
            var filterModel = new FilterModelDto { Filters = new List<FilterModel>() };

            // Act
            var response = await _customerService.SearchCustomers(filterModel);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("No filters were found!.", response.Message);
        }

        [Fact]
        public async Task SearchCustomers_MoreThanTwoFilters_ReturnsError()
        {
            // Arrange
            var filterModel = new FilterModelDto
            {
                Filters = new List<FilterModel>
            {
                new FilterModel { PropertyName = "Name", PropertyValue = "John", Operator = Operator.Equals },
                new FilterModel { PropertyName = "Job", PropertyValue = "Developer", Operator = Operator.Equals },
                new FilterModel { PropertyName = "Address", PropertyValue = "123 Main St", Operator = Operator.Equals }
            }
            };

            // Act
            var response = await _customerService.SearchCustomers(filterModel);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Can't filter with more than 2 filters.", response.Message);
        }

        [Fact]
        public async Task SearchCustomers_TotalAmountFilterWithAnotherFilter_ReturnsError()
        {
            // Arrange
            var filterModel = new FilterModelDto
            {
                Filters = new List<FilterModel>
            {
                new FilterModel { PropertyName = "orders.totalamount", PropertyValue = "100", Operator = Operator.GreaterThan },
                new FilterModel { PropertyName = "Name", PropertyValue = "John", Operator = Operator.Equals }
            }
            };

            // Act
            var response = await _customerService.SearchCustomers(filterModel);

            // Assert
            Assert.False(response.IsSuccess);
            Assert.Equal("Can't send another filter with this property orders.totalamount.", response.Message);
            
        }

        [Fact]
        public async Task SearchCustomers_ValidTotalAmountFilter_ReturnsCustomers()
        {
            // Arrange
            var filterModel = new FilterModelDto
            {
                Filters = new List<FilterModel>
            {
                new FilterModel { PropertyName = "orders.totalamount", PropertyValue = "100", Operator = Operator.GreaterThan }
            }
            };
            _mockCustomerRepository.Setup(repo => repo.SearchCustomers(It.IsAny<FilterModelService<Customer>>()))
                .ReturnsAsync(_customers);

            // Act
            var response = await _customerService.SearchCustomers(filterModel);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal(_customers, response.Data);
        }

        [Fact]
        public async Task SearchCustomers_ValidFilters_ReturnsFilteredCustomers()
        {
            // Arrange
            var filterModel = new FilterModelDto
            {
                Filters = new List<FilterModel>
            {
                new FilterModel { PropertyName = "Name", PropertyValue = "Jane", Operator = Operator.Equals }
            }
            };
            var expectedCustomers = _customers.Where(c => c.Name == "Jane").ToList();
            _mockCustomerRepository.Setup(repo => repo.SearchCustomers(It.IsAny<FilterModelService<Customer>>()))
                .ReturnsAsync(expectedCustomers);

            // Act
            var response = await _customerService.SearchCustomers(filterModel);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal(expectedCustomers, response.Data);
        }

        [Fact]
        public async Task SearchCustomers_NoMatchingCustomers_ReturnsEmptyList()
        {
            // Arrange
            var filterModel = new FilterModelDto
            {
                Filters = new List<FilterModel>
            {
                new FilterModel { PropertyName = "Name", PropertyValue = "NonExistent", Operator = Operator.Equals }
            }
            };
            var expectedCustomers = new List<Customer>();
            _mockCustomerRepository.Setup(repo => repo.SearchCustomers(It.IsAny<FilterModelService<Customer>>()))
                .ReturnsAsync(expectedCustomers);

            // Act
            var response = await _customerService.SearchCustomers(filterModel);

            // Assert
            Assert.True(response.IsSuccess);
            Assert.Equal(expectedCustomers, response.Data);
        }

      
    }

}