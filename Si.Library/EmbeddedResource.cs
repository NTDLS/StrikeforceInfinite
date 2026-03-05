using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace Si.Library
{
    /// <summary>
    /// Used to read EmbeddedResources from assemblies.
    /// </summary>
    public static class EmbeddedResource
    {
        private static readonly MemoryCache _cache = new(new MemoryCacheOptions());

        public static string Load(string resourcePath)
        {
            string cacheKey = $":{resourcePath.ToLowerInvariant()}".Replace('.', ':').Replace('\\', ':').Replace('/', ':');

            if (_cache.Get(cacheKey) is string cachedResourceText)
            {
                return cachedResourceText;
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var resourceText = SearchAssembly(assembly, cacheKey, resourcePath);
                if (resourceText != null)
                {
                    return resourceText;
                }
            }

            throw new Exception($"The embedded resource could not be found after enumeration: '{resourcePath}'");
        }

        /// <summary>
        /// Searches the given assembly for a file.
        /// </summary>
        private static string? SearchAssembly(Assembly assembly, string resourceCacheKey, string resourceName)
        {
            var assemblyCacheKey = $"EmbeddedResources:SearchAssembly:{assembly.FullName}";

            var allResourceNames = _cache.Get(assemblyCacheKey) as List<string>;
            if (allResourceNames == null)
            {
                allResourceNames = assembly.GetManifestResourceNames().Select(o => $":{o}".Replace('.', ':')).ToList();
                _cache.Set(assemblyCacheKey, allResourceNames, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                });
            }

            if (allResourceNames.Count > 0)
            {
                var resource = allResourceNames.Where(o => o.EndsWith(resourceCacheKey, StringComparison.InvariantCultureIgnoreCase)).ToList();
                if (resource.Count > 1)
                {
                    throw new Exception($"Ambiguous resource name: [{resourceName}].");
                }
                else if (resource.Count == 0)
                {
                    return null;
                }

                using var stream = assembly.GetManifestResourceStream(resource.Single().Replace(':', '.').Trim(['.']))
                    ?? throw new InvalidOperationException($"Resource not found: [{resourceName}].");

                using var reader = new StreamReader(stream);
                var resourceText = reader.ReadToEnd();

                _cache.Set(resourceCacheKey, resourceText, new MemoryCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromHours(1)
                });

                return resourceText;
            }

            return null;
        }
    }
}
