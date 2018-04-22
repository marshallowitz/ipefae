using System;
using System.Collections.Generic;
using System.Linq;

namespace TGV.IPEFAE.Web.BL.Data
{
    public class GrauInstrucaoData
    {
        internal static List<GrauInstrucaoModel> Listar()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.grau_instrucao.ToList().ConvertAll(gi => gi.CopyObject<GrauInstrucaoModel>());
            }
        }
    }

    public class GrauInstrucaoModel
    {
        #region [ Propriedades ]

        public int id       { get; set; }
        public string nome  { get; set; }
        public bool ativo   { get; set; }

        #endregion [ FIM - Propriedades ]
    }
}
