namespace DependencyInjectionExercise.Services;

public class DiscountService
{
    private int _orderCountInSession;
    private string _currentCustomerName = string.Empty;

    public decimal LastAppliedDiscount { get; private set; }
    public string LastCustomerName => _currentCustomerName;
    public int OrderCountInSession => _orderCountInSession;

    public decimal CalculateDiscount(string category, int quantity, string customerName)
    {
        _currentCustomerName = customerName;
        _orderCountInSession++;

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
        if (_orderCountInSession % 3 == 0)
        {
            discountPercent += 0.05m;
        }

        LastAppliedDiscount = discountPercent;

        return discountPercent;
    }
}
