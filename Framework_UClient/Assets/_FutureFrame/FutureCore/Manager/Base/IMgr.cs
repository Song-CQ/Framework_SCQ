namespace FutureCore
{
    public interface IMgr
    {
        public bool IsInit { get;}
        public bool IsStartUp { get; }
        public bool IsDispose { get; }
        void Init();

        void StartUp();

        void DisposeBefore();
        void Dispose();
        
    }
}