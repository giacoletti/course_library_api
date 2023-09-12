using AutoMapper;
using CourseLibrary.API.Controllers;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Profiles;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Moq;

namespace CourseLibrary.Test
{
    public class CoursesControllerTests
    {
        private readonly Guid TestAuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35");
        private readonly Guid TestCourseId = Guid.Parse("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b");
        private readonly CoursesController _coursesController;

        public CoursesControllerTests()
        {
            var courseLibraryRepositoryMock = new Mock<ICourseLibraryRepository>();
            courseLibraryRepositoryMock
                .Setup(m => m.AuthorExistsAsync(TestAuthorId))
                .ReturnsAsync(true);
            courseLibraryRepositoryMock
                .Setup(m => m.GetCoursesAsync(TestAuthorId))
                .ReturnsAsync(
                    new List<Course>()
                    {
                        new Course("Commandeering a Ship Without Getting Caught")
                        {
                            Id = Guid.Parse("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b"),
                            AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                            Description = "Commandeering a ship in rough waters isn't easy.  Commandeering it without getting caught is even harder.  In this course you'll learn how to sail away and avoid those pesky musketeers."
                        },
                        new Course("Overthrowing Mutiny")
                        {
                            Id = Guid.Parse("d8663e5e-7494-4f81-8739-6e0de1bea7ee"),
                            AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                            Description = "In this course, the author provides tips to avoid, or, if needed, overthrow pirate mutiny."
                        }
                    });
            courseLibraryRepositoryMock
                .Setup(m => m.GetCourseAsync(TestAuthorId, TestCourseId))
                .ReturnsAsync(
                    new Course("Commandeering a Ship Without Getting Caught")
                    {
                        Id = Guid.Parse("5b1c2b4d-48c7-402a-80c3-cc796ad49c6b"),
                        AuthorId = Guid.Parse("d28888e9-2ba9-473a-a40f-e38cb54f9b35"),
                        Description = "Commandeering a ship in rough waters isn't easy.  Commandeering it without getting caught is even harder.  In this course you'll learn how to sail away and avoid those pesky musketeers."
                    });

            var mapperConfiguration = new MapperConfiguration(cfg => cfg.AddProfile<CoursesProfile>());
            var mapper = new Mapper(mapperConfiguration);

            _coursesController = new CoursesController(courseLibraryRepositoryMock.Object, mapper);
        }

        [Fact]
        public async Task GetCoursesForAuthor_GetAction_MustReturnOkObjectResult()
        {
            // Act
            var result = await _coursesController.GetCoursesForAuthor(TestAuthorId);

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<CourseDto>>>(result);
            Assert.IsType<OkObjectResult>(actionResult.Result);
        }

        [Fact]
        public async Task GetCoursesForAuthor_GetActionWithNonexistentAuthorId_MustReturnNotFoundResult()
        {
            // Act
            var result = await _coursesController.GetCoursesForAuthor(Guid.Parse("10000000-0000-0000-0000-000000000000"));

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<CourseDto>>>(result);
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Fact]
        public async Task PartiallyUpdateCourseForAuthor_PatchAction_MustReturnNoContentResult()
        {
            // Arrange
            var patchDocument = new JsonPatchDocument<CourseForUpdateDto>();
            patchDocument.Replace(x => x.Title, "Updated title");

            var validatorMock = new Mock<IObjectModelValidator>();

            validatorMock.Setup(x => x.Validate(It.IsAny<ActionContext>(),
                                                It.IsAny<ValidationStateDictionary>(),
                                                It.IsAny<string>(),
                                                It.IsAny<Object>()));

            _coursesController.ObjectValidator = validatorMock.Object;

            // Act
            var result = await _coursesController.PartiallyUpdateCourseForAuthor(TestAuthorId, TestCourseId, patchDocument);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteCourseForAuthor_DeleteAction_MustReturnNoContentResult()
        {
            // Act
            var result = await _coursesController.DeleteCourseForAuthor(TestAuthorId, TestCourseId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }
    }
}
