// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;
using NakedObjects.Architecture.Util;
using NakedObjects.Metamodel.Facet;
using NakedObjects.Reflector.DotNet.Facets.AutoComplete;
using NakedObjects.Reflector.DotNet.Facets.Propcoll.Access;
using NakedObjects.Reflector.DotNet.Facets.Propcoll.NotPersisted;
using NakedObjects.Reflector.DotNet.Facets.Properties.Choices;
using NakedObjects.Reflector.DotNet.Facets.Properties.Defaults;
using NakedObjects.Reflector.DotNet.Facets.Properties.Modify;
using NakedObjects.Reflector.DotNet.Facets.Properties.Validate;
using NakedObjects.Util;

namespace NakedObjects.Reflector.FacetFactory {
    public class PropertyMethodsFacetFactory : PropertyOrCollectionIdentifyingFacetFactoryAbstract {
        private static readonly ILog Log;
        private static readonly string[] prefixes;

        static PropertyMethodsFacetFactory() {
            Log = LogManager.GetLogger(typeof (PropertyMethodsFacetFactory));

            prefixes = new[] {
                PrefixesAndRecognisedMethods.ClearPrefix,
                PrefixesAndRecognisedMethods.ModifyPrefix
            };
        }

        public PropertyMethodsFacetFactory(IReflector reflector)
            : base(reflector, FeatureType.PropertiesOnly) {}

        public PropertyMethodsFacetFactory(IReflector reflector, string[] subPefixes)
            : base(reflector, FeatureType.PropertiesOnly) {}

        public override string[] Prefixes {
            get { return prefixes; }
        }

        public override bool Process(PropertyInfo property, IMethodRemover methodRemover, ISpecification specification) {
            string capitalizedName = property.Name;
            var paramTypes = new[] {property.PropertyType};

            var facets = new List<IFacet> {new PropertyAccessorFacetViaAccessor(property, specification)};

            if (property.PropertyType.IsGenericType && (property.PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>))) {
                facets.Add(new NullableFacetAlways(specification));
            }

            if (property.GetSetMethod() != null) {
                if (property.PropertyType == typeof (byte[])) {
                    facets.Add(new DisabledFacetAlways(specification));
                }
                else {
                    facets.Add(new PropertySetterFacetViaSetterMethod(property, specification));
                }
                facets.Add(new PropertyInitializationFacetViaSetterMethod(property, specification));
            }
            else {
                //facets.Add(new DerivedFacetInferred(specification));
                facets.Add(new NotPersistedFacetAnnotation(specification));
                facets.Add(new DisabledFacetAlways(specification));
            }
            FindAndRemoveModifyMethod(facets, methodRemover, property.DeclaringType, capitalizedName, paramTypes, specification);
            FindAndRemoveClearMethod(facets, methodRemover, property.DeclaringType, capitalizedName, specification);

            FindAndRemoveAutoCompleteMethod(facets, methodRemover, property.DeclaringType, capitalizedName, property.PropertyType, specification);
            FindAndRemoveChoicesMethod(facets, methodRemover, property.DeclaringType, capitalizedName, property.PropertyType, specification);
            FindAndRemoveDefaultMethod(facets, methodRemover, property.DeclaringType, capitalizedName, property.PropertyType, specification);
            FindAndRemoveValidateMethod(facets, methodRemover, property.DeclaringType, paramTypes, capitalizedName, specification);

            AddHideForSessionFacetNone(facets, specification);
            AddDisableForSessionFacetNone(facets, specification);
            FindDefaultHideMethod(facets, methodRemover, property.DeclaringType, MethodType.Object, "PropertyDefault", new Type[0], specification);
            FindAndRemoveHideMethod(facets, methodRemover, property.DeclaringType, MethodType.Object, capitalizedName, property.PropertyType, specification);
            FindDefaultDisableMethod(facets, methodRemover, property.DeclaringType, MethodType.Object, "PropertyDefault", new Type[0], specification);
            FindAndRemoveDisableMethod(facets, methodRemover, property.DeclaringType, MethodType.Object, capitalizedName, property.PropertyType, specification);

            return FacetUtils.AddFacets(facets);
        }

