using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace IconFontTool
{
    public class ZipDirectory
    {
        /// <summary>
        /// 压缩文件夹解压目录
        /// </summary>
        private readonly DirectoryInfo _zipDirectory;

        /// <summary>
        /// 字体文件路径
        /// </summary>
        public string TTFFilePath => Path.Combine(_zipDirectory.FullName, "iconfont.ttf");


        public ZipDirectory(DirectoryInfo directory)
        {
            _zipDirectory = directory;
        }

        public List<IconFontContent> GetIconFontContents()
        {
            var result = new List<IconFontContent>();

            var indexPath = Path.Combine(_zipDirectory.FullName, "demo_index.html");
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(indexPath);

            var div = htmlDoc.DocumentNode.SelectSingleNode(@"//div[@class='content font-class']");

            var spans = div.SelectNodes(@".//span");
            var divs = div.SelectNodes(@".//div[@class='name']");

            var count = spans.Count;

            for (var i = 0; i < spans.Count; i++)
            {
                var iconFontContent = new IconFontContent();


                iconFontContent.ClassName =
                    spans[i].Attributes.First().Value.Substring($"icon {Program.FontFamily} {Program.IconPrefix}".Length);
                iconFontContent.DisplayName = divs[i].InnerHtml.Trim();

                result.Add(iconFontContent);
            }

            //查找FontCode
            var fontCodeDoc = File.ReadAllText(Path.Combine(_zipDirectory.FullName, "iconfont.css"));
            var matchOffset = 0;

            foreach (var iconFontContent in result)
            {
                var key = $".{Program.IconPrefix}{iconFontContent.ClassName}:before {{";

                matchOffset = fontCodeDoc.IndexOf(key, matchOffset, StringComparison.Ordinal);
                var start = fontCodeDoc.IndexOf('"', matchOffset);
                var end = fontCodeDoc.IndexOf('"', start + 1);

                //截取，去掉前面的引号和斜杠
                var code = fontCodeDoc.Substring(start + 2, end - start - 2);
                iconFontContent.FontCode = $"\\u{code}";

                matchOffset = end;
            }

            return result;
        }
    }
}