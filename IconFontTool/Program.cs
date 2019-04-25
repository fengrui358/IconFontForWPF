using System;
using System.IO;
using System.Linq;

namespace IconFontTool
{
    class Program
    {
        private static string _factoryPath = string.Empty;
        private static string _kindPath = string.Empty;
        private static string _iconFilePath = string.Empty;

        private static string _kindName;

        static void Main(string[] args)
        {
            if (args != null && args.Any())
            {
                var argsList = args.ToList();

                _factoryPath = argsList[argsList.IndexOf("-factoryPath") + 1];
                _kindPath = argsList[argsList.IndexOf("-kindPath") + 1];
                _iconFilePath = argsList[argsList.IndexOf("-iconFilePath") + 1];

                if (string.IsNullOrEmpty(_factoryPath) || !Directory.Exists(Path.GetDirectoryName(_factoryPath)))
                {
                    _factoryPath = Path.Combine(AppDomain.CurrentDomain.DynamicDirectory, "IconFontFactory.cs");
                }

                if (string.IsNullOrEmpty(_kindPath) || !Directory.Exists(Path.GetDirectoryName(_kindPath)))
                {
                    _kindPath = Path.Combine(AppDomain.CurrentDomain.DynamicDirectory, "IconFontKind.cs");
                }

                _kindName = Path.GetFileName(_kindPath);

                if (string.IsNullOrEmpty(_iconFilePath) || !Directory.Exists(Path.GetDirectoryName(_iconFilePath)))
                {
                    _iconFilePath = AppDomain.CurrentDomain.DynamicDirectory;
                }
            }
        }

        private static bool CopyFontFile()
        {


            return false;
        }
    }
}
