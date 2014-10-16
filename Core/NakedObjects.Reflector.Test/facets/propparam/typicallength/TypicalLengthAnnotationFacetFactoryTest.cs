// Copyright � Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Reflection;
using NakedObjects.Architecture.Facets;
using NakedObjects.Architecture.Facets.Objects.TypicalLength;
using NakedObjects.Architecture.Reflect;
using NUnit.Framework;

namespace NakedObjects.Reflector.DotNet.Facets.Propparam.TypicalLength {
    [TestFixture]
    public class TypicalLengthAnnotationFacetFactoryTest : AbstractFacetFactoryTest {
        #region Setup/Teardown

        [SetUp]
        public override void SetUp() {
            base.SetUp();
            facetFactory = new TypicalLengthAnnotationFacetFactory(Reflector);
        }

        [TearDown]
        public override void TearDown() {
            facetFactory = null;
            base.TearDown();
        }

        #endregion

        private TypicalLengthAnnotationFacetFactory facetFactory;

        protected override Type[] SupportedTypes {
            get { return new[] {typeof (ITypicalLengthFacet)}; }
        }

        protected override IFacetFactory FacetFactory {
            get { return facetFactory; }
        }

        [TypicalLength(16)]
        private class Customer {}

        private class Customer1 {
            [TypicalLength(30)]
            public string FirstName {
                get { return null; }
            }
        }

        private class Customer2 {
            public void someAction([TypicalLength(20)] int foo) {}
        }

        [Test]
        public override void TestFeatureTypes() {
            FeatureType[] featureTypes = facetFactory.FeatureTypes;
            Assert.IsTrue(Contains(featureTypes, FeatureType.Objects));
            Assert.IsTrue(Contains(featureTypes, FeatureType.Property));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Collection));
            Assert.IsFalse(Contains(featureTypes, FeatureType.Action));
            Assert.IsTrue(Contains(featureTypes, FeatureType.ActionParameter));
        }

        [Test]
        public void TestTypicalLengthAnnotationPickedUpOnActionParameter() {
            MethodInfo method = FindMethod(typeof (Customer2), "someAction", new[] {typeof (int)});
            facetFactory.ProcessParams(method, 0, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITypicalLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypicalLengthFacetAnnotation);
            var typicalLengthFacetAnnotation = (TypicalLengthFacetAnnotation) facet;
            Assert.AreEqual(20, typicalLengthFacetAnnotation.Value);
        }

        [Test]
        public void TestTypicalLengthAnnotationPickedUpOnClass() {
            facetFactory.Process(typeof (Customer), MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITypicalLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypicalLengthFacetAnnotation);
            var typicalLengthFacetAnnotation = (TypicalLengthFacetAnnotation) facet;
            Assert.AreEqual(16, typicalLengthFacetAnnotation.Value);
        }

        [Test]
        public void TestTypicalLengthAnnotationPickedUpOnProperty() {
            PropertyInfo property = FindProperty(typeof (Customer1), "FirstName");
            facetFactory.Process(property, MethodRemover, Specification);
            IFacet facet = Specification.GetFacet(typeof (ITypicalLengthFacet));
            Assert.IsNotNull(facet);
            Assert.IsTrue(facet is TypicalLengthFacetAnnotation);
            var typicalLengthFacetAnnotation = (TypicalLengthFacetAnnotation) facet;
            Assert.AreEqual(30, typicalLengthFacetAnnotation.Value);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}