// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Meta.Facet;
using NakedObjects.Meta.Utils;

namespace NakedObjects.Reflect.FacetFactory {
    public sealed class ConcurrencyCheckAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public ConcurrencyCheckAnnotationFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Properties) {}

        public override void Process(IReflector reflector, PropertyInfo property, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            Attribute attribute = property.GetCustomAttribute<ConcurrencyCheckAttribute>();
            FacetUtils.AddFacet(Create(attribute, specification));
        }

        private static IConcurrencyCheckFacet Create(Attribute attribute, ISpecification holder) {
            return attribute == null ? null : new ConcurrencyCheckFacet(holder);
        }
    }
}