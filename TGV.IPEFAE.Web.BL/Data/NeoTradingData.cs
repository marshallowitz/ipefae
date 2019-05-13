using System;
using System.Linq;
using TGV.Framework.Criptografia;

namespace TGV.IPEFAE.Web.BL.Data
{
    public class NeoTradingData
    {
        public static string GerarSenha(string email, string nome, string codigo)
        {
            string scripto = "Th1@g0V@l3nt3".Criptografar(codigo);

            using (IPEFAEEntities db = BaseData.Contexto)
            {
                var neo = db.neotrading.SingleOrDefault(n => n.codigo == codigo);

                if (neo == null)
                {
                    db.neotrading.Add(neo);
                    neo.data_entrega = DateTime.Now;
                    neo.is_active = true;
                }

                neo.nome = nome;
                neo.email = email;
                neo.codigo = codigo;
                neo.senha = scripto;

                db.SaveChangesWithErrors();

                return scripto;
            }
        }

        public static string ObterSenha(string codigo)
        {
            string senhaManual = "Th1@g0";

            if (codigo.Equals(senhaManual))
                return senhaManual.Criptografar(senhaManual);

            using (IPEFAEEntities db = BaseData.Contexto)
            {
                var neo = db.neotrading.SingleOrDefault(n => n.codigo == codigo);

                return neo == null ? "ERRO!" : neo.senha;
            }
        }
    }
}
