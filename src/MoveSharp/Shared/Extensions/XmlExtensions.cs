//
// XmlExtensions.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using MoveSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace MoveSharp
{
    internal static class XmlExtensions
    {
        public static IEnumerable<XElement> GetDescendants(this XElement element, string localName)
        {
            return from child in element.Descendants() where child.Name.LocalName == localName select child;
        }

        public static XElement GetFirstDescendant(this XElement element, string localName)
        {
            foreach (var child in element.Descendants())
            {
                if (child.Name.LocalName == localName)
                    return child;
            }

            return null;
        }

        public static T GetFirstDescendantValue<T>(this XElement element, string localName, IFormatProvider formatProvider = null)
        {
            var descendant = GetFirstDescendant(element, localName);
            if (descendant == null || string.IsNullOrEmpty(descendant.Value))
                return default(T);

            object value = descendant.Value;
            if (typeof(T) == typeof(DateTime))
            {
                value = ConversionHelper.GetDateTimeFromUtcString(descendant.Value);
            }

            return (T)Convert.ChangeType(value, typeof(T), formatProvider);
        }

        public static XElement GetFirstElement(this XElement element, string localName)
        {
            return element.Elements().FirstOrDefault(x => x.Name.LocalName == localName);
        }

        public static XElement ReadAsXElement(this XmlReader reader)
        {
            var node = XElement.ReadFrom(reader);
            if (node == null)
                return null;

            return XElement.Parse(node.ToString());
        }
    }
}
