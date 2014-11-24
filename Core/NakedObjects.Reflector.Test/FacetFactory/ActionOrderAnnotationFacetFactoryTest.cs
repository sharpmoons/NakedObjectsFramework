// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestFixture]
    public class ActionOrderAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new ActionOrderAnnotationFacetFactory();
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private ActionOrderAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IActionOrderFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [ActionOrder("foo,bar")]
        private class Customer {}

        [Test]
        public void TestActionOrderAnnotationPickedUpOnClass() {
            facetFactory.Process(Reflector, typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IActionOrderFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ActionOrderFacetAnnotation);
            var actionOrderFacetAnnotation = (ActionOrderFacetAnnotation) facet;
            Assert.AreEqual("foo,bar", actionOrderFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Property));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Action));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameter));
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}