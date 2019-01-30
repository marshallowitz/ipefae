using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGV.IPEFAE.Web.BL.Business;

namespace TGV.IPEFAE.Web.BL.Data
{
    internal class PDFData
    {
        public static int ListarTotalGeradosMesCorrente()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                int mesAtual = BaseBusiness.DataAgora.Month;
                List<relatorio_pdf> relatorios = db.relatorio_pdf.Where(r => r.id > 0 && r.data.Month == mesAtual).ToList();

                if (relatorios.Count == 0) // Se não achou ninguém
                    return 0;

                return relatorios.Sum(r => r.quantidade);
            }
        }

        public static int ObterChaveRelatorio()
        {
            AtualizarDatasChave();

            using (IPEFAEEntities db = BaseData.Contexto)
            {
                relatorio_pdf gerando = db.relatorio_pdf.FirstOrDefault(pdf => pdf.id > 0 && pdf.quantidade < 200);

                if (gerando == null) // Se não achou ninguém
                    return 0;

                gerando.quantidade++;
                db.SaveChangesWithErrors();

                return gerando.id;
            }
        }

        private static void AtualizarDatasChave()
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                relatorio_pdf relatorio = db.relatorio_pdf.FirstOrDefault(pdf => pdf.id == 1);

                if (relatorio == null) // Se não achou ninguém
                    return;

                int anoAtual = BaseBusiness.DataAgora.Year;
                int mesAtual = BaseBusiness.DataAgora.Month;

                // Verifica se mudou de mês
                if (relatorio.data.Month == mesAtual && relatorio.data.Year == anoAtual)
                    return;

                // Se mudou, atualiza as chaves
                foreach (var rel in db.relatorio_pdf.Where(pdf => pdf.id > 0))
                {
                    rel.data = new DateTime(anoAtual, mesAtual, 1);
                    rel.quantidade = 0;
                }

                db.SaveChangesWithErrors();
            }
        }

        internal static void Atualizar5Chave(int ctrRelatorio = 1)
        {
            using (IPEFAEEntities db = BaseData.Contexto)
            {
                relatorio_pdf relatorio = db.relatorio_pdf.FirstOrDefault(pdf => pdf.id == ctrRelatorio);

                if (relatorio == null) // Se não achou ninguém
                    return;

                int anoAtual = BaseBusiness.DataAgora.Year;
                int mesAtual = BaseBusiness.DataAgora.Month;

                // Verifica se mudou de mês
                if (relatorio.data.Month == mesAtual && relatorio.data.Year == anoAtual) // Se não mudou, adiciona 1
                {
                    if (relatorio.quantidade >= 200)
                    {
                        Atualizar5Chave(++ctrRelatorio);
                        return;
                    }

                    relatorio.quantidade++;
                }
                else // Se mudou, zera, atualiza a data e adiciona 1
                {
                    relatorio.data = new DateTime(anoAtual, mesAtual, 1);
                    relatorio.quantidade = 1;
                }

                db.SaveChangesWithErrors();
            }
        }
    }
}
