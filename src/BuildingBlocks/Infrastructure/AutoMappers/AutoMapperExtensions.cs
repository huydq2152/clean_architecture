﻿using System.Reflection;
using AutoMapper;

namespace Infrastructure.AutoMappers;

public static class AutoMapperExtensions
{
    public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> expression)
    {
        var flags = BindingFlags.Public | BindingFlags.Instance;
        var sourceType = typeof(TSource);
        var destinationProperties = typeof(TDestination).GetProperties(flags);

        foreach (var property in destinationProperties)
        {
            if (sourceType.GetProperty(property.Name, flags) == null)
            {
                expression.ForMember(property.Name, configurationExpression => configurationExpression.Ignore());
            }
        }
        return expression;
    }
    
    public static IMappingExpression<TSource, TDestination> IgnoreProperty<TSource, TDestination>(
        this IMappingExpression<TSource, TDestination> expression, string propertyName)
    {
        expression.ForMember(propertyName, opt => opt.Ignore());
        return expression;
    }
}