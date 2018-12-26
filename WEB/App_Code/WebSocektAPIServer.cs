using SuperWebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSocket4Net;

/// <summary>
/// 用于连接DGS或者其它节点的WebSocekt 服务程序
/// </summary>
public class WebSocektAPIServer
{



    /// <summary>
    /// 用于外部可以访问的单例
    /// </summary>
    private static WebSocektAPIServer _instance = new WebSocektAPIServer();

    /// <summary>
    /// WebSocket 对像
    /// </summary>
    private WebSocketServer ws = null;

    /// <summary>
    /// 用于外部可以访问的单例
    /// </summary>
    public static WebSocektAPIServer Instance
    {
        get { return _instance; }
    }

    /// <summary>
    /// 用于外部可以访问的单例
    /// </summary>
    public WebSocketServer Ws
    {
        get
        {
            return ws;
        }


    }

    public ConcurrentDictionary<string, SocketClientMapping> SessionDic
    {
        get
        {
            return sessionDic;
        }

        set
        {
            sessionDic = value;
        }
    }




    /// <summary>
    /// 开始方法 
    /// </summary>
    public void Init()
    {

        ws = new WebSocketServer();
        Ws.NewMessageReceived += Ws_NewMessageReceived; //当有信息传入时
        Ws.NewSessionConnected += Ws_NewSessionConnected; //当有用户连入时
        Ws.SessionClosed += Ws_SessionClosed; //当有用户退出时
        Ws.NewDataReceived += Ws_NewDataReceived; //当有数据传入时

        int port = 10086;
        //从配置文件 中获取WEB socket 对像
        if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("WebSocketAPIPort"))
        {
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["WebSocketAPIPort"], out port);
        }


        if (Ws.Setup(port)) //绑定端口
        {
            Ws.Start(); //启动服务   
        }
    }

    #region WebSocekt 的各项操作



    System.Collections.Concurrent.ConcurrentDictionary<string, SocketClientMapping> sessionDic = new System.Collections.Concurrent.ConcurrentDictionary<string, SocketClientMapping>();

    /// <summary>
    /// 当有信息传入时
    /// </summary>
    /// <param name="session"></param>
    /// <param name="value"></param>
    private void Ws_NewDataReceived(WebSocketSession session, byte[] value)
    {



    }

    /// <summary>
    /// 当有用户连入时
    /// </summary>
    /// <param name="session"></param>
    /// <param name="value"></param>
    private void Ws_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
    {


        SocketClientMapping mapping;
        if (SessionDic.TryRemove(session.SessionID, out mapping))
        {

            try { mapping.SocketClient.Close(); } catch { }

        }


    }

    /// <summary>
    /// 当有用户退出时
    /// </summary>
    /// <param name="session"></param>
    private void Ws_NewSessionConnected(WebSocketSession session)
    {
        //没办法传地址，所以这里不能用了
        //string key = session.SessionID;
        //string url = "ws://127.0.0.1:4618";

        //SocketClientMapping mapping = new SocketClientMapping()
        //{
        //    Key = key,
        //    WsUrl = url,
        //    WebClientObj = session,
        //    LastActiveTime = DateTime.Now

        //};

        //WebSocket websocket = new WebSocket(url);
        //#region 接收服务端的WebSocket信息
        //websocket.Opened += (s, e) => {
        //    try { session.Send("与：" + url + "信息通道建立"); } catch { }

        //};
        //websocket.Closed += (s, e) => {

        //    try { session.Send("与：" + url + "信息通道断开，请刷新页面重新在联接");
        //        session.Close();
        //    } catch { }
        //};
        //websocket.MessageReceived += (s, e) => {
        //    mapping.LastActiveTime = DateTime.Now;
        //    mapping.WebClientObj.Send(e.Message);
        //};
        ////运行打开联接
        //if (websocket.State != WebSocketState.Open &&
        //websocket.State != WebSocketState.Connecting
        //)
        //{
        //    websocket.Open();
        //}

        //#endregion
        //mapping.SocketClient = websocket;
        ////添加信息
        //SessionDic.TryAdd(key, mapping);
    }

    /// <summary>
    /// 当有数据传入时
    /// </summary>
    /// <param name="session"></param>
    /// <param name="value"></param>
    private void Ws_NewMessageReceived(WebSocketSession session, string value)
    {
        //示例数据 IPV6 fe80::d885:5059:cb49:6241%10,
        //{"ToKen":"5f6d4912-1a39-4640-a77f-a5b7ac7f0751|172.16.15.158|DGS.Reciver","MsgID":1,"Content":"最后活动时间： 2018-10-09 15:03:46"}
        WSEntity et = null;
        try {
            et = Newtonsoft.Json.JsonConvert.DeserializeObject<WSEntity>(value);
        }
        catch {

           
        }

 
 
         string url = et.ToKen ;
 
  
        try
        {


            SocketClientMapping mapx;
            if (SessionDic.TryGetValue(session.SessionID, out mapx))
            {
                switch (et.MsgID)
                {
                    //表示消息
                    case 1:
                        Msg msg = Newtonsoft.Json.JsonConvert.DeserializeObject<Msg>(value);
                        mapx.Records.Add(msg);
                        if (mapx.Records.Count > 20)
                        {
                            mapx.Records.RemoveAt(0);

                        }
                        break;
                }
            }
            else
            {

                #region 以下代码可以封装为方法 
                string key = session.SessionID;

                SocketClientMapping mapping = new SocketClientMapping()
                {
                    Key = key,
                    WsUrl = url,
                    WebClientObj = session,
                    LastActiveTime = DateTime.Now,
                    Records = new List<IWSEntity>()

                };

                mapx = mapping;
                


 

                #endregion
               
                //添加信息
                SessionDic.TryAdd(key, mapping);
                #endregion

            }
            //更新最后通讯时间
            mapx.LastActiveTime = DateTime.Now;

            #region 接收客户端数据后的各项操作



            #endregion


        }
        catch (Exception ex)
        {
            session.TrySend("传入数据信息错误；" + ex);
        }



    }

    #region 以前的代码
    ///// <summary>
    ///// 当有数据传入时
    ///// </summary>
    ///// <param name="session"></param>
    ///// <param name="value"></param>
    //private void Ws_NewMessageReceived(WebSocketSession session, string value)
    //{
 

    //    string content = string.Empty;
    //    string url = "";

    //    var str = value.Split(';');
    //    if (str.Length > 1)
    //    {
    //        content = str[1];
    //        url = str[0];
    //    }
    //    else
    //    {
    //        session.TrySend("传入数据信息错误；格式应该为xxx;xxx");
    //        return;
    //    }

    //    try
    //    {


    //        SocketClientMapping mapx;
    //        if (SessionDic.TryGetValue(session.SessionID, out mapx))
    //        {

    //        }
    //        else
    //        {

    //            #region 以下代码可以封装为方法 
    //            string key = session.SessionID;

    //            SocketClientMapping mapping = new SocketClientMapping()
    //            {
    //                Key = key,
    //                WsUrl = url,
    //                WebClientObj = session,
    //                LastActiveTime = DateTime.Now

    //            };

    //            mapx = mapping;





    //            #endregion

    //            //添加信息
    //            SessionDic.TryAdd(key, mapping);
    //            #endregion

    //        }
    //        //更新最后通讯时间
    //        mapx.LastActiveTime = DateTime.Now;

    //        #region 接收客户端数据后的各项操作



    //        #endregion


    //    }
    //    catch (Exception ex)
    //    {
    //        session.TrySend("传入数据信息错误；" + ex);
    //    }



    //}
