﻿/*
 * Copyright 2013-2014 DigitasLBi Netherlands B.V.
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
using System.Xml;
using LBi.LostDoc.Templating.IO;

namespace LBi.LostDoc.Templating
{
    public class TemplateException : Exception
    {
        public static TemplateException MissingAttribute(FileReference templateSource, IXmlLineInfo lineInfo, string attributeName)
        {
            return new TemplateException(templateSource, lineInfo, $"Missing required attribute '{attributeName}'");
        }


        public TemplateException(FileReference templateSource, IXmlLineInfo lineInfo, string message)
            : this(templateSource, lineInfo, message, null)
        {
        }

        public TemplateException(FileReference templateSource, IXmlLineInfo lineInfo, string message, Exception innerException)
            : base(WrapMessage(lineInfo, message, innerException), innerException)
        {
            this.LineInfo = lineInfo;
            this.Template = templateSource;
        }

        private static string WrapMessage(IXmlLineInfo lineInfo, string message, Exception innerException)
        {
            if (innerException == null)
                return $"{message} (line: {lineInfo.LineNumber}, col: {lineInfo.LinePosition})";

            return
                $"{message} (line: {lineInfo.LineNumber}, col: {lineInfo.LinePosition}): [{innerException.GetType().Name}] {innerException.Message}";
        }

        public IXmlLineInfo LineInfo { get; protected set; }
        public FileReference Template { get; protected set; }
    }
}
