using BLL.Interfaces;

namespace BLL.Services;

public class DepositService : IDepositService
{
    private const decimal RegularBikeBaseDeposit = 50.00m;
    private const decimal ElectricBikeBaseDeposit = 150.00m;
    private const decimal DamageIncidentFee = 10.00m;

    private readonly IRepository<Bike> _bikeRepository;
    private readonly IRepository<DamageRecord> _damageRepository;

    public DepositService(IRepository<Bike> bikeRepository, IRepository<DamageRecord> damageRepository)
    {
        _bikeRepository = bikeRepository;
        _damageRepository = damageRepository;
    }

    public async Task<decimal> CalculateDepositAsync(Guid bikeId, Guid customerId)
    {
        var bike = await _bikeRepository.GetByIdAsync(bikeId);
        if (bike == null)
            return 0;

        var baseDeposit = GetBaseDeposit(bike.Type);
        var damageCount = await GetCustomerDamageCountAsync(customerId);

        return baseDeposit + (damageCount * DamageIncidentFee);
    }

    public decimal GetBaseDeposit(BikeType bikeType)
    {
        return bikeType == BikeType.Electric ? ElectricBikeBaseDeposit : RegularBikeBaseDeposit;
    }

    public async Task<int> GetCustomerDamageCountAsync(Guid customerId)
    {
        var damageRecords = await _damageRepository.GetAllAsync();
        return damageRecords.Count(d => d.CustomerId == customerId);
    }
}
