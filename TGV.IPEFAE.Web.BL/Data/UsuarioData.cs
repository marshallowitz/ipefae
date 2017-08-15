using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGV.IPEFAE.Web.BL.Data
{
    internal class UsuarioData
    {
        internal static tb_usu_usuario EditarAtivacao(int usu_idt_usuario)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                tb_usu_usuario toUpdate = db.tb_usu_usuario.SingleOrDefault(usu => usu.usu_idt_usuario == usu_idt_usuario);

                if (toUpdate == null)
                    return null;

                toUpdate.usu_bit_ativo = !toUpdate.usu_bit_ativo;
                db.SaveChangesWithErrors();

                return toUpdate;
            }
        }

        internal static List<tb_usu_usuario> Listar(int per_idt_permissao)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from usu in db.tb_usu_usuario
                        where (per_idt_permissao == 1 || (per_idt_permissao & usu.per_idt_permissao) == usu.per_idt_permissao)
                        orderby usu.usu_nom_usuario
                        select usu).ToList();
            }
        }

        internal static tb_usu_usuario Obter(int usu_idt_usuario)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from usu in db.tb_usu_usuario
                        where usu.usu_idt_usuario == usu_idt_usuario
                        select usu).SingleOrDefault();
            }
        }

        internal static tb_usu_usuario Obter(string usu_des_email)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from usu in db.tb_usu_usuario
                        where usu.usu_bit_ativo
                        && usu.usu_des_email.ToLower() == usu_des_email.ToLower()
                        select usu).SingleOrDefault();
            }
        }

        internal static tb_usu_usuario Obter(string usu_des_email, string usu_des_senha)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                return (from usu in db.tb_usu_usuario
                        where usu.usu_bit_ativo
                        && usu.usu_des_email.ToLower() == usu_des_email.ToLower()
                        && usu.usu_des_senha == usu_des_senha
                        select usu).SingleOrDefault();
            }
        }

        internal static tb_usu_usuario Salvar(tb_usu_usuario usuario)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                tb_usu_usuario toUpdate = db.tb_usu_usuario.SingleOrDefault(usu => usu.usu_idt_usuario == usuario.usu_idt_usuario);

                if (toUpdate != null)
                    toUpdate.usu_idt_usuario = usuario.usu_idt_usuario;
                else
                {
                    toUpdate = new tb_usu_usuario();
                    db.tb_usu_usuario.Add(toUpdate);
                }

                toUpdate.per_idt_permissao = usuario.per_idt_permissao;
                toUpdate.usu_nom_usuario = usuario.usu_nom_usuario;
                toUpdate.usu_bit_ativo = usuario.usu_bit_ativo;
                toUpdate.usu_des_email = usuario.usu_des_email;
                toUpdate.usu_num_telefone = usuario.usu_num_telefone;

                if (!String.IsNullOrEmpty(usuario.usu_des_senha))
                    toUpdate.usu_des_senha = usuario.usu_des_senha;

                db.SaveChangesWithErrors();

                usuario = toUpdate;
            }

            return usuario;
        }
    }
}