        private void FindAndRemoveModifyMethod(ICollection<IFacet> propertyFacets,
                                               IMethodRemover methodRemover,
                                               Type type,
                                               string capitalizedName,
                                               Type[] parms,
                                               ISpecification property) {
            MethodInfo method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.ModifyPrefix + capitalizedName, typeof (void), parms);
            RemoveMethod(methodRemover, method);
            if (method != null) {
                propertyFacets.Add(new PropertySetterFacetViaModifyMethod(method, property));
            }
        }

        private void FindAndRemoveClearMethod(ICollection<IFacet> propertyFacets, IMethodRemover methodRemover, Type type, string capitalizedName, ISpecification property) {
            MethodInfo method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.ClearPrefix + capitalizedName, typeof (void), Type.EmptyTypes);
            RemoveMethod(methodRemover, method);
            if (method != null) {
                Log.WarnFormat(@"'Clear' method '{0}' has been found on '{1}' : The 'Clear' method is considered obsolete, use 'Modify' instead", PrefixesAndRecognisedMethods.ClearPrefix + capitalizedName, type.FullName);
                propertyFacets.Add(new PropertyClearFacetViaClearMethod(method, property));
            }
        }


        private void FindAndRemoveValidateMethod(ICollection<IFacet> propertyFacets, IMethodRemover methodRemover, Type type, Type[] parms, string capitalizedName, ISpecification property) {
            MethodInfo method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.ValidatePrefix + capitalizedName, typeof (string), parms);
            RemoveMethod(methodRemover, method);
            if (method != null) {
                propertyFacets.Add(new PropertyValidateFacetViaMethod(method, property));
                AddAjaxFacet(method, property);
            }
            else {
                AddAjaxFacet(null, property);
            }
        }

        private void FindAndRemoveDefaultMethod(ICollection<IFacet> propertyFacets,
                                                IMethodRemover methodRemover,
                                                Type type,
                                                string capitalizedName,
                                                Type returnType,
                                                ISpecification property) {
            MethodInfo method = FindMethod(type, MethodType.Object, PrefixesAndRecognisedMethods.DefaultPrefix + capitalizedName, returnType, Type.EmptyTypes);
            RemoveMethod(methodRemover, method);
            if (method != null) {
                propertyFacets.Add(new PropertyDefaultFacetViaMethod(method, property));
                AddOrAddToExecutedWhereFacet(method, property);
            }
        }

        private void FindAndRemoveChoicesMethod(ICollection<IFacet> propertyFacets,
                                                IMethodRemover methodRemover,
                                                Type type,
                                                string capitalizedName,
                                                Type returnType,
                                                ISpecification property) {
            MethodInfo[] methods = FindMethods(type,
                MethodType.Object,
                PrefixesAndRecognisedMethods.ChoicesPrefix + capitalizedName,
                typeof (IEnumerable<>).MakeGenericType(returnType));

            if (methods.Length > 1) {
                methods.Skip(1).ForEach(m => Log.WarnFormat("Found multiple choices methods: {0} in type: {1} ignoring method(s) with params: {2}",
                    PrefixesAndRecognisedMethods.ChoicesPrefix + capitalizedName,
                    type,
                    m.GetParameters().Select(p => p.Name).Aggregate("", (s, t) => s + " " + t)));
            }

            MethodInfo method = methods.FirstOrDefault();
            RemoveMethod(methodRemover, method);
            if (method != null) {
                var parameterNamesAndTypes = method.GetParameters().Select(p => new Tuple<string, IObjectSpecImmutable>(p.Name.ToLower(), Reflector.LoadSpecification(p.ParameterType))).ToArray();
                propertyFacets.Add(new PropertyChoicesFacetViaMethod(method, parameterNamesAndTypes, property));
                AddOrAddToExecutedWhereFacet(method, property);
            }
        }

        private void FindAndRemoveAutoCompleteMethod(ICollection<IFacet> propertyFacets,
                                                     IMethodRemover methodRemover,
                                                     Type type,
                                                     string capitalizedName,
                                                     Type returnType,
                                                     ISpecification property) {
            // only support if property is string or domain type
            if (returnType.IsClass || returnType.IsInterface) {
                MethodInfo method = FindMethod(type,
                    MethodType.Object,
                    PrefixesAndRecognisedMethods.AutoCompletePrefix + capitalizedName,
                    typeof (IQueryable<>).MakeGenericType(returnType),
                    new[] {typeof (string)});

                if (method != null) {
                    var pageSizeAttr = AttributeUtils.GetCustomAttribute<PageSizeAttribute>(method);
                    var minLengthAttr = (MinLengthAttribute) Attribute.GetCustomAttribute(method.GetParameters().First(), typeof (MinLengthAttribute));

                    int pageSize = pageSizeAttr != null ? pageSizeAttr.Value : 0; // default to 0 ie system default
                    int minLength = minLengthAttr != null ? minLengthAttr.Length : 0;

                    RemoveMethod(methodRemover, method);
                    propertyFacets.Add(new AutoCompleteFacetViaMethod(method, pageSize, minLength, property));
                    AddOrAddToExecutedWhereFacet(method, property);
                }
            }
        }


        public override void FindProperties(IList<PropertyInfo> candidates, IList<PropertyInfo> methodListToAppendTo) {
            foreach (PropertyInfo property in candidates) {
                if (property.GetGetMethod() != null &&
                    AttributeUtils.GetCustomAttribute<NakedObjectsIgnoreAttribute>(property) == null &&
                    !CollectionUtils.IsQueryable(property.PropertyType)) {
                    methodListToAppendTo.Add(property);
                }
            }
            candidates.Clear();
        }
    }
}