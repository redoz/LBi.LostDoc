/*
 * Copyright 2012-2014 DigitasLBi Netherlands B.V.
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
using System.IO;

namespace LBi.LostDoc.Templating
{ 
    public abstract class UnitOfWork
    {
        protected UnitOfWork(Uri output, int ordinal)
        {
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (!output.IsAbsoluteUri)
                throw new ArgumentException("Output Uri must be absolute.", nameof(output));
            if (ordinal < 0) throw new ArgumentOutOfRangeException(nameof(ordinal));

            this.Output = output;
            this.Ordinal = ordinal;
        }

        public int Ordinal { get; protected set; }
        public Uri Output { get; protected set; }

        public abstract void Execute(ITemplatingContext context, Stream outputStream);
    }
}