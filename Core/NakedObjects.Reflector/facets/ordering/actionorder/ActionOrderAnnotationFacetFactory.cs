// Copyright � Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Ordering.MemberOrder;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;

namespace NakedObjects.Reflector.DotNet.Facets.Ordering.ActionOrder {
    public class ActionOrderAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        public ActionOrderAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, FeatureType.ObjectsOnly) {}

        public override bool Process(Type type, IMethodRemover methodRemover, ISpecification specification) {
            var attribute = type.GetCustomAttributeByReflection<ActionOrderAttribute>();
            return FacetUtils.AddFacet(Create(attribute, specification));
        }

        private static IActionOrderFacet Create(ActionOrderAttribute attribute, ISpecification specification) {
            return attribute == null ? null : new ActionOrderFacetAnnotation(attribute.Value, specification);
        }
    }
}