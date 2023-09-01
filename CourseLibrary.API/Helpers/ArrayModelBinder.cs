using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.Reflection;

namespace CourseLibrary.API.Helpers;

public class ArrayModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        // binder works only on enumerable types
        if (!bindingContext.ModelMetadata.IsEnumerableType)
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        // Get inputted value through the value provider
        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).ToString();

        // if value is null or whitespace
        if (string.IsNullOrWhiteSpace(value)) 
        {
            bindingContext.Result = ModelBindingResult.Success(null);
            return Task.CompletedTask;
        }

        // the value is not null, type of model is enumerable, get enumerable type and converter
        var elementType = bindingContext.ModelType.GetTypeInfo().GenericTypeArguments[0];
        var converter = TypeDescriptor.GetConverter(elementType);

        // convert each item in the value list to the enumerable type
        var values = value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => converter.ConvertFromString(x.Trim())).ToArray();

        // create an array of that type, and set it as Model value
        var typedValues = Array.CreateInstance(elementType, values.Length);
        values.CopyTo(typedValues, 0);
        bindingContext.Model = typedValues;

        // return successful result
        bindingContext.Result = ModelBindingResult.Success(bindingContext.Model);
        return Task.CompletedTask;
    }
}
