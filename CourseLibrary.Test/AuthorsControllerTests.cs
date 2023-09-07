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
    }
}
