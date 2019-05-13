using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.BL.Business
{
    public class NeotradingBusiness
    {
        public static string GerarSenha(string email, string nome, string codigo)
        {
            return NeoTradingData.GerarSenha(email, nome, codigo);
        }

        public static string ObterSenha(string codigo)
        {
            return NeoTradingData.ObterSenha(codigo);
        }
    }
}
