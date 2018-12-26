<%@ Page Language="C#" AutoEventWireup="true" CodeFile="base64.aspx.cs" Inherits="base64" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        数据源(base64)<br />
        <asp:TextBox ID="txtBase64" runat="server" Height="131px" TextMode="MultiLine" Width="456px"></asp:TextBox>
&nbsp;<br />
        <asp:RadioButton ID="Base64ToHex" runat="server" GroupName="b"  Text="Base64转Hex" />
        <asp:RadioButton ID="Base64ToAsc" Checked="true" runat="server" GroupName="b" Text="Base64ToAsc||" />
        <asp:RadioButton ID="Base64ToGBK" runat="server" GroupName="b" Text="Base64GBK" />
        <asp:RadioButton ID="Base64ToUtf8" runat="server" GroupName="b" Text="Base64UTF-8" />
        <br />
        <asp:TextBox ID="txtResult" runat="server" Height="131px" TextMode="MultiLine" Width="456px"></asp:TextBox>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="转换" />
    </form>
</body>
</html>
