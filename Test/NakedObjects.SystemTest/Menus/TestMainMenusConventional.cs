﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity;
using System.Linq;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Architecture.Configuration;
using NakedObjects.Architecture.Menu;
using NakedObjects.Core.Configuration;
using NakedObjects.Menu;
using NakedObjects.Meta.Menu;
using NakedObjects.Xat;
using TestObjectMenu;

namespace NakedObjects.SystemTest.Menus.Service2 {
    [TestClass]
    public class TestMainMenusConventional : AbstractSystemTest<MenusDbContext> {
        [TestMethod]
        public virtual void TestMainMenuCount() {
            AssertMainMenuCountIs(7);
        }

        [TestMethod]
        public virtual void TestAllMainMenus() {
            var menus = AllMainMenus();
            Assert.AreEqual(menus.Count(), menus.OfType<ITestMenu>().Count());
        }

        [TestMethod]
        public void TestGetMainMenu() {
            GetMainMenu("Foo Service").AssertNameEquals("Foo Service");
        }

        [TestMethod]
        public virtual void TestMainMenuNames() {
            var menus = AllMainMenus();

            menus[0].AssertNameEquals("Foo Service");
            menus[1].AssertNameEquals("Bars"); //Picks up Named attribute on service
            menus[2].AssertNameEquals("Qs"); //Named attribute overridden in menu construction
        }

        [TestMethod]
        public virtual void TestAddAllActions() {
            var foo = GetMainMenu("Foo Service");
            foo.AssertItemCountIs(3);
            Assert.AreEqual(3, foo.AllItems().OfType<ITestMenuItem>().Count());

            foo.AllItems()[0].AssertNameEquals("Foo Action0");
            foo.AllItems()[1].AssertNameEquals("Foo Action1");
            foo.AllItems()[2].AssertNameEquals("Foo Action2");
        }

        [TestMethod]
        public virtual void TestAddingActionsToAMenu() {
            var q = GetMainMenu("Qs");
            q.AssertItemCountIs(4);

            q.AllItems()[0].AssertIsAction().AssertNameEquals("Qux Action0");
            q.AllItems()[1].AssertIsAction().AssertNameEquals("Qux Action3");
            q.AllItems()[2].AssertIsAction().AssertNameEquals("Qux Action1");
            q.AllItems()[3].AssertIsAction().AssertNameEquals("Qux Action2");
        }

        [TestMethod]
        public virtual void TestAddingSubMenuToAMenu() {
            var subs = GetMainMenu("Subs");
            subs.AssertItemCountIs(2);
            var sub1 = subs.AllItems()[0].AssertIsSubMenu().AssertNameEquals("Sub1").AsSubMenu();
            sub1.AssertItemCountIs(2);
            sub1.AllItems()[0].AssertIsAction().AssertNameEquals("Action1");
            sub1.AllItems()[1].AssertIsAction().AssertNameEquals("Action3");

            var sub2 = subs.AllItems()[1].AssertIsSubMenu().AssertNameEquals("Sub2").AsSubMenu();
            sub2.AssertItemCountIs(2);
            sub2.AllItems()[0].AssertIsAction().AssertNameEquals("Action2");
            sub2.AllItems()[1].AssertIsAction().AssertNameEquals("Action0");
        }

        [TestMethod]
        public virtual void TestAddAllActionsRecognisesMemberOrder() {
            var bars = GetMainMenu("Bars");
            bars.AssertItemCountIs(4);

            bars.AllItems()[0].AssertIsAction().AssertNameEquals("Bar Action1");
            bars.AllItems()[1].AssertIsAction().AssertNameEquals("Bar Action0");
            bars.AllItems()[2].AssertIsAction().AssertNameEquals("Bar Action2");
            bars.AllItems()[3].AssertIsAction().AssertNameEquals("Bar Action3");
        }

        [TestMethod]
        public void TestHybridMenu() {
            var hyb = GetMainMenu("Hybrid");
            hyb.AssertItemCountIs(6);

            hyb.AllItems()[0].AssertIsAction().AssertNameEquals("Foo Action0");
            hyb.AllItems()[1].AssertIsAction().AssertNameEquals("Bar Action0");
            hyb.AllItems()[2].AssertIsAction().AssertNameEquals("Qux Action0");
            hyb.AllItems()[3].AssertIsAction().AssertNameEquals("Qux Action1");
            hyb.AllItems()[4].AssertIsAction().AssertNameEquals("Qux Action2");
            hyb.AllItems()[5].AssertIsAction().AssertNameEquals("Qux Action3");
        }

