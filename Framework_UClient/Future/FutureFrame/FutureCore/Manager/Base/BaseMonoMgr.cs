namespace FutureCore
{
    public abstract class BaseMonoMgr<T> : SingletonMono<T>, IMgr where T : BaseMonoMgr<T>
    {
        protected override string ParentRootName
        {
            get { return AppObjConst.MonoManagerGoName; }
        }
           
        public bool IsInit { get; private set; }
        public bool IsStartUp { get; private set; }
        public bool IsDispose { get; private set; }

        protected override void New()
        {
            base.New();
            IsDispose = false;
        }

        public override void Init()
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

        private void OnDisable()
        {

        }

        public virtual void Dispose()
        {

        }
    }
}