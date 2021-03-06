﻿using System;
using Enyim.Caching.Memcached;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching
{
	public interface IMemcachedClient : IDisposable
	{
        void Add(string key, object value, int cacheSeconds);
        Task AddAsync(string key, object value, int cacheSeconds);

        Task<IGetOperationResult<T>> GetAsync<T>(string key);
        Task<T> GetValueAsync<T>(string key);
        object Get(string key);
		T Get<T>(string key);
		IDictionary<string, object> Get(IEnumerable<string> keys);
        IDictionary<string, T> Get<T>(IEnumerable<string> keys);

        bool TryGet(string key, out object value);
        bool TryGetWithCas<T>(string key, out CasResult<T> value);

		CasResult<object> GetWithCas(string key);
		CasResult<T> GetWithCas<T>(string key);
		IDictionary<string, CasResult<object>> GetWithCas(IEnumerable<string> keys);
        IDictionary<string, CasResult<T>> GetWithCas<T>(IEnumerable<string> keys);


        bool Append(string key, ArraySegment<byte> data);
		CasResult<bool> Append(string key, ulong cas, ArraySegment<byte> data);

		bool Prepend(string key, ArraySegment<byte> data);
		CasResult<bool> Prepend(string key, ulong cas, ArraySegment<byte> data);

		bool Store(StoreMode mode, string key, object value);
		bool Store(StoreMode mode, string key, object value, DateTime expiresAt);
		bool Store(StoreMode mode, string key, object value, TimeSpan validFor);
        Task<bool> StoreAsync(StoreMode mode, string key, object value, DateTime expiresAt);
        Task<bool> StoreAsync(StoreMode mode, string key, object value, TimeSpan validFor);

        CasResult<bool> Cas(StoreMode mode, string key, object value);
		CasResult<bool> Cas(StoreMode mode, string key, object value, ulong cas);
		CasResult<bool> Cas(StoreMode mode, string key, object value, DateTime expiresAt, ulong cas);
		CasResult<bool> Cas(StoreMode mode, string key, object value, TimeSpan validFor, ulong cas);

		ulong Decrement(string key, ulong defaultValue, ulong delta);
		ulong Decrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt);
		ulong Decrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor);

		CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, ulong cas);
		CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas);
		CasResult<ulong> Decrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas);

		ulong Increment(string key, ulong defaultValue, ulong delta);
		ulong Increment(string key, ulong defaultValue, ulong delta, DateTime expiresAt);
		ulong Increment(string key, ulong defaultValue, ulong delta, TimeSpan validFor);

		CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, ulong cas);
		CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas);
		CasResult<ulong> Increment(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas);

		bool Remove(string key);
        Task<bool> RemoveAsync(string key);

        void FlushAll();

		ServerStats Stats();
		ServerStats Stats(string type);

		event Action<IMemcachedNode> NodeFailed;
	}
}
