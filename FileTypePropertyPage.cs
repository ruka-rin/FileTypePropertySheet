using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SharpShell.SharpPropertySheet;

namespace FileTypePropertySheet
{
    public partial class FileTypePropertyPage : SharpPropertyPage
    {

        static bool IsInPATH(string command, out string path)
        {
            path = string.Empty;
            var PATH = Environment.GetEnvironmentVariable("PATH");

            if (PATH == null)
            {
                return false;
            }

            foreach (string pathItem in PATH.Split(';'))
            {
                path = pathItem.Trim();
                if (!string.IsNullOrEmpty(path))
                {
                    if (File.Exists(Path.Combine(path, command)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public FileTypePropertyPage()
        {
            InitializeComponent();
            //Font = SystemFonts.DefaultFont;
            //Font = new Font("更纱黑体 UI", 10);
            Font = SystemInformation.MenuFont;
        }

        private void Panel1_SizeChanged(object sender, EventArgs e)
        {
            listView1.Size = panel1.Size - new Size(30, 30);
            listView1.Location = new Point(15, 15);
        }

        protected override void OnPropertyPageInitialised(SharpPropertySheet parent)
        {

            if (IsInPATH("trid.exe", out string path))
            {
                if (File.Exists(Path.Combine(path, "triddefs.trd")))
                {
                    var filePath = parent.SelectedItemPaths.First();

                    var tws = new ThreadWithState(filePath, listView1);
                    listView1.BeginInvoke((Action)tws.ThreadProc);
                    //var thread = new Thread(tws.ThreadProc);
                    //thread.Start();
                    下载toolStripMenuItem.Visible = false;
                }
            }
        }

        private static StringBuilder TrimEndNewLine(StringBuilder strBuilder)
        {
            var newLine = Environment.NewLine;
            return strBuilder.Remove(strBuilder.Length - newLine.Length, newLine.Length);
        }

        private void 复制值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems?.Count > 0)
            {
                var strBuilder = new StringBuilder();
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    strBuilder.AppendLine(item.SubItems[1].Text);
                }
                Clipboard.SetText(TrimEndNewLine(strBuilder).ToString());
            }
        }

        private void 复制属性和值ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems?.Count > 0)
            {
                var strBuilder = new StringBuilder();
                foreach (ListViewItem item in listView1.SelectedItems)
                {
                    strBuilder.Append(item.Text);
                    strBuilder.Append(" : ");
                    strBuilder.AppendLine(item.SubItems[1].Text);
                }
                Clipboard.SetText(TrimEndNewLine(strBuilder).ToString());
            }
        }

        private static bool HasLink(ListViewItem item)
        {
            return item.Text == "相关链接" && item.SubItems[1].Text != "无";
        }

        private void ContextMenuStrip1_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            打开链接ToolStripMenuItem.Visible = false;
            if (listView1.SelectedItems?.Count == 1)
            {
                var item = listView1.SelectedItems[0];
                打开链接ToolStripMenuItem.Visible = HasLink(item);
            }
        }

        private void 打开链接ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems?.Count == 1)
            {
                var item = listView1.SelectedItems[0];
                if (HasLink(item))
                {
                    Process.Start(item.SubItems[1].Text);
                }
            }
        }

        private void 下载toolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://mark0.net/soft-trid-e.html");
        }

    }
}
