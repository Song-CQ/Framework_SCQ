/****************************************************
    文件: DispatcherMgr.cs
    作者: Clear
    日期: 2026/3/16 15:55:44
    类型: 框架核心脚本(请勿修改)
    功能: Nothing
*****************************************************/
using FutureCore;

namespace ProjectApp
{
    
    public class DispatcherMgr :BaseMgr<DispatcherMgr>
    {
        public GenericDispatcher GenericDispatcher {private set;get;}
        public AppDispatcher AppDispatcher {private set;get;}
        public CtrlDispatcher CtrlDispatcher {private set;get;}
        public MainThreadDispatcher MainThreadDispatcher {private set;get;}
        public ModelDispatcher ModelDispatcher {private set;get;}
        public UICtrlDispatcher UICtrlDispatcher {private set;get;}
        public QuestDispatcher QuestDispatcher {private set;get;}
        public PlayerDataDispatcher PlayerDataDispatcher {private set;get;}

        public override void Init()
        {
            base.Init();

            EventData.Init();
            GenericDispatcher = GenericDispatcher.Instance;
            AppDispatcher = AppDispatcher.Instance;
            MainThreadDispatcher = MainThreadDispatcher.Instance;
            ModelDispatcher = ModelDispatcher.Instance;
            UICtrlDispatcher = UICtrlDispatcher.Instance;
            CtrlDispatcher = CtrlDispatcher.Instance;
            QuestDispatcher = QuestDispatcher.Instance;
            PlayerDataDispatcher = PlayerDataDispatcher.Instance;
        }


        public override void StartUp()
        {
            base.StartUp();


        }

        public override void Dispose()
        {
            base.Dispose();
            EventData.Clear();
            GenericDispatcher = null;
            AppDispatcher = null;
            MainThreadDispatcher = null;
            ModelDispatcher = null;
            UICtrlDispatcher = null;
            CtrlDispatcher = null;
            QuestDispatcher = null;
            PlayerDataDispatcher = null;



        }





    }

    



}