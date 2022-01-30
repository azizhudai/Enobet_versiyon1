using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Configuration;

namespace Enobet_versiyon1.Models
{
    public class ModelContext: DbContext
    {
        public ModelContext()
           : base(CS)
        {

        }
        private static string CS
        {
            get { return ConfigurationManager.ConnectionStrings["cs"].ConnectionString; }
        }

    }
}