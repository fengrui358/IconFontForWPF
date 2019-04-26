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

        public string DecriptionAndValue
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

                var description = new StringBuilder("        [Description(\"");
                if (!string.IsNullOrEmpty(CustomDescription))
                {
                    description.Append($"{CustomDescription} ");
                }

                if (DisplayName == ClassName)
                {
                    description.Append($"{DisplayName}");
                }
                else
                {
                    description.Append($"{DisplayName}/{ClassName}");
                }

                description.Append("\")]");
                sb.AppendLine(description.ToString());

                sb.AppendLine($"        {RealName},");

                return sb.ToString();
            }
        }
    }
}
