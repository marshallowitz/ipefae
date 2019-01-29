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
        public RacaModel() { }

        public RacaModel(raca raca)
        {
            if (raca == null)
                return;

            this.id = raca.id;
            this.nome = raca.nome;
            this.ativo = raca.ativo;
        }

        #region [ Propriedades ]

        public int id       { get; set; }
        public string nome  { get; set; }
        public bool ativo   { get; set; }

        #endregion [ FIM - Propriedades ]
    }
}
