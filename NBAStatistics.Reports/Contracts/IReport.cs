namespace NBAStatistics.Reports.Contracts
{
    public interface IReport<T> where T : class
    {
        void Generate(string savePath);
    }
}
