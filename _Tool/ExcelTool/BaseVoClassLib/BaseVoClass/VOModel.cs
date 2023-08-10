using System.Collections.Generic;
using System.Diagnostics;

namespace ProjectApp.Data
{
    public abstract class VOModel<MyModel,VOClass>:BaseVOModel
        where MyModel:class,new()
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
        
        public void SetData(VOClass[] voLists)
        {
            foreach (VOClass _vo in voLists)
            {
                m_voLst.Add(_vo);
                if (HasStringKey)
                {
                    m_keyDic.Add(_vo.key,_vo);
                }
                if (HasStringId)
                {
                    m_idDic.Add(_vo.id,_vo);
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
                   UnityEngine.Debug.Log("键"+key+"不存在");   
                }
            }
            else
              
            {
                UnityEngine.Debug.Log("未初始化");   
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
                    UnityEngine.Debug.Log("id"+id+"不存在");   
                }
            }
            else
            {
                UnityEngine.Debug.Log("未初始化");   
            }
            return vo;
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
        
        
        

    }
}