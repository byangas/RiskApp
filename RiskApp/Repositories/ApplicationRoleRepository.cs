using RiskApp.Data;
using RiskApp.Models.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiskApp.Repositories
{

    public class ApplicationRoleRepository : RepositoryBase
    {
        public const string ROLE_SYSTEM_ADMIM = "ROLE_SYSTEM_ADMIN";
        public const string ROLE_COMPANY_OWNER = "ROLE_COMPANY_OWNER";
        public const string ROLE_BROKER = "ROLE_BROKER";
        public const string ROLE_CARRIER_REP = "ROLE_CARRIER_REP";

        public ApplicationRoleRepository(ConnectionManager connectionManager) : base(connectionManager)
        {
        }

        public Guid GetCarrierRoleId()
        {
            return GetRoleId(ROLE_CARRIER_REP);
        }

        public Guid GetBrokerRoleId()
        {
            return GetRoleId(ROLE_BROKER);
        }


        public Guid GetRoleId(string name)
        {
            string SQL = @"SELECT application_role_id, name
                     FROM application_role where name = @Name";

            using var reader = ExecuteReader(SQL, queryparams: new Dictionary<string, object>() { { "Name", name } });
            reader.Read();
            return GetValue<Guid>(reader, "application_role_id");
        }

        public Guid GetCompanyOwnerRoleId()
        {
            return GetRoleId(ROLE_COMPANY_OWNER);
        }
    }
}
