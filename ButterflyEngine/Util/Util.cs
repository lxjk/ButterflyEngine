using System;
using System.Collections.Generic;
using System.Xml;

namespace ButterflyEngine
{
   public static class Util
    {
        public static XmlAttribute AppendAttribute(this XmlNode node, XmlDocument xml, string attributeName, string attributeValue)
        {
            XmlAttribute attr = xml.CreateAttribute(attributeName);
            attr.Value = attributeValue;
            return node.Attributes.Append(attr);
        }
    }
}
