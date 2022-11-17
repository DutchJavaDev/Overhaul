namespace Overhaul.Interface
{
    internal interface ICrud
    {
        /// <summary>
        /// Creates a new entity in the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns>Inserted entity</returns>
        T Create<T>(T entity) where T : class;

        /// <summary>
        /// Reads the first entity from the databatase
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
        /// Delete an existing entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        void Delete<T>(T entity) where T : class;
    }
}
