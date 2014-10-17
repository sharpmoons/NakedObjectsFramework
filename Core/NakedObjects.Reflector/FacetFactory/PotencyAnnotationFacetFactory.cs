// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Reflection;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Reflector.DotNet.Facets.Actions.Potency;
using NakedObjects.Util;

namespace NakedObjects.Reflector.FacetFactory {
    /// <summary>
    ///     Creates an <see cref="IQueryOnlyFacet" /> or <see cref="IIdempotentFacet" />  based on the presence of a
    ///     <see cref="QueryOnlyAttribute" /> or <see cref="IdempotentAttribute" /> annotation
    /// </summary>
    public class PotencyAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public PotencyAnnotationFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.ActionsOnly) {}

        private static bool Process(MemberInfo member, ISpecification holder) {
            // give priority to Idempotent as more restrictive 
            if (AttributeUtils.GetCustomAttribute<IdempotentAttribute>(member) != null) {
                return FacetUtils.AddFacet(new IdempotentFacetAnnotation(holder));
            }
            if (AttributeUtils.GetCustomAttribute<QueryOnlyAttribute>(member) != null) {
                return FacetUtils.AddFacet(new QueryOnlyFacetAnnotation(holder));
            }
            return false;
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            return Process(method, specification);
        }
    }
}