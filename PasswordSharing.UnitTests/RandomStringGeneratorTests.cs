using PasswordSharing.Interfaces;
using PasswordSharing.Services;
using Shouldly;
using Xunit;

namespace PasswordSharing.UnitTests
{
	public class RandomStringGeneratorTests
	{
		private readonly IRandomStringGenerator _stringGenerator;

		public RandomStringGeneratorTests()
		{
			_stringGenerator = new RandomBase64StringGenerator();
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
			var result = _stringGenerator.Generate(length);

			// Assert
			result.Length.ShouldBe(expectedLength);
		}

		[Fact]
		public void GenerateShouldGenerateDifferentStrings()
		{
			// Arrange
			// Act
			var result1 = _stringGenerator.Generate(5);
			var result2 = _stringGenerator.Generate(5);

			// Assert
			result1.ShouldNotBe(result2);
		}
	}
}
