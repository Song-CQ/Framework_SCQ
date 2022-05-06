using System;

namespace FutureCore
{
    public abstract class BaseMgr<T> : IDisposable, IMgr where T : BaseMgr<T>, new()
    {
        public bool IsInit { get; private set; }
        public bool IsStartUp { get; private set; }
        public bool IsDispose { get; private set; }

        private static T m_instance;

        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new T();
                    m_instance.New();
                }

                return m_instance;
            }
        }

        public BaseMgr()
        {
        }

        protected virtual void New()
        {
            IsDispose = false;
        }

        public virtual void Init()
        {
            IsInit = true;
        }

        public virtual void StartUp()
        {
            IsStartUp = true;
        }

        public virtual void DisposeBefore()
        {
            IsDispose = true;
            IsInit = false;
            IsStartUp = false;
        }

        public virtual void Dispose()
        {
            m_instance = null;
        }
    }
}