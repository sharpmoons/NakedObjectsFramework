// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.SpecImmutable;
using NakedObjects.Reflect.Component;

namespace NakedObjects.Reflect.Test {
    public class TestDomainObject {
        public void Action(DateTime? test) {}
    }

    [TestClass]
    public class ReflectorObjectTest : AbstractReflectorTest {
        protected override IObjectSpecImmutable LoadSpecification(Reflector reflector) {
            return reflector.LoadSpecification<IObjectSpecImmutable>(typeof (TestDomainObject));
        }

        [TestMethod]
        public void TestCollectionFacet() {
            IFacet facet = Specification.GetFacet(typeof (ICollectionFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestDescriptionFaced() {
            IFacet facet = Specification.GetFacet(typeof (IDescribedAsFacet));
            Assert.IsNotNull(facet);
        }

        [TestMethod]
        public void TestFacets() {
            Assert.AreEqual(21, Specification.FacetTypes.Length);
        }

        [TestMethod]
        public void TestName() {
            Assert.AreEqual(typeof (TestDomainObject).FullName, Specification.FullName);
        }

        [TestMethod]
        public void TestNamedFaced() {
            IFacet facet = Specification.GetFacet(typeof (INamedFacet));
            Assert.IsNotNull(facet);
        }

        [TestMethod]
        public void TestNoCollectionFacet() {
            IFacet facet = Specification.GetFacet(typeof (ICollectionFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestNoTypeOfFacet() {
            var facet = (ITypeOfFacet) Specification.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNull(facet);
        }

        [TestMethod]
        public void TestPluralFaced() {
            IFacet facet = Specification.GetFacet(typeof (IPluralFacet));
            Assert.IsNotNull(facet);
        }

        [TestMethod]
        public void TestType() {
            Assert.IsTrue(Specification.IsObject);
        }

        [TestMethod]
        public void TestTypeOfFacet() {
            var facet = (ITypeOfFacet) Specification.GetFacet(typeof (ITypeOfFacet));
            Assert.IsNull(facet);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}