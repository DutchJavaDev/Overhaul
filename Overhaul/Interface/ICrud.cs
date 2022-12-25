namespace Overhaul.Interface
{
    public interface ICrud
    {
        T Create<T>(T entity) where T : class;
        Task<T> CreateAsync<T>(T entity) where T : class;
        T Read<T>(params string[] columns) where T : class;
        Task<T> ReadAsync<T>(params string[] columns) where T : class;
        bool Update<T>(T entity) where T : class;
        Task<bool> UpdateAsync<T>(T entity) where T : class;
        T GetById<T>(object id, params string[] columns) where T : class;
        Task<T> GetByIdAsync<T>(object id, params string[] columns) where T : class;
        T GetBy<T>(string columnName, object value, params string[] columns) where T : class;
        Task<T> GetByAsync<T>(string columnName, object value, params string[] columns) where T : class;
        IEnumerable<T> GetCollection<T>(params string[] columns) where T : class;
        Task<IEnumerable<T>> GetCollectionAsync<T>(params string[] columns) where T : class;
        IEnumerable<T> GetCollectionWhere<T>(string columnName, object value, params string[] columns) where T : class;
        Task<IEnumerable<T>> GetCollectionWhereAsync<T>(string columnName, object value, params string[] columns) where T : class;
        void Delete<T>(T entity) where T : class;
    }
}
