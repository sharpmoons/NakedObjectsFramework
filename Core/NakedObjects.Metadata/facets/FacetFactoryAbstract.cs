// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Collections.Generic;
using System.Reflection;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Facets {
    public abstract class FacetFactoryAbstract : IFacetFactory {
        private readonly NakedObjectFeatureType[] featureTypes;

        protected FacetFactoryAbstract(INakedObjectReflector reflector, NakedObjectFeatureType[] featureTypes) {
            Reflector = reflector;
            this.featureTypes = featureTypes;
        }

        /// <summary>
        ///     Injected
        /// </summary>
        protected INakedObjectReflector Reflector { get; private set; }

        #region IFacetFactory Members

        public virtual NakedObjectFeatureType[] FeatureTypes {
            get { return featureTypes; }
        }

        public virtual bool Process(Type type, IMethodRemover methodRemover, IFacetHolder holder) {
            return false;
        }

        public virtual bool Process(MethodInfo method, IMethodRemover methodRemover, IFacetHolder holder) {
            return false;
        }

        public virtual bool Process(PropertyInfo property, IMethodRemover methodRemover, IFacetHolder holder) {
            return false;
        }

        public virtual bool ProcessParams(MethodInfo method, int paramNum, IFacetHolder holder) {
            return false;
        }

        public virtual void FindCollectionProperties(IList<PropertyInfo> candidates, IList<PropertyInfo> methodListToAppendTo) {}

        public virtual void FindProperties(IList<PropertyInfo> candidates, IList<PropertyInfo> methodListToAppendTo) {}

        #endregion
    }
}