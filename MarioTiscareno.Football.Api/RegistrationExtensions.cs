using FluentValidation;
using MarioTiscareno.Football.Api.Core;
using System.Reflection;

namespace MarioTiscareno.Football.Api;

public static class RegistrationExtensions
{
    public static IServiceCollection AddRequestHandlers(this IServiceCollection services)
    {
        foreach (Type type in GetHandlers(Assembly.GetExecutingAssembly()))
        {
            services.AddTransient(type);
        }

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        foreach (Type type in GetValidators(Assembly.GetExecutingAssembly()))
        {
            var validatorType = type.GetInterfaces()
                .Single(
                    i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)
                );
            services.AddTransient(validatorType, type);
        }

        return services;
    }

    private static IEnumerable<Type> GetHandlers(this Assembly assembly)
    {
        return assembly.DefinedTypes.Where(
            t =>
                !t.ContainsGenericParameters
                && t.IsConcrete()
                && t.IsAssignableToGeneric(typeof(IRequestHandler<,>))
        );
    }

    private static IEnumerable<Type> GetValidators(this Assembly assembly)
    {
        return assembly.DefinedTypes.Where(
            t =>
                !t.ContainsGenericParameters
                && t.IsConcrete()
                && t.IsAssignableToGeneric(typeof(IValidator<>))
        );
    }

    private static bool IsConcrete(this Type type) => !type.IsAbstract && !type.IsInterface;

    public static bool IsAssignableToGeneric(this Type givenType, Type genericType)
    {
        foreach (Type it in givenType.GetInterfaces())
        {
            if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }
        }

        if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
        {
            return true;
        }

        Type? baseType = givenType.BaseType;

        if (baseType == null)
        {
            return false;
        }

        return IsAssignableToGeneric(baseType, genericType);
    }
}
