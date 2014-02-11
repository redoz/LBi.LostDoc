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

using System.Xml.XPath;
using LBi.LostDoc.Templating.IO;
using LBi.LostDoc.Templating.XPath;

namespace LBi.LostDoc.Templating
{
    public interface ITemplatingContext : IContextBase
    {        
        TemplateSettings Settings { get; }
        XPathNavigatorIndex DocumentIndex { get; }
        XPathNavigator Document { get; }
        IAssetUriResolver[] AssetUriResolvers { get; }
        StorageResolver Storage { get; }
        IDependencyProvider DependencyProvider { get; }
    }
}
