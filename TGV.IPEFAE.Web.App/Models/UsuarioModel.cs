using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TGV.IPEFAE.Web.App.Extensions;
using TGV.IPEFAE.Web.BL.Data;

namespace TGV.IPEFAE.Web.App.Models
{
    public class UsuarioModel
    {
        public UsuarioModel()
        {
            this.Id = Int32.MinValue;
            this.IdPermissao = 0;
        }

        public UsuarioModel(tb_usu_usuario usuario) : this()
        {
            if (usuario == null)
                return;

            this.Id = usuario.usu_idt_usuario;
            this.IdPermissao = usuario.per_idt_permissao;
            this.Nome = usuario.usu_nom_usuario;
            this.Email = usuario.usu_des_email;
            this.Senha = usuario.usu_des_senha;
            this.Telefone = usuario.usu_num_telefone;
            this.Ativo = usuario.usu_bit_ativo;
        }

        public UsuarioModel(tb_ues_usuario_estagio usuario) : this()
        {
            if (usuario == null)
                return;

            this.Id = usuario.ues_idt_usuario_estagio;
            this.IdPermissao = (int)PermissaoModel.Tipo.Estagiario;
            this.Nome = usuario.ues_nom_usuario_estagio;
            this.Email = usuario.ues_des_email;
            this.Senha = usuario.ues_des_senha;
            this.Telefone = 0;
            this.Ativo = usuario.ues_bit_ativo;
        }

        public int Id                   { get; set; }
        public int IdPermissao          { get; set; }
        public string Nome              { get; set; }
        public string Email             { get; set; }
        public string Senha             { get; set; }
        public string ConfirmacaoSenha  { get; set; }
        public long Telefone            { get; set; }
        public bool Ativo               { get; set; }

        public PermissaoModel.Tipo Permissao { get { return (PermissaoModel.Tipo)IdPermissao; } }
        
        public string PermissaoString
        {
            get
            {
                if (this.IsAdministrador)
                    return PermissaoModel.Tipo.Administrador.GetDescription();

                string permissao = "Admin";

                switch (Permissao)
                {
                    case PermissaoModel.Tipo.Concurso:
                        return String.Format("{0} {1}", permissao, PermissaoModel.Tipo.Concurso.GetDescription());
                    case PermissaoModel.Tipo.Estagio:
                        return String.Format("{0} {1}", permissao, PermissaoModel.Tipo.Estagio.GetDescription());
                    case PermissaoModel.Tipo.Vestibular:
                        return String.Format("{0} {1}", permissao, PermissaoModel.Tipo.Vestibular.GetDescription());
                    case PermissaoModel.Tipo.Indefinido:
                    case PermissaoModel.Tipo.Publico:
                        return PermissaoModel.Tipo.Publico.GetDescription();
                }

                
                bool poeBarra = false;

                if ((Permissao & PermissaoModel.Tipo.Concurso) == PermissaoModel.Tipo.Concurso)
                {
                    permissao = String.Format("{0} {1}", permissao, PermissaoModel.Tipo.Concurso.GetDescription());
                    poeBarra = true;
                }

                if ((Permissao & PermissaoModel.Tipo.Estagio) == PermissaoModel.Tipo.Estagio)
                {
                    permissao = String.Format("{0}{2}{1}", permissao, PermissaoModel.Tipo.Estagio.GetDescription(), poeBarra ? "|" : " ");
                    poeBarra = true;
                }

                if ((Permissao & PermissaoModel.Tipo.Estagiario) == PermissaoModel.Tipo.Estagiario)
                {
                    permissao = String.Format("{0}{2}{1}", permissao, PermissaoModel.Tipo.Estagiario.GetDescription(), poeBarra ? "|" : " ");
                    poeBarra = true;
                }

                if ((Permissao & PermissaoModel.Tipo.Vestibular) == PermissaoModel.Tipo.Vestibular)
                {
                    permissao = String.Format("{0}{2}{1}", permissao, PermissaoModel.Tipo.Vestibular.GetDescription(), poeBarra ? "|" : " ");
                    poeBarra = true;
                }

                return poeBarra ? permissao : PermissaoModel.Tipo.Publico.GetDescription();
            }
        }

        public string UrlAcessoInicial
        {
            get
            {
                if (this.IsAdministrador)
                    return "Admin/Usuario";

                if ((Permissao & PermissaoModel.Tipo.Concurso) == PermissaoModel.Tipo.Concurso)
                    return "Admin/Concurso";

                if ((Permissao & PermissaoModel.Tipo.Estagio) == PermissaoModel.Tipo.Estagio)
                    return "Admin/Estagio";

                if ((Permissao & PermissaoModel.Tipo.Vestibular) == PermissaoModel.Tipo.Vestibular)
                    return "Admin/Vestibular";


                return "";
            }
        }
        
        public bool IsAdministrador { get { return (Permissao & PermissaoModel.Tipo.Administrador) == PermissaoModel.Tipo.Administrador; } }
        public bool IsConcurso      { get { return this.IsAdministrador || (Permissao & PermissaoModel.Tipo.Concurso) == PermissaoModel.Tipo.Concurso; } }
        public bool IsEstagio       { get { return this.IsAdministrador || (Permissao & PermissaoModel.Tipo.Estagio) == PermissaoModel.Tipo.Estagio; } }
        public bool IsVestibular    { get { return this.IsAdministrador || (Permissao & PermissaoModel.Tipo.Vestibular) == PermissaoModel.Tipo.Vestibular; } }
        public bool IsEstagiario    { get { return this.IsAdministrador || (Permissao & PermissaoModel.Tipo.Estagiario) == PermissaoModel.Tipo.Estagiario; } }

        public string PrimeiroNome { get { return String.IsNullOrEmpty(this.Nome) ? String.Empty : this.Nome.Split(' ')[0]; } }
    }
}