#endregion


    public bool SendObject(string clientUrl, string toClientStr) {


      var arr=   SessionDic.ToArray();

        var token = arr.Where(p => p.Value.WsUrl == clientUrl);
        if (token.Count()>0)
        {
           return  token.FirstOrDefault().Value.WebClientObj.TrySend(toClientStr);
        }

        return false;

    }

 
}


#region 实体及接口定义

 



/// <summary>
/// 接口信息
/// </summary>
public class WSEntity
{
    /// <summary>
    /// 消息类型 
    /// 255 表示关闭
    /// </summary>
    public int MsgID { get; set; }
    /// <summary>
    /// 标识
    /// </summary>
    public string ToKen { get; set; }
}

/// <summary>
/// 消息
/// </summary>
public class Msg : IWSEntity
{

    /// <summary>
    /// ToKen标识  节点flag +|+ 类型+ | + IP 
    /// </summary>
    public string ToKen { get; set; }
    /// <summary>
    /// 消息类型 
    /// 255 表示关闭
    /// </summary>
    public int MsgID { get { return 1; } }

    /// <summary>
    ///内容
    /// </summary>
    public string Content { get; set; }


}

/// <summary>
/// 关闭进程
/// </summary>
public class CloseProcessEntity : IWSEntity
{
    /// <summary>
    /// ToKen标识  节点flag +|+ 类型+ | + IP 
    /// </summary>
    public string ToKen { get; set; }
    /// <summary>
    /// 消息类型 
    /// 255 表示关闭
    /// </summary>
    public int MsgID { get { return 255; } }

    /// <summary>
    ///内容
    /// </summary>
    public string Content { get; set; }

}


/// <summary>
/// 命令实体接口
/// </summary>
public interface IWSEntity
{
    /// <summary>
    /// 标识
    /// </summary>
    string ToKen { get; set; }

    /// <summary>
    /// 消息类型 
    /// 255 表示关闭
    /// </summary>
    int MsgID { get; }

    /// <summary>
    ///内容
    /// </summary>
    string Content { get; set; }
}

/// <summary>
/// 处理WS实体
/// </summary>
public interface IWSDo
{
    /// <summary>
    /// 处理事的方法
    /// </summary>
    /// <param name="ent">实体</param>
    /// <param name="ws">WebSocekt对像</param>
    void DoWork(WebSocket ws, IWSEntity ent);
    /// <summary>
    /// 消息类型 
    /// </summary>
    int MsgID { get; }

}
#endregion