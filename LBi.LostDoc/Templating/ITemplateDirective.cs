/*
 * Copyright 2014 DigitasLBi Netherlands B.V.
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
using System.Diagnostics.Contracts;

namespace LBi.LostDoc.Templating
{
    [ContractClass(typeof(ITemplateDirectiveContract<>))]
    public interface ITemplateDirective<out TUnitOfWork> where TUnitOfWork : UnitOfWork
    {
        IEnumerable<TUnitOfWork> DiscoverWork(ITemplateContext context);
    }

    // ReSharper disable InconsistentNaming
    [ContractClassFor(typeof(ITemplateDirective<>))]
    internal class ITemplateDirectiveContract<T> : ITemplateDirective<T> where T : UnitOfWork
    {
        public IEnumerable<T> DiscoverWork(ITemplateContext context)
        {
            Contract.Requires<ArgumentNullException>(context != null);
            Contract.Ensures(Contract.Result<IEnumerable<T>>() != null);
            Contract.Ensures(Contract.ForAll<T>(Contract.Result<IEnumerable<T>>(), t => t != null));

            return default(IEnumerable<T>);
        }
    }
    // ReSharper restore InconsistentNaming
}