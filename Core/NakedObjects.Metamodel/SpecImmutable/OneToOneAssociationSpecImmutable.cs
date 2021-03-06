// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Runtime.Serialization;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.SpecImmutable;

namespace NakedObjects.Meta.SpecImmutable {
    [Serializable]
    public sealed class OneToOneAssociationSpecImmutable : AssociationSpecImmutable, IOneToOneAssociationSpecImmutable {
        private readonly IObjectSpecImmutable ownerSpec;

        public OneToOneAssociationSpecImmutable(IIdentifier identifier, IObjectSpecImmutable ownerSpec, IObjectSpecImmutable returnSpec)
            : base(identifier, returnSpec) {
            this.ownerSpec = ownerSpec;
        }

        #region ISerializable

        // The special constructor is used to deserialize values. 
        public OneToOneAssociationSpecImmutable(SerializationInfo info, StreamingContext context) : base(info, context) {}

        #endregion

        public override IObjectSpecImmutable ElementSpec {
            get { return null; }
        }

        #region IOneToOneAssociationSpecImmutable Members

        public override IObjectSpecImmutable OwnerSpec {
            get { return ownerSpec; }
        }

        #endregion

        public override string ToString() {
            return "Reference Association [name=\"" + Identifier + ", Type=" + ReturnSpec + " ]";
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}