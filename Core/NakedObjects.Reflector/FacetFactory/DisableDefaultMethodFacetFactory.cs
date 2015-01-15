﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Common.Logging;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Spec;

namespace NakedObjects.Reflect.FacetFactory {
    /// <summary>
    ///     Note - this factory simply removes the class level attribute from the list of methods.  The action and properties look up this attribute directly
    /// </summary>
    public class DisableDefaultMethodFacetFactory : MethodPrefixBasedFacetFactoryAbstract {
        private static readonly string[] FixedPrefixes;
        private static readonly ILog Log = LogManager.GetLogger(typeof (DisableDefaultMethodFacetFactory));

        static DisableDefaultMethodFacetFactory() {
            FixedPrefixes = new[] {
                PrefixesAndRecognisedMethods.DisablePrefix + "Action" + PrefixesAndRecognisedMethods.DefaultPrefix,
                PrefixesAndRecognisedMethods.DisablePrefix + "Property" + PrefixesAndRecognisedMethods.DefaultPrefix
            };
        }

        public DisableDefaultMethodFacetFactory(int numericOrder)
            : base(numericOrder, FeatureType.Objects) {}

        public override string[] Prefixes {
            get { return FixedPrefixes; }
        }

        public override void Process(IReflector reflector, Type type, IMethodRemover methodRemover, ISpecificationBuilder specification) {
            try {
                foreach (string methodName in FixedPrefixes) {
                    MethodInfo methodInfo = FindMethod(reflector, type, MethodType.Object, methodName, typeof (string), Type.EmptyTypes);
                    if (methodInfo != null) {
                        methodRemover.RemoveMethod(methodInfo);
                    }
                }
            }
            catch (Exception e) {
                Log.Warn("Unexpected exception", e);
            }
        }
    }
}