﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Xml;

namespace ClipArtViewer
{
	public class ShapeUtil
	{
		public static Transform ParseTransform(string value)
		{
			string type = ExtractUntil(value, '(');
			string v1 = ExtractBetween(value, '(', ')');
			
			ShapeUtil.StringSplitter split = new ShapeUtil.StringSplitter(v1);
			List<double> values = new List<double>();
			while (split.More)
				values.Add(split.ReadNextValue());
			if (type == SVGTags.sTranslate)
				return new TranslateTransform(values[0], values[1]);
			if (type == SVGTags.sMatrix)
				return Transform.Parse(v1);
			if (type == SVGTags.sScale)
				return new ScaleTransform(values[0], values[1]);
			if (type == SVGTags.sRotate)
				return new RotateTransform(values[0], values[1], values[2]);

			return null;
		}
		public static string ExtractUntil(string value, char ch)
		{
			int index = value.IndexOf(ch);
			if (index <= 0)
				return value;
			return value.Substring(0, index);
		}
		public static string ExtractBetween(string value, char startch, char endch)
		{
			int start = value.IndexOf(startch);
			if (startch < 0)
				return value;
			start++;
			int end = value.IndexOf(endch, start);
			if (endch < 0)
				return value.Substring(start);
			return value.Substring(start, end - start);
		}
		public class StringSplitter
		{
			string m_value;
			int m_curPos = -1;
			char[] NumberChars = "0123456789.-".ToCharArray();
			public StringSplitter(string value)
			{
				m_value = value;
				m_curPos = 0;
			}
			public void SetString(string value, int startpos)
			{
				m_value = value;
				m_curPos = startpos;
			}
			public bool More 
			{
				get
				{
					return m_curPos >= 0 && m_curPos < m_value.Length;
				}
			}
			public double ReadNextValue()
			{
				int startpos = m_curPos;
				if (startpos < 0)
					startpos = 0;
				if (startpos >= m_value.Length)
					return float.NaN;
				string numbers = "0123456789-.";
				// find start of a number
				while (startpos < m_value.Length && numbers.Contains(m_value[startpos]) == false)
					startpos++;
				// end of number
				int endpos = startpos;
				while (endpos < m_value.Length && numbers.Contains(m_value[endpos]))
				{
					// '-' if number is followed by '-' then it indicates the end of the value
					if (endpos != startpos && m_value[endpos] == '-')
						break;
					endpos++;
				}
				int len = endpos - startpos;
				if (len <= 0)
					return float.NaN;
				m_curPos = endpos;
				string s = m_value.Substring(startpos, len);
													  
				// find start of a next number
				startpos = endpos;
				while (startpos < m_value.Length && numbers.Contains(m_value[startpos]) == false)
					startpos++;
				if (startpos >= m_value.Length)
					endpos = startpos;
													  
				m_curPos = endpos;
				return System.Xml.XmlConvert.ToDouble(s);
			}
			public Point ReadNextPoint()
			{
				double x = ReadNextValue();
				double y = ReadNextValue();
				return new Point(x,y);
			}
		}
		public class Attribute
		{
			public string Name {get; set;}
			public string Value {get; set; }
			public Attribute(string name, string value)
			{
				Name = name;
				Value = value;
			}
			public override string ToString()
			{
				return string.Format("{0}:{1}", Name, Value);
			}

		}
		public static Attribute ReadNextAttr(string inputstring, ref int startpos)
		{
			if (inputstring[startpos] != ' ')
				throw new Exception("inputstring[startpos] must be a whitepace character");
			while (inputstring[startpos] == ' ')
				startpos++;

			int namestart = startpos;
			int nameend = inputstring.IndexOf('=', startpos);
			if (nameend < namestart)
				throw new Exception("did not find xml attribute name");

			int valuestart = inputstring.IndexOf('"', nameend);
			valuestart++;
			int valueend = inputstring.IndexOf('"', valuestart);
			if (valueend < 0 || valueend < valuestart)
				throw new Exception("did not find xml attribute value");

			// search for first occurence of x="yy"
			string attrName = inputstring.Substring(namestart, nameend-namestart).Trim();
			string attrValue = inputstring.Substring(valuestart, valueend-valuestart).Trim();
			startpos = valueend + 1;
			return new Attribute(attrName, attrValue);
		}
	}

	public class XmlUtil
	{
		static bool SplitValueUnits(string inputstring, out double value, out string units)
		{
			value = 0;
			units = string.Empty;
			int index = inputstring.LastIndexOfAny(new char[] {'.','-','0','1','2','3','4','5','6','7','8','9'}) ;
			if (index >= 0)
			{
				string svalue = inputstring.Substring(0, index+1);
				if (index + 1 < inputstring.Length)
					units = inputstring.Substring(index+1);
				try
				{
					value = System.Xml.XmlConvert.ToDouble(svalue);
					return true;
				}
				catch (FormatException)
				{
				}
			}
			return false;
		}
		
		public static double AttrValue(ShapeUtil.Attribute attr)
		{
			double result = 0;
			string units = string.Empty;
			SplitValueUnits(attr.Value, out result, out units);
			return result;
		}
		public static double AttrValue(XmlNode node, string id, double defaultvalue)
		{
			XmlAttribute attr = node.Attributes[id];
			if (attr == null)
				return defaultvalue;

			double result = 0;
			string units;

			if (attr != null && SplitValueUnits(attr.Value, out result, out units))
				return result;
			return defaultvalue;
		}
		public static string AttrValue(XmlNode node, string id, string defaultvalue)
		{
			if (node.Attributes == null)
				return defaultvalue;
			XmlAttribute attr = node.Attributes[id];
			if (attr != null)
				return attr.Value;
			return defaultvalue;
		}
		public static string AttrValue(XmlNode node, string id)
		{
			return AttrValue(node, id, string.Empty);
		}

		public static double ParseDouble(SVG svg, string svalue)
		{
			string units = string.Empty;
			double value = 0d;
			if (SplitValueUnits(svalue, out value, out units))
				return value;
			return 0.1;
		}
		public class StyleItem : XmlAttribute
		{
			public StyleItem(XmlNode owner, string name, string value) : base(string.Empty, name, string.Empty, owner.OwnerDocument)
			{
				Value = value;
			}
		}
		public static List<ShapeUtil.Attribute> SplitStyle(SVG svg, string fullstyle)
		{
			List<ShapeUtil.Attribute> list = new List<ShapeUtil.Attribute>();
			if (fullstyle.Length == 0)
				return list;
			// style contains attributes in format of "attrname:value;attrname:value"
			string[] attrs = fullstyle.Split(';');
			foreach (string attr in attrs)
			{
				string[] s = attr.Split(':');
				if (s.Length != 2)
					continue;
				list.Add(new ShapeUtil.Attribute(s[0].Trim(), s[1].Trim()));
			}
			return list;
		}
	}
}
