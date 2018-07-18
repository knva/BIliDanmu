using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mydanmu.common;
namespace mydanmu
{
    public delegate void ShowTextCallBack(string text);
    public partial class danmu : Form
    {
        GetLiveRoom glr;
        bool liveroom;
        public danmu()
        {
            InitializeComponent();
            liveroom = false;
        }

        
        //然后怎么写

        private async void btn_getRoom__ClickAsync(object sender, EventArgs e)
        {
            if (!liveroom)
            {
                glr = new GetLiveRoom(textBox_roomid.Text,showText);
                await glr.JoinLiveRoomAsync();
                btn_getRoom.Text = "离开";
                liveroom = true;
            }
            else
            {
                glr.LeaveLiveRoomAsync();
                btn_getRoom.Text = "连接";
                liveroom = false;
            }

        }

        public  void showText(string text) {
            if (this.textBox_showdanmu.InvokeRequired)
            {
                ShowTextCallBack stcb = new ShowTextCallBack(showText);
                this.Invoke(stcb, new object[] { text });
            }
            else
            {
                this.textBox_showdanmu.Text += text;
                textBox_showdanmu.SelectionStart = textBox_showdanmu.Text.Length; //设定光标位置
                textBox_showdanmu.ScrollToCaret();
            }
        }
 
    }
}
