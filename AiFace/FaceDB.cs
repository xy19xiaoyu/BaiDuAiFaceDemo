using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AiFace
{
    public partial class FaceDB : Form
    {
        private int id { get; set; }
        public FaceDB()
        {
            InitializeComponent();
            string strid = File.ReadAllText("num.txt");
            if (string.IsNullOrEmpty(strid))
            {
                id = 1;
            }
            else
            {
                id = Int32.Parse(strid);
            }

        }
        private string FileName { get; set; }
        private Bitmap Map { get; set; }
        private void button2_Click(object sender, EventArgs e)
        {
            this.txtID.Text = string.Empty;
            this.txtName.Text = string.Empty;
            this.txtTel.Text = string.Empty;

            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileName = ofd.FileName;

                using (Stream stream = File.Open(ofd.FileName, FileMode.Open))
                {
                    Map = new Bitmap(stream);
                    pictureBox1.Image = Map;
                }
                if (CheckFace())
                {
                    button1_Click(null, null);
                }


            }
        }
        private bool CheckFace()
        {
            JObject result = FaceHelper.Detect(FileName);
            if (result.Value<int>("result_num") > 0)
            {
                foreach (var tmpResult in result["result"])
                {
                    var location = tmpResult["location"];
                    using (Graphics g = Graphics.FromImage(Map))
                    {
                        g.DrawRectangle(new Pen(Brushes.Blue, 4), location.Value<int>("left"), location.Value<int>("top"), location.Value<int>("width"), location.Value<int>("height"));
                    }
                }
                pictureBox1.Image = Map;
                return true;
            }
            else
            {
                MessageBox.Show("没有发现面部特征");
                return false;
            }

        }

        private void btnNewFace_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(this.txtName.Text.Trim()))
            {
                txtName.Focus();
                return;
            }

            if (string.IsNullOrEmpty(this.txtTel.Text.Trim()))
            {
                txtTel.Focus();
                return;
            }

            string userinfo = this.txtName.Text + "," + this.txtTel.Text;
            FaceHelper.AddUserFace(this.txtID.Text, userinfo, "JXGF", this.FileName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UserInfo user = FaceHelper.IdentifyDemo(this.FileName);
            if (user != null)
            {
                this.txtID.Text = user.id.ToString();
                this.txtName.Text = user.name;
                this.txtTel.Text = user.tel;
                this.btnNewFace.Visible = false;
            }
            else
            {
                MessageBox.Show("没找到用户，请创建");
                id = id + 1;
                File.WriteAllText("num.txt", id.ToString());
                this.txtID.Text = id.ToString();
                this.btnNewFace.Visible = true;
            }

        }
    }
}
