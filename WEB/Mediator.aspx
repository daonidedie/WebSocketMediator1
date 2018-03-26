<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Mediator.aspx.cs" Inherits="Mediator" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html>
<head>
    <title>websocket client ver1.2</title>
    <style type="text/css">
        body {
            /*background-color: #000000;
            color: #ffffff;*/
            font-size: 12px;
           
        }
        #sendForm{ display: block;}
        #bmap {display: none;}
        ul {
            list-style:decimal;
        }
    </style>
    <script type="text/javascript" src="jquery.min.js"></script>
    <script id="sc1" type="text/javascript">

        //webSocket对像
        var webSocket = { n: 1, maxloglength: 1000, logs: [], spn: 1, funs: {}, pos: {}};

        


        //开始
        webSocket.start = function () {
            webSocket.n++;
            var inc = document.getElementById('incomming');
            var nx = document.getElementById('nx');
            var wsImpl = window.WebSocket || window.MozWebSocket;
            var form = document.getElementById('sendForm');
            var input = document.getElementById('sendText');
            var ipv = document.getElementById('ip');
            var portv = document.getElementById('port');
            //服务器地址
            var serverStr = ipv.value + ':' + portv.value;

            inc.innerHTML = "";
            inc.innerHTML += "正在连接到服务器" + serverStr + "<br/>";



            // create a new websocket and connect

            window.ws = new wsImpl('ws://' + serverStr + '/');

            // when data is comming from the server, this metod is called
            ws.onmessage = function (evt) {
                var logdiv = $('#logs');
                if (webSocket.logs.length > webSocket.maxloglength) {
                    //删除原素自身
                    webSocket.logs[0].remove();
                    //就是删除下标为1的数组元素。
                    webSocket.logs.splice(0, 1);
                }
                var obj = $("<li>" + webSocket.ParseLocation(evt.data) + "</li>");
                webSocket.logs.push(obj);
                logdiv.prepend(obj);

               
            };

            // when the connection is established, this method is called
            ws.onopen = function () {
                inc.innerHTML = '服务器 ' + serverStr + ' 连接成功<br/>';
               var al= $('#sel').val() + ';' 
                ws.send(al);
            };

            // when the connection is closed, this method is called
            ws.onclose = function () {
                inc.innerHTML += '.. connection closed<br/>';
                inc.innerHTML += "第" + webSocket.n + '次2秒后重新开始联接...<br/>';
                nx.innerHTML = webSocket.n;
                if (window.ws != undefined) {
                    window.ws.close();
                }
                delete wsImpl;
                delete window.ws;
                setTimeout(webSocket.start, 2000);
            }

            form.addEventListener('submit', function (e) {
                e.preventDefault();
                var val =  $('#sel').val() + ';' + input.value;
               ws.send(val);
                input.value = "";
            });
        }

        webSocket.ParseLocation = function (location) {
            var dd = location;

            //检查有效经纬度
            if (dd.indexOf("经度:") != -1 && dd.indexOf("纬度:") != -1) {
                var sts = dd.split(',');
                var lon = 0;
                var lat = 0;

                for (n = 0; n < sts.length; n++) {
                    var obj = sts[n];

                    if (obj.indexOf("经度:") != -1) {
                        lon = parseFloat(obj.split(':')[1]);
                    }
                    if (obj.indexOf("纬度:") != -1) {
                        lat = parseFloat(obj.split(':')[1]);
                    }
                    if (lat > 0 && lon > 0) {
                        if (webSocket.spn > 20000) { webSocket.spn = 1; }
                        var domid = 'a' + (webSocket.spn++);
                        var strsxxx ="<span class='ters'>"+ location + "<span style='color:green' id=" + domid + "></span></span>";
                        //动态函数
                        var funstr = "webSocket.funs.func" + domid;
                        eval(funstr + " = function( aa){   webSocket.pos." + domid + "=aa;  }");
                        var url = "http://api.map.baidu.com/telematics/v3/reverseGeocoding?location=" + lon + "," + lat + "&coord_type=wgs84&ak=417E3B5574a6d9e9c3830d3ef7b89c73&output=json&callback=" + funstr;
                        jQuery.support.cors = true;
                        $.getScript(url, function () {
                            var addr = aaa(eval("webSocket.pos." + domid));
                            var strmap="<a href=\"javascript:run($(\'#bmap\').val().replace(\'#lat#\', "+lat+").replace(\'#lon#\',"+lon+"))\"><span style='color:red; font-size: 18px; font-weight:900;' >¤</span></a>";
                            $("#" + domid).html(',' + addr + strmap);
                        });
                        return strsxxx;
                    }
                }
            }
            return location;
        }
        function aaa(json) {
             if (json.description != undefined) {
                return json.description;
            }
            return '末知..';
        }
        
        function run(code) {
            var newWin = window.open("", "_blank", "");
            newWin.opener = null; // 防止代码对页面修改
            newWin.document.open();
            newWin.document.write(code);
            newWin.document.close();
        }
        window.onload = webSocket.start;

        

    </script>



