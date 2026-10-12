using BackendService.Services;

namespace BackendService.ApiTests.Unit;

public sealed class EligibilityServiceTests
{
    private readonly EligibilityService _service = new();

    [Theory]
    [InlineData(1000, 0, 0, 1000)]
    [InlineData(1000, 1000, 2, 1800)]
    [InlineData(0, 1000, 0, 0)]
    [InlineData(1000.005, 0, 0, 1000.01)]
    public void CalculateEligibility_ValidInputs_AppliesFormulaFloorAndRounding(
        decimal baseAmount, decimal income, int children, decimal expected)
    {
        Assert.Equal(expected, _service.CalculateEligibility(baseAmount, income, children));
    }

    [Theory]
    [InlineData(-1, 0, 0)]
    [InlineData(0, -1, 0)]
    [InlineData(0, 0, -1)]
    public void CalculateEligibility_NegativeInput_Throws(
        decimal baseAmount, decimal income, int children)
    {
        Assert.Throws<ArgumentException>(() =>
            _service.CalculateEligibility(baseAmount, income, children));
    }
}
