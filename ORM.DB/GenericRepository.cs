/// https://github.com/muratkuru/DapperInfrastructure/blob/master/DapperInfrastructure.Data/GenericRepository.cs
using Dapper;
using Dapper.Contrib.Extensions;
using ORM.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
//using System.Web.Script.Serialization;


namespace ORM.DB
{
    public interface IRepository<T> where T : class
    {
        T GetById(object id);

        //T GetByQuery(string query, object parameters);

        ICollection<T> GetAll();

        //ICollection<T> GetAllByQuery(string query, object parameters);

        ICollection<T> GetAllByCustomQuery(string where, int limit = 0, string orderBy = "");
        List<T> SQLQuery(string sql);

        ICollection<T> GetAllByCustomQuery(DynamicParameters parameters);


        bool Add(T entity);

        bool Update(T entity);

        bool Delete(T entity);

        bool Delete(object id);

        bool Delete(string where);
    }

    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly IConnectionProvider connectionProvider;

        public GenericRepository(IConnectionProvider connectionProvider)
        {
            this.connectionProvider = connectionProvider;
        }

        public T GetById(object id)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                connection.Open();

                return connection.Get<T>(id);
            }
        }

        //public T GetByQuery(string query, object parameters)
        //{
        //    using (var connection = connectionProvider.GetConnection())
        //    {
        //        connection.Open();

        //        return connection.QueryFirstOrDefault<T>(query, parameters);
        //    }
        //}

        public ICollection<T> GetAll()
        {
            using (var connection = connectionProvider.GetConnection())
            {
                connection.Open();

                return connection.GetAll<T>().ToList();
            }
        }

        //public ICollection<T> GetAllByQuery(string query, object parameters)
        //{
        //    using (var connection = connectionProvider.GetConnection())
        //    {
        //        connection.Open();

        //        return connection.Query<T>(query, parameters).ToList();
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="where"></param>
        /// <param name="limit">0 girilirse limit uygulanmaz</param>
        /// <param name="orderBy">" UserName DESC" şeklinde yazılmalı</param>
        /// <returns></returns>
        public ICollection<T> GetAllByCustomQuery(string where, int limit = 0, string orderBy = "")
        {
            // POSTGRESQL:
            string limitPostgreSql = (limit > 0) ? " LIMIT " + limit + " " : "";

            // MSSQL:
            //string limitSql = (limit > 0) ? " TOP " + limit + " " : "";

            string orderBySql = (orderBy != "") ? " ORDER BY " + orderBy + " " : "";

            using (var connection = connectionProvider.GetConnection())
            {
                connection.Open();

                //string sql = "SELECT " + limitSql + " * FROM [" + typeof(T).Name.Replace("[", "").Replace("]", "") + "] WHERE 1=1 " + where + orderBySql;

                // MSSQL:
                //string sql = "SELECT " + limitSql + " * FROM " + typeof(T).Name + " WHERE 1=1 " + where + orderBySql;

                // POSTGRESQL:
                string sql = "SELECT * FROM " + typeof(T).Name + " WHERE 1=1 " + where + orderBySql + limitPostgreSql;

                return connection.Query<T>(sql).ToList();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public List<T> SQLQuery(string sql)
        {

            using (var connection = connectionProvider.GetConnection())
            {
                connection.Open();

                return connection.Query<T>(sql).ToList();
            }
        }

        public ICollection<T> GetAllByCustomQuery(DynamicParameters parameters)
        {
            string where = "";

            foreach (var item in parameters.ParameterNames)
            {
                where += string.Format(" AND {0}=@{0}", item);
            }

            using (var connection = connectionProvider.GetConnection())
            {
                connection.Open();

                string sql = "SELECT * FROM " + typeof(T).Name + " WHERE 1=1 " + where;


                return connection.Query<T>(sql, parameters).ToList();

            }

        }

        /// <summary>
        /// Kullanılıyor
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Add(T entity)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                connection.Open();

                try
                {
                    connection.Insert(entity);

                    WriteLog(connection, entity, "", ORM.Shared.Base.DbLogTypes.Add);

                    return true;
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }

        // AÇ!
        /// <summary>
        /// Log kaydı yazar
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="entity"></param>
        /// <param name="sql">Direkt sql ile </param>
        /// <param name="dbLogTypes"></param>
        private void WriteLog(IDbConnection connection, T entity, string sql, Base.DbLogTypes dbLogTypes)
        {
            //try
            //{
            //    if (typeof(T) == typeof(Log)
            //        || typeof(T) == typeof(TcmsDataValue)
            //        || typeof(T) == typeof(IntercomCall)
            //        || typeof(T) == typeof(ServiceTaskDataPacket)
            //        || typeof(T) == typeof(ServiceTaskDataPacketCompleted)
            //        || typeof(T) == typeof(ServiceTaskCompleted)) // Log tablolsuna yazılıyor ise kaydetme
            //    {
            //        return;
            //    }

            //    if (sql.IndexOf("DELETE FROM [" + nameof(ServiceTask) + "]") >= 0) // Otomatik silme işlemi
            //    {
            //        return;
            //    }

            //    string version = MetroScada.Shared.Base.METRO_SCADA_VERSION;
            //    Guid sessionId = Shared.Lib.isGuid(MetroScada.Shared.Base.OCCS_SESSIONID);
            //    Guid userId = Shared.Lib.isGuid(MetroScada.Shared.Base.OCCS_USERID);
            //    string module_name = "";
            //    string table_name = "";
            //    string url = "";
            //    string entity_ = "";


            //    if (entity != null)
            //    {
            //        module_name = entity.GetType().Module.Name;
            //        table_name = entity.GetType().Name;
            //        url = entity.GetType().Module.FullyQualifiedName;
            //        entity_ = new JavaScriptSerializer().Serialize(entity);
            //    }
            //    else
            //    {
            //        module_name = "";
            //        table_name = "";
            //        url = "";
            //        entity_ = sql;
            //    }

            //    Log log = new Log()
            //    {
            //        //Id = Guid.NewGuid(),
            //        ComputerName = System.Environment.MachineName,
            //        Entity = entity_,
            //        SessionId = sessionId,
            //        Type = (int)dbLogTypes,
            //        UserId = userId,
            //        Version = version,

            //        Source = module_name,
            //        TableName = table_name,
            //        Url = url,
            //        //CreationTime= DateTime.Now
            //    };

            //    connection.Insert(log);
            //}
            //catch (Exception ex)
            //{

            //}

        }

        public bool Update(T entity)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                connection.Open();

                bool ret = connection.Update(entity);

                WriteLog(connection, entity, "", Base.DbLogTypes.Update);

                return ret;
            }
        }

        public bool Delete(T entity)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                connection.Open();

                bool ret = connection.Delete(entity);

                WriteLog(connection, entity, "", Base.DbLogTypes.Delete);

                return ret;
            }
        }

        public bool Delete(object id)
        {
            var entity = GetById(id);
            return Delete(entity);
        }
        public bool Delete(string where)
        {
            if (where == null || where == "")
            {
                return false;
            }

            using (var connection = connectionProvider.GetConnection())
            {
                connection.Open();

                string sql = "DELETE FROM " + typeof(T).Name.Replace("[", "").Replace("]", "") + " WHERE 1=1 " + where;

                connection.Query<T>(sql).ToList();

                WriteLog(connection, null, sql, Base.DbLogTypes.Delete);

                return true;

            }
        }

        /// <summary>
        /// Gönderilen sql'e ait decimal sonuc döndürür (sum, avg vb.)
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public decimal? GetScalar(string sql)
        {
            using (var connection = connectionProvider.GetConnection())
            {
                connection.Open();

                var tst = connection.ExecuteScalar(sql);
                decimal? ret = Convert.ToDecimal(tst);

                //18.08.2020 12:03 tarihinde değiştirildi check-in atıldı.
                //decimal? ret = (decimal?)connection.ExecuteScalar(sql);

                return ret;

            }
        }
    }

}
