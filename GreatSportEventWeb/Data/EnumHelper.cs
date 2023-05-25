using System.ComponentModel.DataAnnotations;
using GreatSportEventWeb.Models;
using Type = System.Type;

namespace GreatSportEventWeb.Data;

public static class EnumHelper
{
    public static List<EnumDropdownModel> GetEnumDropdownList<T>()
    {
        var enumType = typeof(T);
        var enumValues = Enum.GetValues(enumType);
        var dropdownList = new List<EnumDropdownModel>();

        foreach (var enumValue in enumValues)
            if (enumValue is T value)
                dropdownList.Add(new EnumDropdownModel
                {
                    Id = Convert.ToInt32(value),
                    Name = GetEnumDisplayName(enumType, value.ToString() ?? string.Empty)
                });

        return dropdownList;
    }

    private static string GetEnumDisplayName(Type enumType, string enumValue)
    {
        var fieldInfo = enumType.GetField(enumValue);
        var displayAttribute =
            fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;

        if (displayAttribute?.Name != null) return displayAttribute.Name;

        return enumValue;
    }

    public static string GetDisplayName(this Enum value)
    {
        var enumType = value.GetType();
        var fieldInfo = enumType.GetField(value.ToString());

        if (fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false) is DisplayAttribute[]
            {
                Length: > 0
            } attributes)
        {
            var displayName = attributes[0].Name;
            if (displayName != null) return displayName;
        }

        return value.ToString();
    }
}