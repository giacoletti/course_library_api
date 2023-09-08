using AutoMapper;
using CourseLibrary.API.Controllers;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Profiles;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CourseLibrary.Test
{
    public class AuthorsControllerTests
    {
        private readonly Guid TestAuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35");
        private readonly AuthorsController _authorsController;

        public AuthorsControllerTests()
        {
            var courseLibraryRepositoryMock = new Mock<ICourseLibraryRepository>();
            courseLibraryRepositoryMock
                .Setup(m => m.GetAuthorsAsync())
                .ReturnsAsync(
                    new List<Author>()
                    {
                        new Author("Jaimy", "Johnson", "Navigation"),
                        new Author("Anne", "Adams", "Rum")
                    });
            courseLibraryRepositoryMock
                .Setup(m => m.GetAuthorAsync(TestAuthorId))
                .ReturnsAsync(
                        new Author("Jaimy", "Johnson", "Navigation")
                        {
                            Id = TestAuthorId
                        }
                       );

            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<AuthorsProfile>());
            var mapper = new Mapper(mapperConfiguration);

            _authorsController = new AuthorsController(courseLibraryRepositoryMock.Object, mapper);
        }

        [Fact]
        public async Task GetAuthors_GetAction_MustReturnOkObjectResult()
        {
            // Act
            var result = await _authorsController.GetAuthors();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<AuthorDto>>>(result);
            Assert.IsType<OkObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetAuthor_GetAction_MustReturnOkObjectResult()
        {
            // Act
            var result = await _authorsController.GetAuthor(TestAuthorId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AuthorDto>>(result);
            Assert.IsType<OkObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetAuthor_GetActionWithNonexistentAuthorId_MustReturnNotFoundResult()
        {
            // Act
            var result = await _authorsController.GetAuthor(Guid.Parse("00000000-0000-0000-0000-000000000000"));

            // Assert
            var actionResult = Assert.IsType<ActionResult<AuthorDto>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task CreateAuthor_PostActionWithValidAuthor_MustReturnCreatedAtRouteResult()
        {
            // Arrange
            var author = new AuthorForCreationDto()
            {
                FirstName = "Jane",
                LastName = "Skewers",
                DateOfBirth = DateTimeOffset.Parse("1968-03-04T00:00:00"),
                MainCategory = "Rum"
            };

            // Act
            var result = await _authorsController.CreateAuthor(author);

            // Assert
            var actionResult = Assert.IsType<ActionResult<AuthorDto>>(result);
            Assert.IsType<CreatedAtRouteResult>(actionResult.Result);
        }
    }
}
