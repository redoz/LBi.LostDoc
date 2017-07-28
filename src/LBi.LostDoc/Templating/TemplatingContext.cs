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

using System.Collections.Generic;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Runtime.Caching;
using System.Xml.Linq;
using System.Xml.XPath;
using LBi.LostDoc.Templating.IO;

namespace LBi.LostDoc.Templating
{
    public class TemplatingContext : ITemplatingContext
    {
        public TemplatingContext(ObjectCache cache,
                                 ComposablePartCatalog catalog,
                                 TemplateSettings settings,
                                 XDocument document,
                                 IEnumerable<IAssetUriResolver> resolvers,
                                 StorageResolver storageResolver,
                                 IDependencyProvider dependencyProvider,
                                 IIndexProvider indexProvider)
        {
            this.Catalog = catalog;
            this.Settings = settings;
            this.AssetUriResolvers = resolvers.ToArray();
            this.Cache = cache;
            this.StorageResolver = storageResolver;
            this.DependencyProvider = dependencyProvider;

            XPathDocument xpathDoc;
            using (var reader = document.CreateReader(ReaderOptions.OmitDuplicateNamespaces))
                xpathDoc = new XPathDocument(reader);
            this.Document = xpathDoc.CreateNavigator();
            this.IndexProvider = indexProvider;
        }


        #region ITemplatingContext Members

        public TemplateSettings Settings { get; protected set; }

        public XPathNavigator Document { get; protected set; }

        public IAssetUriResolver[] AssetUriResolvers { get; protected set; }

        public StorageResolver StorageResolver { get; protected set; }

        public IDependencyProvider DependencyProvider { get; protected set; }
        
        public IIndexProvider IndexProvider { get; private set; }

        #endregion

        public ObjectCache Cache { get; private set; }

        public ComposablePartCatalog Catalog { get; private set; }
    }
}