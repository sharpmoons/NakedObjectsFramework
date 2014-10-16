// Copyright � Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 


using Common.Logging;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Util;
using MemberInfo = System.Reflection.MemberInfo;
using MethodInfo = System.Reflection.MethodInfo;
using PropertyInfo = System.Reflection.PropertyInfo;
using ParameterInfo = System.Reflection.ParameterInfo;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.Validate.Mandatory {
    public class OptionalAnnotationFacetFactory : AnnotationBasedFacetFactoryAbstract {
        private static readonly ILog Log = LogManager.GetLogger(typeof (OptionalAnnotationFacetFactory));

        public OptionalAnnotationFacetFactory(INakedObjectReflector reflector)
            :base(reflector, FeatureType.PropertiesAndParameters) { }

        private static bool Process(MemberInfo member, ISpecification holder) {
            var attribute = member.GetCustomAttribute<OptionallyAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        public override bool Process(MethodInfo method, IMethodRemover methodRemover, ISpecification specification) {
            if ((method.ReturnType.IsPrimitive || TypeUtils.IsEnum(method.ReturnType)) && method.GetCustomAttribute<OptionallyAttribute>() != null) {
                Log.Warn("Ignoring Optionally annotation on primitive parameter on " + method.ReflectedType + "." + method.Name);
                return false;
            }
            return Process(method, specification);
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            if ((property.PropertyType.IsPrimitive || TypeUtils.IsEnum(property.PropertyType)) && property.GetCustomAttribute<OptionallyAttribute>() != null) {
                Log.Warn("Ignoring Optionally annotation on primitive or un-readable parameter on " + property.ReflectedType + "." + property.Name);
                return false;
            }
            if (property.GetGetMethod() != null && !property.PropertyType.IsPrimitive) {
                return Process(property, specification);
            }
            return false;
        }

        public override bool ProcessParams(MethodInfo method, int paramNum, ISpecification holder) {
            ParameterInfo parameter = method.GetParameters()[paramNum];
            if ((parameter.ParameterType.IsPrimitive || TypeUtils.IsEnum(parameter.ParameterType))) {
                if (method.GetCustomAttribute<OptionallyAttribute>() != null) {
                    Log.Warn("Ignoring Optionally annotation on primitive parameter " + paramNum + " on " + method.ReflectedType + "." +
                             method.Name);
                }
                return false;
            }
            var attribute = parameter.GetCustomAttributeByReflection<OptionallyAttribute>();
            return FacetUtils.AddFacet(Create(attribute, holder));
        }

        private static IMandatoryFacet Create(OptionallyAttribute attribute, ISpecification holder) {
            return attribute != null ? new OptionalFacet(holder) : null;
        }
    }
}