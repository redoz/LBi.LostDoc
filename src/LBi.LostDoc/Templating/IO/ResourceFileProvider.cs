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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LBi.LostDoc.Templating.IO
{
    public class ResourceFileProvider : IFileProvider
    {
        private Assembly _asm;
        private string _ns;

        public ResourceFileProvider(string ns)
            : this(ns, Assembly.GetCallingAssembly())
        {
        }

        public ResourceFileProvider(string ns, Assembly asm)
        {
            if (!string.IsNullOrEmpty(ns))
                this._ns = ns + '.';
            else
                this._ns = ns;

            this._asm = asm;
        }

        private string ConvertPath(string path)
        {
            return this._ns + path.Replace('\\', '.').Replace('/', '.');
        }

        #region IReadOnlyFileProvider Members

        public bool FileExists(string path)
        {
            string name = this.ConvertPath(path);
            return this._asm.GetManifestResourceNames().Any(n => StringComparer.OrdinalIgnoreCase.Equals(n, name));
        }

        public Stream OpenFile(string path, FileMode mode)
        {
            if (mode != FileMode.Open)
                throw new ArgumentOutOfRangeException("mode", "Only FileMode.Open is supported.");

            string name = this.ConvertPath(path.TrimStart('/'));

            var realName = this._asm.GetManifestResourceNames().SingleOrDefault(n => StringComparer.OrdinalIgnoreCase.Equals(n, name));
            if (realName == null)
                throw new FileNotFoundException($"Resource not found: {this.ConvertPath(path)} (Was: {path})", path);

            var ret = this._asm.GetManifestResourceStream(realName);
            if (ret == null)
                throw new FileNotFoundException($"Resource not found: {this.ConvertPath(path)} (Was: {path})", path);
            return ret;
        }

        public bool SupportsDiscovery => true;

        public IEnumerable<string> GetDirectories(string path)
        {
            if (path == ".")
                path = "";

            path = this.ConvertPath(path);

            if (!path.EndsWith("."))
                path += ".";

            var ret = this._asm.GetManifestResourceNames()
                          .Where(n => n.StartsWith(path))
                          .Select(n => n.Substring(path.Length))
                          .Where(n => n.IndexOf('.') < n.LastIndexOf('.'))
                          .Select(n => n.Substring(0, n.IndexOf('.')))
                          .Distinct(StringComparer.Ordinal);

            return ret;
        }

        public IEnumerable<string> GetFiles(string path)
        {
            if (path == ".")
                path = "";

            path = this.ConvertPath(path);

            var descendants = this._asm.GetManifestResourceNames()
                                  .Where(n => n.StartsWith(path));

            foreach (var descendant in descendants)
            {
                string pathSuffix = descendant.Substring(path.Length);
                int fileExtSeperator = pathSuffix.LastIndexOf('.');
                int lastDirSeperator = pathSuffix.LastIndexOf('.', fileExtSeperator - 1);
                if (lastDirSeperator == 0)
                    yield return pathSuffix.Substring(1);
            }
        }

        #endregion

        public override string ToString()
        {
            return $"[{this._asm.GetName().Name}]{this._ns}";
        }
    }
}