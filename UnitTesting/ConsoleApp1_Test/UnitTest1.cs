using Xunit;
using UTCalculator; // Needed to access UTCalculator class
namespace ConsoleApp1_Test;

public class UnitTest1
{
    //Fact indicates a test
    [Fact]
    public void TestAddMethodResultShouldBeNine()
    {
        int result = CalculatorFeatures.AddTwoNumbers(5, 4);
        Assert.Equal(9, result);
    }

    //Theory gives the ability to pass multiple data tests to the same unit test using InlineData
    [Theory]
    [InlineData(5, 4, 9)]
    [InlineData(6, 1, 7)]
    [InlineData(2, 3, 5)]
    public void AddTwoNumbersAndGetResult(int firstNum, int secondNum, int expectedResult)
    {
        int result = CalculatorFeatures.AddTwoNumbers(firstNum, secondNum);
        Assert.Equal(expectedResult, result);
    }
}