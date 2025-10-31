using NUnit.Framework;
using StockTracker.CrossCutting.Utils;
using StockTracker.MarketStack.Services.Models;
using System.Text.Json;

namespace StockTracker.MarketStack.Services.UnitTests
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void Test1()
        {
            var filePath = "./response.json";
            MemoryStream memStream = new MemoryStream();
            using (FileStream fileStream = File.OpenRead(filePath))
            {
                memStream.SetLength(fileStream.Length);
                fileStream.ReadExactly(memStream.GetBuffer(), 0, (int)fileStream.Length);
            }

            var result = JsonSerializer.Deserialize<EndOfDayResponse>(memStream);
            Assert.That(result, Is.Not.Null);
        }

        [Theory]
        public void TestPreviousWeek(DateTime source)
        {
            // Arrange
            DayOfWeek[] test = [DayOfWeek.Saturday, DayOfWeek.Sunday];

            // Act
            var result = source.GetPreviousWeek();

            // Assert
            Assert.That(result.All(item => !test.Contains(item.DayOfWeek)), Is.True);
        }


        [DatapointSource]
        public IEnumerable<DateTime> dateRange =
            new List<DateTime>
            {
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(-1),
                DateTime.UtcNow.AddDays(1),
                DateTime.UtcNow.AddDays(3),
                DateTime.UtcNow.AddDays(-3)
            };
    }

}
