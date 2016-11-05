namespace NBAStatistics.Reports.Handlers.Contracts
{
    public interface IJsonHandler
    {
        string Serialize<T>(T obj) where T : class;

        T Deserialize<T>(string json) where T : class;
    }
}
