using NTDLS.Helpers;
using NTDLS.Semaphore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ae.Engine.Helpers
{
    /// <summary>
    /// Provides utility methods for performing reflection-based operations, such as retrieving and setting property
    /// values, creating instances, and querying type information at runtime.
    /// </summary>
    /// <remarks>AeReflection offers a set of static methods to simplify common reflection tasks, including
    /// accessing properties by name, determining type relationships, and caching type information for improved
    /// performance. The methods are designed to work with both instance and static members, and include support for
    /// generic type queries. Thread safety is maintained internally for cached resources. Use these methods to reduce
    /// boilerplate code when working with reflection in .NET applications.</remarks>
    public static class AeReflection
    {
        private static readonly PessimisticCriticalResource<Dictionary<string, Type>> _typeCache = new();
        private static readonly PessimisticCriticalResource<Dictionary<string, PropertyInfo>> _staticPropertyCache = new();
        private static readonly PessimisticCriticalResource<Dictionary<Type, List<Type>>> _subClassesOfCache = new();

        /// <summary>
        /// Retrieves the value of a public instance property from the specified object by property name.
        /// </summary>
        /// <remarks>If the property does not exist or the parameters are invalid, the method returns
        /// null. Only public instance properties are considered.</remarks>
        /// <param name="obj">The object from which to retrieve the property value. Must not be null.</param>
        /// <param name="propertyName">The name of the property to retrieve. Case-insensitive. Cannot be null or whitespace.</param>
        /// <returns>The value of the specified property if found; otherwise, null.</returns>
        public static object? GetPropertyValue(object obj, string propertyName)
        {
            if (obj == null || string.IsNullOrWhiteSpace(propertyName))
                return null;

            var prop = obj.GetType().GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

            return prop?.GetValue(obj);
        }

        /// <summary>
        /// Sets the value of a writable property on the specified object instance by property name.
        /// </summary>
        /// <param name="instance">The object whose property value will be set. Must not be null and must have the specified property.</param>
        /// <param name="propertyName">The name of the property to set. The property must exist and be writable on the object's type.</param>
        /// <param name="value">The value to assign to the property. Can be null if the property type allows null values.</param>
        /// <exception cref="InvalidOperationException">Thrown if the specified property does not exist on the object's type or if the property is read-only.</exception>
        public static void SetPropertyValue(object instance, string propertyName, object? value)
        {
            var type = instance.GetType();

            var prop = type.GetProperty(propertyName)
                ?? throw new InvalidOperationException($"Property '{propertyName}' not found on type {type.Name}.");

            if (!prop.CanWrite)
                throw new InvalidOperationException($"Property '{propertyName}' is read-only.");

            prop.SetValue(instance, value);
        }

        /// <summary>
        /// Determines whether the specified type is assignable to a given generic type definition, either directly or
        /// through its inheritance hierarchy.
        /// </summary>
        /// <remarks>This method checks both implemented interfaces and base types to determine
        /// assignability to the generic type definition. Use this method when you need to verify if a type or its
        /// ancestors match a generic type, regardless of the specific type arguments.</remarks>
        /// <param name="givenType">The type to check for assignability to the generic type definition.</param>
        /// <param name="genericType">The generic type definition to compare against.</param>
        /// <returns>true if the specified type is assignable to the generic type definition; otherwise, false.</returns>
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            return givenType.GetInterfaces()
                .Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                || givenType.BaseType != null && (givenType.BaseType.IsGenericType &&
                                                   givenType.BaseType.GetGenericTypeDefinition() == genericType ||
                                                   IsAssignableToGenericType(givenType.BaseType, genericType));
        }

        /// <summary>
        /// Returns all types in the current application domain that are subclasses of the specified type parameter.
        /// </summary>
        /// <remarks>This method scans all loaded assemblies in the current application domain to find
        /// subclasses of the specified type. Results are cached for subsequent calls to improve performance.</remarks>
        /// <typeparam name="T">The base type for which subclasses are to be retrieved.</typeparam>
        /// <returns>An enumerable collection of types that inherit from the specified type parameter. The collection is empty if
        /// no subclasses are found.</returns>
        public static IEnumerable<Type> GetSubClassesOf<T>()
        {
            var cached = _subClassesOfCache.Use(o =>
            {
                o.TryGetValue(typeof(T), out var cached);
                return cached;
            });
            if (cached != null)
            {
                return cached;
            }

            List<Type> allTypes = [];

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                allTypes.AddRange(assembly.GetTypes().Where(type => type.IsSubclassOf(typeof(T))));
            }

            _subClassesOfCache.Use(o => o.TryAdd(typeof(T), allTypes));

            return allTypes;
        }

        /// <summary>
        /// Retrieves the value of a public static string property from a specified type by name.
        /// </summary>
        /// <remarks>The method uses an internal cache to optimize repeated access to static property
        /// values. Only public static properties of type string are supported.</remarks>
        /// <param name="typeName">The fully qualified name of the type containing the static property.</param>
        /// <param name="propertyName">The name of the public static property whose value is to be retrieved.</param>
        /// <returns>The value of the specified static string property. Returns an empty string if the property exists but its
        /// value is null.</returns>
        /// <exception cref="Exception">Thrown if the specified type cannot be found or if the static property does not exist on the type.</exception>
        public static string GetStaticPropertyValue(string typeName, string propertyName)
        {
            string key = $"[{typeName}].[{propertyName}]";

            var cached = _staticPropertyCache.Use(o =>
            {
                if (o.TryGetValue(key, out var cachedPropertyInfo))
                {
                    return cachedPropertyInfo.GetValue(null) as string;
                }
                return null;
            });

            if (cached != null)
            {
                return cached;
            }

            var type = GetTypeByName(typeName) ?? throw new Exception("Type not found.");

            var propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
            if (propertyInfo != null)
            {
                _staticPropertyCache.Use(o => o.TryAdd(key, propertyInfo));
                return propertyInfo.GetValue(null) as string ?? string.Empty;
            }

            throw new Exception("Static property not found: {typeName}->{propertyName}.");
        }

        /// <summary>
        /// Creates an instance of the specified type using the provided constructor arguments.
        /// </summary>
        /// <remarks>If the specified type does not have a matching constructor or cannot be instantiated,
        /// an exception will be thrown. The created instance is cast to type T; ensure that the type is assignable to T
        /// to avoid runtime errors.</remarks>
        /// <typeparam name="T">The type of the object to create. Must be compatible with the specified type.</typeparam>
        /// <param name="type">The type to instantiate. Must have a constructor matching the provided arguments.</param>
        /// <param name="constructorArgs">An array of arguments to pass to the constructor of the specified type. Can be empty if the type has a
        /// parameterless constructor.</param>
        /// <returns>An instance of type T created from the specified type and constructor arguments.</returns>
        public static T CreateInstanceFromType<T>(Type type, object[] constructorArgs)
        {
            return (T)Activator.CreateInstance(type, constructorArgs).EnsureNotNull();
        }

        //public static T CreateInstanceFromType<T>(Type type)
        //{
        //    return (T)Activator.CreateInstance(type).EnsureNotNull();
        //}

        //public static T CreateInstanceFromTypeName<T>(string typeName, object[] constructorArgs)
        //{
        //    var type = GetTypeByName(typeName);
        //    return (T)Activator.CreateInstance(type, constructorArgs).EnsureNotNull();
        //}

        //public static T CreateInstanceFromTypeName<T>(string typeName)
        //{
        //    var type = GetTypeByName(typeName);
        //    return (T)Activator.CreateInstance(type).EnsureNotNull();
        //}

        //public static bool DoesTypeExist(string typeName)
        //{
        //    return GetTypeByName(typeName) != null;
        //}

        /// <summary>
        /// Retrieves a type object for the specified type name from the loaded assemblies in the current application
        /// domain.
        /// </summary>
        /// <remarks>The search is performed across all assemblies currently loaded in the application
        /// domain. The method uses a cache to improve performance for repeated lookups. Only types with a matching
        /// simple name are considered; namespace is not taken into account.</remarks>
        /// <param name="typeName">The name of the type to locate. This should be the simple name of the type as defined in its assembly.</param>
        /// <returns>A Type object representing the type with the specified name if found; otherwise, an exception is thrown.</returns>
        /// <exception cref="Exception">Thrown if no type with the specified name is found in any loaded assembly.</exception>
        public static Type GetTypeByName(string typeName)
        {
            var cached = _typeCache.Use(o =>
            {
                o.TryGetValue(typeName, out var cachedType);
                return cachedType;
            });

            if (cached != null)
            {
                return cached;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetTypes().SingleOrDefault(t => t.Name == typeName);
                if (type != null)
                {
                    _typeCache.Use(o => o.TryAdd(typeName, type));
                    return type;
                }
            }

            throw new Exception($"Type not found: {typeName}");
        }

        /// <summary>
        /// Builds a reflection cache for all subclasses of the specified type parameter. This method attempts to
        /// resolve and cache type information for each subclass, optionally reporting progress and logging errors.
        /// </summary>
        /// <remarks>If a subclass type cannot be resolved, the error is either logged using the provided
        /// delegate or the exception is thrown if no delegate is specified. This method is useful for preparing type
        /// information for later reflection-based operations.</remarks>
        /// <typeparam name="T">The base type for which subclasses will be discovered and cached. Must be a class type.</typeparam>
        /// <param name="progressCallback">An optional callback that receives progress updates as the cache is built. The first parameter is the name
        /// of the type being processed; the second parameter is the progress value as a float between 0 and 1.</param>
        /// <param name="writeLog">An optional delegate used to log errors encountered during caching. If not provided, exceptions will be
        /// thrown when a type cannot be cached.</param>
        public static void BuildReflectionCacheOfType<T>(Action<string, float>? progressCallback, WriteLogDelegate? writeLog = null)
        {
            foreach (var item in GetSubClassesOf<T>())
            {
                try
                {
                    _ = AeReflection.GetTypeByName(item.Name);
                }
                catch (Exception ex)
                {
                    if (writeLog != null)
                    {
                        writeLog?.Invoke($"Failed to cache type {item.Name}: {ex.Message}", AeLoggingLevel.Error);
                    }
                    else throw;
                }
            }

            //public static T? CreateInstanceOf<T>(object[] constructorArgs)
            //{
            //    return (T?)Activator.CreateInstance(typeof(T), constructorArgs);
            //}
        }
    }
}
