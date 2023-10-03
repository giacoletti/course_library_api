using AutoMapper;
using CourseLibrary.API.Controllers;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.Profiles;
using CourseLibrary.API.ResourceParameters;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Dynamic;

namespace CourseLibrary.Test
{
    public class AuthorsControllerTests
    {
        private readonly Guid TestAuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35");
        private readonly string TestMainCategory = "Singing";
        private readonly string TestSearchQuery = "sea";
        private readonly int TestPageNumber = 1;
        private readonly int TestPageSize = 5;
        private readonly string InvalidOrderBy = "dateofbirth";
        private readonly AuthorsController _authorsController;

        public AuthorsControllerTests()
        {
            var courseLibraryRepositoryMock = new Mock<ICourseLibraryRepository>();
            courseLibraryRepositoryMock
                .Setup(m => m.GetAuthorsAsync(It.IsAny<AuthorsResourceParameters>()))
                .ReturnsAsync(
                    new PagedList<Author>(
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
                        },
                        2,
                        1,
                        2));
            courseLibraryRepositoryMock
                .Setup(m => m.GetAuthorsAsync(
                    It.Is<AuthorsResourceParameters>(p => p.MainCategory == TestMainCategory)))
                .ReturnsAsync(
                    new PagedList<Author>(
                        new List<Author>()
                        {
                            new Author("Eli", "Ivory Bones Sweet", TestMainCategory)
                            {
                                Id = Guid.Parse("2902b665-1190-4c70-9915-b9c2d7680450"),
                                DateOfBirth = new DateTime(1957, 12, 16)
                            },
                            new Author("Arnold", "The Unseen Stafford", TestMainCategory)
                            {
                                Id = Guid.Parse("102b566b-ba1f-404c-b2df-e2cde39ade09"),
                                DateOfBirth = new DateTime(1957, 3, 6)
                            }
                        },
                        2,
                        1,
                        2));
            courseLibraryRepositoryMock
                .Setup(m => m.GetAuthorsAsync(
                    It.Is<AuthorsResourceParameters>(p => p.SearchQuery == TestSearchQuery)))
                .ReturnsAsync(
                    new PagedList<Author>(
                        new List<Author>()
                        {
                            new Author("Seabury", "Toxic Reyson", "Maps")
                            {
                                Id = Guid.Parse("5b3621c0-7b12-4e80-9c8b-3398cba7ee05"),
                                DateOfBirth = new DateTime(1956, 11, 23)
                            },
                            new Author("Tom", "Toxic Reyson", "Seas")
                            {
                                Id = Guid.Parse("123451c0-7b12-4e80-9c8b-3398cba7ee05"),
                                DateOfBirth = new DateTime(1976, 09, 23)
                            }
                        },
                        2,
                        1,
                        2));
            courseLibraryRepositoryMock
                .Setup(m => m.GetAuthorsAsync(
                    It.Is<AuthorsResourceParameters>(p => p.MainCategory == TestMainCategory && p.SearchQuery == TestSearchQuery)))
                .ReturnsAsync(
                    new PagedList<Author>(
                        new List<Author>()
                        {
                            new Author("Seabury", "Toxic Reyson", TestMainCategory)
                            {
                                Id = Guid.Parse("5b3621c0-7b12-4e80-9c8b-3398cba7ee05"),
                                DateOfBirth = new DateTime(1956, 11, 23)
                            }
                        },
                        1,
                        1,
                        1));
            courseLibraryRepositoryMock
                .Setup(m => m.GetAuthorsAsync(
                    It.Is<AuthorsResourceParameters>(p => p.PageNumber == TestPageNumber && p.PageSize == TestPageSize)))
                .ReturnsAsync(
                    new PagedList<Author>(
                        new List<Author>()
                        {
                            new Author("Berry", "Griffin Beak Eldritch", "Ships")
                            {
                                Id = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                                DateOfBirth = new DateTime(1980, 7, 23)
                            },
                            new Author("Nancy", "Swashbuckler Rye", "Rum")
                            {
                                Id = Guid.Parse("da2fd609-d754-4feb-8acd-c4f9ff13ba96"),
                                DateOfBirth = new DateTime(1978, 5, 21)
                            }
                        },
                        2,
                        TestPageNumber,
                        TestPageSize));
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
            var propertyMappingServiceMock = new Mock<IPropertyMappingService>();
            propertyMappingServiceMock
                .Setup(m => m.ValidMappingExistsFor<AuthorDto, Author>("Name"))
                .Returns(true);
            propertyMappingServiceMock
                .Setup(m => m.ValidMappingExistsFor<AuthorDto, Author>(InvalidOrderBy))
                .Returns(false);

