using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Zbugar75.PlayersWallet.Api.Common.Configurations;

namespace Zbugar75.PlayersWallet.Api.Infrastructure.Services.Implementations
{
    public class SimpleMemoryCache<TItem> : ISimpleMemoryCache<TItem>
    {
        private readonly ILogger<SimpleMemoryCache<TItem>> _logger;
        private readonly SimpleMemoryCacheConfiguration _simpleMemoryCacheConfiguration;

        private readonly MemoryCache _cache;

        public SimpleMemoryCache(ILogger<SimpleMemoryCache<TItem>> logger, SimpleMemoryCacheConfiguration simpleMemoryCacheConfiguration)
        {
            _logger = logger;
            _simpleMemoryCacheConfiguration = simpleMemoryCacheConfiguration;
            _cache = new MemoryCache(new MemoryCacheOptions() { SizeLimit = _simpleMemoryCacheConfiguration.SizeLimit });
        }

        public async Task<TItem> GetOrCreateAsync(object key, Func<Task<TItem>> createItem)
        {
            _logger.LogTrace("Try to get entry from the memory cache.");
            if (_cache.TryGetValue(key, out TItem cacheEntry))
            {
                _logger.LogTrace("Entry found in memory cache: {cacheEntry}", cacheEntry);
                return cacheEntry;
            }

            _logger.LogTrace("Entry not found. Let's create.");
            cacheEntry = await createItem().ConfigureAwait(false);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSize(1)
                .SetPriority(CacheItemPriority.High)
                .SetSlidingExpiration(_simpleMemoryCacheConfiguration.SlidingExpiration)
                .SetAbsoluteExpiration(_simpleMemoryCacheConfiguration.AbsoluteExpiration);

            _cache.Set(key, cacheEntry, cacheEntryOptions);
            _logger.LogTrace("Entry registered in memory cache: {cacheEntry}", cacheEntry);

            return cacheEntry;
        }
    }
}