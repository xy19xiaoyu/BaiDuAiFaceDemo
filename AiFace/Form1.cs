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
    public partial class Form1 : Form
    {
        private Bitmap Map { get; set; }
        private Bitmap Map2 { get; set; }
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            JObject result = FaceHelper.Detect(this.textBox1.Text.Trim());
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
            }
            else
            {
                MessageBox.Show("没有");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = ofd.FileName;

                using (Stream stream = File.Open(ofd.FileName, FileMode.Open))
                {
                    Map = new Bitmap(stream);
                    pictureBox1.Image = Map;
                }

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int score = FaceHelper.Match(textBox1.Text, textBox2.Text);
            MessageBox.Show("相似分：" + score);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.textBox2.Text = ofd.FileName;

                using (Stream stream = File.Open(ofd.FileName, FileMode.Open))
                {
                    Map2 = new Bitmap(stream);
                    pictureBox2.Image = Map2;
                }

            }

        }

        private void button5_Click(object sender, EventArgs e)
        {

            JObject result = FaceHelper.Detect(this.textBox2.Text.Trim());
            if (result.Value<int>("result_num") > 0)
            {
                foreach (var tmpResult in result["result"])
                {
                    var location = tmpResult["location"];
                    using (Graphics g = Graphics.FromImage(Map2))
                    {
                        g.DrawRectangle(new Pen(Brushes.Blue, 4), location.Value<int>("left"), location.Value<int>("top"), location.Value<int>("width"), location.Value<int>("height"));
                    }
                }
                pictureBox2.Image = Map2;
            }
            else
            {
                MessageBox.Show("没有");
            }
        }
    }
}
