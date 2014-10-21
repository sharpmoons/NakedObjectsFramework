// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Metamodel.SpecImmutable {
    public class OneToManyAssociationSpecImmutable : AssociationSpecImmutable {
        private Type elementType;
        private IObjectSpecImmutable specification;

        public OneToManyAssociationSpecImmutable(IIdentifier name, Type returnType, IObjectSpecImmutable returnSpec)
            : base(name, returnType, returnSpec) {}

        public Type ElementType {
            get { return elementType; }
        }

        /// <summary>
        ///     Return the <see cref="IObjectSpec" /> for the  Type that the collection holds.
        /// </summary>
        public override IObjectSpecImmutable Specification {
            get { return specification; }
        }

        public override bool IsOneToMany {
            get { return true; }
        }

        public override bool IsOneToOne {
            get { return false; }
        }

        public void SetupElementType(Type type, IObjectSpecImmutable spec) {
            elementType = type;
            specification = spec;
        }

        public override string ToString() {
            return "OneToManyAssociation [name=\"" + Identifier + "\",Type=" + Specification + " ]";
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}