        [TestMethod]
        public virtual void TestMenuWithNoActions() {
            var e = GetMainMenu("Empty");
            e.AssertItemCountIs(0);
        }

        [TestMethod]
        public virtual void TestSubMenuWithNoActions() {
            var e = GetMainMenu("Empty2");
            e.AssertItemCountIs(1);
            e.AllItems()[0].AssertIsSubMenu().AssertNameEquals("Sub").AsSubMenu().AssertItemCountIs(0);
        }

        [TestMethod]
        public virtual void TestActionVisibility() {
            var q = GetMainMenu("Qs");
            q.AssertItemCountIs(4);

            q.GetAction("Qux Action0").AssertIsVisible();
            q.GetAction("Qux Action3").AssertIsInvisible();
        }

        #region Setup/Teardown

        [ClassInitialize]
        public static void ClassInitialize(TestContext tc) {
            Database.Delete(MenusDbContext.DatabaseName);
            var context = Activator.CreateInstance<MenusDbContext>();

            context.Database.Create();
        }

        [ClassCleanup]
        public static void ClassCleanup() {
            CleanupNakedObjectsFramework(new TestMainMenusConventional());
        }

        [TestInitialize()]
        public void TestInitialize() {
            InitializeNakedObjectsFrameworkOnce();
            StartTest();
        }

        [TestCleanup()]
        public void TestCleanup() {}

        #endregion

        #region System Config

        protected override object[] SystemServices {
            get {
                return new object[] {
                    new FooService(),
                    new ServiceWithSubMenus(),
                    new BarService(),
                    new QuxService()
                };
            }
        }

        protected override void RegisterTypes(IUnityContainer container) {
            base.RegisterTypes(container);
            container.RegisterType<IMenuFactory, MenuFactory>();
            container.RegisterInstance<IReflectorConfiguration>(MyReflectorConfig(), (new ContainerControlledLifetimeManager()));
        }

        private IReflectorConfiguration MyReflectorConfig() {
            return new ReflectorConfiguration(
                this.Types ?? new Type[] {},
                this.Services,
                Types.Select(t => t.Namespace).Distinct().ToArray(),
                LocalMainMenus.MainMenus);
        }

        #endregion
    }

    #region Classes used in test

    public class LocalMainMenus {
        public static IMenu[] MainMenus(IMenuFactory factory) {
            var foos = factory.NewMenu<FooService>(true);
            var bars = factory.NewMenu<BarService>(true);

            var q = factory.NewMenu<QuxService>(false, "Qs");
            q.AddAction("QuxAction0");
            q.AddAction("QuxAction3");
            q.AddRemainingNativeActions();

            var subs = factory.NewMenu<ServiceWithSubMenus>(false);
            var sub1 = subs.CreateSubMenu("Sub1");
            sub1.AddAction("Action1");
            sub1.AddAction("Action3");
            var sub2 = subs.CreateSubMenu("Sub2");
            sub2.AddAction("Action2");
            sub2.AddAction("Action0");

            var hyb = factory.NewMenu<object>(false, "Hybrid");
            hyb.Type = typeof (FooService);
            hyb.AddAction("FooAction0");
            hyb.Type = typeof (BarService);
            hyb.AddAction("BarAction0");
            hyb.Type = typeof (QuxService);
            hyb.AddRemainingNativeActions();

            var empty = factory.NewMenu<object>(false, "Empty");

            var empty2 = factory.NewMenu<object>(false, "Empty2");
            empty2.CreateSubMenu("Sub");

            return new[] {foos, bars, q, subs, hyb, empty, empty2};
        }
    }

    public class FooService {
        public void FooAction0() {}

        public void FooAction1() {}

        public void FooAction2(string p1, int p2) {}
    }

    [Named("Quxes")]
    public class QuxService {
        public void QuxAction0() {}

        public void QuxAction1() {}

        public void QuxAction2() {}

        [Hidden(WhenTo.Always)]
        public void QuxAction3() {}
    }

    [Named("Subs")]
    public class ServiceWithSubMenus {
        public void Action0() {}

        public void Action1() {}

        public void Action2() {}

        public void Action3() {}
    }

    [Named("Bars")]
    public class BarService {
        [MemberOrder(10)]
        public void BarAction0() {}

        [MemberOrder(1)]
        public void BarAction1() {}

        public void BarAction2() {}

        public void BarAction3() {}
    }

    #endregion
}