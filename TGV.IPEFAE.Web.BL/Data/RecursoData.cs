using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGV.IPEFAE.Web.BL.Data
{
    internal class RecursoData
    {
        internal static bool Apagar(int con_idt_concurso, string diretorioBase)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                List<int> recursos = db.tb_rec_recurso.Where(rec => rec.con_idt_concurso == con_idt_concurso).Select(rec => rec.rec_idt_recurso).ToList();

                foreach(int rec_idt_recurso in recursos)
                {
                    string diretorio = String.Format("{1}/Recurso/{0}", rec_idt_recurso, diretorioBase);
                    System.IO.DirectoryInfo directory = new System.IO.DirectoryInfo(diretorio);

                    directory.Empty();
                }

                // Apaga os recursos
                db.DeleteWhere<tb_rec_recurso>(rec => rec.con_idt_concurso == con_idt_concurso);
            }

            return true;
        }

        internal static tb_rec_recurso Obter(int rec_idt_recurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from rec in db.tb_rec_recurso.Include("tb_ico_inscrito_concurso.tb_est_estado").Include("tb_ico_inscrito_concurso.tb_cid_cidade.tb_est_estado").Include("tb_ico_inscrito_concurso.tb_cci_concurso_cargo_inscrito.tb_cco_cargo_concurso").Include("tb_usu_usuario")
                        where rec.rec_idt_recurso == rec_idt_recurso
                        select rec).SingleOrDefault();
            }
        }

        internal static tb_rec_recurso Salvar(tb_rec_recurso recurso)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                tb_rec_recurso toUpdate = db.tb_rec_recurso.SingleOrDefault(rec => rec.rec_idt_recurso == recurso.rec_idt_recurso);

                if (toUpdate != null)
                    toUpdate.rec_idt_recurso = recurso.rec_idt_recurso;
                else
                {
                    toUpdate = new tb_rec_recurso();
                    db.tb_rec_recurso.Add(toUpdate);
                }

                toUpdate.con_idt_concurso = recurso.con_idt_concurso;
                toUpdate.ico_idt_inscrito_concurso = recurso.ico_idt_inscrito_concurso;
                toUpdate.rec_bit_ativo = recurso.rec_bit_ativo;
                toUpdate.rec_dat_abertura = recurso.rec_dat_abertura;
                toUpdate.rec_dat_atendimento = recurso.rec_dat_atendimento;
                toUpdate.rec_des_comentario = recurso.rec_des_comentario;
                toUpdate.rec_des_mensagem = recurso.rec_des_mensagem;
                toUpdate.sre_idt_status_recurso = recurso.sre_idt_status_recurso;
                toUpdate.usu_idt_usuario = recurso.usu_idt_usuario;

                db.SaveChangesWithErrors();

                recurso = toUpdate;
                recurso.tb_con_concurso = db.tb_con_concurso.Single(con => con.con_idt_concurso == recurso.con_idt_concurso);
                recurso.tb_ico_inscrito_concurso = db.tb_ico_inscrito_concurso.Single(ico => ico.ico_idt_inscrito_concurso == recurso.ico_idt_inscrito_concurso);
            }

            return recurso;
        }
    }
}
