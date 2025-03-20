namespace Sulimov.MyChat.Server.Core.Services;

using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using Sulimov.MyChat.Server.Core.Enums;
using System;
using System.Threading.Tasks;

/// <inheritdoc/>
public class CacheService : ICacheService
{
    private readonly IConfiguration configuration;
    private readonly ConnectionMultiplexer connectionMultiplexer;

    public CacheService(IConfiguration configuration)
    {
        this.configuration = configuration;
        this.connectionMultiplexer = GetConnectionMultiplexer();
    }

    /// <inheritdoc/>
    public async Task ClearAsync()
    {
        string? redisConnectionString = this.configuration.GetSection("Redis").GetValue<string>("Endpoint");
        if (string.IsNullOrEmpty(redisConnectionString))
        {
            redisConnectionString = Environment.GetEnvironmentVariable("REDIS_URL") ?? string.Empty;
        }

        var server = this.connectionMultiplexer.GetServer(redisConnectionString);
        await server.FlushDatabaseAsync();
    }

    /// <inheritdoc/>
    public async Task<T?> GetAsync<T>(CachedDataType dataType, string key) where T : class
    {
        var db = this.connectionMultiplexer.GetDatabase();

        RedisValue redisResult = await db.StringGetAsync(GetFullKey(dataType, key));
        if (redisResult.IsNull)
        {
            return default;
        }

        return JsonConvert.DeserializeObject<T>(redisResult.ToString());
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(CachedDataType dataType, string key)
    {
        var db = this.connectionMultiplexer.GetDatabase();
        bool isKeyDeleted = await db.KeyDeleteAsync(GetFullKey(dataType, key));

        if (!isKeyDeleted)
        {
            throw new InvalidOperationException($"Failed to remove key from cache. Key: {key}; DataType: {dataType}");
        }
    }

    /// <inheritdoc/>
    public async Task SetAsync(CachedDataType dataType, string key, object value, TimeSpan? expirationTime = null)
    {
        string serializedValue = JsonConvert.SerializeObject(value);
        var db = this.connectionMultiplexer.GetDatabase();

        bool result = await db.StringSetAsync(GetFullKey(dataType, key), serializedValue, expirationTime);

        if (!result)
        {
            throw new InvalidOperationException($"Failed to set value to cache. Key: {key}; DataType: {dataType}");
        }
    }

    private static string GetFullKey(CachedDataType dataType, string key)
    {
        return $"{dataType}:{key}";
    }

    private ConnectionMultiplexer GetConnectionMultiplexer()
    {
        string? connectionString = this.configuration.GetSection("Redis").GetValue<string>("Endpoint");
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = Environment.GetEnvironmentVariable("REDIS_URL") ?? string.Empty;
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException("Redis connection string is empty.");
        }

        var options = new ConfigurationOptions()
        {
            AbortOnConnectFail = false,
            EndPoints = { connectionString }
        };

        return ConnectionMultiplexer.Connect(options);
    }
}
