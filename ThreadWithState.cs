using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FileTypePropertySheet
{
    class ThreadWithState
    {
        private readonly string FilePath;
        private readonly Dictionary<string, ListViewItem>[] ItemsGroups;

        public ThreadWithState(string filePath, ListView listView)
        {
            FilePath = filePath;
            ItemsGroups = listView.Items.OfType<ListViewItem>()
                .GroupBy(item => item.Group.Header)
                .Select(group => group.ToDictionary(value => value.Text))
                .ToArray();
        }

        string ExecuteTrid()
        {
            var process = new Process();
            process.StartInfo.FileName = "trid.exe";
            process.StartInfo.Arguments = $"\"{FilePath}\" -v -r:5";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            var strBuilder = new StringBuilder();
            process.OutputDataReceived += (_, args) => strBuilder.AppendLine(args.Data);
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            return strBuilder.ToString();
        }

        public void ThreadProc()
        {
            string output = ExecuteTrid();

            var matches = Regex.Matches(output, @"(?<p>\d{1,3}(\.\d)?%) \(\.(?<ext>[^\)]+)\) (?<desc>.+) \((\d+\/)+\d+\)");
            var mineTypes = Regex.Matches(output, @"(?<=Mime type  : ).+");
            var relatedURL = Regex.Matches(output, @"(?<=Related URL: ).+");
            var definition = Regex.Matches(output, @"(?<=Definition   : ).+");

            if (matches.Count > 0)
            {
                for (int i = 0; i < matches.Count; i++)
                {
                    var itemsGroup = ItemsGroups[i];
                    itemsGroup["扩展名"].SubItems[1].Text = matches[i].Groups["ext"].Value;
                    itemsGroup["概率"].SubItems[1].Text = matches[i].Groups["p"].Value;
                    itemsGroup["描述"].SubItems[1].Text = matches[i].Groups["desc"].Value;
                    itemsGroup["Mime"].SubItems[1].Text = i < mineTypes.Count ? mineTypes[i].Value?.Trim() : "未知";
                    itemsGroup["定义"].SubItems[1].Text = i < definition.Count ? definition[i].Value?.Trim() : "无";
                    itemsGroup["相关链接"].SubItems[1].Text = i < relatedURL.Count ? relatedURL[i].Value?.Trim() : "无";
                }
            }
        }
    }
}
