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
    public class ImmutableAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();

            facetFactory = new ImmutableAnnotationFacetFactory();
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private ImmutableAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (IImmutableFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        public void TestImmutableAnnotationPickedUpOnClassAndDefaultsToAlways() {
            facetFactory.Process(Reflector, typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            var immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(WhenTo.Always, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        public void TestImmutableAnnotationAlwaysPickedUpOnClass() {
            facetFactory.Process(Reflector, typeof (Customer1), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            var immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(WhenTo.Always, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        public void TestImmutableAnnotationNeverPickedUpOnClass() {
            facetFactory.Process(Reflector, typeof (Customer2), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            var immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(WhenTo.Never, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        [Immutable]
        private class Customer {}

        [Immutable(WhenTo.Always)]
        private class Customer1 {}

        [Immutable(WhenTo.Never)]
        private class Customer2 {}

        [Immutable(WhenTo.OncePersisted)]
        private class Customer3 {}

        [Immutable(WhenTo.UntilPersisted)]
        private class Customer4 {}

        [Test]
        public override void TestFeatureTypes() {
            FeatureType featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(featureTypes.HasFlag(FeatureType.Objects));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Property));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Collections));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.Action));
            Assert.IsFalse(featureTypes.HasFlag(FeatureType.ActionParameter));
        }

        [Test]
        public void TestImmutableAnnotationOncePersistedPickedUpOnClass() {
            facetFactory.Process(Reflector, typeof (Customer3), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            var immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(WhenTo.OncePersisted, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }

        [Test]
        public void TestImmutableAnnotationUntilPersistedPickedUpOnClass() {
            facetFactory.Process(Reflector, typeof (Customer4), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (IImmutableFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is ImmutableFacetAnnotation);
            var immutableFacetAnnotation = (ImmutableFacetAnnotation) facet;
            Assert.AreEqual(WhenTo.UntilPersisted, immutableFacetAnnotation.Value);
            AssertNoMethodsRemoved();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}