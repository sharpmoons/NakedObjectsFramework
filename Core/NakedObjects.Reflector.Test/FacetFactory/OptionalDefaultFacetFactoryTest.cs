// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Reflection;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.FacetFactory;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Meta.Facet;
using NakedObjects.Reflect.FacetFactory;
using NUnit.Framework;

namespace NakedObjects.Reflect.Test.FacetFactory {
    [TestFixture]
    public class OptionalDefaultFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new OptionalDefaultFacetFactory();
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private OptionalDefaultFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IMandatoryFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        private class Customer1 {
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer2 {
            public void SomeAction(string foo) {}
        }

        private class Customer3 {
            public int NumberOfOrders {
                get { return 0; }
            }
        }

        private class Customer4 {
            public void SomeAction(int foo) {}
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Property));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Action));
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.ActionParameter));
        }

        [Test]
        public void TestOptionalDefaultIgnoredForPrimitiveOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer4), "SomeAction", new[] {typeof (int)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMandatoryFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MandatoryFacetDefault);
        }

        [Test]
        public void TestOptionalDefaultIgnoredForPrimitiveOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer3), "NumberOfOrders");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMandatoryFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is MandatoryFacetDefault);
        }

        [Test]
        public void TestOptionalDefaultPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "SomeAction", new[] {typeof (string)});
            facetFactory.ProcessParams(Reflector, method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMandatoryFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OptionalFacetDefault);
        }

        [Test]
        public void TestOptionalDefaultPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(Reflector, property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IMandatoryFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is OptionalFacetDefault);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}