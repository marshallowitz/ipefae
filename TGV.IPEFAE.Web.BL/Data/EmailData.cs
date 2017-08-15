using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGV.IPEFAE.Web.BL.Data
{
    internal class EmailData
    {
        internal static bool Salvar(tb_ema_email email)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                db.tb_ema_email.Add(email);
                db.SaveChangesWithErrors();
            }

            return true;
        }
    }
}
