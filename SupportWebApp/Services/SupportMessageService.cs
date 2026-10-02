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
        if (message.Category is null)
            throw new ArgumentException("Supporthenvendelsen skal have en kategori.", nameof(message));

        await _container.CreateItemAsync(message, new PartitionKey(message.Category.Value.ToString()));
    }

    public async Task<List<SupportMessage>> GetAllAsync()
    {
        var messages = new List<SupportMessage>();
        using var iterator = _container.GetItemQueryIterator<SupportMessage>("SELECT * FROM c");

        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync();
            messages.AddRange(page);
        }

        return messages.OrderByDescending(message => message.SubmittedAt).ToList();
    }

    // guide: Hent kun dokumenter fra den valgte kategoris partition.
    public async Task<List<SupportMessage>> GetByCategoryAsync(SupportCategory category)
    {
        var categoryValue = category.ToString();
        var queryDefinition = new QueryDefinition(
            "SELECT * FROM c WHERE c.category = @category ORDER BY c.SubmittedAt DESC")
            .WithParameter("@category", categoryValue);
        using var iterator = _container.GetItemQueryIterator<SupportMessage>(
            queryDefinition,
            requestOptions: new QueryRequestOptions
            {
                PartitionKey = new PartitionKey(categoryValue)
            });

        var messages = new List<SupportMessage>();
        while (iterator.HasMoreResults)
        {
            var page = await iterator.ReadNextAsync();
            messages.AddRange(page);
        }

        return messages;
    }

    public void Dispose() => _client.Dispose();
}
