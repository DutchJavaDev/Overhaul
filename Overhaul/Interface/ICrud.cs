namespace Overhaul.Interface
{
    internal interface ICrud
    {
        T Create<T>(T entity) where T : class;

        T Read<T>() where T : class;

        bool Update<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;
    }
}
