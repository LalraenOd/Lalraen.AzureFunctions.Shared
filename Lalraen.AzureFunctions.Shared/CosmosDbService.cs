using Azure;
using Azure.Data.Tables;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lalraen.AzureFunctions.Shared
{
    public class CosmosDbService<T> where T : class, ITableEntity, new()
    {
        private readonly TableClient _tableClient;

        // ReSharper disable once MemberCanBePrivate.Global
        public readonly string TableName;

        // ReSharper disable once MemberCanBePrivate.Global
        public readonly string PartitionKey;

        public CosmosDbService(string connectionString, string tableName, string partitionKey)
        {
            TableName = tableName;
            _tableClient = new TableClient(connectionString, tableName);
            PartitionKey = partitionKey;
        }

        public async Task Add(T entity)
        {
            await _tableClient
                .AddEntityAsync(entity)
                .ConfigureAwait(false);
        }

        public async Task<T?> Get(string rowKey)
        {
            var response = await _tableClient
                .GetEntityIfExistsAsync<T>(PartitionKey, rowKey)
                .ConfigureAwait(false);

            return response.HasValue ? response.Value : null;
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public async Task<IEnumerable<T>> GetAllPartition()
        {
            var entities = await _tableClient
                .QueryAsync<T>(x => x.PartitionKey == PartitionKey)
                .ToListAsync()
                .ConfigureAwait(false);

            return entities;
        }

        public async Task Update(T entity)
        {
            await _tableClient
                .UpdateEntityAsync(entity, ETag.All)
                .ConfigureAwait(false);
        }

        public async Task Delete(string rowKey)
        {
            await _tableClient
                .DeleteEntityAsync(PartitionKey, rowKey)
                .ConfigureAwait(false);
        }

        public async Task DeleteAllPartition()
        {
            var entities = await GetAllPartition()
                .ConfigureAwait(false);

            foreach (var entity in entities)
            {
                await _tableClient
                    .DeleteEntityAsync(PartitionKey, entity.RowKey)
                    .ConfigureAwait(false);
            }
        }
    }
}