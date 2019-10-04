using PasswordSharing.Interfaces;
using PasswordSharing.Services;
using Shouldly;
using Xunit;

namespace PasswordSharing.UnitTests
{
	public class RandomStringGeneratorTests
	{
		private readonly IRandomBase64StringGenerator _base64StringGenerator;

		public RandomStringGeneratorTests()
		{
			_base64StringGenerator = new RandomBase64StringGenerator();
		}

		[Theory]
		[InlineData(1, 4)]
		[InlineData(2, 4)]
		[InlineData(3, 4)]
		[InlineData(4, 8)]
		[InlineData(5, 8)]
		[InlineData(6, 8)]
		public void GenerateShouldGenerateCorrectLengthString(int length, int expectedLength)
		{
			// Arrange
			// Act
			var result = _base64StringGenerator.Generate(length);

			// Assert
			result.Length.ShouldBe(expectedLength);
		}

		[Fact]
		public void GenerateShouldGenerateDifferentStrings()
		{
			// Arrange
			// Act
			var result1 = _base64StringGenerator.Generate(5);
			var result2 = _base64StringGenerator.Generate(5);

			// Assert
			result1.ShouldNotBe(result2);
		}
	}
}
