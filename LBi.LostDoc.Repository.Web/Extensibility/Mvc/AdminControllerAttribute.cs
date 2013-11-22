﻿/*
 * Copyright 2013 DigitasLBi Netherlands B.V.
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
using System.ComponentModel.Composition;
using System.Web.Mvc;

namespace LBi.LostDoc.Repository.Web.Extensibility.Mvc
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AdminControllerAttribute : ExportAttribute
    {
        private readonly string _name;

        public AdminControllerAttribute(string name) : base(ContractNames.AdminController, typeof(IController))
        {
            this._name = name;
            this.Text = name;
        }

        public string Name { get { return this._name; } }

        public string Text { get; set; }

        public string Group { get; set; }

        public double Order { get; set; }
    }
}
