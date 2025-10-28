using StockTracker.CrossCutting.Utils;
using StockTracker.MarketStack.Services.Models;

namespace StockTracker.Services.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var filePath = "./response.json";
            MemoryStream memStream = new MemoryStream();
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                memStream.SetLength(fileStream.Length);
                fileStream.ReadExactly(memStream.GetBuffer(), 0, (int)fileStream.Length);
            }

            var result = System.Text.Json.JsonSerializer.Deserialize<EndOfDayResponse>(memStream);
            Assert.NotNull(result);
        }

        [Theory]
        [MemberData(nameof(dateRange))]
        public void TestPreviuosWeek(DateTime source)
        {
            // Arrange
            DayOfWeek[] test = [DayOfWeek.Saturday, DayOfWeek.Sunday];

            // Act
            var result = source.GetPreviousWeek();

            // Assert
            Assert.True(result.All(item => !test.Contains(item.DayOfWeek)));
        }

        public static IEnumerable<object[]> dateRange =>
            new List<object[]>
            {
                new object[] {DateTime.UtcNow},
                new object[] {DateTime.UtcNow.AddDays(-1)},
                new object[] {DateTime.UtcNow.AddDays(1)},
                new object[] {DateTime.UtcNow.AddDays(3)},
                new object[] {DateTime.UtcNow.AddDays(-3)},
            };
    }

}
