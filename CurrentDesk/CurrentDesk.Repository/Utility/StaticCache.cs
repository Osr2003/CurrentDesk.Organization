

#region Namespace
using System;
using System.Collections;
using System.Data.EntityClient;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using CurrentDesk.DAL;
using CurrentDesk.Repository.CurrentDesk;
#endregion

namespace CurrentDesk.Repository.Utility
{
    public static class StaticCache
    {
        #region Fields

        private static readonly Cache _cache;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates a new instance of the StaticCache class
        /// </summary>
        static StaticCache()
        {
            HttpContext current = HttpContext.Current;

            if (current != null)
            {
                _cache = current.Cache;
            }
            else
            {
                _cache = HttpRuntime.Cache;
            }
        }

       
        #endregion

        #region Methods

        /// <summary>
        /// Removes all keys and values from the cache
        /// </summary>
        public static void Clear()
        {
            IDictionaryEnumerator enumerator = _cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                _cache.Remove(enumerator.Key.ToString());
            }
        }

        /// <summary>
        /// Validate if a key exist
        /// </summary>
        /// <param name="key">The key of the value to validate.</param>
        /// <returns>The value associated with the specified key exist.</returns>
        public static bool Exist(string key)
        {
            if (_cache[key] != null)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Return value based on a key
        /// </summary>
        /// <param name="key">The key of the value to validate.</param>
        /// <returns>The value associated with the specified key.</returns>
        public static object Get(string key)
        {
            return _cache[key];
        }



        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="obj">object</param>
        public static void Max(string key, object obj)
        {
            Max(key, obj, null);
        }

        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="obj">object</param>
        /// <param name="dep">cache dependency</param>
        public static void Max(string key, object obj, CacheDependency dep)
        {
            if (IsEnabled && (obj != null))
            {
                _cache.Insert(key, obj, dep, DateTime.MaxValue, TimeSpan.Zero, CacheItemPriority.AboveNormal, null);
            }
        }
        /// <summary>
        /// Adds the specified key and object to the cache.
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="obj">object</param>
        /// <param name="dep">cache dependency</param>
        public static void Insert(string key, object obj, CacheDependency dep, DateTime expiration, TimeSpan slidingExpiration, CacheItemPriority priority)
        {
            if (IsEnabled && (obj != null))
            {
                _cache.Insert(key, obj, dep, expiration, slidingExpiration, priority, null);
            }
        }

        /// <summary>
        /// Update the value with the specified key 
        /// </summary>
        /// <param name="key"></param>
        public static void Update(string key, object value)
        {
            _cache[key] = value;
        }


        /// <summary>
        /// Removes the value with the specified key from the cache
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// Removes items by pattern
        /// </summary>
        /// <param name="pattern">pattern</param>
        public static void RemoveByPattern(string pattern)
        {
            IDictionaryEnumerator enumerator = _cache.GetEnumerator();
            Regex regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            while (enumerator.MoveNext())
            {
                if (regex.IsMatch(enumerator.Key.ToString()))
                {
                    _cache.Remove(enumerator.Key.ToString());
                }
            }
        }

        public static void RemoveByLikeCondition(string likeKey)
        {

            IDictionaryEnumerator enumerator = _cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Key.ToString().Contains(likeKey))
                {
                    _cache.Remove(enumerator.Key.ToString());
                }

            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the cache is enabled
        /// </summary>
        public static bool IsEnabled
        {
            get
            {
                return true;
            }
        }

        #endregion

        public static string ConnectionString
        {
            get
            {
                string strConnection = string.Empty;
                using (var unitOfWork = new EFUnitOfWork())
                {
                   
                    var context = (CurrentDeskClientsEntities)unitOfWork.Context;
                    strConnection = ((EntityConnection)context.Connection).StoreConnection.ConnectionString;
                }

                return strConnection;
            }
        }
    }
}
