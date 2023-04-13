using EmergencyRoadsideAssistanceService.Models;

namespace EmergencyRoadsideAssistanceService.Interfaces
{
    public interface ICustomerService
    {
        Customer? GetCustomer(int customerId);
        void UpsertCustomer(Customer customer);
    }
}
