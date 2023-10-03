using System.Dynamic;
using System.Reflection;

namespace CourseLibrary.API.Helpers;

public static class IEnumerableExtensions
{
    public static IEnumerable<ExpandoObject> ShapeData<TSource>(
        this IEnumerable<TSource> source,
        string? fields)
    {
        if (source is null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        // create list to hold our ExpandoObjects
        var expandoObjectList = new List<ExpandoObject>();

        // create a list with PropertyInfo objects on TSource. Reflection is expensive,
        // so we do it once and reuse the results.
        var propertyInfoList = new List<PropertyInfo>();

        if (string.IsNullOrWhiteSpace(fields))
        {
            // all public properties should be in the ExpandoObject
            var propertyInfos = typeof(TSource)
                .GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            propertyInfoList.AddRange(propertyInfos);
        }
        else
        {
            // the fields are separated by "," so we split it
            var fieldsAfterSplit = fields.Split(',');

            foreach (var field in fieldsAfterSplit)
            {
                var propertyName = field.Trim();

                // use reflection to get the property on the source object
                // we need to include public and instance. Note: specifying a binding
                // flag overwrites the already existing binding flags
                var propertyInfo = typeof(TSource)
                    .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (propertyInfo is null)
                {
                    throw new Exception($"Property {propertyName} wasn't found on {typeof(TSource)}");
                }

                propertyInfoList.Add(propertyInfo);
            }
        }

        // run through the source objects
        foreach (TSource sourceObject in source)
        {
            // the ExpandoObject will hold the selected properties and values
            var dataShapedObject = new ExpandoObject();

            // get the value of each property we have to return
            foreach (var propertyInfo in propertyInfoList)
            {
                // GetValue returns the value of the property on the source object
                var propertyValue = propertyInfo.GetValue(sourceObject);

                // add field to the ExpandoObject
                ((IDictionary<string, object?>)dataShapedObject).Add(propertyInfo.Name, propertyValue);
            }

            // add ExpandoObject to the list
            expandoObjectList.Add(dataShapedObject);
        }

        return expandoObjectList;
    }
}
