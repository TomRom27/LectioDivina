using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace LectioDivina.Model
{
    public class SerializationHelper
    {
        /// <summary>
        /// Deserializes a <c>string</c> into type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to deserialize the <paramref name="xml"/> to.</typeparam>
        /// <param name="xml">The string to be deserialized.</param>
        /// <returns>The deserialized object of type <typeparamref name="T"/>.</returns>
        public static T Deserialize<T>(string xml)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            using (StringReader stringReader = new StringReader(xml))
            {
                return (T)deserializer.Deserialize(stringReader);
            }
        }

        /// <summary>
        /// Serializes an object to a <c>string</c>.
        /// </summary>
        /// <typeparam name="T">The type of the input object to be serialized.</typeparam>
        /// <param name="obj">The object to be serialized</param>
        /// <param name="rootAttribute">Optional, the desired XML root element.</param>
        /// <returns>A <c>string</c> containing the serialized <paramref name="obj"/> object.</returns>
        public static string Serialize(object obj, XmlRootAttribute rootAttribute = null)
        {
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer serializer;
            if (rootAttribute != null)
                serializer = new XmlSerializer(obj.GetType(), rootAttribute);
            else
                serializer = new XmlSerializer(obj.GetType());
            StringBuilder stringBuilder = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, new XmlWriterSettings { OmitXmlDeclaration = true }))
            {
                serializer.Serialize(xmlWriter, obj, ns);
            }
            return stringBuilder.ToString();
        }

        public static XmlRootAttribute XmlRootForCollection(Type type, string rootName)
        {
            XmlRootAttribute result = null;

            Type typeInner = null;
            if (type.IsGenericType)
            {
                var typeGeneric = type.GetGenericArguments()[0];
                var typeCollection = typeof(ICollection<>).MakeGenericType(typeGeneric);
                if (typeCollection.IsAssignableFrom(type))
                    typeInner = typeGeneric;
            }
            else if (typeof(ICollection).IsAssignableFrom(type)
                && type.HasElementType)
            {
                typeInner = type.GetElementType();
            }

            if (typeInner != null)
            {
                result = new XmlRootAttribute(rootName);
            }
            return result;
        }

    }
}
