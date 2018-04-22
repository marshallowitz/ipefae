using System;
using System.Collections.Generic;
using System.Linq;

namespace TGV.IPEFAE.Web.BL.Data
{
    public class BancoData
    {
        internal static List<BancoModel> Listar()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.banco.ToList().ConvertAll(r => r.CopyObject<BancoModel>());
            }
        }
    }

    public class BancoModel
    {
        #region [ Propriedades ]

        public int id       { get; set; }
        public string nome  { get; set; }
        public bool ativo   { get; set; }

        #endregion [ FIM - Propriedades ]
    }
}
