﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Globalization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Core;
using NakedObjects.Core.Util;

namespace NakedObjects.Meta.SemanticsProvider {
    [Serializable]
    public sealed class ULongValueSemanticsProvider : ValueSemanticsProviderAbstract<ulong>, IUnsignedLongValueFacet {
        private const ulong DefaultValueConst = 0;
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 20;

        public ULongValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) {}

        public static Type Type {
            get { return typeof (IUnsignedLongValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (ulong); }
        }

        #region IUnsignedLongValueFacet Members

        public ulong UnsignedLongValue(INakedObjectAdapter nakedObjectAdapter) {
            return nakedObjectAdapter.GetDomainObject<ulong>();
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (ulong);
        }

        protected override ulong DoParse(string entry) {
            try {
                return ulong.Parse(entry, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, ulong.MinValue, ulong.MaxValue));
            }
        }

        protected override ulong DoParseInvariant(string entry) {
            return ulong.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(ulong obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, ulong value) {
            return value.ToString(mask);
        }

        protected override string DoEncode(ulong obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override ulong DoRestore(string data) {
            return ulong.Parse(data, CultureInfo.InvariantCulture);
        }

        public override string ToString() {
            return "ULongAdapter: ";
        }
    }
}