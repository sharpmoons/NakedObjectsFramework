﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System.Collections.ObjectModel;
using System.Linq;

namespace NakedObjects.Web.UnitTests.Selenium {

    /// <summary>
    /// Tests only that a given URLs return the correct views. No actions performed on them
    /// </summary>
    /// 
    public abstract class UrlTestsRoot : AWTest
    {
        public virtual void UnrecognisedUrlGoesToHome()
        {
            GeminiUrl("unrecognised");
            WaitForView(Pane.Single, PaneType.Home, "Home");
            Assert.IsTrue(br.FindElements(By.CssSelector(".actions")).Count == 0);
        }
        #region Single pane Urls
        public virtual void Home()
        {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home, "Home");
            Assert.IsTrue(br.FindElements(By.CssSelector(".actions")).Count == 0);
        }
        public virtual void HomeWithMenu()
        {
            GeminiUrl("home?menu1=CustomerRepository");
            WaitForView(Pane.Single, PaneType.Home, "Home");
            wait.Until(d => d.FindElement(By.CssSelector(".actions")));
            ReadOnlyCollection<IWebElement> actions = br.FindElements(By.CssSelector(".action"));

            Assert.AreEqual("Find Customer By Account Number", actions[0].Text);
            Assert.AreEqual("Find Store By Name", actions[1].Text);
            Assert.AreEqual("Create New Store Customer", actions[2].Text);
            Assert.AreEqual("Random Store", actions[3].Text);
            Assert.AreEqual("Find Individual Customer By Name", actions[4].Text);
            Assert.AreEqual("Create New Individual Customer", actions[5].Text);
            Assert.AreEqual("Random Individual", actions[6].Text);
            Assert.AreEqual("Customer Dashboard", actions[7].Text);
            Assert.AreEqual("Throw Domain Exception", actions[8].Text);
        }
        public virtual void Object()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Store-350");
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
            wait.Until(d => d.FindElement(By.CssSelector(".view")));
            AssertObjectElementsPresent();
        }
        private void AssertObjectElementsPresent()
        {
            wait.Until(d => d.FindElement(By.CssSelector(".single")));
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
            wait.Until(d => d.FindElement(By.CssSelector(".view")));
            wait.Until(d => d.FindElement(By.CssSelector(".header")));
            wait.Until(d => d.FindElement(By.CssSelector(".menu")).Text == "Actions");
            wait.Until(d => d.FindElement(By.CssSelector(".main-column")));
            wait.Until(d => d.FindElement(By.CssSelector(".collections")));

            Assert.IsTrue(br.FindElements(By.CssSelector(".error")).Count == 0);

        }
        public virtual void ObjectWithNoSuchObject()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Foo-555");
            wait.Until(d => d.FindElement(By.CssSelector(".error")));
        }
        public virtual void ObjectWithActions()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Store-350&actions1=open");
            GetObjectAction("Create New Address");
            AssertObjectElementsPresent();
        }
        //TODO:  Need to add tests for object & home (later, list) with action (dialog) open
        public virtual void ObjectWithCollections()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Store-350&&collection1_Addresses=List&collection1_Contacts=Table");
            wait.Until(d => d.FindElement(By.CssSelector(".collections")));
            AssertObjectElementsPresent();
            wait.Until(d => d.FindElements(By.CssSelector(".collection")).Count == 2);
            var collections = br.FindElements(By.CssSelector(".collection"));
            wait.Until(d => d.FindElements(By.CssSelector(".collection")).First().FindElement(By.TagName("table")));
            //Assert.IsNotNull(collections[0].FindElement(By.TagName("table")));

            wait.Until(d => d.FindElements(By.CssSelector(".collection")).First().FindElement(By.CssSelector(".icon-table")));
            //Assert.IsNotNull(collections[0].FindElement(By.CssSelector(".icon-table")));
            wait.Until(d => d.FindElements(By.CssSelector(".collection")).First().FindElement(By.CssSelector(".icon-summary")));

            //Assert.IsNotNull(collections[0].FindElement(By.CssSelector(".icon-summary")));
            wait.Until(d => d.FindElements(By.CssSelector(".collection")).First().FindElements(By.CssSelector(".icon-list")).Count == 0);

            //Assert.IsTrue(collections[0].FindElements(By.CssSelector(".icon-list")).Count == 0);
        }
        public virtual void ObjectInEditMode()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Store-350&edit1=true");
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
            wait.Until(d => d.FindElement(By.CssSelector(".edit")));
            SaveButton();
            GetCancelEditButton();
            // AssertObjectElementsPresent();
        }
        public virtual void ListZeroParameterAction()
        {
            GeminiUrl("list?menu1=OrderRepository&action1=HighestValueOrders");
            //todo: test that title is correct
            Reload(Pane.Single);
            wait.Until(d => d.FindElement(By.CssSelector(".list")));
            WaitForView(Pane.Single, PaneType.List, "Highest Value Orders");
        }
        #endregion
        #region Split pane Urls
        public virtual void SplitHomeHome()
        {
            GeminiUrl("home/home");
            WaitForView(Pane.Left, PaneType.Home, "Home");
            WaitForView(Pane.Right, PaneType.Home, "Home");
        }
        public virtual void SplitHomeObject()
        {
            GeminiUrl("home/object?object2=AdventureWorksModel.Store-350");
            WaitForView(Pane.Left, PaneType.Home, "Home");
            WaitForView(Pane.Right, PaneType.Object, "Twin Cycles");
        }
        public virtual void SplitHomeList()
        {
            GeminiUrl("home/list?&menu2=OrderRepository&action2=HighestValueOrders");
            WaitForView(Pane.Left, PaneType.Home, "Home");
            WaitForView(Pane.Right, PaneType.List, "Highest Value Orders");
            Reload(Pane.Right);
            wait.Until(dr => dr.FindElement(By.CssSelector("#pane2 .summary .details")).Text == "Page 1 of 1574; viewing 20 of 31465 items");
        }
        public virtual void SplitObjectHome()
        {
            GeminiUrl("object/home?object1=AdventureWorksModel.Store-350");
            WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
            WaitForView(Pane.Right, PaneType.Home, "Home");
        }
        public virtual void SplitObjectObject()
        {
            GeminiUrl("object/object?object1=AdventureWorksModel.Store-350&object2=AdventureWorksModel.Store-604");
            WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
            WaitForView(Pane.Right, PaneType.Object, "Mechanical Sports Center");
        }
        public virtual void SplitObjectList()
        {
            GeminiUrl("object/list?object1=AdventureWorksModel.Store-350&menu2=OrderRepository&action2=HighestValueOrders");
            WaitForView(Pane.Left, PaneType.Object, "Twin Cycles");
            Reload(Pane.Right);
            WaitForView(Pane.Right, PaneType.List, "Highest Value Orders");
        }
        public virtual void SplitListHome()
        {
            GeminiUrl("list/home?menu1=OrderRepository&action1=HighestValueOrders");
            Reload(Pane.Left);
            WaitForView(Pane.Left, PaneType.List, "Highest Value Orders");
            WaitForView(Pane.Right, PaneType.Home, "Home");
        }
        public virtual void SplitListObject()
        {
            GeminiUrl("list/object?menu1=OrderRepository&action1=HighestValueOrders&object2=AdventureWorksModel.Store-604");
            Reload(Pane.Left);
            WaitForView(Pane.Left, PaneType.List, "Highest Value Orders");
            WaitForView(Pane.Right, PaneType.Object, "Mechanical Sports Center");
        }
        public virtual void SplitListList()
        {
            GeminiUrl("list/list?menu2=PersonRepository&parm2_firstName=%2522a%2522&parm2_lastName=%2522a%2522&action2=FindContactByName&page2=1&pageSize2=20&selected2=0&menu1=SpecialOfferRepository&action1=CurrentSpecialOffers&page1=1&pageSize1=20&selected1=0");
            Reload(Pane.Left);
            WaitForView(Pane.Left, PaneType.List, "Current Special Offers");
            Reload(Pane.Right);
            WaitForView(Pane.Right, PaneType.List, "Find Contact By Name");
        }
        #endregion
    }
    public abstract class UrlTests : UrlTestsRoot
    {
        [TestMethod]
        public override void UnrecognisedUrlGoesToHome()
        {
            base.UnrecognisedUrlGoesToHome();
        }

        #region Single pane Urls
        [TestMethod]
        public override void Home()
        {
            base.Home();
        }

        [TestMethod]
        public override void HomeWithMenu()
        {
            base.HomeWithMenu();
        }
       [TestMethod]
        public override void Object()
        {
            base.Object();
        }
        private void AssertObjectElementsPresent()
        {
            wait.Until(d => d.FindElement(By.CssSelector(".single")));
            wait.Until(d => d.FindElement(By.CssSelector(".object")));
            wait.Until(d => d.FindElement(By.CssSelector(".view")));
            wait.Until(d => d.FindElement(By.CssSelector(".header")));
            wait.Until(d => d.FindElement(By.CssSelector(".menu")).Text == "Actions");
            wait.Until(d => d.FindElement(By.CssSelector(".main-column")));
            wait.Until(d => d.FindElement(By.CssSelector(".collections")));

            Assert.IsTrue(br.FindElements(By.CssSelector(".error")).Count == 0);

        }

        [TestMethod]
        public override void ObjectWithNoSuchObject()
        {
            base.ObjectWithNoSuchObject();
        }

        [TestMethod]
        public override void ObjectWithActions()
        {
            base.ObjectWithActions();
        }

        [TestMethod]
        public override void ObjectWithCollections()
        {
            base.ObjectWithCollections();
        }

        [TestMethod]
        public override void ObjectInEditMode()
        {
            base.ObjectInEditMode();
        }

        [TestMethod]
        public override void ListZeroParameterAction()
        {
            base.ListZeroParameterAction();
        }
        #endregion

        #region Split pane Urls

        [TestMethod]
        public override void SplitHomeHome()
        {
            base.SplitHomeHome();
        }

        [TestMethod]
        public override void SplitHomeObject()
        {
            base.SplitHomeObject();
        }

        [TestMethod]
        public override void SplitHomeList()
        {
            base.SplitHomeList();        }

        [TestMethod]
        public override void SplitObjectHome()
        {
            base.SplitObjectHome();
        }

        [TestMethod]
        public override void SplitObjectObject()
        {
            base.SplitObjectObject();
        }

        [TestMethod]
        public override void SplitObjectList()
        {
            base.SplitObjectList();
        }

        [TestMethod]
        public override void SplitListHome()
        {
            base.SplitListHome();
        }

        [TestMethod] 
        public override void SplitListObject()
        {
            base.SplitListObject();
        }

        [TestMethod]
        public override void SplitListList()
        {
            base.SplitListList();
        }
        #endregion

    }


    #region browsers specific subclasses 

   // [TestClass, Ignore]
    public class UrlTestsIe : UrlTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.IEDriverServer.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitIeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

    //[TestClass]
    public class UrlTestsFirefox : UrlTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitFirefoxDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }
    }

   // [TestClass, Ignore]
    public class UrlTestsChrome : UrlTests {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context) {
            FilePath(@"drivers.chromedriver.exe");
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest() {
            InitChromeDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest() {
            base.CleanUpTest();
        }

        protected override void ScrollTo(IWebElement element) {
            string script = string.Format("window.scrollTo(0, {0})", element.Location.Y);
            ((IJavaScriptExecutor) br).ExecuteScript(script);
        }
    }

    #endregion

    #region Running all tests in one go
    [TestClass]
    public class UrlMegaTestFirefox : UrlTestsRoot
    {
        [ClassInitialize]
        public new static void InitialiseClass(TestContext context)
        {
            AWTest.InitialiseClass(context);
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitFirefoxDriver();
            Url(BaseUrl);
        }

        [TestCleanup]
        public virtual void CleanupTest()
        {
            base.CleanUpTest();
        }

        [TestMethod]
        public virtual void MegaUrlTest()
        {
            Home();
            UnrecognisedUrlGoesToHome();
            HomeWithMenu();
            Object();
            ObjectInEditMode();
            ObjectWithActions();
            ObjectWithCollections();
            ObjectWithNoSuchObject();
            SplitHomeHome();
            SplitHomeObject();
            SplitHomeList();
            SplitObjectHome();
            SplitObjectObject();
            SplitObjectList();
            SplitListHome();
            SplitListObject();
            SplitListList();
        }
    }
    #endregion
}