using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TGV.Framework.Criptografia;

namespace TGV.IPEFAE.Web.BL.Data
{
    internal static class BaseData
    {
        internal static IPEFAEEntities Contexto
        {
            get
            {
                if (CriptografarConnString)
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["IPEFAEEntities"].ConnectionString;
                    string connectionStringSemCriptografia = connectionString.Descriptografar(TGV.IPEFAE.Web.BL.Business.BaseBusiness.ParametroSistema).Replace("&quot;", "'");
                    return new IPEFAEEntities(connectionStringSemCriptografia);
                }

                return new IPEFAEEntities();
            }
        }

        private static bool CriptografarConnString = false;

        internal static void Empty(this System.IO.DirectoryInfo directory)
        {
            if (!directory.Exists)
                return;

            foreach (System.IO.FileInfo file in directory.GetFiles()) file.Delete();
            foreach (System.IO.DirectoryInfo subDirectory in directory.GetDirectories()) subDirectory.Delete(true);
        }

        internal static void DeleteWhere<TEntity>(this DbContext context, Expression<Func<TEntity, bool>> predicate = null) where TEntity : class
        {
            try
            {
                var dbSet = context.Set<TEntity>();
                if (predicate != null)
                    dbSet.RemoveRange(dbSet.Where(predicate));
                else
                    dbSet.RemoveRange(dbSet);

                context.SaveChangesWithErrors();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException("Entity Validation Failed - errors follow:\n" + sb.ToString(), ex); // Add the original exception as the innerException
            }
            catch (Exception exc)
            {
                throw exc;
            }
        } 

        internal static int SaveChangesWithErrors(this DbContext context)
        {
            try
            {
                return context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException("Entity Validation Failed - errors follow:\n" + sb.ToString(), ex); // Add the original exception as the innerException
            }
            catch (Exception exc)
            {
                if (exc.Message.Equals("An error occurred while updating the entries. See the inner exception for details."))
                    exc = ObterException(exc.InnerException);

                throw exc;
            }
        }

        private static Exception ObterException(Exception exc)
        {
            if (exc.Message.Equals("An error occurred while updating the entries. See the inner exception for details."))
                exc = ObterException(exc.InnerException);

            return exc;
        }

        internal static async Task<int> SaveChangesWithErrorsAsync(this DbContext context)
        {
            try
            {
                return await context.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException("Entity Validation Failed - errors follow:\n" + sb.ToString(), ex); // Add the original exception as the innerException
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
    }
}
