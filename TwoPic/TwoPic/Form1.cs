using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
/*
      BOOL RegisterHotKey(HWND hWnd,int id,UINT fsModifiers,UINT vk);
      BOOL UnregisterHotKey(HWND hWnd,int id);　
*/
namespace TwoPic
{
    public partial class Form1 : Form
    {
        //注册和卸载全局快捷键
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [Flags()]
        public enum KeyModifiers
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            Windows = 8
        }
        public static Form1 frm;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            frm = this;
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            // m.WParam.ToInt32() 要和 注册热键时的第2个参数一样
            if (m.Msg == WM_HOTKEY &&( m.WParam.ToInt32() == 100|| m.WParam.ToInt32() == 101)) //判断热键
            {
                if(listView1.SelectedItems.Count>0)
                {
                    if (listView1.SelectedItems[0].Index >0)
                    {
                        int j = listView1.SelectedItems[0].Index;
                        foreach (ListViewItem i in listView1.SelectedItems)i.Selected = false;
                        listView1.Items[j - 1].Selected = true;
                    }
                }
            }
            if (m.Msg == WM_HOTKEY &&( m.WParam.ToInt32() == 102|| m.WParam.ToInt32() == 103)) //判断热键
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    if (listView1.SelectedItems[0].Index < listView1.Items.Count - 1)
                    {
                        int j = listView1.SelectedItems[0].Index;
                        foreach (ListViewItem i in listView1.SelectedItems) i.Selected = false;
                        listView1.Items[j + 1].Selected = true;
                    }
                }
            }
            base.WndProc(ref m);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            RegisterHotKey(Handle, 100, 0, Keys.Up);
            RegisterHotKey(Handle, 101, 0, Keys.Left);
            RegisterHotKey(Handle, 102, 0, Keys.Down);
            RegisterHotKey(Handle, 103, 0, Keys.Right);
            Form1.frm.saveFileDialog1.RestoreDirectory = true;
            //下右是下一张
            //左上是上一张
            for (int i = 0; i < update.obj.Count(); update.obj[i] = new object(), i++) ;
            if (Directory.Exists(bl.flie + "pic") == false) Directory.CreateDirectory(bl.flie + "pic");

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            UnregisterHotKey(Handle, 100);
            UnregisterHotKey(Handle, 101);
            UnregisterHotKey(Handle, 102);
            UnregisterHotKey(Handle, 103);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) return;
            try
            {
                bl.oTime.Restart();
                bl.K = -1;
                toolStripLabel2.Text = "当前阈值:" + bl.K.ToString();
                update.ToGrey(0);
                pictureBox1.Image = bl.img[0];
                bl.oTime.Stop();
                toolStripLabel1.Text = bl.oTime.Elapsed.TotalMilliseconds.ToString() + "毫秒";
            }
            finally { }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (listView1.SelectedItems.Count <= 0) return;
                bl.oTime.Restart();
                update.ToGrey(0);
                if (bl.K == -1)
                {
                    try
                    {
                        bl.K = update.Value1(0);
                        toolStripLabel2.Text = "当前阈值:" + bl.K.ToString();
                    }
                    finally { }
                }
                update.Thresholding(bl.K, 0);
                pictureBox1.Image = bl.img[0];
                bl.oTime.Stop();
                toolStripLabel1.Text = bl.oTime.Elapsed.TotalMilliseconds.ToString() + "毫秒";
            }
            finally { }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) return;
            try
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    bl.img[0].Save(saveFileDialog1.FileName, bl.img[0].RawFormat);
                }
            }
            catch (ArgumentNullException a)
            {
                MessageBox.Show(a.Message);
            }
            catch (ExternalException a)
            {
                MessageBox.Show(a.Message);
            }
            finally { }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) return;
            try
            {
                bl.oTime.Restart();
                update.NegativeImage(0);
                pictureBox1.Image = bl.img[0];
                bl.oTime.Stop();
                toolStripLabel1.Text = bl.oTime.Elapsed.TotalMilliseconds.ToString() + "毫秒";
            }
            finally { }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    bl.XC = int.Parse(textBox1.Text) - 1;
                }
                catch(Exception)
                {
                    bl.XC = 2;
                    textBox1.Text = "2";
                }
                if (bl.XC > 10)
                {
                    bl.XC = 10; textBox1.Text = "10";
                }
                pictureBox1.Image = null;
                bl.oTime.Restart();
                bl.LS = 0;
                update.Pset(listView1.Items.Count);
                update.count = listView1.Items.Count - 1;
                bl.BK = 0;
                for (int i = 0; i <= bl.XC; i++) bl.threads[i] = new Thread(new ThreadStart(update.pic));
                for (int i = 0; i <= bl.XC; bl.threads[i].Start(), i++) ;
            }
            finally { }
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) return;
            try
            {
                bl.oTime.Restart();
                update.RotateImage(0);
                pictureBox1.Image = bl.img[0];
                bl.oTime.Stop();
                toolStripLabel1.Text = bl.oTime.Elapsed.TotalMilliseconds.ToString() + "毫秒";
            }
            finally { }
        }
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < listView1.SelectedItems.Count; i++)
                {
                    bl.ImageP.RemoveAt(listView1.SelectedItems[i].Index);
                    bl.ImageN.RemoveAt(listView1.SelectedItems[i].Index);
                    listView1.Items.RemoveAt(listView1.SelectedItems[i].Index);
                }
            }
            catch (ArgumentOutOfRangeException a)
            {
                MessageBox.Show(a.Message);
            }
        }
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            bl.ImageP.Clear();
            bl.ImageN.Clear();
            listView1.Items.Clear();
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                openFileDialog1.RestoreDirectory = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    using (MemoryStream mem = new MemoryStream())
                    {
                        foreach (string bj in bl.ImageP) if (openFileDialog1.FileName == bj) goto step2;//判断重复
                        bl.ImageP.Add(openFileDialog1.FileName);
                        bl.ImageN.Add(openFileDialog1.SafeFileName);
                        imageList1.Images.Add(Image.FromFile(openFileDialog1.FileName));
                        ListViewItem Litem = new ListViewItem();
                        Litem.ImageIndex = imageList1.Images.Count - 1;
                        listView1.Items.Add(Litem);
                    step2:;
                    }
                }
            }
            catch (ArgumentException a)
            {
                MessageBox.Show(a.Message);
            }
            catch (FileNotFoundException a)
            {
                MessageBox.Show(a.Message);
            }
            catch (OutOfMemoryException a)
            {
                MessageBox.Show(a.Message);
            }
        }
        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            for (int i = 0; i < ((Array)e.Data.GetData(DataFormats.FileDrop)).Length; i++)
            {
                string sPath = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(i).ToString();
                string sName = Path.GetFileName(sPath);
                foreach(string bj in bl.ImageP)if (sPath == bj) goto step1;//判断重复
                bl.ImageP.Add(sPath);
                bl.ImageN.Add(sName);
                imageList1.Images.Add(Image.FromFile(sPath));
                ListViewItem Litem = new ListViewItem();
                Litem.ImageIndex = imageList1.Images.Count - 1;
                listView1.Items.Add(Litem);
                step1:;
            }
        }
        private void listView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }
        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;
            contextMenuStrip1.Show(MousePosition);
        }
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count <= 0) return;
            try
            {
                bl.K = -1;
                toolStripLabel2.Text = "当前阈值:" + bl.K.ToString();
                Image Img = Image.FromFile(bl.ImageP[listView1.SelectedItems[0].Index]);
                pictureBox1.Image = Img;
                bl.img[0]=(Bitmap)Img;
                try
                {
                    bl.img[0].SetPixel(1, 1, bl.img[0].GetPixel(1, 1));
                }
                catch (Exception)
                {
                    bl.img[0] = bl.img[0].Clone(new Rectangle(0, 0, bl.img[0].Width, bl.img[0].Height), PixelFormat.Format32bppRgb);
                }
            }
            finally { }
        }
        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (textBox1.ForeColor != Color.Black)
            {
                textBox1.ForeColor = Color.Black;
                textBox1.Text = "";
            }
        }
    }
    class update
    {
        /// <summary>
        /// 多线程
        /// </summary>
        public static object[] obj = new object[2];
        public static  int count;
        public static void pic()
        {
            int BS = ++bl.BK;
            while (count >= 0)
            {
                int O, K;
                lock (obj[0]) O = count--;
                string fileName = bl.flie + "pic\\" + bl.ImageN[Form1.frm.listView1.Items[O].Index];
                Image Img = Image.FromFile(bl.ImageP[Form1.frm.listView1.Items[O].Index]);
                bl.img[BS] = (Bitmap)Img;
                try
                {
                    bl.img[BS].SetPixel(1, 1, bl.img[BS].GetPixel(1, 1));
                }
                catch (Exception)
                {
                    bl.img[BS] = bl.img[BS].Clone(new Rectangle(0, 0, bl.img[BS].Width, bl.img[BS].Height), PixelFormat.Format32bppRgb);
                }
                ToGrey(BS);
                K = Value1(BS);
                Thresholding(K, BS);
                bl.img[BS].Save(fileName, bl.img[BS].RawFormat);
                Padd();
                Form1.frm.toolStripLabel2.Text = "当前剩余:" + Form1.frm.toolStripProgressBar1.Value.ToString() + "/" + Form1.frm.toolStripProgressBar1.Maximum.ToString();
            }
            bl.LS++;
            lock (obj[1])
            {
                if (bl.LS == bl.BK && bl.oTime.IsRunning)
                {
                    bl.oTime.Stop();
                    Form1.frm.toolStripLabel1.Text = bl.oTime.Elapsed.TotalMilliseconds.ToString() + "毫秒";
                    MessageBox.Show("已保存到当前目录的pic/下");
                }
            }
        }
        /// <summary>
        /// 重设图像大小
        /// </summary>
        public static void ResizeImage(int number, Size size)
        {
            Bitmap newbmp = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(newbmp))
            {
                g.DrawImage(bl.img[number], new Rectangle(Point.Empty, size));
            }
            return ;
        }
        /// <summary>
        /// 图像反色
        /// </summary>
        public static void NegativeImage(int number)
        {
            Color pixel;
            for (int x = 1; x < bl.img[number].Width; x++)
            {
                for (int y = 1; y < bl.img[number].Height; y++)
                {
                    int r, g, b;
                    pixel = bl.img[number].GetPixel(x, y);
                    r = 255 - pixel.R;
                    g = 255 - pixel.G;
                    b = 255 - pixel.B;
                    bl.img[number].SetPixel(x, y, Color.FromArgb(r, g, b));
                }
            }
            return;
        }
        /// <summary>
        /// 图像旋转
        /// </summary>
        public static void RotateImage(int number)
        {
            Bitmap newbmp = new Bitmap(bl.img[number].Height, bl.img[number].Width);
            using (Graphics g = Graphics.FromImage(newbmp))
            {
                Point[] destinationPoints = {
                    new Point(bl.img[number].Height, 0), // destination for upper-left point of original
                    new Point(bl.img[number].Height, bl.img[number].Width),// destination for upper-right point of original
                    new Point(0, 0)}; // destination for lower-left point of original
                g.DrawImage(bl.img[number], destinationPoints);
            }
            bl.img[number] = newbmp;
            return ;
        }
        /// <summary>
        /// 图像灰度化
        /// </summary>
        public static void ToGrey(int number)
        {
        try
        {
            Color pixelColor;
            for (int i = 0, grey; i < bl.img[number].Width; i++)
            {
                for (int j = 0; j < bl.img[number].Height; j++)
                {
                    pixelColor = bl.img[number].GetPixel(i, j);
                    //计算灰度值
                    grey = (int)(0.299 * pixelColor.R + 0.587 * pixelColor.G + 0.114 * pixelColor.B);
                    bl.img[number].SetPixel(i, j, Color.FromArgb(grey, grey, grey));
                }
            }
        }
        catch (ArgumentOutOfRangeException a)
        {
            MessageBox.Show(a.Message);
        }
        catch (ArgumentException a)
        {
            MessageBox.Show(a.Message);
        }
        catch (Exception a)
        {
            MessageBox.Show(a.Message);
        }
    }
        /// <summary>
        /// 图像二值化
        /// </summary>
        /// <param name="p">阈值</param>
        /// <returns></returns>
        public static void Thresholding(int p, int number)
        {
            try
            {
                //计算二值化
                Color pixelColor;
                for (int i = 0; i < bl.img[number].Width; i++)
                {
                    for (int j = 0; j < bl.img[number].Height; j++)
                    {
                        pixelColor = bl.img[number].GetPixel(i, j);
                        if (pixelColor.R > p) bl.img[number].SetPixel(i, j, Color.FromArgb(255, 255, 255));
                        else bl.img[number].SetPixel(i, j, Color.FromArgb(0, 0, 0));
                    }
                }
            }
            catch (ArgumentException a)
            {
                MessageBox.Show(a.Message);
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
            }
        }
        /// <summary>
        /// 计算阈值方法1(迭代计算阀值)
        /// </summary>
        /// 迭代法 直方图分割阈值
        public static unsafe int Value1(int number)
        {
            try
            {
                int[] histogram = new int[256];
                int minGrayValue = 255, maxGrayValue = 0;
                Color pixelColor;
                int threshold = -1;
                //求取颜色直方图
                fixed (int* k = &histogram[0])
                {

                    for (int i = 0; i < bl.img[number].Width; i++)
                    {
                        for (int j = 0; j < bl.img[number].Height; j++)
                        {
                            pixelColor = bl.img[number].GetPixel(i, j);
                            (*(k + pixelColor.R))++;
                            if (pixelColor.R > maxGrayValue) maxGrayValue = pixelColor.R;
                            if (pixelColor.R < minGrayValue) minGrayValue = pixelColor.R;
                        }
                    }
                    int newThreshold = (minGrayValue + maxGrayValue) / 2;
                    for (int iterationTimes = 0; threshold != newThreshold && iterationTimes < 100; iterationTimes++)
                    {
                        threshold = newThreshold;
                        int lP1 = 0, lP2 = 0, lS1 = 0, lS2 = 0;
                        //求两个区域的灰度的平均值
                        int* i = k + minGrayValue;
                        int js = minGrayValue;
                        while (i < k + threshold)
                        {
                            lP1 += (*i) * js++;
                            lS1 += *i++;
                        }
                        i = k + maxGrayValue;
                        while (i > k + threshold)
                        {
                            lP2 += (*i) * js++;
                            lS2 += *i--;
                        }
                        //原来
                        /*for (int i = minGrayValue; i < threshold; i++)
                        {
                            lP1 += histogram[i]* i;
                            lS1 += histogram[i];
                        }
                        for (int i = threshold + 1; i < maxGrayValue; i++)
                        {
                            lP2 += histogram[i] * i;
                            lS2 += histogram[i];
                        }*/
                        newThreshold = (lP1 / lS1 + lP2 / lS2) / 2;
                    }
                }
                return threshold;
            }
            catch (ArgumentOutOfRangeException a)
            {
                MessageBox.Show(a.Message);
                return -1;
            }
            catch (Exception a)
            {
                MessageBox.Show(a.Message);
                return -1;
            }
        }
        /// <summary>
        /// 增加进度条值
        /// </summary>
        public static void Padd()
        {
            Form1.frm.toolStripProgressBar1.Value ++;
            return;
        }
        /// <summary>
        /// 设置进度条最大值
        /// </summary>
        public static void Pset(int z)
        {
            Form1.frm.toolStripProgressBar1.Maximum = z;
            Form1.frm.toolStripProgressBar1.Value = 0;
            return;
        }
    }
    public class bl
    {
        /// <summary>
        /// 储存图片地址
        /// </summary>
        public static List<string> ImageP = new List<string>();
        /// <summary>
        /// 储存图片名称
        /// </summary>
        public static List<string> ImageN = new List<string>();
        /// <summary>
        /// 当前选定图片
        /// </summary>
        public static Bitmap[] img=new Bitmap[11];
        /// <summary>
        /// 开启到几个线程
        /// </summary>
        public static int BK;
        /// <summary>
        /// 线程
        /// </summary>
        public static Thread[] threads = new Thread[10];
        /// <summary>
        /// 关闭到几个线程
        /// </summary>
        public static int LS;
        /// <summary>
        /// 非列表时的阈值
        /// </summary>
        public static int K;
        /// <summary>
        /// 计时
        /// </summary>
        public static Stopwatch oTime = new Stopwatch();
        /// <summary>
        /// 当前目录
        /// </summary>
        public static string flie = AppDomain.CurrentDomain.BaseDirectory;
        public static int XC;
    }
}
