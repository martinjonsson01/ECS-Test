using System;
using System.Linq.Expressions;
using System.Reflection;

using Unity.Physics;

namespace Tests
{
public static class TestExtensions
{
    public static void SetProperty<TSource, TProperty>(
        this TSource source,
        Expression<Func<TSource, TProperty>> prop,
        TProperty value)
    {
        var propertyInfo = (PropertyInfo) ((MemberExpression) prop.Body).Member;
        propertyInfo.SetValue(source, value);
    }

    public static void SetField<TSource, TField>(
        this TSource source,
        string fieldName,
        TField value)
    {
        FieldInfo[] fields = source.GetType().GetFields(
            BindingFlags.Public);
        foreach (FieldInfo field in fields)
        {
            if (!field.Name.Equals(fieldName)) continue;
            field.SetValue(source, value);
        }
    }

    public static void SetFieldOfField<TSource, TField>(
        this TSource source,
        string fieldName1,
        string fieldName2,
        TField value)
    {
        FieldInfo[] fields = source.GetType().GetFields(
            BindingFlags.NonPublic |
            BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            if (!field.Name.Equals(fieldName1)) continue;
            object obj2 = field.GetValue(source);
            obj2.SetField(fieldName2, value);
        }
    }

    public static void SetEntityPair<TSource>(
        this TSource source,
        EntityPair pair)
    {
        FieldInfo[] fields = source.GetType().GetFields(
            BindingFlags.NonPublic |
            BindingFlags.Instance);
        FieldInfo triggerEventDataField = fields[0];
        Type type = triggerEventDataField.FieldType;
        object eventDataObject = triggerEventDataField.GetValue(source);
        FieldInfo entitiesField = type.GetField("Entities");
        entitiesField.SetValue(eventDataObject, pair);
        triggerEventDataField.SetValue(source, eventDataObject);
    }
}
}
