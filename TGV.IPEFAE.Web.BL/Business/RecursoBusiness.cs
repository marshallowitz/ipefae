using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGV.IPEFAE.Web.BL.Data;
using TGV.Framework.Criptografia;

namespace TGV.IPEFAE.Web.BL.Business
{
    public static class RecursoBusiness
    {
        public static tb_rec_recurso Obter(int rec_idt_recurso)
        {
            return RecursoData.Obter(rec_idt_recurso);
        }

        public static tb_rec_recurso Salvar(tb_rec_recurso recurso)
        {
            return RecursoData.Salvar(recurso);
        }
    }
}