            _authorsController = new AuthorsController(courseLibraryRepositoryMock.Object, mapper, propertyMappingServiceMock.Object);
            // Ensure the controller can add response headers
            _authorsController.ControllerContext = new ControllerContext();
            _authorsController.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task GetAuthors_GetAction_MustReturnOkObjectResultWithExpandoObjectList()
        {
            // Act
            var result = await _authorsController.GetAuthors(new AuthorsResourceParameters());

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var expandoObjectList = Assert.IsType<List<ExpandoObject>>(objectResult.Value);
            Assert.True(expandoObjectList.Any());
        }

        [Fact]
        public async Task GetAuthors_GetAction_MustReturnCustomPaginationHeader()
        {
            // Act
            await _authorsController.GetAuthors(new AuthorsResourceParameters());

            // Assert
            Assert.True(_authorsController.Response.Headers.ContainsKey("X-Pagination"));
        }

        [Fact]
        public async Task GetAuthors_GetActionWithMainCategoryFilter_MustReturnOkObjectResult()
        {
            // Act
            var result = await _authorsController.GetAuthors(
                new AuthorsResourceParameters()
                {
                    MainCategory = TestMainCategory
                });

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var expandoObjectList = Assert.IsType<List<ExpandoObject>>(objectResult.Value);
            Assert.True(expandoObjectList.Any());
            Assert.All(expandoObjectList, author => Assert.Equal(TestMainCategory.ToLower(), ((dynamic)author).MainCategory.ToLower()));
        }

        [Fact]
        public async Task GetAuthors_GetActionWithSearchQueryString_MustReturnOkObjectResult()
        {
            // Act
            var result = await _authorsController.GetAuthors(
                new AuthorsResourceParameters()
                {
                    SearchQuery = TestSearchQuery
                });

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var expandoObjectList = Assert.IsType<List<ExpandoObject>>(objectResult.Value);
            Assert.True(expandoObjectList.Any());
            Assert.All(expandoObjectList,
                author => Assert.True(((dynamic)author).Name.ToLower().Contains(TestSearchQuery)
                || ((dynamic)author).MainCategory.ToLower().Contains(TestSearchQuery)));
        }

        [Fact]
        public async Task GetAuthors_GetActionWithMainCategoryFilterAndSearchQuery_MustReturnOkObjectResult()
        {
            // Act
            var result = await _authorsController.GetAuthors(
                new AuthorsResourceParameters()
                {
                    MainCategory = TestMainCategory,
                    SearchQuery = TestSearchQuery
                });

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var expandoObjectList = Assert.IsType<List<ExpandoObject>>(objectResult.Value);
            Assert.True(expandoObjectList.Any());
            Assert.All(expandoObjectList,
                author => Assert.True(
                    ((dynamic)author).MainCategory.ToLower() == TestMainCategory.ToLower()
                    && (((dynamic)author).Name.ToLower().Contains(TestSearchQuery)
                    || ((dynamic)author).MainCategory.ToLower().Contains(TestSearchQuery))));
        }

        [Fact]
        public async Task GetAuthors_GetActionWithPageNumberAndPageSize_MustReturnOkObjectResult()
        {
            // Act
            var result = await _authorsController.GetAuthors(
                new AuthorsResourceParameters()
                {
                    PageNumber = TestPageNumber,
                    PageSize = TestPageSize
                });

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var expandoObjectList = Assert.IsType<List<ExpandoObject>>(objectResult.Value);
            Assert.Equal(2, expandoObjectList.Count());
        }

        [Fact]
        public async Task GetAuthors_GetActionWithFieldsIdName_MustReturnOnlyRequestedFields()
        {
            // Act
            var result = await _authorsController.GetAuthors(
                new AuthorsResourceParameters()
                {
                    Fields = "id,name"
                });

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var expandoObjectList = Assert.IsType<List<ExpandoObject>>(objectResult.Value);
            Assert.True(expandoObjectList.Any());
            ((IDictionary<string, object?>)expandoObjectList[0]).TryGetValue("Id", out var obj);
            Assert.NotNull(obj);
            ((IDictionary<string, object?>)expandoObjectList[0]).TryGetValue("Name", out obj);
            Assert.NotNull(obj);
            ((IDictionary<string, object?>)expandoObjectList[0]).TryGetValue("Age", out obj);
            Assert.Null(obj);
            ((IDictionary<string, object?>)expandoObjectList[0]).TryGetValue("MainCategory", out obj);
            Assert.Null(obj);
        }

        [Fact]
        public async Task GetAuthors_GetActionWithInvalidOrderByParameter_MustReturnBadRequest()
        {
            // Act
            var result = await _authorsController.GetAuthors(
                new AuthorsResourceParameters()
                {
                    OrderBy = InvalidOrderBy
                });

            // Assert
            Assert.IsType<BadRequestResult>(result);
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
