// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Runtime.Serialization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Meta.Facet {
    [Serializable]
    public class RangeFacet : IRangeFacet, ISerializable {
        // not using FacetAbstract because of implementing ISerializable
        private readonly Type facetType;
        private ISpecification holder;

        public RangeFacet(IConvertible min, IConvertible max, bool isDateRange, ISpecification holder) {
            this.holder = holder;
            Min = min;
            Max = max;
            IsDateRange = isDateRange;
            facetType = Type;
        }

        public RangeFacet(SerializationInfo info, StreamingContext context) {
            Min = info.GetValue<IConvertible>("Min");
            Max = info.GetValue<IConvertible>("Max");
            IsDateRange = info.GetValue<bool>("IsDateRange");
            facetType = info.GetValue<Type>("facetType");
            holder = info.GetValue<ISpecification>("holder");
        }

        public static Type Type {
            get { return typeof (IRangeFacet); }
        }

        #region IRangeFacet Members

        public bool IsDateRange { get; set; }

        public virtual int OutOfRange(INakedObjectAdapter nakedObjectAdapter) {
            if (nakedObjectAdapter == null) {
                return 0; //Date fields can contain nulls
            }
            var origVal = ((IConvertible) nakedObjectAdapter.Object);
            if (IsSIntegral(origVal)) {
                return Compare(origVal.ToInt64(null), Min.ToInt64(null), Max.ToInt64(null));
            }
            if (IsUIntegral(origVal)) {
                return Compare(origVal.ToUInt64(null), Min.ToUInt64(null), Max.ToUInt64(null));
            }
            if (IsFloat(origVal)) {
                return Compare(origVal.ToDouble(null), Min.ToDouble(null), Max.ToDouble(null));
            }
            if (IsDecimal(origVal)) {
                return Compare(origVal.ToDecimal(null), Min.ToDecimal(null), Max.ToDecimal(null));
            }
            if (IsDateTime(origVal)) {
                return DateCompare(origVal.ToDateTime(null), Min.ToDouble(null), Max.ToDouble(null));
            }
            return 0;
        }

        public virtual string Invalidates(IInteractionContext ic) {
            INakedObjectAdapter proposedArgument = ic.ProposedArgument;
            if (OutOfRange(proposedArgument) == 0) {
                return null;
            }

            if (IsDateTime(proposedArgument.Object)) {
                string minDate = DateTime.Today.AddDays(Min.ToDouble(null)).ToShortDateString();
                string maxDate = DateTime.Today.AddDays(Max.ToDouble(null)).ToShortDateString();
                return string.Format(Resources.NakedObjects.RangeMismatch, minDate, maxDate);
            }
            return string.Format(Resources.NakedObjects.RangeMismatch, Min, Max);
        }

        public virtual Exception CreateExceptionFor(IInteractionContext ic) {
            return new InvalidRangeException(ic, Min, Max, Invalidates(ic));
        }

        public IConvertible Min { get; private set; }
        public IConvertible Max { get; private set; }

        public virtual ISpecification Specification {
            get { return holder; }
            set { holder = value; }
        }

        /// <summary>
        ///     Assume implementation is <i>not</i> a no-op.
        /// </summary>
        /// <para>
        ///     No-op implementations should override and return <c>true</c>.
        /// </para>
        public virtual bool IsNoOp {
            get { return false; }
        }

        public Type FacetType {
            get { return facetType; }
        }

        /// <summary>
        ///     Default implementation of this method that returns <c>true</c>, ie
        ///     should replace non-<see cref="IsNoOp" /> implementations.
        /// </summary>
        /// <para>
        ///     Implementations that don't wish to replace non-<see cref="IsNoOp" /> implementations
        ///     should override and return <c>false</c>.
        /// </para>
        public virtual bool CanAlwaysReplace {
            get { return true; }
        }

        #endregion

        #region ISerializable Members

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue<IConvertible>("Min", Min);
            info.AddValue<IConvertible>("Max", Max);
            info.AddValue<bool>("IsDateRange", IsDateRange);
            info.AddValue<Type>("facetType", facetType);
            info.AddValue<ISpecification>("holder", holder);
        }

        #endregion

        protected int Compare<T>(T val, T min, T max) where T : struct, IComparable {
            if (val.CompareTo(min) < 0) {
                return -1;
            }

            if (val.CompareTo(max) > 0) {
                return 1;
            }

            return 0;
        }

        protected int DateCompare(DateTime date, double min, double max) {
            DateTime earliest = (DateTime.Today).AddDays(min);
            DateTime latest = (DateTime.Today).AddDays(max);
            if (date < earliest) return -1;
            if (date > latest) return +1;
            return 0;
        }

        private static bool IsSIntegral(object o) {
            return o is sbyte || o is short || o is int || o is long;
        }

        private static bool IsUIntegral(object o) {
            return o is byte || o is ushort || o is uint || o is ulong;
        }

        private static bool IsFloat(object o) {
            return o is float || o is double;
        }

        private static bool IsDecimal(object o) {
            return o is decimal;
        }

        private static bool IsDateTime(object o) {
            return o is DateTime;
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}