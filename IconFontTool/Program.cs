﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;

namespace IconFontTool
{
    class Program
    {
        private static string _factoryPath = string.Empty;
        private static string _kindPath = string.Empty;
        private static string _iconFilePath = string.Empty;
        private static string _mappingFilePath = string.Empty;

        private static string _factoryNameSpace = "IconFont";
        private static string _kindNameSpace = "IconFont";

        private static string _factoryName;
        private static string _kindName;

        static void Main(string[] args)
        {
            if (args != null && args.Any())
            {
                var argsList = args.ToList();

                if (argsList.Contains("-factoryPath"))
                {
                    _factoryPath = argsList[argsList.IndexOf("-factoryPath") + 1];
                }

                if (argsList.Contains("-kindPath"))
                {
                    _kindPath = argsList[argsList.IndexOf("-kindPath") + 1];
                }

                if (argsList.Contains("-iconFilePath"))
                {
                    _iconFilePath = argsList[argsList.IndexOf("-iconFilePath") + 1];
                }

                if (argsList.Contains("-mappingFilePath"))
                {
                    _mappingFilePath = argsList[argsList.IndexOf("-mappingFilePath") + 1];
                }

                if (argsList.Contains("-factoryNameSpace"))
                {

                    _factoryNameSpace = argsList[argsList.IndexOf("-factoryNameSpace") + 1];
                }

                if (argsList.Contains("-kindNameSpace"))
                {

                    _kindNameSpace = argsList[argsList.IndexOf("-kindNameSpace") + 1];
                }

                _factoryPath = GetFilePath(_factoryPath, "IconFontFactory.cs");
                _factoryName = Path.GetFileNameWithoutExtension(_factoryPath);
                _kindPath = GetFilePath(_kindPath, "IconFontKind.cs");
                _kindName = Path.GetFileNameWithoutExtension(_kindPath);
                _iconFilePath = GetFilePath(_iconFilePath);

                _mappingFilePath = GetFilePath(_mappingFilePath, "mapping.txt");

                //查找当前目录下的唯一ZIP文件
                var zips = new DirectoryInfo(Directory.GetCurrentDirectory()).GetFiles("*.zip",
                    SearchOption.TopDirectoryOnly);

                if (zips.Length != 1)
                {
                    throw new Exception("本地文件夹Zip文件错误。");
                }

                var zip = zips.First();

                //在临时目录解压
                var zipDirectory = new ZipDirectory(CopyZipFile(zip.FullName));

                //得到关键信息
                var iconFontContents = zipDirectory.GetIconFontContents();

                //匹配映射信息
                if (_mappingFilePath != null && File.Exists(_mappingFilePath))
                {
                    ModifyName(_mappingFilePath, iconFontContents);
                }

                //反写文件
                WriteFactoryFile(iconFontContents);
                WriteKindFile(iconFontContents);

                //Copy字体文件
                File.Copy(zipDirectory.TTFFilePath, _iconFilePath);

                //删除拷贝过来的Zip文件
                File.Delete(zip.FullName);
            }
        }

        private static string GetFilePath(string filePath, string defaultFileName = null)
        {
            var result = string.Empty;

            if (string.IsNullOrEmpty(filePath))
            {
                if (defaultFileName == null)
                {
                    result = Directory.GetCurrentDirectory();
                }
                else
                {
                    result = Path.Combine(Directory.GetCurrentDirectory(), defaultFileName);
                }
            }
            else
            {
                if (filePath.StartsWith("."))
                {
                    result = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                }

                if (!Directory.Exists(Path.GetDirectoryName(result)))
                {
                    if (defaultFileName == null)
                    {
                        result = Directory.GetCurrentDirectory();
                    }
                    else
                    {
                        result = Path.Combine(Directory.GetCurrentDirectory(), defaultFileName);
                    }
                }
            }

            return result;
        }

        private static DirectoryInfo CopyZipFile(string sourceFilePath)
        {
            var targetFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName(), Path.GetFileName(sourceFilePath));
            var targetDirectory = Path.GetDirectoryName(targetFileName);

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            //在临时目录解压
            File.Copy(sourceFilePath, targetFileName);
            ZipFile.ExtractToDirectory(targetFileName, targetDirectory);

            return new DirectoryInfo(targetDirectory).GetDirectories().First();
        }

        private static bool CopyFontFile()
        {
            return false;
        }

        private static void ModifyName(string mappingFilePath, List<IconFontContent> iconFontContents)
        {
            var contentLines = File.ReadAllLines(mappingFilePath, Encoding.UTF8);
            var dictionary = new Dictionary<string, string>();

            foreach (var contentLine in contentLines)
            {
                var keyValue = contentLine.Split(new[] {'/'}, StringSplitOptions.RemoveEmptyEntries);
                if (keyValue.Length == 2)
                {
                    dictionary.Add(keyValue[0], keyValue[1]);
                }
            }

            foreach (var iconFontContent in iconFontContents)
            {
                if (dictionary.ContainsKey(iconFontContent.ClassName))
                {
                    var value = dictionary[iconFontContent.ClassName];
                    string description = null;

                    var index = value.IndexOf('(');
                    if (index > 0)
                    {
                        var endIndex = value.IndexOf(')', index);
                        description = value.Substring(index + 1, endIndex - index - 1);

                        value = value.Substring(0, index);
                    }

                    iconFontContent.CustomName = value;
                    iconFontContent.CustomDescription = description;
                }
            }
        }

        private static void WriteFactoryFile(List<IconFontContent> iconFontContents)
        {
            using (var contentStream =
                Assembly.GetEntryAssembly().GetManifestResourceStream("IconFontTool.FactoryCs.txt"))
            {
                using (var streamReader = new StreamReader(contentStream))
                {
                    var content = streamReader.ReadToEnd();
                    var sb = new StringBuilder();

                    foreach (var iconFontContent in iconFontContents)
                    {
                        sb.AppendLine("                {");
                        sb.AppendLine($"                    {_kindName}.{iconFontContent.RealName},");
                        sb.AppendLine($"                    \"{iconFontContent.FontCode}\"");
                        sb.AppendLine("                },");
                    }

                    sb.Remove(sb.Length - 3, 3);

                    content = content.Replace("{NameSpace}", _factoryNameSpace);
                    content = content.Replace("{FactoryClassName}", _factoryName);
                    content = content.Replace("{KindType}", _kindName);
                    content = content.Replace("{Content}", sb.ToString());

                    File.WriteAllText(_factoryPath, content, Encoding.UTF8);
                }
            }
        }

        private static void WriteKindFile(List<IconFontContent> iconFontContents)
        {
            using (var contentStream =
                Assembly.GetEntryAssembly().GetManifestResourceStream("IconFontTool.KindCs.txt"))
            {
                using (var streamReader = new StreamReader(contentStream))
                {
                    var content = streamReader.ReadToEnd();
                    var sb = new StringBuilder();

                    foreach (var iconFontContent in iconFontContents)
                    {
                        sb.AppendLine($"{iconFontContent.DecriptionAndValue}");
                    }

                    sb.Remove(sb.Length - 6, 6);

                    content = content.Replace("{NameSpace}", _factoryNameSpace);
                    content = content.Replace("{KindType}", _kindName);
                    content = content.Replace("{Content}", sb.ToString());

                    File.WriteAllText(_kindPath, content, Encoding.UTF8);
                }
            }
        }
    }
}