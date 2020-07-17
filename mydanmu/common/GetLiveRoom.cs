//=============================================================
// Copyright (C) 2018-2018
// CLR版本:          4.0.30319.42000
// 机器名称:          DESKTOP-CSLKP88
// 命名空间名称/文件名:    mydanmu.common/GetLiveRoom 
// 创建人:               knva   
// 创建时间:            2018/7/17 16:22:46
//==============================================================
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

using WebSocketSharp;
using zlib;

namespace mydanmu.common
{
    public class GetLiveRoom
    {
        NetWorkAPI netApi;
        WebSocket ws;
        int roomid;
        string serverhost;
        int serverport;
        string roomName;
        Timer t;
        bool linkStats;

        ShowTextCallBack callback;
        public GetLiveRoom(string roomid, ShowTextCallBack callback)
        {
            netApi = new NetWorkAPI();
            this.roomid = Convert.ToInt32(roomid);
            this.callback = callback;
            linkStats = false;

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomid"></param>
        /// 
        /// <returns>
        /// string[0]:roomname
        /// string[1]:level
        /// </returns>
        private async Task GetRoomInfo()
        {

            //Hashtable hs = new Hashtable();
            //hs.Add("id", "cid:" + roomid);
            string info = await netApi.CreateGetHttpResponse("https://api.live.bilibili.com/xlive/web-room/v1/index/getInfoByRoom?room_id=" + roomid, 100, null);

            var jo = JObject.Parse(info);
            JObject jo1 = (JObject)jo["data"]["room_info"];
            roomName = jo1["title"].ToString();
            roomid = Convert.ToInt32(jo1["room_id"].ToString());

            var html = await netApi.CreateGetHttpResponse("https://api.live.bilibili.com/xlive/web-room/v1/index/getDanmuInfo?id="+ roomid + "&type=0", 100, null);
       

             var josnd = JObject.Parse(html);
            JObject jo2 = (JObject)josnd["data"]["host_list"][0];
            serverhost = jo2["host"].ToString();
            serverport = Convert.ToInt32(jo2["wss_port"].ToString());

          


        }


        private void GetDmServer(XmlDocument doc)
        {
            XmlNode x_list = doc.SelectSingleNode("root/dm_host_list");
            XmlNode x_port = doc.SelectSingleNode("root/dm_wss_port");
            string str_list = x_list.InnerText;
            string[] serlist = str_list.Split(',');
            string port = x_port.InnerText;
            serverhost = serlist[0];
            serverport = Convert.ToInt32(port);

        }
        private XmlDocument LoadStr(string xml)
        {
            xml = "<root>" + xml + "</root>";
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }
        public void LeaveLiveRoomAsync()
        {
            callback("离开房间"+Environment.NewLine);
            linkStats = false;
            t.Dispose();
            ws.CloseAsync(CloseStatusCode.Normal);
        }
        public async Task JoinLiveRoomAsync()
        {
            await GetRoomInfo();
            ws = new WebSocket("wss://" + this.serverhost + ":" + this.serverport + "/sub");
            //ws.SetProxy("http://127.0.0.1:8888", "", "");
            ws.OnMessage += (sender, e) =>
            {
                if (!linkStats)
                {
                    Console.WriteLine("closed");
                    return;
                }
                if (e.IsText)
                {
                    // Do something with e.Data.
                    Console.WriteLine(e.Data);

                    return;
                }

                if (e.IsBinary)
                {
                    // Do something with e.RawData.
                    // Console.WriteLine(e.RawData);

                    var data = e.RawData;

                    JObject[] decodejsons = decodePackage(data);
                    string sstr = "";
                    foreach (JObject decodejson in decodejsons)
                    {
                        if (decodejson["cmd"].ToString() == "DANMU_MSG")
                        {
                            JArray ja = (JArray)decodejson["info"];
                           
                            sstr += ja[2][1].ToString() + "说了:" + ja[1].ToString()+Environment.NewLine;
                        }
                        else if (decodejson["cmd"].ToString() == "open")
                        {

                            sstr += "打开房间" + roomid + Environment.NewLine + "房间名:" + roomName + Environment.NewLine;

                        }
                    }
                    callback(sstr);
                    return;
                }
            };
            ws.OnOpen += (sender, e) =>
            {
                linkStats = true;
                t = new Timer(p => acSocket(), ws, 500, 30000);

            };
            ws.OnClose += (sender, e) =>
            {
                linkStats = false;
            };
            ws.Connect();
            JObject jo = new JObject();
            jo["uid"] = 0;
            jo["roomid"] = Convert.ToInt32(roomid);
            jo["protover"] = 1;
            jo["platform"] = "web";
            jo["clientver"] = "1.14.3";
            jo["type"] = "1";
            string hellopackage = jo.ToString().Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "");
            acSocket(7, hellopackage);

        }


