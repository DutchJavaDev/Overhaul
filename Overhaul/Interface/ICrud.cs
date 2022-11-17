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
        /// <summary>
        /// Gets a entity by its primary key/id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById<T>(object id) where T : class;
        /// <summary>
        /// Gets an entity by a specific column field
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        T GetBy<T>(string columnName, object value) where T : class;
        /// <summary>
        /// Gets a collection of entity's
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IEnumerable<T> GetCollection<T>() where T : class;
        /// <summary>
        /// Gets s collection of entity's based of a column value 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        IEnumerable<T> GetCollectionWhere<T>(string columnName, object value) where T : class;
        /// <summary>
        /// Delete an existing entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void Delete<T>(T entity) where T : class;
    }
}
