using Dapper;
using ORM.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ORM.Business
{
    /// <summary>
    /// v.2020.04.08
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GenericBusiness<T> : IRepository<T> where T : class
    {
        GenericRepository<T> repository = new GenericRepository<T>(new ConnectionProvider());

        public bool Add(T entity)
        {
            bool ret = false;
            try
            {
                var result = repository.Add(entity);
                ret = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;
        }


        public bool Delete(T entity)
        {
            try
            {
                return repository.Delete(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Mümkünse Delete(T entity) kullanılmalı. Çünkü entity'i bulup sonra delete yapar.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(object id)
        {
            try
            {
                return repository.Delete(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// where == "" olarak gönderilirse işlem yapılmaz (önlem amaçlı)
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool Delete(string where)
        {
            try
            {
                if (where == "")
                {
                    return false;
                }

                return repository.Delete(where);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Tablodaki tüm kayıtları koşulsuz döndürür
        /// </summary>
        /// <returns></returns>
        public ICollection<T> GetAll()
        {
            try
            {
                return repository.GetAll().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// SQL Sorgusu Çalıştırıp, list döndürür
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<T> SQLQuery(string sql)
        {
            try
            {
                return repository.SQLQuery(sql).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <param name="limit">0 girilirse limit uygulanmaz</param>
        /// <param name="orderBy">" UserName DESC" şeklinde yazılmalı</param>
        /// <returns></returns>
        public ICollection<T> GetAllByCustomQuery(string where, int limit = 0, string orderBy = "")
        {
            try
            {
                return repository.GetAllByCustomQuery(where, limit, orderBy).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ICollection<T> GetAllByCustomQuery(DynamicParameters parameters)
        {
            try
            {
                return repository.GetAllByCustomQuery(parameters).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public ICollection<T> GetAllByQuery(string query, object parameters)
        //{
        //    throw new NotImplementedException();
        //}

        public T GetById(object id)
        {
            try
            {
                return repository.GetById(id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public T GetById(object id, bool is_active, bool is_deleted)
        {
            try
            {
                string where = $" AND id = '{id}' AND is_active = {is_active} AND is_deleted = {is_deleted}";
                return repository.GetAllByCustomQuery(where).FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //public T GetByQuery(string query, object parameters)
        //{
        //    throw new NotImplementedException();
        //}

        public bool Update(T entity)
        {
            try
            {
                return repository.Update(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gönderilen sql'e ait decimal sonuc döndürür (sum, avg vb.)
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public decimal? GetScalar(string sql)
        {
            try
            {
                return repository.GetScalar(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