        private JObject[] decodePackage(byte[] data)
        {

            List<JObject> decodejos = new List<JObject>();
            Console.WriteLine("datalen:"+data.Length);
            for (int doffset = 0; doffset < data.Length;)
            {
                Dictionary<string, int> decode = decodeHeader(data, doffset);
                byte[] rcvdata = new byte[decode["Len"]];
                Buffer.BlockCopy(data, doffset, rcvdata, 0, decode["Len"]);
                JObject jo = decodeJson(rcvdata, decode);
                decodejos.Add(jo);
                doffset += decode["Len"];

                Console.WriteLine("doffset:" + doffset);
            }

            return decodejos.ToArray();
        }
        private Dictionary<string, int> decodeHeader(byte[] data, int offset)
        {
            Dictionary<string, int> decode = new Dictionary<string, int>();
            //先找到包长度,版本号,头长度,操作号

            decode["Len"] = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, offset));
            offset += 4;
            decode["hLen"] = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, offset));
            offset += 2;
            decode["wz"] = IPAddress.NetworkToHostOrder(BitConverter.ToInt16(data, offset));
            offset += 2;
            decode["control"] = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, offset));
            offset += 4;
            decode["type"] = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, offset));
            offset += 4;
            decode["xdata"] = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(data, offset));
            //Console.WriteLine(decode["Len"]);
            //Console.WriteLine(decode["hLen"]);
            //Console.WriteLine(decode["wz"]);
            //Console.WriteLine(decode["control"]);
            return decode;

        }
        /// <summary>
        /// 复制流
        /// </summary>
        /// <param name="input">原始流</param>
        /// <param name="output">目标流</param>
        public static void CopyStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[2000];
            int len;
            while ((len = input.Read(buffer, 0, 2000)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }
        /// <summary>
        /// 解压缩流
        /// </summary>
        /// <param name="sourceStream">需要被解压缩的流</param>
        /// <returns>解压后的流</returns>
        private static Stream deCompressStream(Stream sourceStream)
        {
            MemoryStream outStream = new MemoryStream();
            ZOutputStream outZStream = new ZOutputStream(outStream);
            CopyStream(sourceStream, outZStream);
            outZStream.finish();
            return outStream;
        }
        /// <summary>
        /// 解压缩字节数组
        /// </summary>
        /// <param name="sourceByte">需要被解压缩的字节数组</param>
        /// <returns>解压后的字节数组</returns>
        private static byte[] deCompressBytes(byte[] sourceByte)
        {
            MemoryStream inputStream = new MemoryStream(sourceByte);
            Stream outputStream = deCompressStream(inputStream);
            byte[] outputBytes = new byte[outputStream.Length];
            outputStream.Position = 0;
            outputStream.Read(outputBytes, 0, outputBytes.Length);
            outputStream.Close();
            inputStream.Close();
            return outputBytes;
        }

        private JObject decodeJson(byte[] data, Dictionary<string, int> decode)
        {
            JObject decodejo = new JObject();

            if (decode["control"] == 5)
            {

                int datalen = decode["Len"] - decode["hLen"];
                byte[] rcvdata = new byte[datalen];
                Buffer.BlockCopy(data, 16, rcvdata, 0, datalen);

                var xdata = deCompressBytes(rcvdata);
                byte[] decodedata = new byte[xdata.Length - 16];
                Buffer.BlockCopy(xdata, 16, decodedata, 0, xdata.Length-16);

                string str = Encoding.UTF8.GetString(decodedata);
                decodejo = JObject.Parse(str);
            }
            else if (decode["control"] == 8)
            {
                decodejo["cmd"] = "open";
            }
            else if (decode["control"] == 3)
            {
                decodejo["cmd"] = "heart";
            }
            return decodejo;
        }
        private void acSocket(int type = 2, string load = "[object Object]")
        {
            byte[] info = Encoding.Default.GetBytes(load);

            byte[] package = new byte[info.Length + 16];
            int i = 0;
            byte[] packagelength = BitConverter.GetBytes(package.Length);
            Array.Reverse(packagelength);
            Buffer.BlockCopy(packagelength, 0, package, i, packagelength.Length);
            i = i + 4;
            byte[] magicBuf = { 0x00, 0x10 };

            Buffer.BlockCopy(magicBuf, 0, package, i, magicBuf.Length);
            i = i + 2;
            byte[] versionBuf = { 0x00, 0x01 };
            Buffer.BlockCopy(versionBuf, 0, package, i, versionBuf.Length);
            i = i + 2;

            byte[] joinControl = BitConverter.GetBytes(type);
            Array.Reverse(joinControl);
            Buffer.BlockCopy(joinControl, 0, package, i, joinControl.Length);
            i = i + 4;

            byte[] typeControl = { 0x00, 0x00, 0x00, 0x01 };
            Buffer.BlockCopy(typeControl, 0, package, i, typeControl.Length);
            i = i + 4;
            Buffer.BlockCopy(info, 0, package, i, info.Length);
            ws.Send(package);
        }
    }
}
