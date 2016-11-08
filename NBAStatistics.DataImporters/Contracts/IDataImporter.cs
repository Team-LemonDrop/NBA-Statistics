namespace NBAStatistics.DataImporters.Contracts
{
    public interface IDataImporter<T> where T : class
    {
        void Import();
    }
}
