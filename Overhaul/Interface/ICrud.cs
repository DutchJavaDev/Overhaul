namespace Overhaul.Interface
{
    public interface ICrud
    {
        /// <summary>
        /// Creates a new entity in the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>Inserted entity</returns>
        T Create<T>(T entity) where T : class;

        /// <summary>
        /// Reads the first entity from the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Read<T>() where T : class;

        /// <summary>
        /// Updates an existing entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Update<T>(T entity) where T : class;
        T GetById<T>(object id) where T : class;
        T GetBy<T>(string columnName, object value) where T : class;
        IEnumerable<T> GetCollection<T>() where T : class;
        IEnumerable<T> GetCollectionWhere<T>(string columnName, object value) where T : class;
        /// <summary>
        /// Delete an existing entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void Delete<T>(T entity) where T : class;
    }
}
