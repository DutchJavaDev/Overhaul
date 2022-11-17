namespace Overhaul.Interface
{
    public interface IModelTracker
    {
        /// <summary>
        /// Track a set of entity's
        /// </summary>
        /// <param name="types"></param>
        public void Track(IEnumerable<Type> types);

        /// <summary>
        /// Get a CRUD instance to modify entity's
        /// </summary>
        /// <returns></returns>
        public ICrud GetCrudInstance();
    }
}
