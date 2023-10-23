namespace CourseLibrary.API.Services;
using System.Reflection;

public class PropertyCheckerService : IPropertyCheckerService
{
    public bool TypeHasProperties<T>(string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields))
        {
            return true;
        }

        // the fields are separated by a comma ","
        var fieldsAfterSplit = fields.Split(',');

        // check if the requested fields exist on source
        foreach (var field in fieldsAfterSplit)
        {
            var propertyName = field.Trim();

            // use reflection to check if the property can be found on T
            var propertyInfo = typeof(T)
                .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            // if can't be found return false
            if (propertyInfo is null)
            {
                return false;
            }
        }

        // all checks out
        return true;
    }
}
