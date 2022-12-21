using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;

namespace Lalraen.AzureFunctions.Shared
{
    public class CosmosDbService<T> where T : class, ITableEntity, new()
    {
        private readonly ILogger _logger;
        private readonly TableClient _tableClient;

        // ReSharper disable once MemberCanBePrivate.Global
        public readonly string TableName;

        // ReSharper disable once MemberCanBePrivate.Global
        public readonly string PartitionKey;

        public CosmosDbService(ILogger logger, string connectionString, string tableName, string partitionKey)
        {
            TableName = tableName;
            _logger = logger;
            _tableClient = new TableClient(connectionString, tableName);
            PartitionKey = partitionKey;
        }

        public async Task Add(T entity)
        {
            await _tableClient
                .AddEntityAsync(entity)
                .ConfigureAwait(false);

            _logger.LogDebug($"Entity with {entity.RowKey} row key added to {TableName}");
        }

        public async Task<T?> Get(string rowKey)
        {
            var response = await _tableClient
                .GetEntityAsync<T>(PartitionKey, rowKey)
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
                .QueryAsync<T>(x => x.PartitionKey == PartitionKey)
                .ToListAsync()
                .ConfigureAwait(false);

            _logger.LogDebug($"Loaded {entities.Count} for partition key {PartitionKey}");

            return entities;
        }

        public async Task Update(T entity)
        {
            await _tableClient
                .UpdateEntityAsync(entity, ETag.All)
                .ConfigureAwait(false);

            _logger.LogDebug($"Entity with {entity.RowKey} row key updated in {TableName}");
        }

        public async Task Delete(string rowKey)
        {
            await _tableClient
                .DeleteEntityAsync(PartitionKey, rowKey)
                .ConfigureAwait(false);

            _logger.LogDebug($"Entity with {rowKey} row key deleted from {TableName}");
        }
    }
}