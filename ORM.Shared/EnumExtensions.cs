using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Shared
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();

            return attribute?.Name ?? value.ToString();
        }
        public static string GetDisplayName<TEnum>(string value) where TEnum : Enum
        {
            if (!int.TryParse(value, out int intValue))
                return value; // parse edilemezse aynen döndür

            if (!Enum.IsDefined(typeof(TEnum), intValue))
                return value;

            var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), intValue);

            var field = typeof(TEnum).GetField(enumValue.ToString());
            var attribute = field?.GetCustomAttribute<DisplayAttribute>();

            return attribute?.Name ?? enumValue.ToString();
        }
    }
}
