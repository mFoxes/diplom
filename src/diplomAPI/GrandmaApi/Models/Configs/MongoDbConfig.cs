using System.Transactions;

namespace GrandmaApi.Models.Configs;

public class MongoDbConfig
{
    public string DatabaseName { get; set; }
    public IsolationLevel IsolationLevel {get;set; }
    public int TransactionTimeout {get;set;}
}