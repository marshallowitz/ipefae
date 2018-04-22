using System;
using System.Collections.Generic;
using System.Linq;

namespace TGV.IPEFAE.Web.BL.Data
{
    public class RacaData
    {
        internal static List<RacaModel> Listar()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.raca.ToList().ConvertAll(r => r.CopyObject<RacaModel>());
            }
        }
    }

    public class RacaModel
    {
        #region [ Propriedades ]

        public int id       { get; set; }
        public string nome  { get; set; }
        public bool ativo   { get; set; }

        #endregion [ FIM - Propriedades ]
    }
}
