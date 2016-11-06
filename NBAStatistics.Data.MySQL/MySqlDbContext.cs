using System.Linq;

using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata;

using NBAStatistics.Data.MySQL.Models;

namespace NBAStatistics.Data.MySQL
{
    public class MySqlContext : OpenAccessContext
    {
        private static string connectionStringName = @"MySqlConnection";
        private static MetadataSource metadataSource = new ModelMetadataSource();

        private static BackendConfiguration backend = GetBackendConfiguration();
        private static BackendConfiguration GetBackendConfiguration()
        {
            BackendConfiguration backend = new BackendConfiguration();
            backend.Backend = "MySql";
            backend.ProviderName = "MySql.Data.MySqlClient";
            return backend;
        }
        public MySqlContext()
            : base(connectionStringName, backend, metadataSource)
        {
            // Chack if the database schema exist on server.
            if (!this.GetSchemaHandler().DatabaseExists())
            {
                this.CreateUpdateSchema();
            }
        }

        public IQueryable<PlayerPointsPerGame> Categories
        {
            get
            {
                return this.GetAll<PlayerPointsPerGame>();
            }
        }

        public void CreateUpdateSchema()
        {
            var handler = this.GetSchemaHandler();
            string script = null;
            try
            {
                script = handler.CreateUpdateDDLScript(null);
            }
            catch
            {
                bool throwException = false;
                try
                {
                    handler.CreateDatabase();
                    script = handler.CreateDDLScript();
                }
                catch
                {
                    throwException = true;
                }
                if (throwException)
                    throw;
            }

            if (string.IsNullOrEmpty(script) == false)
            {
                handler.ExecuteDDLScript(script);
            }
        }
    }
}
