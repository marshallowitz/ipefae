using System;
using System.Collections.Generic;
using System.Linq;

namespace TGV.IPEFAE.Web.BL.Data
{
    public class FuncaoData
    {
        internal static List<FuncaoModel> Listar()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.funcao.ToList().ConvertAll(f => new FuncaoModel(f.id, f.nome, f.codigo, f.cbo, f.is_active));
            }
        }
    }

    public class FuncaoModel
    {
        public FuncaoModel(Guid id, string nome, int codigo, int cbo, bool is_active)
        {
            this.id = id;
            this.nome = nome;
            this.codigo = codigo;
            this.cbo = cbo;
            this.is_active = is_active;
        }

        public Guid id          { get; set; }
        public string nome      { get; set; }
        public int codigo       { get; set; }
        public int cbo          { get; set; }
        public bool is_active   { get; set; }
    }
}
