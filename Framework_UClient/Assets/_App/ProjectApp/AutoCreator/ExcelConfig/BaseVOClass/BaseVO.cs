namespace ProjectApp.Data
{
    public class BaseVO
    {
        /// <summary>
        /// id
        /// </summary>
        public int id;

        /// <summary>
        /// key
        /// </summary>
        public string key;

    }

    public class BaseStaticVO
    {
        public virtual string Key { get; }

    }
}