</head>
    <body>
        <form id="form1"  runat="server" >项目节点信息:
        
            <!-- onchange="$('#ip').val(this.value.split(':')[0]);$('#port').val(this.value.split(':')[1]);window.ws.close();"-->

            <select id="sel" >
                <asp:Repeater ID="Repeater1" runat="server">
                    <ItemTemplate>
                                        <option value="<%#Eval("val") %>"><%#Eval("key") %></option>
                     </ItemTemplate>
                </asp:Repeater>
             </select>
  </form>

       远程地址: <input id="ip"  value="127.0.0.1"  />远程端口:<input id="port"  value="4780" /> <button onclick=" window.ws.close();">手动重联</button>
        <form id="sendForm">
            输入数据: <input id="sendText" placeholder="Text to send" /><input type="button" onclick ="window.ws.send($('#sel').val()+';'+ $('#sendText').val()); $('#sendText').val('');" value="发送数据" />
        </form><button onclick="window.ws.send($('#sel').val()+';'+ Math.random());">发送随机数据</button>
                <pre id="incomming"></pre>
        <ul id="logs"></ul>
        <pre id="nx"></pre>
        <textarea id="bmap">
            &lt;!DOCTYPE html&gt;
            &lt;html&gt;
            &lt;head&gt;
            &lt;meta http-equiv=&quot;Content-Type&quot; content=&quot;text/html; charset=utf-8&quot; /&gt;
            &lt;meta name=&quot;viewport&quot; content=&quot;initial-scale=1.0, user-scalable=no&quot; /&gt;
            &lt;style type=&quot;text/css&quot;&gt;
            body, html,#allmap {width: 100%;height: 100%;overflow: hidden;margin:0;font-family:&quot;微软雅黑&quot;;}
            &lt;/style&gt;
            &lt;script type=&quot;text/javascript&quot; src=&quot;http://api.map.baidu.com/api?v=2.0&amp;ak=417E3B5574a6d9e9c3830d3ef7b89c73&quot;&gt;&lt;/script&gt;
            &lt;script type=&quot;text/javascript&quot; &gt;
            //2011-7-25
            (function(){        //闭包
            function load_script(xyUrl, callback){
            var head = document.getElementsByTagName('head')[0];
            var script = document.createElement('script');
            script.type = 'text/javascript';
            script.src = xyUrl;
            //借鉴了jQuery的script跨域方法
            script.onload = script.onreadystatechange = function(){
            if((!this.readyState || this.readyState === "loaded" || this.readyState === "complete")){
            callback && callback();
            // Handle memory leak in IE
            script.onload = script.onreadystatechange = null;
            if ( head && script.parentNode ) {
            head.removeChild( script );
            }
            }
            };
            // Use insertBefore instead of appendChild  to circumvent an IE6 bug.
            head.insertBefore( script, head.firstChild );
            }
            function translate(point,type,callback){
            var callbackName = 'cbk_' + Math.round(Math.random() * 10000);    //随机函数名
            var xyUrl = "http://api.map.baidu.com/ag/coord/convert?from="+ type + "&to=4&x=" + point.lng + "&y=" + point.lat + "&callback=BMap.Convertor." + callbackName;
            //动态创建script标签
            load_script(xyUrl);
            BMap.Convertor[callbackName] = function(xyResult){
            delete BMap.Convertor[callbackName];    //调用完需要删除改函数
            var point = new BMap.Point(xyResult.x, xyResult.y);
            callback && callback(point);
            }
            }
            window.BMap = window.BMap || {};
            BMap.Convertor = {};
            BMap.Convertor.translate = translate;
            })();
            &lt;/script&gt;
            &lt;title&gt;GPS转百度&lt;/title&gt;
            &lt;/head&gt;
            &lt;body&gt;
            &lt;div id=&quot;allmap&quot;&gt;&lt;/div&gt;
            &lt;/body&gt;
            &lt;/html&gt;
            &lt;script type=&quot;text/javascript&quot;&gt;
            // 百度地图API功能
            //GPS坐标
            var xx = #lon#;
            var yy = #lat#;
            var gpsPoint = new BMap.Point(xx,yy);
            //地图初始化
            var bm = new BMap.Map(&quot;allmap&quot;);
            bm.centerAndZoom(gpsPoint, 15);
            bm.addControl(new BMap.NavigationControl());
            //添加谷歌marker和label
            var markergps = new BMap.Marker(gpsPoint);
            //坐标转换完之后的回调函数
            translateCallback = function (point){
            var marker = new BMap.Marker(point);
            bm.addOverlay(marker);
            bm.setCenter(point);
            }
            setTimeout(function(){
            BMap.Convertor.translate(gpsPoint,0,translateCallback);     //真实经纬度转成百度坐标
            }, 1000);
            &lt;/script&gt;
        </textarea>
           
    </body>
</html>