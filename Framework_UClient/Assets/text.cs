using System.Collections;
using System.Collections.Generic;
using ProjectApp;
using ProjectApp.Data;
using UnityEngine;
using UnityEngine.UI;
using ProjectApp.Data;
public class text : MonoBehaviour
{
    // Start is called before the first frame update

    public Text realtimeSinceStartupUI;
    public Text timeUI;
    
    void Start()
    {
        ExcelDataMgr.Instance.Init();
        realtimeSinceStartupUI.text = Sheet1VOModel.Instance.GetVO(0).name[0];
        timeUI.text = CommonsStaticVO.Instance.hdaslk[0].ToString();
      
    }

    private float val = 0;
    // Update is called once per frame
    void Update()
    {
        //MainThreadDispatcher.Instance.Update();
        return;
        val += Time.deltaTime;
        if (val>=1)
        {
            realtimeSinceStartupUI.text=(Time.realtimeSinceStartup).ToString();
            timeUI.text=(Time.time).ToString();
            val = 0;
        }
    }
    
    
    
    
}
