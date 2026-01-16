namespace BLL.Interfaces;

public interface IDepositService
{
    Task<decimal> CalculateDepositAsync(Guid bikeId, Guid customerId);
    decimal GetBaseDeposit(BikeType bikeType);
    Task<int> GetCustomerDamageCountAsync(Guid customerId);
}
