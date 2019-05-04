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
                return (from b in db.banco
                        orderby b.ordem
                        select new BancoModel()
                        {
                            id = b.id,
                            nome = b.nome,
                            ordem = b.ordem,
                            ativo = b.ativo
                        }).ToList();
            }
        }
    }

    public class BancoModel
    {
        public BancoModel() { }

        public BancoModel(banco banco)
        {
            this.id = banco.id;
            this.nome = banco.nome;
            this.ativo = banco.ativo;
        }

        #region [ Propriedades ]

        public int id       { get; set; }
        public string nome  { get; set; }
        public int ordem    { get; set; }
        public bool ativo   { get; set; }

        #endregion [ FIM - Propriedades ]
    }
}
