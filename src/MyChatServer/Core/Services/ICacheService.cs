namespace Sulimov.MyChat.Server.Core.Services;

using Sulimov.MyChat.Server.Core.Enums;
using System;
using System.Threading.Tasks;

/// <summary>
/// Service for interaction with cache.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get object from cache.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="dataType">Data type for key prefix.</param>
    /// <param name="key">Key.</param>
    /// <returns>Object from cache.</returns>
    Task<T?> GetAsync<T>(CachedDataType dataType, string key) where T : class;

    /// <summary>
    /// Set object to cache.
    /// </summary>
    /// <param name="dataType">Data type for key prefix.</param>
    /// <param name="key">Key.</param>
    /// <param name="value">Object to store in cache.</param>
    /// <param name="expirationTime">Storing expiration time.</param>
    Task SetAsync(CachedDataType dataType, string key, object value, TimeSpan? expirationTime = null);

    /// <summary>
    /// Remove object from cache.
    /// </summary>
    /// <param name="dataType">Data type for key prefix.</param>
    /// <param name="key">Key.</param>
    Task RemoveAsync(CachedDataType dataType, string key);

    /// <summary>
    /// Clear cache.
    /// </summary>
    Task ClearAsync();
}
