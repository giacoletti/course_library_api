using CourseLibrary.API.Controllers;
using CourseLibrary.API.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CourseLibrary.Test;

public class RootControllerTests
{
    private readonly RootController _rootController;

    public RootControllerTests()
    {
        /* UrlHelper mock for Link generation */
        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(x => x.Link("GetRoot", It.IsAny<object>())).Returns($"http://localhost:5000/api");
        urlHelper.Setup(x => x.Link("GetAuthors", It.IsAny<object>())).Returns($"http://localhost:5000/api/authors");
        urlHelper.Setup(x => x.Link("CreateAuthor", It.IsAny<object>())).Returns($"http://localhost:5000/api/authors");

        _rootController =
            new RootController()
            {
                Url = urlHelper.Object
            };
    }

    [Fact]
    public void GetRoot_GetAction_MustReturnOkObjectResultWithLinksList()
    {
        // Act
        var response = _rootController.GetRoot();

        // Assert
        var objectResult = Assert.IsType<OkObjectResult>(response);
        var linksList = Assert.IsType<List<LinkDto>>(objectResult.Value);
        Assert.Equal(3, linksList.Count);
    }
}
