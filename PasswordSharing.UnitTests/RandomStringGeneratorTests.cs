using PasswordSharing.Services;
using Shouldly;
using Xunit;

namespace PasswordSharing.UnitTests
{
	public class RandomStringGeneratorTests
	{
		[Fact]
		public void GeneratedStringShouldBeDifferent()
		{
			// Arrange
			var stringGenerator = new RandomStringGenerator();

			// Act
			var str1 = stringGenerator.Generate(16);
			var str2 = stringGenerator.Generate(16);

			// Assert
			str1.ShouldNotBe(str2);
		}
	}
}
