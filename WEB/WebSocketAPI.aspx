<%@ Page Language="C#" AutoEventWireup="true" CodeFile="WebSocketAPI.aspx.cs" Inherits="WebSocketAPI" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>DGS 交互操作界面</title>
        <script type="text/javascript" src="jquery.min.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
        </div>
       <h4>请在下拉框中选择需要操作的DGS节点</h4>
    <script type="text/javascript">
        function geturl(c,b ) {

            var str = "?uri=" + c + "&cont=" + b +"&"+ Math.random();
            //alert(str);
            $.get(str);

        }

    </script>
 
        <asp:DropDownList ID="dr" runat="server">
        </asp:DropDownList>
        <input id="Button1" type="button" value="发送关闭命令"  onclick="geturl($('#<%=dr.ClientID%>').val(),'close');"/><br />
        <br />
       
    </form>
    </body>
</html>
