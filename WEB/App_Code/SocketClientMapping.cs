using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SuperWebSocket;
using WebSocket4Net;

/// <summary>
/// 映射外部Socket 与内部Web节点对像信息
/// SocketClient映射关系
/// </summary>
public class SocketClientMapping
{

    /// <summary>
    /// 联接到服务器的URL
    /// </summary>
    public string WsUrl { get; set; }
    /// <summary>
    /// 用户GUID
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 从网站外联接到网站的WebSocket对像
    /// </summary>
    public WebSocketSession WebClientObj { get; set; }

    /// <summary>
    /// 联接服务器的Socket
    /// </summary>

    public WebSocket SocketClient { get; set; }


    /// <summary>
    /// 最后活动时间
    /// </summary>
   public DateTime LastActiveTime { get; set; }
}