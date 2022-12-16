using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;

namespace Lalraen.AzureFunctions.Shared
{
    public class CosmosDbService<T> where T : class, ITableEntity, new()
    {
        private readonly ILogger _logger;
        private readonly TableClient _tableClient;
        private readonly string _partitionKey;
        private readonly string _tableName;

        public CosmosDbService(ILogger logger, string connectionString, string tableName, string partitionKey)
        {
            _tableName = tableName;
            _logger = logger;
            _tableClient = new TableClient(connectionString, tableName);
            _partitionKey = partitionKey;
        }

        public async Task Add(T entity)
        {
            await _tableClient
                .AddEntityAsync(entity)
                .ConfigureAwait(false);

            _logger.LogDebug($"Entity with {entity.RowKey} row key added to {_tableName}");
        }

        public async Task<T?> Get(string rowKey)
        {
            var response = await _tableClient
                .GetEntityAsync<T>(_partitionKey, rowKey)
                .ConfigureAwait(false);

            if (response.HasValue)
            {
                _logger.LogDebug($"Entity with {rowKey} row key  retrieved");

                return response.Value;
            }

            return null;
        }

        public async Task<List<T>> GetAll()
        {
            var entities = await _tableClient
                .QueryAsync<T>(x => x.PartitionKey == _partitionKey)
                .ToListAsync()
                .ConfigureAwait(false);

            _logger.LogDebug($"Loaded {entities.Count} for partition key {_partitionKey}");

            return entities;
        }

        public async Task Update(T entity)
        {
            await _tableClient
                .UpdateEntityAsync(entity, ETag.All)
                .ConfigureAwait(false);

            _logger.LogDebug($"Entity with {entity.RowKey} row key updated in {_tableName}");
        }

        public async Task Delete(string rowKey)
        {
            await _tableClient
                .DeleteEntityAsync(_partitionKey, rowKey)
                .ConfigureAwait(false);

            _logger.LogDebug($"Entity with {rowKey} row key deleted from {_tableName}");
        }
    }
}