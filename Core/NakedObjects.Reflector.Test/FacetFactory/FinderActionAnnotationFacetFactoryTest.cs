// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestClass]
    public class FinderActionAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        private FinderActionFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IFinderActionFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        #region Setup/Teardown

        [TestInitialize]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new FinderActionFacetFactory(0);
        }

        [TestCleanup]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private class Customer {
// ReSharper disable once UnusedMember.Local
            public void SomeAction() {}
        }

        private class Customer1 {
            [FinderAction]
// ReSharper disable once UnusedMember.Local
            public void SomeAction() {}
        }

        [TestMethod]
        public void TestFinderActionFacetNullByDefault() {
            MethodInfo actionMethod = FindMethod(typeof (Customer), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IExecutedFacet));
            Assert.IsNull(facet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public void TestFinderActionAnnotationPickedUp() {
            MethodInfo actionMethod = FindMethod(typeof (Customer1), "SomeAction");
            facetFactory.Process(Reflector, actionMethod, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IFinderActionFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is FinderActionFacet);
            AssertNoMethodsRemoved();
        }

        [TestMethod]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Properties));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Actions));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameters));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}