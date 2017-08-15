using System.Web;
using System.Web.Mvc;

namespace TGV.IPEFAE.Web.App
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
