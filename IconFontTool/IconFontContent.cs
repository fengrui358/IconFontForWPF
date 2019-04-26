using System;
using System.Text;

namespace IconFontTool
{
    public class IconFontContent
    {
        public string DisplayName { get; set; }

        public string ClassName { get; set; }

        public string FontCode { get; set; }

        public string CustomName { get; set; }

        public string CustomDescription { get; set; }

        public string RealName
        {
            get
            {
                if (string.IsNullOrEmpty(CustomName))
                {
                    return ClassName;
                }
                else
                {
                    return CustomName;
                }
            }
        }

        public string Decription
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine("        /// <summary>");
                if (!string.IsNullOrEmpty(CustomDescription))
                {
                    sb.AppendLine($"        /// {CustomDescription}");
                }

                sb.AppendLine($"        /// {DisplayName}");
                sb.AppendLine($"        /// Class/{ClassName}");
                sb.AppendLine("        /// </summary>");

                var description = "        [Description(\"{0}{1}\")]";
                if (!string.IsNullOrEmpty(CustomDescription))
                {
                    string.Format(description, $"{CustomDescription} ");
                }
                else
                {
                    string.Format(description, string.Empty);
                }

                if (DisplayName == ClassName)
                {
                    //sb.AppendLine($"        [Description(\"{!string.IsNullOrEmpty(CustomDescription)?}selectfiles / selectfiles\")]");
                }
                else
                {

                }

                return sb.ToString();
            }
        }
    }
}
