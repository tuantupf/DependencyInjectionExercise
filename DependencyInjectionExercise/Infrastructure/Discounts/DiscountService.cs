namespace DependencyInjectionExercise.Infrastructure.Discounts;

public class DiscountService
{
    private readonly Dictionary<string, int> _customerOrderCounts = new();
    private string _currentCustomerName = string.Empty;

    public decimal LastAppliedDiscount { get; private set; }
    public string LastCustomerName => _currentCustomerName;
    public int OrderCountInSession => _customerOrderCounts.GetValueOrDefault(_currentCustomerName, 0);

    public decimal CalculateDiscount(string category, int quantity, string customerName)
    {
        _currentCustomerName = customerName;

        if (!_customerOrderCounts.ContainsKey(customerName))
        {
            _customerOrderCounts[customerName] = 0;
        }

        _customerOrderCounts[customerName]++;

        var customerOrderCount = _customerOrderCounts[customerName];

        decimal discountPercent = 0;

        if (category == "fiction" && quantity >= 5)
        {
            discountPercent = 0.10m;
        }
        else if (category == "non-fiction" && quantity >= 3)
        {
            discountPercent = 0.15m;
        }

        // Loyalty bonus: every 3rd order gets an extra 5% off
        if (customerOrderCount % 3 == 0)
        {
            discountPercent += 0.05m;
        }

        LastAppliedDiscount = discountPercent;

        return discountPercent;
    }
}
