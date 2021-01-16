using System;
using System.Linq;
using System.Reflection;
using System.Text;

using Unity.Entities;

namespace Game.Utils
{
public static class ComponentUtil
{
    public static string ToString<T>(T component) where T : struct, IComponentData
    {
        Type type = typeof(T);
        string structName = type.Name;
        var builder = new StringBuilder($"{structName}(");

        FieldInfo[] fields = type.GetFields(BindingFlags.Instance |
                                            BindingFlags.NonPublic |
                                            BindingFlags.Public);
        foreach (FieldInfo field in fields)
        {
            builder.Append($"{field.Name}={field.GetValue(component)}");

            if (field != fields.Last()) builder.Append(", ");
        }

        builder.Append(')');
        return builder.ToString();
    }
}
}
