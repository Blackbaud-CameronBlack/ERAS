using EmergencyRoadsideAssistanceService.Interfaces;
using EmergencyRoadsideAssistanceService.Models;

namespace EmergencyRoadsideAssistanceService.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly Dictionary<int, Customer> _customers = new();

        public Customer? GetCustomer(int customerId)
        {
            if (_customers.ContainsKey(customerId))
            {
                return _customers[customerId];
            }
            return null;
        }

        public void UpsertCustomer(Customer customer)
        {
            if (!_customers.ContainsKey(customer.CustomerId))
            {
                _customers.Add(customer.CustomerId, customer);
            }
            else
            {
                _customers[customer.CustomerId] = customer;
            }
        }
    }
}
