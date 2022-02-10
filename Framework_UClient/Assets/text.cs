using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using XLNet;

public class text : MonoBehaviour
{
    // Start is called before the first frame update

    public Text realtimeSinceStartupUI;
    public Text timeUI;

    public Image imgtest1;
    public Image imgtest2;



    void Init()
    {


        DownTexture(TestUre, (texture) =>
        {
            imgtest1.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
            WriteHead(texture);

            UnityWebRequestFile(GetHeadPath());

        });
    }

    public void UnityWebRequestFile(string fileName)
    {
        string url;
        #region 分平台判断 StreamingAssets 路径
        //如果在编译器或者单机中
#if UNITY_EDITOR || UNITY_STANDALONE

        url = "file://" + fileName;
        //否则如果在Iphone下
#elif UNITY_IPHONE
        url = "file://"+ fileName;
            //否则如果在android下
#elif UNITY_ANDROID
        url = "file://"+ fileName;
#endif
        #endregion
        DownTexture(url, (texture) =>
        {
            imgtest2.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2);
            Debug.Log("加载本地fb头像");
        });
    }


    private void WriteHead(Texture2D texture)
    {
        byte[] datas = texture.EncodeToPNG();
        string headpath = GetHeadPath();
        if (File.Exists(headpath))
        {
            File.Delete(headpath);
        }
        using (FileStream fs = new FileStream(headpath, FileMode.Create))
        {
            fs.Write(datas, 0, datas.Length);
            fs.Dispose();
        }


    }

    private string GetHeadPath()
    {
        string pal = string.Empty;
        pal = UnityEngine.Application.persistentDataPath;
        pal += "/" + "asas.png";
        Debug.Log(pal);
        return pal;
    }
    private string TestUre = "https://platform-lookaside.fbsbx.com/platform/profilepic/?asid=779586112905584&height=50&width=50&ext=1638613024&hash=AeR2chwRDdqSyyvm5X0";

    private float val = 0;
    // Update is called once per frame
    void Update()
    {
        //MainThreadDispatcher.Instance.Update();
        return;
        val += Time.deltaTime;
        if (val >= 1)
        {
            realtimeSinceStartupUI.text = (Time.realtimeSinceStartup).ToString();
            timeUI.text = (Time.time).ToString();
            val = 0;
        }
    }
    public void DownTexture(string webUrl, Action<Texture2D> callback)
    {
        StartCoroutine(OnDownTexture(webUrl, callback));
    }

    IEnumerator OnDownTexture(string webUrl, Action<Texture2D> callback)
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(webUrl))
        {
            uwr.timeout = 5;
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError || uwr.isHttpError)
            {
                LogUtil.LogError("[HttpMgr]" + uwr.error);
                callback(null);
                yield break;
            }

            Texture2D texture2D = DownloadHandlerTexture.GetContent(uwr);
            callback(texture2D);
        }
    }

    private void Start()
    {
        XLNetTool.SetLog((e) => Debug.Log(e), (e) => Debug.LogWarning(e), (e) => Debug.LogError(e));

        XLSocket<XLNetMsg, SocketLink> xLSocket = new XLSocket<XLNetMsg, SocketLink>();
        xLSocket.StartAsClient("127.0.0.1", 8037);

        //PENet.PESocket<swedas, peMsg> xLSocket33 = new PENet.PESocket<swedas, peMsg>();
        //xLSocket33.StartAsClient("127.0.0.1", 8037);

    }

 
 
}


namespace XLNet
{
   

    public class SocketLink : XLSocketLink<XLNetMsg>
    {
        protected override void OnConnected()
        {
            base.OnConnected();
            XLNetMsg Val = new XLNetMsg();
            Val.Name = "服务器";
            Val.Val = "你好";
            Send(Val);
        }
        protected override void OnReciveMsg(XLNetMsg msg)
        {
            base.OnReciveMsg(msg);
            Debug.Log(msg.Name);
            Debug.Log(msg.Val);
        }
    }   

}