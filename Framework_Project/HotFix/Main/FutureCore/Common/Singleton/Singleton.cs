using System;

namespace ProjectApp.HotFix
{
    /// <summary>
    /// ������
    /// </summary>
    public class Singleton<T> : IDisposable where T : class, new()
    {
        private static T m_instance;
        public static T Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new T();
                return m_instance;
            }
        }

        public virtual void Dispose()
        {
            m_instance = null;
        }
    }
}
