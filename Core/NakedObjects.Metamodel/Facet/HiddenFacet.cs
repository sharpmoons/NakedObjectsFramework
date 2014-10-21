// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Resolve;

namespace NakedObjects.Metamodel.Facet {
    public class HiddenFacet : SingleWhenValueFacetAbstract, IHiddenFacet {
        public HiddenFacet(WhenTo when, ISpecification holder)
            : base(typeof(IHiddenFacet), holder, when) { }

        public string HiddenReason(INakedObject target) {
            if (Value == WhenTo.Always) {
                return Resources.NakedObjects.AlwaysHidden;
            }
            if (Value == WhenTo.Never) {
                return null;
            }

            // remaining tests depend on target in question.
            if (target == null) {
                return null;
            }

            if (Value == WhenTo.UntilPersisted) {
                return target.ResolveState.IsTransient() ? Resources.NakedObjects.HiddenUntilPersisted : null;
            }
            if (Value == WhenTo.OncePersisted) {
                return target.ResolveState.IsPersistent() ? Resources.NakedObjects.HiddenOncePersisted : null;
            }
            return null;
        }


        #region IHiddenFacet Members

        public virtual string Hides(InteractionContext ic, ILifecycleManager persistor) {
            return HiddenReason(ic.Target);
        }

        public virtual HiddenException CreateExceptionFor(InteractionContext ic, ILifecycleManager persistor) {
            return new HiddenException(ic, Hides(ic, persistor));
        }

        #endregion
    }

    // Copyright (c) Naked Objects Group Ltd.
}