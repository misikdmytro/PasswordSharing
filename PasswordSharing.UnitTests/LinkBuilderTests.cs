using System;
using Moq;
using PasswordSharing.Contracts;
using PasswordSharing.Models;
using PasswordSharing.Services;
using Shouldly;
using Xunit;

namespace PasswordSharing.UnitTests
{
	public class LinkBuilderTests
	{
		private readonly Mock<IStringGenerator> _generatorMock;
		private readonly LinkBuilder _builder;

		public LinkBuilderTests()
		{
			_generatorMock = new Mock<IStringGenerator>();
			_builder = new LinkBuilder(_generatorMock.Object);
		}

		[Fact]
		public void BuildShouldCreateLink()
		{
			var password = new Password();

			// Act
			var expiration = TimeSpan.FromDays(1);
			var link = _builder.Build(password, expiration);

			// Assert
			(link.ExpiresAt - DateTime.Now - expiration).ShouldBeLessThan(TimeSpan.FromMilliseconds(100));
			_generatorMock.Verify(x => x.Generate(16), Times.Once);
		}
	}
}
