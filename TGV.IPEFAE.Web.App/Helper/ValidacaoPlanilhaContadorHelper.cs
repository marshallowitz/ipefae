using System;
using System.Collections.Generic;
using System.Linq;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Helper
{
    public static class ValidacaoPlanilhaContadorHelper
    {
        public static Tuple<bool, string> ObterCodigoBanco(string valor, List<BancoModel> listaBancos)
        {
            if (String.IsNullOrEmpty(valor))
                return new Tuple<bool, string>(false, "Nome do Banco está em branco");

            var banco = listaBancos.FirstOrDefault(b => b.nome.Equals(valor.Trim(), StringComparison.InvariantCultureIgnoreCase));

            if (banco == null)
                return new Tuple<bool, string>(false, "Nome do Banco não cadastrado no sistema");

            if (banco.codigo.Length > 6)
                return new Tuple<bool, string>(false, "Código do Banco inválido");

            return new Tuple<bool, string>(true, banco.codigo.PadLeft(6, '0'));
        }

        public static Tuple<bool, string> ObterCodigoCargo(string valor, List<ConcursoFuncaoModel> listaFuncoes)
        {
            if (String.IsNullOrEmpty(valor))
                return new Tuple<bool, string>(false, "Cargo está em branco");

            var cargo = listaFuncoes.FirstOrDefault(c => c.funcao.Equals(valor.Trim(), StringComparison.InvariantCultureIgnoreCase));

            if (cargo == null)
                return new Tuple<bool, string>(false, "Função não cadastrada no sistema");

            if (cargo.id.ToString().Length > 10)
                return new Tuple<bool, string>(false, "Código da Função inválido");

            return new Tuple<bool, string>(true, cargo.id.ToString().PadLeft(10, '0'));
        }

        public static Tuple<bool, string> ObterCPF(string valor)
        {
            if (String.IsNullOrEmpty(valor))
                return new Tuple<bool, string>(false, "CPF está em branco");

            var cpf = new String(valor.Where(Char.IsDigit).ToArray());

            if (cpf.Length == 11)
                return new Tuple<bool, string>(true, cpf);

            return new Tuple<bool, string>(false, "CPF inválido");
        }

        public static Tuple<bool, string> ObterData(string valor, string campo)
        {
            if (String.IsNullOrEmpty(valor))
                return new Tuple<bool, string>(false, $"{campo} está em branco");

            var dataNumerico = new String(valor.Where(Char.IsDigit).ToArray());

            if (dataNumerico.Length == 7)
                dataNumerico = dataNumerico.PadLeft(8, '0');

            if (dataNumerico.Length != 8)
                return new Tuple<bool, string>(false, $"{campo} inválida");

            var dataFormatoCorreto = $"{dataNumerico.Substring(4)}{dataNumerico.Substring(2, 2)}{dataNumerico.Substring(0, 2)}";

            return new Tuple<bool, string>(true, dataFormatoCorreto);
        }

        public static Tuple<bool, string> ObterNomeContribuinte(string valor)
        {
            if (String.IsNullOrEmpty(valor))
                return new Tuple<bool, string>(false, "Nome do Contribuinte está em branco");

            valor = valor.Trim();

            if (valor.Length < 70)
                valor = valor.PadRight(70, ' ');

            var nome = valor.Substring(0, 70);

            return new Tuple<bool, string>(true, nome);
        }

        public static Tuple<bool, string> ObterNumeroConta(string valor)
        {
            if (String.IsNullOrEmpty(valor))
                return new Tuple<bool, string>(false, "Número da Conta está em branco");

            var numeroConta = new String(valor.Where(Char.IsDigit).ToArray());

            if (numeroConta.Length > 12)
                return new Tuple<bool, string>(false, "Número da Conta inválida");

            return new Tuple<bool, string>(true, numeroConta.PadLeft(12, '0'));
        }

        public static Tuple<bool, string> ObterNumeroContaDigito(string valor)
        {
            if (String.IsNullOrEmpty(valor))
                return new Tuple<bool, string>(false, "Digito da Conta está em branco");

            if (valor.Trim().Length > 2)
                return new Tuple<bool, string>(false, "Digito da Conta inválida");

            return new Tuple<bool, string>(true, valor.Trim().PadLeft(2, '0'));
        }

        public static Tuple<bool, string> ObterNumeroContaTipo(string valor)
        {
            if (String.IsNullOrEmpty(valor))
                return new Tuple<bool, string>(false, "Tipo da Conta está em branco");

            if (valor.Trim().Equals("corrente", StringComparison.InvariantCultureIgnoreCase))
                return new Tuple<bool, string>(true, "02");

            if (valor.Trim().Equals("poupança", StringComparison.InvariantCultureIgnoreCase))
                return new Tuple<bool, string>(true, "04");

            return new Tuple<bool, string>(true, "01"); // Não Informada
        }

        public static Tuple<bool, string> ObterPIS(string valor)
        {
            if (String.IsNullOrEmpty(valor))
                return new Tuple<bool, string>(false, "PIS está em branco");

            var pis = new String(valor.Where(Char.IsDigit).ToArray());

            if (pis.Length == 11)
                return new Tuple<bool, string>(true, pis);

            return new Tuple<bool, string>(false, "PIS inválido");
        }

        public static Tuple<bool, string> ObterValorSalario(string valor)
        {
            if (String.IsNullOrEmpty(valor))
                return new Tuple<bool, string>(false, "Valor Bruto Salário está em branco");

            var salario = new String(valor.Where(Char.IsDigit).ToArray());

            return new Tuple<bool, string>(true, salario.PadLeft(8, '0'));
        }
    }
}