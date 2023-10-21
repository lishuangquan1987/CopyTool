using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopyTool
{
    public partial class Form1 : Form
    {
        Random random = new Random();
        List<int> hotIds;
        List<Keys> hotKeys = new List<Keys>() { Keys.F1, Keys.F2, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.F10 };
        List<RichTextBox> rtbs;
        public Form1()
        {
            InitializeComponent();
            var number = random.Next(100_00_0, 100_00_00);
            hotIds = Enumerable.Range(number, 10).ToList();
            rtbs = new List<RichTextBox>() { this.richTextBox1, this.richTextBox2, this.richTextBox3, this.richTextBox4, this.richTextBox5, this.richTextBox6, this.richTextBox7, this.richTextBox8, this.richTextBox9, this.richTextBox10, };
            btnStop.Enabled = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            List<Keys> errRegKeys = new List<Keys>();
            for (int i = 0; i < hotKeys.Count; i++)
            {
                bool result = SystemHotKey.RegisterHotKey(this.Handle, hotIds[i], SystemHotKey.KeyModifiers.None, hotKeys[i]);
                if (!result)
                {
                    errRegKeys.Add(hotKeys[i]);
                }
            }
            if (errRegKeys.Count > 0)
            {
                MessageBox.Show($"如下热键注册失败：\n{string.Join(",", errRegKeys)}");
            }
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x312)
            {
                var hotId=m.WParam.ToInt32();
                var index=hotIds.IndexOf(hotId);
                if (index == -1) return;

                try
                {
                    if (string.IsNullOrEmpty(this.rtbs[index].Text)) return;
                    //先复制内容，再粘贴
                    Clipboard.SetText(this.rtbs[index].Text);
                    SendKeys.Send("^{V}");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            List<Keys> errRegKeys = new List<Keys>();
            for (int i = 0; i < hotKeys.Count; i++)
            {
                bool result = SystemHotKey.UnregisterHotKey(this.Handle, hotIds[i]);
                if (!result)
                {
                    errRegKeys.Add(hotKeys[i]);
                }
            }
            if (errRegKeys.Count > 0)
            {
                MessageBox.Show($"如下热键取消注册失败：\n{string.Join(",", errRegKeys)}");
            }
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }
    }
    public class SystemHotKey
    {
        /// <summary>
        /// 如果函数执行成功，返回值不为0。
        /// 如果函数执行失败，返回值为0。要得到扩展错误信息，调用GetLastError。
        /// </summary>
        /// <param name="hWnd">要定义热键的窗口的句柄</param>
        /// <param name="id">定义热键ID（不能与其它ID重复）</param>
        /// <param name="fsModifiers">标识热键是否在按Alt、Ctrl、Shift、Windows等键时才会生效</param>
        /// <param name="vk">定义热键的内容</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, KeyModifiers fsModifiers, Keys vk);

        /// <summary>
        /// 注销热键
        /// </summary>
        /// <param name="hWnd">要取消热键的窗口的句柄</param>
        /// <param name="id">要取消热键的ID</param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        /// <summary>
        /// 辅助键名称。
        /// Alt, Ctrl, Shift, WindowsKey
        /// </summary>
        [Flags()]
        public enum KeyModifiers { None = 0, Alt = 1, Ctrl = 2, Shift = 4, WindowsKey = 8 }

        /// <summary>
        /// 注册热键
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="hotKey_id">热键ID</param>
        /// <param name="keyModifiers">组合键</param>
        /// <param name="key">热键</param>
        public static void RegHotKey(IntPtr hwnd, int hotKeyId, KeyModifiers keyModifiers, Keys key)
        {
            if (!RegisterHotKey(hwnd, hotKeyId, keyModifiers, key))
            {
                int errorCode = Marshal.GetLastWin32Error();
                if (errorCode == 1409)
                {
                    MessageBox.Show("热键被占用 ！");
                }
                else
                {
                    MessageBox.Show("注册热键失败！错误代码：" + errorCode);
                }
            }
        }

        /// <summary>
        /// 注销热键
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="hotKey_id">热键ID</param>
        public static void UnRegHotKey(IntPtr hwnd, int hotKeyId)
        {
            //注销指定的热键
            UnregisterHotKey(hwnd, hotKeyId);
        }

    }
}
