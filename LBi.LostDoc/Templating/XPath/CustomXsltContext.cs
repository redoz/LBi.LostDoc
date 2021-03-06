/*
 * Copyright 2012 DigitasLBi Netherlands B.V.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License. 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace LBi.LostDoc.Templating.XPath
{
    public class  ResolveEventArgs : EventArgs
    {
        public string Name { get; protected set; }
        public object Value { get; protected set; }
    }


    public class CustomXsltContext : System.Xml.Xsl.XsltContext
    {
        protected Dictionary<string, IXsltContextFunction> Functions;
        protected Stack<XPathVariableList> Variables;

        public CustomXsltContext()
        {
            this.Functions = new Dictionary<string, IXsltContextFunction>();
            this.Variables = new Stack<XPathVariableList>();
        }

        public CustomXsltContext(NameTable nameTable)
            : base(nameTable)
        {
            this.Functions = new Dictionary<string, IXsltContextFunction>();
            this.Variables = new Stack<XPathVariableList>();
        }

        public override bool Whitespace
        {
            get { return false; }
        }

        public override string LookupNamespace(string prefix)
        {
            return base.LookupNamespace(prefix);
        }

        public override int CompareDocument(string baseUri, string nextbaseUri)
        {
            return string.CompareOrdinal(baseUri, nextbaseUri);
        }

        public override bool PreserveWhitespace(XPathNavigator node)
        {
            return false;
        }

        public void RegisterFunction(string prefix, string name, IXsltContextFunction function)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (name == null)
                throw new ArgumentNullException("name");

            this.Functions[prefix + ":" + name] = function;
        }

        public override IXsltContextFunction ResolveFunction(string prefix, string name,
                                                             XPathResultType[] argTypes)
        {
            IXsltContextFunction function;

            if (this.Functions.TryGetValue(prefix + ":" + name, out function))
            {
                return function;
            }

            return null;
        }

        public override IXsltContextVariable ResolveVariable(string prefix, string name)
        {
            IXsltContextVariable ret = this.Variables.Peek().Resolve(name);

            if (ret == null)
                throw new XsltException("Invalid variable: " + name);

            return ret;
        }

        internal static string GetValue(object v)
        {
            if (v == null)
                return null;

            if (v is XPathNodeIterator)
            {
                foreach (XPathNavigator n in v as XPathNodeIterator)
                    return n.Value;
            }

            return Convert.ToString(v);
        }

        #region Nested type: XPathVariableList
        protected class XPathVariableList : List<XPathVariable>
        {
            public XPathVariableList(XPathVariableList parent, XNode scope, IXmlNamespaceResolver resolver)
            {
                this.Parent = parent;
                this.Scope = scope;
                this.Resolver = resolver;
            }

            protected IXmlNamespaceResolver Resolver { get; set; }

            protected XNode Scope { get; set; }

            protected XPathVariableList Parent { get; set; }

            public IXsltContextVariable Resolve(string name)
            {
                IXsltContextVariable ret = null;
                XPathVariable var = this.Find(x => StringComparer.InvariantCulture.Equals(x.Name, name));

                if (var != null)
                    ret = var.Evaluate(this.Scope, this.Resolver);
                else if (this.Parent != null)
                    ret = this.Parent.Resolve(name);

                return ret;
            }

        }
        #endregion


        public void PushVariableScope(XNode scope, params XPathVariable[] variables)
        {
            var list = new XPathVariableList(this.Variables.Count == 0 ? null : this.Variables.Peek(), scope, this);
            list.AddRange(variables);
            this.Variables.Push(list);
        }

        public void PopVariableScope()
        {
            this.Variables.Pop();
        }
    }
}
