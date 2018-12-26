using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;

namespace ClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var urlx = new Uri("http://192.168.1.78:5600/configapi/getcontent?token={0}");

            var url = new UriBuilder() {
             Host = urlx.Host, Port = urlx.Port,  Scheme = urlx.Scheme};                        




            WebSocket4Net.WebSocket ws = new WebSocket("ws://175.19.140.4:4618","", WebSocketVersion.Rfc6455);
            ws.Open();
            ws.MessageReceived += (s, e) => {
                Console.WriteLine(e.Message);
            };
            while (true) {
                ws.Send(   Console.ReadLine());
            }


        }
    }
}
