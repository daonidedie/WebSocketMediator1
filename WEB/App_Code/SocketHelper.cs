using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SuperWebSocket;

using SuperSocket.SocketBase.Config;
using WebSocket4Net;

/// <summary>
/// SocketHelper 的摘要说明
/// </summary>
public class SocketHelper
{

    /// <summary>
    /// 用于外部可以访问的单例
    /// </summary>
    private static SocketHelper _instance = new SocketHelper();

    /// <summary>
    /// WebSocket 对像
    /// </summary>
    private WebSocketServer ws = null;

    /// <summary>
    /// 用于外部可以访问的单例
    /// </summary>
    public static SocketHelper Instance
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
        if (System.Configuration.ConfigurationManager.AppSettings.AllKeys.Contains("WebSocketHostPort"))
        {
            int.TryParse(System.Configuration.ConfigurationManager.AppSettings["WebSocketHostPort"], out port);
        }


        if (Ws.Setup(port)) //绑定端口
        {
            Ws.Start(); //启动服务   
        }
    }

    #region WebSocekt 的各项操作



    System.Collections.Concurrent.ConcurrentDictionary<string, SocketClientMapping> SessionDic = new System.Collections.Concurrent.ConcurrentDictionary<string, SocketClientMapping>();

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

            try { mapping.SocketClient.Close(); }catch{ }
            
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
        string content = string.Empty;
        string url = "";

        var str = value.Split(';');
        if (str.Length > 1)
        {
              content = str[1];
              url = str[0];
        }
        else
        {
            session.TrySend("传入数据信息错误；格式应该为xxx;xxx");
            return;
        }

        try
        {
           

            SocketClientMapping mapx;
            if (SessionDic.TryGetValue(session.SessionID, out mapx))
            {


            }
            else
            {

                #region 以下代码可以封装为方法 
                string key = session.SessionID;
                url = "ws://" + url;

                SocketClientMapping mapping = new SocketClientMapping()
                {
                    Key = key,
                    WsUrl = url,
                    WebClientObj = session,
                    LastActiveTime = DateTime.Now

                };

                mapx = mapping;

                WebSocket websocket = new WebSocket(url) {
                   
                    //30分钟
                      AutoSendPingInterval=60*30,
                };
                #region 接收服务端的WebSocket信息
                websocket.Opened += (s, e) =>
                {
                    try { session.Send("与：" + url + "信息通道建立"); } catch { }

                };
                websocket.Closed += (s, e) =>
                {

                    try { session.Send("与：" + url + "信息通道断开，请刷新页面重新在联接"); } catch { }
                };
                websocket.MessageReceived += (s, e) =>
                {
 
                    //发送失败断开联接
                    if (!session.TrySend(e.Message))
                    {
                        if (session.Connected)
                        {
                            try { session.Send(e.Message); }catch
                            {
                                try { session.Close(); } catch { }
                                try { websocket.Close(); } catch { }
                                return;
                            }
                        }
                        else {
                            try { session.Close(); } catch { }
                            try { websocket.Close(); } catch { }
                            return;
                        }
                        
                    }
                    mapping.LastActiveTime = DateTime.Now;


                };

                #endregion
                mapping.SocketClient = websocket;
                //添加信息
                SessionDic.TryAdd(key, mapping);
                #endregion

            }

            if (mapx.SocketClient.State != WebSocketState.Open &&
                mapx.SocketClient.State != WebSocketState.Connecting
                )
            {
                mapx.SocketClient.Open();
            }
            else
            {
                mapx.SocketClient.Send(content);
            }
        }
        catch (Exception ex ){
            session.TrySend("传入数据信息错误；"+ex);
        }



}

 



    #endregion



}