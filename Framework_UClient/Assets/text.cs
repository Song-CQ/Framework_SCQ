using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class text : MonoBehaviour
{
    // Start is called before the first frame update

    public Text realtimeSinceStartupUI;
    public Text timeUI;
    
    void Start()
    {
        
    }

    private float val = 0;
    // Update is called once per frame
    void Update()
    {
        val += Time.deltaTime;
        if (val>=1)
        {
            realtimeSinceStartupUI.text=(Time.realtimeSinceStartup).ToString();
            timeUI.text=(Time.time).ToString();
            val = 0;
        }
    }
    
    
    
    
}
