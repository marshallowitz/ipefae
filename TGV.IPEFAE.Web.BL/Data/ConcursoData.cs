using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace TGV.IPEFAE.Web.BL.Data
{
    public class ConcursoData
    {
        internal static ConcursoFuncaoModel Funcao_Salvar(ConcursoFuncaoModel cfM)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                concurso_funcao toUpdate = db.concurso_funcao.SingleOrDefault(cf => cf.id == cfM.id);

                if (toUpdate == null) // Nao encontrou
                {
                    toUpdate = new concurso_funcao();
                    db.concurso_funcao.Add(toUpdate);
                    cfM.ativo = true;
                }

                toUpdate.ativo = cfM.ativo;
                toUpdate.concurso_id = cfM.concurso_id;
                toUpdate.funcao = cfM.funcao?.FirstCharOfEachWordToUpper();
                toUpdate.valor_liquido = cfM.valor_liquido;

                db.SaveChangesWithErrors();

                cfM.id = toUpdate.id;

                return cfM;
            }
        }

        internal static bool Funcao_Excluir(int idFuncao)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                db.DeleteWhere<concurso_funcao>(cf => cf.id == idFuncao);
                return true;
            }
        }

        internal static List<ConcursoModel> Listar()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.concurso.ToList().ConvertAll(c => c.CopyObject<ConcursoModel>());
            }
        }

        internal static ConcursoModel Obter(int id)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return ConcursoModel.Clone(db.concurso.SingleOrDefault(con => con.id == id));
            }
        }

        internal static ConcursoModel Salvar(ConcursoModel cM)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                concurso toUpdate = db.concurso.SingleOrDefault(con => con.id == cM.id);

                if (toUpdate == null) // Nao encontrou
                {
                    toUpdate = new concurso();
                    db.concurso.Add(toUpdate);
                    cM.ativo = true;
                }

                toUpdate.ativo = cM.ativo;
                toUpdate.data = cM.data;
                toUpdate.nome = cM.nome?.FirstCharOfEachWordToUpper();

                db.SaveChangesWithErrors();

                return Obter(toUpdate.id);
            }
        }
    }

    public class ConcursoModel
    {
        #region [ Propriedades ]

        public int id                           { get; set; } = 0;
        public string nome                      { get; set; }
        public DateTime data                    { get; set; }
        public string nacionalidade             { get; set; }
        public bool ativo                       { get; set; }

        public string codigo                    { get { return this.id.ToString().PadLeft(6, '0'); } }
        public string data_formatada            { get { return this.data.ToString("dd/MM/yyyy"); } }

        public List<ConcursoFuncaoModel> funcoes   { get; set; } = new List<ConcursoFuncaoModel>();
        public List<ConcursoLocalModel> locais     { get; set; } = new List<ConcursoLocalModel>();

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public static ConcursoModel Clone(concurso con)
        {
            if (con == null)
                return null;

            ConcursoModel c = con.CopyObject<ConcursoModel>();

            if (con.concurso_funcao != null)
                c.funcoes = con.concurso_funcao.ToList().ConvertAll(cf => cf.CopyObject<ConcursoFuncaoModel>());

            if (con.concurso_local != null)
                c.locais = con.concurso_local.ToList().ConvertAll(cf => cf.CopyObject<ConcursoLocalModel>());

            return c;
        }

        #endregion [ FIM - Metodos ]
    }

    public class ConcursoFuncaoModel
    {
        #region [ Propriedades ]

        public int id                   { get; set; } = 0;
        public int concurso_id          { get; set; } = 0;
        public string funcao            { get; set; }
        public decimal valor_liquido    { get; set; }
        public bool sem_desconto        { get; set; }
        public bool ativo               { get; set; }

        public string valor_liquido_formatado   { get { return String.Format("{0:C2}", this.valor_liquido); } }

        #endregion [ FIM - Propriedades ]
    }

    public class ConcursoLocalModel
    {
        #region [ Propriedades ]

        public int id           { get; set; } = 0;
        public int concurso_id  { get; set; } = 0;
        public string local     { get; set; }
        public bool ativo       { get; set; }

        #endregion [ FIM - Propriedades ]
    }
}
