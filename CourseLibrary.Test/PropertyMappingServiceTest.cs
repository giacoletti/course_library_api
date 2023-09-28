using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;

namespace CourseLibrary.Test;

public class PropertyMappingServiceTest
{
    private readonly IPropertyMappingService _propertyMappingService;

    public PropertyMappingServiceTest()
    {
        _propertyMappingService = new PropertyMappingService();
    }

    [Fact]
    public void GetPropertyMapping_FromAuthorDtoToAuthor_MustReturnAuthorPropertyMappingDictionary()
    {
        // Act
        var authorPropertyMappingDictionary = _propertyMappingService.GetPropertyMapping<AuthorDto, Author>();

        // Assert
        Assert.IsType<Dictionary<string, PropertyMappingValue>>(authorPropertyMappingDictionary);
        Assert.Contains("Id", authorPropertyMappingDictionary["Id"].DestinationProperties);
        Assert.False(authorPropertyMappingDictionary["Id"].Revert);
        Assert.Contains("MainCategory", authorPropertyMappingDictionary["MainCategory"].DestinationProperties);
        Assert.False(authorPropertyMappingDictionary["MainCategory"].Revert);
        Assert.Contains("DateOfBirth", authorPropertyMappingDictionary["Age"].DestinationProperties);
        Assert.True(authorPropertyMappingDictionary["Age"].Revert);
        Assert.Contains("FirstName", authorPropertyMappingDictionary["Name"].DestinationProperties);
        Assert.Contains("LastName", authorPropertyMappingDictionary["Name"].DestinationProperties);
        Assert.False(authorPropertyMappingDictionary["Name"].Revert);
    }
}
