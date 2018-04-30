﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Script.Serialization;

namespace TGV.IPEFAE.Web.BL.Data
{
    public class ConcursoData
    {
        internal static bool Excluir(int id)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    db.DeleteWhere<concurso_local_colaborador>(clc => clc.concurso_local.concurso_id == id);
                    db.DeleteWhere<concurso_local>(cl => cl.concurso_id == id);
                    db.DeleteWhere<concurso_funcao>(cf => cf.concurso_id == id);
                    db.DeleteWhere<concurso>(c => c.id == id);

                    scope.Complete();
                }

                return true;
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

        internal static List<ConcursoModel> Listar()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.concurso.ToList().ConvertAll(c => c.CopyObject<ConcursoModel>());
            }
        }

        internal static bool Local_Colaborador_Excluir(int idColaborador)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                db.DeleteWhere<concurso_local_colaborador>(clc => clc.id == idColaborador);

                return true;
            }
        }

        internal static bool Local_Excluir(int idLocal)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    db.DeleteWhere<concurso_local_colaborador>(clc => clc.concurso_local.id == idLocal);
                    db.DeleteWhere<concurso_local>(cl => cl.id == idLocal);

                    scope.Complete();
                }
                
                return true;
            }
        }

        internal static ConcursoLocalColaboradorModel Local_Colaborador_Salvar(ConcursoLocalColaboradorModel clcM)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                concurso_local_colaborador toUpdate = db.concurso_local_colaborador.SingleOrDefault(clc => clc.id == clcM.id);

                if (toUpdate == null) // Nao encontrou
                {
                    toUpdate = new concurso_local_colaborador();
                    db.concurso_local_colaborador.Add(toUpdate);
                    clcM.ativo = true;
                }

                toUpdate.ativo = clcM.ativo;
                toUpdate.colaborador_id = clcM.colaborador_id;
                toUpdate.concurso_local_id = clcM.concurso_local_id;
                toUpdate.funcao_id = clcM.funcao_id;
                toUpdate.tem_empresa = clcM.tem_empresa;
                toUpdate.valor = clcM.valor;

                db.SaveChangesWithErrors();

                return clcM;
            }
        }

        internal static ConcursoLocalModel Local_Salvar(ConcursoLocalModel clM)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                concurso_local toUpdate = db.concurso_local.SingleOrDefault(cl => cl.id == clM.id);

                if (toUpdate == null) // Nao encontrou
                {
                    toUpdate = new concurso_local();
                    db.concurso_local.Add(toUpdate);
                    clM.ativo = true;
                }

                toUpdate.ativo = clM.ativo;
                toUpdate.concurso_id = clM.concurso_id;
                toUpdate.local = clM.local?.FirstCharOfEachWordToUpper();

                db.SaveChangesWithErrors();

                clM.id = toUpdate.id;

                return clM;
            }
        }

        internal static ConcursoModel Obter(int id, bool ate_colaborador = false)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                if (!ate_colaborador)
                    return ConcursoModel.Clone(db.concurso.Include("concurso_funcao.concurso_local_colaborador").Include("concurso_local.concurso_local_colaborador").SingleOrDefault(con => con.id == id));
                else
                    return ConcursoModel.Clone(db.concurso.Include("concurso_funcao").Include("concurso_local.concurso_local_colaborador.colaborador").SingleOrDefault(con => con.id == id));
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
                c.funcoes = con.concurso_funcao.ToList().ConvertAll(cf => ConcursoFuncaoModel.Clone(cf));

            if (con.concurso_local != null)
                c.locais = con.concurso_local.ToList().ConvertAll(cl => ConcursoLocalModel.Clone(cl));

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

        public bool modoEdicao          { get; set; } = false;
        public bool temAssociacao       { get; set; } = false;

        public string valor_liquido_formatado   { get { return String.Format("{0:C2}", this.valor_liquido); } }

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public static ConcursoFuncaoModel Clone(concurso_funcao cf)
        {
            if (cf == null)
                return null;

            ConcursoFuncaoModel funcao = cf.CopyObject<ConcursoFuncaoModel>();

            if (cf.concurso_local_colaborador != null && cf.concurso_local_colaborador.Count > 0)
                funcao.temAssociacao = true;

            return funcao;
        }

        public static List<ConcursoFuncaoModel> Listar(int idConcurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.concurso_funcao.Include("concurso_local_colaborador").Where(cf => cf.concurso_id == idConcurso).ToList().ConvertAll(cf => ConcursoFuncaoModel.Clone(cf));
            }
        }

        #endregion [ FIM - Metodos ]
    }

    public class ConcursoLocalModel
    {
        #region [ Propriedades ]

        public int id           { get; set; } = 0;
        public int concurso_id  { get; set; } = 0;
        public string local     { get; set; }
        public bool ativo       { get; set; }

        public List<ConcursoLocalColaboradorModel> Colaboradores { get; set; } = new List<ConcursoLocalColaboradorModel>();

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public static ConcursoLocalModel Clone(concurso_local cl)
        {
            if (cl == null)
                return null;

            ConcursoLocalModel local = cl.CopyObject<ConcursoLocalModel>();

            if (cl.concurso_local_colaborador != null)
                local.Colaboradores = cl.concurso_local_colaborador.ToList().ConvertAll(clc => ConcursoLocalColaboradorModel.Clone(clc));

            return local;
        }

        #endregion [ FIM - Metodos ]
    }

    public class ConcursoLocalColaboradorModel
    {
        #region [ Propriedades ]

        public int id                   { get; set; } = 0;
        public int concurso_local_id    { get; set; } = 0;
        public int colaborador_id       { get; set; } = 0;
        public int funcao_id            { get; set; }
        public decimal valor            { get; set; }
        public bool tem_empresa         { get; set; } = false;
        public bool ativo               { get; set; }

        public bool modoEdicao          { get; set; } = false;

        public ColaboradorModel colaborador { get; set; } = new ColaboradorModel();
        public ConcursoFuncaoModel funcao { get; set; } = new ConcursoFuncaoModel();

        #endregion [ FIM - Propriedades ]

        #region [ Metodos ]

        public static ConcursoLocalColaboradorModel Clone(concurso_local_colaborador clc)
        {
            if (clc == null)
                return null;

            ConcursoLocalColaboradorModel local_colaborador = clc.CopyObject<ConcursoLocalColaboradorModel>();

            if (clc.colaborador != null)
                local_colaborador.colaborador = clc.colaborador.CopyObject<ColaboradorModel>();

            if (clc.concurso_funcao != null)
                local_colaborador.funcao = clc.concurso_funcao.CopyObject<ConcursoFuncaoModel>();

            return local_colaborador;
        }

        public static List<ConcursoLocalColaboradorModel> Listar(int idConcurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return db.concurso_local_colaborador.Include("concurso_funcao").Where(clc => clc.concurso_local.concurso_id == idConcurso).ToList().ConvertAll(clc => ConcursoLocalColaboradorModel.Clone(clc));
            }
        }

        #endregion [ FIM - Metodos ]
    }
}
