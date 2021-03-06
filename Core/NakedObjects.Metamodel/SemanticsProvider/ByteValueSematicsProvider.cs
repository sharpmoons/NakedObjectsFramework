// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
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
    public sealed class ByteValueSemanticsProvider : ValueSemanticsProviderAbstract<byte>, IByteValueFacet {
        private const byte DefaultValueConst = 0;
        private const bool EqualByContent = true;
        private const bool Immutable = true;
        private const int TypicalLengthConst = 3;

        public ByteValueSemanticsProvider(IObjectSpecImmutable spec, ISpecification holder)
            : base(Type, holder, AdaptedType, TypicalLengthConst, Immutable, EqualByContent, DefaultValueConst, spec) {}

        public static Type Type {
            get { return typeof (IByteValueFacet); }
        }

        public static Type AdaptedType {
            get { return typeof (byte); }
        }

        #region IByteValueFacet Members

        public byte ByteValue(INakedObjectAdapter nakedObjectAdapter) {
            return nakedObjectAdapter.GetDomainObject<byte>();
        }

        #endregion

        public static bool IsAdaptedType(Type type) {
            return type == typeof (byte);
        }

        protected override byte DoParse(string entry) {
            try {
                return byte.Parse(entry);
            }
            catch (FormatException) {
                throw new InvalidEntryException(FormatMessage(entry));
            }
            catch (OverflowException) {
                throw new InvalidEntryException(OutOfRangeMessage(entry, byte.MinValue, byte.MaxValue));
            }
        }

        protected override byte DoParseInvariant(string entry) {
            return byte.Parse(entry, CultureInfo.InvariantCulture);
        }

        protected override string GetInvariantString(byte obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override string TitleStringWithMask(string mask, byte value) {
            return value.ToString(mask);
        }

        protected override string DoEncode(byte obj) {
            return obj.ToString(CultureInfo.InvariantCulture);
        }

        protected override byte DoRestore(string data) {
            return byte.Parse(data, CultureInfo.InvariantCulture);
        }

        public override string ToString() {
            return "ByteAdapter: ";
        }
    }
}