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

namespace LBi.LostDoc.Repository.Web.Host.Areas.Administration.Models
{
    public class SearchResultModel : ModelBase
    {
        public AddInSourceModel[] AddInSources { get; set; }

        public int? NextOffset { get; set; }

        public AddInModel[] Results { get; set; }

        public string Terms { get; set; }

        public string Title { get; set; }
        
        public bool IncludePrerelease { get; set; }
    }
}