namespace mydanmu
{
    partial class danmu
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_getRoom = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_roomid = new System.Windows.Forms.TextBox();
            this.textBox_showdanmu = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btn_getRoom
            // 
            this.btn_getRoom.Location = new System.Drawing.Point(193, 4);
            this.btn_getRoom.Name = "btn_getRoom";
            this.btn_getRoom.Size = new System.Drawing.Size(74, 23);
            this.btn_getRoom.TabIndex = 0;
            this.btn_getRoom.Text = "连接";
            this.btn_getRoom.UseVisualStyleBackColor = true;
            this.btn_getRoom.Click += new System.EventHandler(this.btn_getRoom__ClickAsync);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "房间号:";
            // 
            // textBox_roomid
            // 
            this.textBox_roomid.Location = new System.Drawing.Point(65, 6);
            this.textBox_roomid.Name = "textBox_roomid";
            this.textBox_roomid.Size = new System.Drawing.Size(100, 21);
            this.textBox_roomid.TabIndex = 2;
            this.textBox_roomid.Text = "249500";
            // 
            // textBox_showdanmu
            // 
            this.textBox_showdanmu.Location = new System.Drawing.Point(12, 33);
            this.textBox_showdanmu.Multiline = true;
            this.textBox_showdanmu.Name = "textBox_showdanmu";
            this.textBox_showdanmu.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox_showdanmu.Size = new System.Drawing.Size(364, 220);
            this.textBox_showdanmu.TabIndex = 3;
            // 
            // danmu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 263);
            this.Controls.Add(this.textBox_showdanmu);
            this.Controls.Add(this.textBox_roomid);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_getRoom);
            this.Name = "danmu";
            this.Text = "弹幕姬";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_getRoom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_roomid;
        private System.Windows.Forms.TextBox textBox_showdanmu;
    }
}

