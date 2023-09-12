﻿using AutoMapper;
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
                .Setup(m => m.GetAuthorsAsync(""))
                .ReturnsAsync(
                    new List<Author>()
                    {
                        new Author("Jaimy", "Johnson", "Navigation")
                        {
                            Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                            DateOfBirth = new DateTime(1980, 7, 23)
                        },
                        new Author("Anne", "Adams", "Rum")
                        {
                            Id = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                            DateOfBirth = new DateTime(1978, 5, 21)
                        }
                    });
            courseLibraryRepositoryMock
                .Setup(m => m.GetAuthorsAsync("Singing"))
                .ReturnsAsync(
                    new List<Author>()
                    {
                        new Author("Eli", "Ivory Bones Sweet", "Singing")
                        {
                            Id = Guid.Parse("2902b665-1190-4c70-9915-b9c2d7680450"),
                            DateOfBirth = new DateTime(1957, 12, 16)
                        },
                        new Author("Arnold", "The Unseen Stafford", "Singing")
                        {
                            Id = Guid.Parse("102b566b-ba1f-404c-b2df-e2cde39ade09"),
                            DateOfBirth = new DateTime(1957, 3, 6)
                        }
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
            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var authorDtoList = Assert.IsType<List<AuthorDto>>(objectResult.Value);
            Assert.True(authorDtoList.Any());
        }

        [Fact]
        public async Task GetAuthors_GetActionWithMainCategoryFilter_MustReturnOkObjectResult()
        {
            // Act
            var result = await _authorsController.GetAuthors("Singing");

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<AuthorDto>>>(result);
            var objectResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var authorDtoList = Assert.IsType<List<AuthorDto>>(objectResult.Value);
            Assert.True(authorDtoList.Any());
            Assert.All(authorDtoList, author => Assert.Equal("Singing", author.MainCategory));
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
