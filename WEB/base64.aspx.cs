using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class base64 : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    /// <summary>
    /// Base64转换页面
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button1_Click(object sender, EventArgs e)
    {
        string tt = txtBase64.Text.Replace(" ",string.Empty).Replace("\r","").Replace("\n","");
        System.Text.Encoding encodestr = System.Text.Encoding.ASCII;

        if (Base64ToAsc.Checked) {
            encodestr = System.Text.Encoding.ASCII;
        }

               if (Base64ToGBK.Checked) {
            encodestr = System.Text.Encoding.GetEncoding("GBK");
        }
               if (Base64ToHex.Checked) {
            txtResult.Text = BitConverter.ToString(Convert.FromBase64String(tt));
            return;
        }

                       if (Base64ToUtf8.Checked) {
            encodestr = System.Text.Encoding.UTF8;
        }

        txtResult.Text = encodestr.GetString(Convert.FromBase64String(tt));


    }
}