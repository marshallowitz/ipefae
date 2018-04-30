using System;
using System.Collections.Generic;
using System.Linq;

namespace TGV.IPEFAE.Web.BL.Data
{
    public class IRPFData
    {
        internal static List<IRPFModel> Listar()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.irpf.ToList().ConvertAll(r => r.CopyObject<IRPFModel>());
            }
        }
    }

    public class IRPFModel
    {
        #region [ Propriedades ]

        public int id               { get; set; }
        public decimal valor_min    { get; set; }
        public decimal valor_max    { get; set; }
        public decimal taxa         { get; set; }
        public decimal deducao      { get; set; }
        public bool ativo           { get; set; }

        #endregion [ FIM - Propriedades ]
    }
}
