using System;
using System.Linq;
using System.Reflection;

namespace BotFramework.Utils;

public static class FieldFinder
{
    public static object? FindField(object obj, Type fieldType, string fieldName)
    {
        try
        {
            var type = obj.GetType();
        
            var field = type
                       .GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                       .FirstOrDefault(field => field.FieldType == fieldType && field.Name == fieldName);
            return field?.GetValue(obj);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
        }
        return null;
    }
}
