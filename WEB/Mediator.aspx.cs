using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Mediator : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {    
        StartWebSocketServer();

        BindData();


    }

    public void BindData()
    {


        var sb = "一汽DGSNode1;175.19.140.4:4618,本地测试代理;127.0.0.1:4618;,胡家豪;172.16.15.121:4618;";
        var data = sb.Split(',');

         Repeater1.DataSource = from p in data
                               let s = p.Split(';')
                               select new { key = s[0], val = s[1] };
        Repeater1.DataBind();
    }

    /// <summary>
    /// 开始WebSocket 服务
    /// </summary>
    public void StartWebSocketServer()
    {
        if (SocketHelper.Instance.Ws == null)
        {
            SocketHelper.Instance.Init();
        }
        else
        {
            switch (SocketHelper.Instance.Ws.State)
            {
                //case SuperSocket.SocketBase.ServerState.Initializing:
                //case SuperSocket.SocketBase.ServerState.NotInitialized:
                case SuperSocket.SocketBase.ServerState.NotStarted:

                    SocketHelper.Instance.Init();

                    break;
            }


        }

    }
}