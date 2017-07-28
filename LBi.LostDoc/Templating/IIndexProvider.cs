using System;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace LBi.LostDoc.Templating
{
    public interface IIndexProvider
    {
        void Add(string name,
                 int ordinal,
                 Uri inputUri,
                 string matchExpression,
                 string keyExpression,
                 string selectExpression,
                 XsltContext xsltContext = null);

        XPathNodeIterator Get(string name, int ordinal, object value);
    }
}