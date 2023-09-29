using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CourseLibrary.Test
{
    public class CourseLibraryRepositoryTests
    {
        private readonly ICourseLibraryRepository _courseLibraryRepository;
        public CourseLibraryRepositoryTests()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
            var optionsBuilder = new DbContextOptionsBuilder<CourseLibraryContext>().UseSqlite(connection);
            var dbContext = new CourseLibraryContext(optionsBuilder.Options);
            dbContext.Database.Migrate();

            var propertyMappingServiceMock = new Mock<IPropertyMappingService>();

            _courseLibraryRepository = new CourseLibraryRepository(dbContext, propertyMappingServiceMock.Object);
        }

        [Fact]
        public async Task GetAuthorsAsync_MustReturnListOfAuthorsOrderedByFirstNameAndLastName()
        {
            // Act
            var result = await _courseLibraryRepository.GetAuthorsAsync(new API.ResourceParameters.AuthorsResourceParameters());

            // Assert
            Assert.IsType<PagedList<Author>>(result);
            Assert.True(result.Any());
            var expectedResult = result.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
            result.Should().ContainInOrder(expectedResult);
        }

        [Fact]
        public async Task GetAuthorAsync_ExistingAuthorId_MustReturnAuthor()
        {
            // Act
            var result = await _courseLibraryRepository.GetAuthorAsync(Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"));

            // Assert
            Assert.IsType<Author>(result);
        }

        [Fact]
        public async Task GetAuthorAsync_NonexistentAuthorId_MustReturnNull()
        {
            // Act
            var result = await _courseLibraryRepository.GetAuthorAsync(Guid.Parse("10000000-0000-0000-0000-000000000000"));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAuthorAsync_EmptyGuid_MustThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () =>
                await _courseLibraryRepository.GetAuthorAsync(Guid.Parse("00000000-0000-0000-0000-000000000000"))
                );
        }
    }
}
