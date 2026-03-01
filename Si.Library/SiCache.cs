using System.Diagnostics.CodeAnalysis;
using System.Runtime.Caching;

namespace Si.Library
{
    /// <summary>
    /// Implements a cache where we do not allow the caching of null values. This allows us to use the TryGet pattern for
    /// retrieving items from the cache without having to worry about distinguishing between a cached null value and a cache miss.
    /// </summary>
    public class SiCache
    {
        private readonly MemoryCache _memCache;

        public TimeSpan DefaultCacheTime { get; set; }
        public CacheExpirationScheme DefaultCacheExpirationScheme { get; set; }


        public enum CacheExpirationScheme
        {
            Absolute,
            Sliding
        }

        public SiCache(CacheExpirationScheme defaultCacheExpirationScheme, TimeSpan defaultCacheTime)
        {
            _memCache = new MemoryCache("SiCache");
            DefaultCacheExpirationScheme = defaultCacheExpirationScheme;
            DefaultCacheTime = defaultCacheTime;
        }

        private CacheItemPolicy GetDefaultCacheItemPolicy()
        {
            var policy = new CacheItemPolicy();
            if (DefaultCacheExpirationScheme == CacheExpirationScheme.Absolute)
            {
                policy.AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(DefaultCacheTime.TotalSeconds);
            }
            else
            {
                policy.SlidingExpiration = TimeSpan.FromSeconds(DefaultCacheTime.TotalSeconds);
            }
            return policy;
        }

        public T AddOrGet<T>(string cacheKey, Func<T> getValueDelegate, CacheItemPolicy? cacheItemPolicy = null)
        {
            if (TryGet<T>(cacheKey, out var cached))
                return cached;

            var result = getValueDelegate()
                ?? throw new InvalidOperationException("getValueDelegate returned null, which is not allowed for caching.");

            _memCache.Add(cacheKey, result, cacheItemPolicy ?? GetDefaultCacheItemPolicy());

            return result;
        }

        public async Task<T> AddOrGetAsync<T>(string cacheKey, Func<Task<T>> getValueDelegate, CacheItemPolicy? cacheItemPolicy = null)
        {
            if (TryGet<T>(cacheKey, out var cached))
                return cached;

            var result = await getValueDelegate().ConfigureAwait(false)
                ?? throw new InvalidOperationException("getValueDelegate returned null, which is not allowed for caching.");

            _memCache.Add(cacheKey, result, cacheItemPolicy ?? GetDefaultCacheItemPolicy());

            return result;
        }

        /// <summary>
        /// Gets an item from the cache.
        /// </summary>
        public bool TryGet<T>(string cacheKey, [NotNullWhen(true)] out T? result)
        {
            var cached = _memCache.Get(cacheKey);

            if (cached == null)
            {
                result = default;
                return false;
            }

            result = (T)cached;

            return true;
        }

        public void Remove(string cacheKey)
            => _memCache.Remove(cacheKey);

        public void Clear()
            => _memCache.Trim(100);
    }
}
