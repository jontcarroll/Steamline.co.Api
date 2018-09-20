using System;
using System.Runtime.Caching;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Steamline.co.Content.Helpers
{

    public class InMemoryCache : ICacheService
    {
        public T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class
        {
            T item = MemoryCache.Default.Get(cacheKey) as T;
            if (item == null)
            {
                item = getItemCallback();
                if (item == null)
                    return null;
                MemoryCache.Default.Add(cacheKey, item, new DateTime(9999, 12, 31));
            }
            return item;
        }
    }

    interface ICacheService
    {
        T GetOrSet<T>(string cacheKey, Func<T> getItemCallback) where T : class;
    }
}