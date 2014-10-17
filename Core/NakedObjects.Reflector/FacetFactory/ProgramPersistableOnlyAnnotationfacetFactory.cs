﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Reflector.DotNet.Facets.Objects.NotPersistable;
using NakedObjects.Util;

namespace NakedObjects.Reflector.FacetFactory {
    public class ProgramPersistableOnlyAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public ProgramPersistableOnlyAnnotationFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.ObjectsOnly) {}

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            var attribute = type.GetCustomAttributeByReflection<ProgramPersistableOnlyAttribute>();
            return FacetUtils.AddFacet(Create(attribute, specification));
        }

        private static IProgramPersistableOnlyFacet Create(ProgramPersistableOnlyAttribute attribute, ISpecification holder) {
            return attribute == null ? null : new ProgramPersistableOnlyFacetAnnotation(holder);
        }
    }
}