/*
 * Copyright 2012-2013 DigitasLBi Netherlands B.V.
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

using System.Collections;
using System.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace LBi.LostDoc.Templating.XPath
{
    public class XsltContextAssetVersionGetter : IXsltContextFunction
    {
        private VersionComponent? _ignoredVersionComponent;

        public XsltContextAssetVersionGetter(VersionComponent? ignoredVersionComponent = null)
        {
            this._ignoredVersionComponent = ignoredVersionComponent;
        }

        #region IXsltContextFunction Members

        public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
        {
            AssetIdentifier aid = AssetIdentifier.Parse(Template.ResultToString(args[0]));

            if (this._ignoredVersionComponent.HasValue)
                return aid.Version.ToString((int)this._ignoredVersionComponent.Value);

            return aid.Version.ToString();
        }

        public int Minargs
        {
            get { return 1; }
        }

        public int Maxargs
        {
            get { return 1; }
        }

        public XPathResultType ReturnType
        {
            get { return XPathResultType.String; }
        }

        public XPathResultType[] ArgTypes
        {
            get { return new[] {XPathResultType.String}; }
        }

        #endregion
    }
}
