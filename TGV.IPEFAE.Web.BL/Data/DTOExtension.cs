using System;
using System.Linq;
using System.Reflection;

namespace TGV.IPEFAE.Web.BL.Data
{
    public static class DTOExtension
    {
        public static T CopyObject<T>(this object objFrom) where T : new()
        {
            if (objFrom == null)
                return default(T);

            Type tObjFrom = objFrom.GetType();
            Type tObjTo = typeof(T);
            T objTo = new T();

            var listPropObj1 = tObjFrom.GetProperties().Where(p => p.GetValue(objFrom) != null).ToList();

            foreach (var item in listPropObj1)
            {
                PropertyInfo pi = tObjTo.GetProperty(item.Name);

                if (pi != null && pi.CanWrite)
                {
                    try
                    {
                        pi.SetValue(objTo, item.GetValue(objFrom));
                    }
                    catch { }
                }
            }

            return objTo;
        }

        public static void CopyObject<T>(this object objFrom, T objTo) where T : new()
        {
            if (objFrom == null)
                return;

            Type tObjFrom = objFrom.GetType();
            Type tObjTo = typeof(T);

            if (objTo == null)
                objTo = new T();

            var listPropObj1 = tObjFrom.GetProperties().Where(p => p.GetValue(objFrom) != null).ToList();

            foreach (var item in listPropObj1)
            {
                PropertyInfo pi = tObjTo.GetProperty(item.Name);

                if (pi != null && pi.CanWrite)
                {
                    try
                    {
                        pi.SetValue(objTo, item.GetValue(objFrom));
                    }
                    catch { }
                }
            }
        }
    }
}
