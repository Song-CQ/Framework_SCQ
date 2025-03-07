using System.Collections.Generic;
using System.Diagnostics;

namespace ProjectApp.Data
{
    public abstract class VOModel<MyModel,VOClass>:BaseVOModel 
        where MyModel: BaseVOModel, new()
        where VOClass:BaseVO 
    {
        private static MyModel m_instance;
        public static MyModel Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new MyModel();
                    m_instance.Init();
                }
                return m_instance;
            }
        }

        public bool IsInit { private set; get; } = false;
        private Dictionary<int, VOClass> m_idDic;
        private Dictionary<string, VOClass> m_keyDic;

        private List<VOClass> m_voLst;
        
        public override void Init()
        {
            m_idDic = new Dictionary<int, VOClass>();
            m_keyDic = new Dictionary<string, VOClass>();
            m_voLst  = new List<VOClass>();
        }

        public override void Reset()
        {
            m_idDic.Clear();
            m_keyDic.Clear();
            m_voLst.Clear();
        }
        
        public override void Dispose()
        {
            Reset();
            m_idDic = null;
            m_keyDic = null;
            m_voLst = null;
        }

        public override void SetData(BaseVO[] voLists)
        {
            foreach (BaseVO _vo in voLists)
            {
                VOClass vOClass = _vo as VOClass;
                m_voLst.Add(vOClass);
                if (HasStringKey)
                {
                    m_keyDic.Add(_vo.key, vOClass);
                }
                if (HasStringId)
                {
                    m_idDic.Add(_vo.id, vOClass);
                }
            }
            IsInit = true;
        }

        public VOClass GetVO(string key)
        {
            VOClass vo = null;
            if (IsInit)
            {
                if (!m_keyDic.TryGetValue(key,out vo))
                {
                    LogUtil.LogError("键"+key+"不存在");   
                }
            }
            else
              
            {
                LogUtil.LogError("未初始化");   
            }
            return vo;
        }

        public VOClass GetVO(int id)
        {
            VOClass vo = null;
            if (IsInit)
            {
                if (!m_idDic.TryGetValue(id,out vo))
                {
                    LogUtil.LogError("id"+id+"不存在");   
                }
            }
            else
            {
                LogUtil.LogError("未初始化");   
            }
            return vo;
        }

        public List<VOClass> GetVOList()
        {
            return m_voLst;
        }

        public VOClass GetFirstVO()
        {
            if (m_voLst.Count>0)
            {
                return m_voLst[0];
            }
            return null;
        }
        public VOClass GetLastVO()
        {
            if (m_voLst.Count>0)
            {
                return m_voLst[m_voLst.Count-1];
            }
            return null;
        }

        public override BaseVO GetBaseVO(string key)
        {
            return GetVO(key);
        
        }
        public override BaseVO GetBaseVO(int id)
        {
            return GetVO(id);
        }
        public override List<BaseVO> GetBaseVOList()
        {
            return GetVOList() as List<BaseVO>;
        }
        public override BaseVO GetFirstBaseVO()
        {
            return GetFirstVO();
        }
        public override BaseVO GetLastBaseVO() 
        {
            return GetLastVO();
        }
        

    }
}