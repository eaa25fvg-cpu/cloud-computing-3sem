using Microsoft.Azure.Cosmos;
using SupportWebApp.Models;

namespace SupportWebApp.Services;

public sealed class SupportMessageService : IDisposable
{
    private readonly CosmosClient _client;
    private readonly Container _container;

    public SupportMessageService(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CosmosDb")
            ?? throw new InvalidOperationException("ConnectionStrings:CosmosDb mangler.");
        var databaseName = configuration["CosmosDb:DatabaseName"]
            ?? throw new InvalidOperationException("CosmosDb:DatabaseName mangler.");
        var containerName = configuration["CosmosDb:ContainerName"]
            ?? throw new InvalidOperationException("CosmosDb:ContainerName mangler.");

        _client = new CosmosClient(connectionString);
        _container = _client.GetContainer(databaseName, containerName);
    }

    public async Task AddAsync(SupportMessage message)
    {
        await _container.CreateItemAsync(message, new PartitionKey(message.Id));
    }

    public void Dispose() => _client.Dispose();
}
