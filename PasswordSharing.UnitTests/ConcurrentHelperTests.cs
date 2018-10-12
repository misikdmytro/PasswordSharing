using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using PasswordSharing.Contracts;
using PasswordSharing.Locks;
using PasswordSharing.Models;
using PasswordSharing.Repositories;
using Shouldly;
using Xunit;

namespace PasswordSharing.UnitTests
{
	public class ConcurrentHelperTests
	{
		public class TestIdentifiable : IIDentifiable
		{
			public int Id { get; set; }
		}

		public class AnotherTestIdentifiable : IIDentifiable
		{
			public int Id { get; set; }
		}

		private readonly IConcurrentHelper<TestIdentifiable> _testHelper;
		private readonly IConcurrentHelper<AnotherTestIdentifiable> _anotherHelper;
		private readonly Mock<IDbRepository<TestIdentifiable>> _testRepositoryMock;
		private readonly Mock<IDbRepository<AnotherTestIdentifiable>> _anotherRepositoryMock;

		public ConcurrentHelperTests()
		{
			_testRepositoryMock = new Mock<IDbRepository<TestIdentifiable>>();
			_testHelper = new ConcurrentHelper<TestIdentifiable>(_testRepositoryMock.Object);

			_testRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<TestIdentifiable, bool>>>()))
				.Returns(Task.FromResult(new TestIdentifiable()));

			_anotherRepositoryMock = new Mock<IDbRepository<AnotherTestIdentifiable>>();
			_anotherHelper = new ConcurrentHelper<AnotherTestIdentifiable>(_anotherRepositoryMock.Object);

			_anotherRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<AnotherTestIdentifiable, bool>>>()))
				.Returns(Task.FromResult(new AnotherTestIdentifiable()));
		}

		[Fact]
		public async Task SingleOrDefaultAsyncShouldLockResource()
		{
			// Arrange
			// Act
			Task<LockWrapper<AnotherTestIdentifiable>> password2Task;
			using (await _anotherHelper.SingleOrDefaultAsync(x => true))
			{
				password2Task = _anotherHelper.SingleOrDefaultAsync(x => true);

				// Assert
				await Task.Delay(TimeSpan.FromMilliseconds(400));
				password2Task.Status.ShouldNotBe(TaskStatus.RanToCompletion);
			}

			using (await password2Task)
			{
			}
		}

		[Fact]
		public async Task SingleOrDefaultAsyncShouldLockTwoResourceWithoutDeadlocks()
		{
			// Arrange
			var id = 0;
			_anotherRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<AnotherTestIdentifiable, bool>>>()))
				.Returns(() => Task.FromResult(new AnotherTestIdentifiable { Id = ++id }));

			// Act
			// Assert
			using (await _anotherHelper.SingleOrDefaultAsync(x => true))
			using (await _anotherHelper.SingleOrDefaultAsync(x => true))
			{
			}
		}

		[Fact]
		public async Task SingleOrDefaultAsyncShouldLockTwoResourceWithoutDeadlocksForDifferentClasses()
		{
			// Arrange
			// Act
			// Assert
			using (await _anotherHelper.SingleOrDefaultAsync(x => true))
			using (await _testHelper.SingleOrDefaultAsync(x => true))
			{
			}
		}

		[Fact]
		public async Task SingleOrDefaultAsyncShouldReturnEmptyLockIfObjectIsNull()
		{
			// Arrange
			_anotherRepositoryMock.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<AnotherTestIdentifiable, bool>>>()))
				.Returns(() => Task.FromResult((AnotherTestIdentifiable)null));

			// Act
			// Assert
			using (var result = await _anotherHelper.SingleOrDefaultAsync(x => true))
			{
				result.ShouldBe(LockWrapper<AnotherTestIdentifiable>.Empty);
			}
		}

		[Fact]
		public async Task FindAsyncShouldReturnOnlyLockedResources()
		{
			// Arrange
			var id = 0;
			var result = new List<AnotherTestIdentifiable> { new AnotherTestIdentifiable { Id = ++id }, new AnotherTestIdentifiable { Id = ++id } };
			_anotherRepositoryMock.Setup(x => x.FindAsync(It.IsAny<Expression<Func<AnotherTestIdentifiable, bool>>>()))
				.Returns(() => Task.FromResult(result.ToArray()));

			var locks = new LockWrapper<AnotherTestIdentifiable>[0];
			try
			{
				// Act
				locks = await _anotherHelper.FindAsync(x => true);

				// Assert
				locks.Length.ShouldBe(2);
			}
			finally
			{
				foreach (var lockWrapper in locks)
				{
					lockWrapper.Dispose();
				}
			}
		}
	}
}
