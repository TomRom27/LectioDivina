using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace LectioDivina.Service
{
    public static class XmlDocumentExtensions
    {
        public static XmlNode SelectFirstNodeByTag(this XmlDocument doc, string tag)
        {
            // this is not proper way seeking by element name with namespace
            // but is more flexible as it doesn't need explicit namespaces and manager
            var nodes = doc.GetElementsByTagName(tag);
            if (nodes.Count >= 1)
                return nodes[0];
            else
                return null;
        }

        public static XmlElement SelectFirstElementOfNamedAttribute(this XmlDocument doc, string elemName, string attrName, string attrValue)
        {
            var els = doc.GetElementsByTagName(elemName);
            foreach (var e in els)
            {
                if (e is XmlElement elem)
                {
                    foreach (var a in elem.Attributes)
                    {
                        if (((XmlAttribute)a).Name.Equals(attrName, StringComparison.InvariantCultureIgnoreCase) &&
                            ((XmlAttribute)a).Value.Equals(attrValue, StringComparison.InvariantCultureIgnoreCase))
                            return elem;
                    }
                }
            }
            return null;
        }

        public static XmlAttribute SetAttributeValue(this XmlElement elem, string attrName, string attrValue)
        {

            foreach (var a in elem.Attributes)

                if (((XmlAttribute)a).Name.Equals(attrName, StringComparison.InvariantCultureIgnoreCase))
                {
                    ((XmlAttribute)a).Value = attrValue;
                    return (XmlAttribute)a;
                }
            return null;
        }

    }
}
