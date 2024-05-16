// See https://aka.ms/new-console-template for more information
using Azure.Identity;
using CosmoAuthTesting2;
using Microsoft.Azure.Cosmos;

Console.WriteLine("Hello, World!");



bool flag = true;

if (flag)
{
    var db = new DatabaseAPIService();
    db.ListDocuments();
    
} else
{
    CosmosClient client = new(
        accountEndpoint: "COSMOSURI",
        tokenCredential: new DefaultAzureCredential()
        );
    
    Database database = client.GetDatabase("test_db");
    Container container = database.GetContainer("messages");
    
    var query = new QueryDefinition(
        query: "SELECT * FROM messages"
        );
        
    using FeedIterator<Message> feed = container.GetItemQueryIterator<Message>(queryDefinition: query);
    
    List<Message> items = new();
    double requestCharge = 0d;
    
    while (feed.HasMoreResults)
    {
        try
        {
            FeedResponse<Message> response = await feed.ReadNextAsync();
            foreach (Message item in response)
            {
                items.Add(item);
            }
            requestCharge += response.RequestCharge;
        } catch (Exception ex) 
        { 
            Console.WriteLine("Exception: " +  ex.Message);
        }
        
        
    }
    
    foreach (Message msg in items)
    {
        Console.WriteLine(msg.client_id);
    }
}
