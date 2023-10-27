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
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
        private readonly string TestFields = "id,name";
        private readonly string InvalidField = "FirstName";
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
            var propertyCheckerServiceMock = new Mock<IPropertyCheckerService>();
            propertyCheckerServiceMock
                .Setup(m => m.TypeHasProperties<AuthorDto>(null))
                .Returns(true);
            propertyCheckerServiceMock
                .Setup(m => m.TypeHasProperties<AuthorDto>(TestFields))
                .Returns(true);
            propertyCheckerServiceMock
                .Setup(m => m.TypeHasProperties<AuthorDto>(InvalidField))
                .Returns(false);
            var problemDetailsFactoryMock = new Mock<ProblemDetailsFactory>();

            /* UrlHelper mock for Link generation */
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper.Setup(x => x.Link("GetAuthor", It.IsAny<object>())).Returns($"http://localhost:5000/api/authors/{TestAuthorId}");
            urlHelper.Setup(x => x.Link("CreateCourseForAuthor", It.IsAny<object>())).Returns($"http://localhost:5000/api/authors/{TestAuthorId}");
            urlHelper.Setup(x => x.Link("GetCoursesForAuthor", It.IsAny<object>())).Returns($"http://localhost:5000/api/authors/{TestAuthorId}");

            _authorsController =
                new AuthorsController(
                    courseLibraryRepositoryMock.Object,
                    mapper,
                    propertyMappingServiceMock.Object,
                    propertyCheckerServiceMock.Object,
                    problemDetailsFactoryMock.Object)
                {
                    Url = urlHelper.Object,
                };

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
            Assert.NotNull(objectResult.Value);
            var linksValue = objectResult.Value.GetType().GetProperty("links")?.GetValue(objectResult.Value);
            var linksList = Assert.IsType<List<LinkDto>>(linksValue);
            Assert.True(linksList.Any());
            var valueData = objectResult.Value.GetType().GetProperty("value")?.GetValue(objectResult.Value);
            var dictionaryList = Assert.IsAssignableFrom<IEnumerable<IDictionary<string, object>>>(valueData);
            Assert.True(dictionaryList.Any());
            var expandoObject = Assert.IsType<ExpandoObject>(dictionaryList.First());
            var dictionaryObj = expandoObject as IDictionary<string, object>;
            var authorDtoProperties = typeof(AuthorDto).GetProperties();
            foreach (var authorDtoProperty in authorDtoProperties)
            {
                dictionaryObj.TryGetValue(authorDtoProperty.Name, out var value);
                Assert.NotNull(value);
            }
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
            Assert.NotNull(objectResult.Value);
            var valueData = objectResult.Value.GetType().GetProperty("value")?.GetValue(objectResult.Value);
            var dictionaryList = Assert.IsAssignableFrom<IEnumerable<IDictionary<string, object>>>(valueData);
            Assert.True(dictionaryList.Any());
            Assert.All(dictionaryList, author => Assert.Equal(TestMainCategory.ToLower(), ((dynamic)author).MainCategory.ToLower()));
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
            Assert.NotNull(objectResult.Value);
            var valueData = objectResult.Value.GetType().GetProperty("value")?.GetValue(objectResult.Value);
            var dictionaryList = Assert.IsAssignableFrom<IEnumerable<IDictionary<string, object>>>(valueData);
            Assert.True(dictionaryList.Any());
            Assert.All(dictionaryList,
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
            Assert.NotNull(objectResult.Value);
            var valueData = objectResult.Value.GetType().GetProperty("value")?.GetValue(objectResult.Value);
            var dictionaryList = Assert.IsAssignableFrom<IEnumerable<IDictionary<string, object>>>(valueData);
            Assert.True(dictionaryList.Any());
            Assert.All(dictionaryList,
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
            Assert.NotNull(objectResult.Value);
            var valueData = objectResult.Value.GetType().GetProperty("value")?.GetValue(objectResult.Value);
            var dictionaryList = Assert.IsAssignableFrom<IEnumerable<IDictionary<string, object>>>(valueData);
            Assert.Equal(2, dictionaryList.Count());
        }

        [Fact]
        public async Task GetAuthors_GetActionWithFieldsIdName_MustReturnOnlyRequestedFields()
        {
            // Act
            var result = await _authorsController.GetAuthors(
                new AuthorsResourceParameters()
                {
                    Fields = TestFields
                });

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(objectResult.Value);
            var valueData = objectResult.Value.GetType().GetProperty("value")?.GetValue(objectResult.Value);
            var dictionaryList = Assert.IsAssignableFrom<IEnumerable<IDictionary<string, object>>>(valueData);
            Assert.True(dictionaryList.Any());
            dictionaryList.First().TryGetValue("Id", out var obj);
            Assert.NotNull(obj);
            dictionaryList.First().TryGetValue("Name", out obj);
            Assert.NotNull(obj);
            dictionaryList.First().TryGetValue("Age", out obj);
            Assert.Null(obj);
            dictionaryList.First().TryGetValue("MainCategory", out obj);
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
        public async Task GetAuthors_GetActionWithInvalidFieldsParameter_MustReturnBadRequest()
        {
            // Act
            var result = await _authorsController.GetAuthors(
                new AuthorsResourceParameters()
                {
                    Fields = InvalidField
                });

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAuthor_GetAction_MustReturnOkObjectResultWithExpandoObjectWithLinksList()
        {
            // Act
            var result = await _authorsController.GetAuthor(TestAuthorId, null);

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var expandoObject = Assert.IsType<ExpandoObject>(objectResult.Value);
            ((IDictionary<string, object?>)expandoObject).TryGetValue("links", out var obj);
            Assert.NotNull(obj);
            Assert.Equal(3, ((List<LinkDto>)obj).Count());
        }

        [Fact]
        public async Task GetAuthor_GetActionWithFieldsIdName_MustReturnOnlyRequestedFields()
        {
            // Act
            var result = await _authorsController.GetAuthor(TestAuthorId, TestFields);

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result);
            var expandoObject = Assert.IsType<ExpandoObject>(objectResult.Value);
            Assert.NotNull(expandoObject);
            ((IDictionary<string, object?>)expandoObject).TryGetValue("Id", out var obj);
            Assert.NotNull(obj);
            ((IDictionary<string, object?>)expandoObject).TryGetValue("Name", out obj);
            Assert.NotNull(obj);
            ((IDictionary<string, object?>)expandoObject).TryGetValue("Age", out obj);
            Assert.Null(obj);
            ((IDictionary<string, object?>)expandoObject).TryGetValue("MainCategory", out obj);
            Assert.Null(obj);
        }

        [Fact]
        public async Task GetAuthor_GetActionWithNonexistentAuthorId_MustReturnNotFoundResult()
        {
            // Act
            var result = await _authorsController.GetAuthor(Guid.Parse("00000000-0000-0000-0000-000000000000"), null);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetAuthor_GetActionWithInvalidFieldsParameter_MustReturnBadRequest()
        {
            // Act
            var result = await _authorsController.GetAuthor(TestAuthorId, InvalidField);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task CreateAuthor_PostActionWithValidAuthor_MustReturnCreatedAtRouteResultWithLinksList()
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
            var objectResult = Assert.IsType<CreatedAtRouteResult>(actionResult.Result);
            var expandoObject = Assert.IsType<ExpandoObject>(objectResult.Value);
            ((IDictionary<string, object?>)expandoObject).TryGetValue("links", out var obj);
            Assert.NotNull(obj);
            Assert.Equal(3, ((List<LinkDto>)obj).Count());
        }
    }
}
