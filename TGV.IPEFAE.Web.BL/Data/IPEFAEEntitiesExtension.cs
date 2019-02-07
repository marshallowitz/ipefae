using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGV.IPEFAE.Web.BL.Data
{
    public partial class IPEFAEEntities
    {
        public IPEFAEEntities(string connectionString) : base(connectionString)
        {
            this.Database.CommandTimeout = 180;
            ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = 180;
        }
    }
}
