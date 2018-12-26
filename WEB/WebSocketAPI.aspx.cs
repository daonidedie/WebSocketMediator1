using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
/// <summary>
/// WEBSocket API 用于外部访问 DMS 集中管理平台进行交互
/// 主要功能有：
/// 1.立即上报当前节点状态
/// 2.定时上报当前节点状态
/// 3.下发重启指令让节点立即重启
/// 4.下发解邦命令让节点立即解邦
/// </summary>
public partial class WebSocketAPI : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        if (string.IsNullOrEmpty(Request.QueryString.ToString()))
        {
            WebSocektAPIServer.Instance.Init();

            this.Label1.Text += string.Join("<br/>", WebSocektAPIServer.Instance.SessionDic.Values.Select(p =>
            string.Format("<a href='#' onclick=\"javascript:geturl('{0}','12121');\"> name:{0}, lastactive:{1}</a>",p.WsUrl,p.LastActiveTime)
            ));


            dr.DataSource = WebSocektAPIServer.Instance.SessionDic.Values.Select(p =>new { t = string.Format("name:{0}, lastactive:{1}", p.WsUrl, p.LastActiveTime), v = p.WsUrl } );
            dr.DataTextField = "t";
            dr.DataValueField = "v";
            dr.DataBind();





        }
        else{

            SendText(Request.QueryString["uri"], Request.QueryString["cont"]);

        }

 
    }


    /// <summary>
    /// 发送数据
    /// </summary>
    public void SendText(string uri,string abc)
    {
        switch (abc) {

            case "close":
                WebSocektAPIServer.Instance.SendObject(uri, Newtonsoft.Json.JsonConvert.SerializeObject(new CloseProcessEntity() { ToKen = Guid.NewGuid().ToString(), Content = "关闭节点" }));
                break;
            default:
                WebSocektAPIServer.Instance.SendObject(uri, abc);
                break;
        }

        
    }
}