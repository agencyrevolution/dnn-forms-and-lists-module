using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Data;
using log4net;

namespace DotNetNuke.Modules.UserDefinedTable.Controllers
{
    public class BaseDataController
    {
        protected static ILog Logger = LogManager.GetLogger(typeof (BaseDataController));

        /// <summary>
        /// Get all items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> Get<T>() where T : class
        {
            try
            {
                using (var context = DataContext.Instance())
                {
                    var repository = context.GetRepository<T>();
                    return repository.Get();
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled)
                    Logger.ErrorFormat("T: {0}, Message: {1}, StackTrace: {2}", typeof (T), ex.Message, ex.StackTrace);
                return new List<T>();
            }
        }

        /// <summary>
        /// Get all items by scope value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> Get<T>(int scopeValue) where T : class
        {
            try
            {
                using (var context = DataContext.Instance())
                {
                    var repository = context.GetRepository<T>();
                    return repository.Get(scopeValue);
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled)
                    Logger.ErrorFormat("T: {0}, Message: {1}, StackTrace: {2}", typeof (T), ex.Message, ex.StackTrace);
                return new List<T>();
            }
        }

        /// <summary>
        /// Get items where a given column has a given value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName"></param>
        /// <param name="columnValue"></param>
        /// <returns></returns>
        public IEnumerable<T> Get<T>(string columnName, object columnValue) where T : class
        {
            try
            {
                using (var context = DataContext.Instance())
                {
                    var repository = context.GetRepository<T>();
                    return repository.Find(string.Format("WHERE {0}=@0", columnName), columnValue);
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled)
                    Logger.ErrorFormat("T: {0}, Message: {1}, StackTrace: {2}", typeof (T), ex.Message, ex.StackTrace);
                return new List<T>();
            }
        }

        /// <summary>
        /// Get items where satisfy two conditions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> Get<T>(KeyValuePair<string, object> firstCondition,
            KeyValuePair<string, object> secondCondition) where T : class
        {
            try
            {
                using (var context = DataContext.Instance())
                {
                    var repository = context.GetRepository<T>();
                    return
                        repository.Find(
                            string.Format("WHERE {0}=@0 AND {1}=@1", firstCondition.Key, secondCondition.Key),
                            firstCondition.Value, secondCondition.Value);
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled)
                    Logger.ErrorFormat("T: {0}, Message: {1}, StackTrace: {2}", typeof (T), ex.Message, ex.StackTrace);
                return new List<T>();
            }
        }

        /// <summary>
        /// Get items where satisfy three conditions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> Get<T>(IDictionary<string, object> conditions) where T : class
        {
            try
            {
                using (var context = DataContext.Instance())
                {
                    var repository = context.GetRepository<T>();

                    var list =
                        conditions.Select(condition => condition.Value != null
                            ? (!string.IsNullOrEmpty(condition.Value as string) 
                                ? string.Format("{0}='{1}'", condition.Key, condition.Value)
                                : string.Format("{0}={1}", condition.Key, condition.Value))
                            : string.Format("{0} IS NULL", condition.Key))
                            .ToList();

                    var sql = string.Format("WHERE {0}", string.Join(" AND ", list));
                    return repository.Find(sql);
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled)
                    Logger.ErrorFormat("T: {0}, Message: {1}, StackTrace: {2}", typeof (T), ex.Message, ex.StackTrace);
                return new List<T>();
            }
        }

        /// <summary>
        /// Get item by id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetById<T>(int id) where T : class
        {
            try
            {
                using (var context = DataContext.Instance())
                {
                    var repository = context.GetRepository<T>();
                    return repository.GetById(id);
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled)
                    Logger.ErrorFormat("T: {0}, Message: {1}, StackTrace: {2}", typeof (T), ex.Message, ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Add a new item or update an existing item
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public T Save<T>(int id, T item) where T : class
        {
            if (item == null)
                return null;

            try
            {
                using (var context = DataContext.Instance())
                {
                    var repository = context.GetRepository<T>();

                    if (repository.GetById(id) == null)
                        repository.Insert(item);
                    else
                        repository.Update(item);

                    return item;
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled)
                    Logger.ErrorFormat("T: {0}, Message: {1}, StackTrace: {2}", typeof (T), ex.Message, ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Delete an item by id
        /// </summary>
        /// <param name="id"></param>
        public bool DeleteById<T>(int id) where T : class
        {
            var item = GetById<T>(id);
            return item != null && Delete(item);
        }

        /// <summary>
        /// Delete an item
        /// </summary>
        /// <param name="item"></param>
        public bool Delete<T>(T item) where T : class
        {
            try
            {
                using (var context = DataContext.Instance())
                {
                    var repository = context.GetRepository<T>();
                    repository.Delete(item);
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled)
                    Logger.ErrorFormat("T: {0}, Message: {1}, StackTrace: {2}", typeof (T), ex.Message, ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Delete all items where a given column has a given value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName"></param>
        /// <param name="columnValue"></param>
        /// <returns></returns>
        public bool Delete<T>(string columnName, object columnValue) where T : class
        {
            try
            {
                using (var context = DataContext.Instance())
                {
                    var repository = context.GetRepository<T>();
                    repository.Delete(string.Format("WHERE {0}=@0", columnName), columnValue);
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled)
                    Logger.ErrorFormat("T: {0}, Message: {1}, StackTrace: {2}", typeof (T), ex.Message, ex.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Delete all items where satisfy two equal conditions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool Delete<T>(KeyValuePair<string, object> firstCondition, KeyValuePair<string, object> secondCondition)
            where T : class
        {
            try
            {
                using (var context = DataContext.Instance())
                {
                    var repository = context.GetRepository<T>();
                    repository.Delete(
                        string.Format("WHERE {0}=@0 AND {1}=@1", firstCondition.Key, secondCondition.Key),
                        firstCondition.Value, secondCondition.Value);
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (Logger.IsErrorEnabled)
                    Logger.ErrorFormat("T: {0}, Message: {1}, StackTrace: {2}", typeof (T), ex.Message, ex.StackTrace);
                return false;
            }
        }
    }
}