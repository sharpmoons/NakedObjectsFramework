﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace NakedObjects.Web.UnitTests.Selenium {

    public abstract class KeyboardNavigationTests : AWTest {

        [TestMethod, Ignore] //Doesn't work with Firefox
        public void SelectFooterIconsWithAccessKeys()
        {
            GeminiUrl("home");
            WaitForView(Pane.Single, PaneType.Home);
            WaitForCss(".header .title").Click();
            var element = br.SwitchTo().ActiveElement();
            element.SendKeys(Keys.Alt + "h");
            element = br.SwitchTo().ActiveElement();
            Assert.AreEqual("Home (Alt-h)", element.GetAttribute("title"));
        }

        [TestMethod]
        public void EnterEquivalentToLeftClick()
        {
            GeminiUrl("object?object1=AdventureWorksModel.Store-350&actions1=open");
            WaitForView(Pane.Single, PaneType.Object, "Twin Cycles");
            var reference = GetReferenceProperty("Sales Person", "Lynn Tsoflias");
            reference.SendKeys(Keys.Enter);
            WaitForView(Pane.Single, PaneType.Object, "Lynn Tsoflias");
        }

        [TestMethod] 
        public virtual void ShiftEnterEquivalentToRightClick()
        {
            Url(CustomersMenuUrl);
            WaitForView(Pane.Single, PaneType.Home, "Home");
            wait.Until(d => d.FindElements(By.CssSelector(".action")).Count == CustomerServiceActions);
            OpenActionDialog("Find Customer By Account Number");
            ClearFieldThenType(".value  input","AW00022262");
            OKButton().SendKeys(Keys.Shift + Keys.Enter);
            WaitForView(Pane.Left, PaneType.Home, "Home");
            WaitForView(Pane.Right, PaneType.Object, "Marcus Collins, AW00022262");
        }


    }

    #region browsers specific subclasses 

       // [TestClass, Ignore]
    public class KeyboardNavigationTestsIe : KeyboardNavigationTests
    {
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

   [TestClass]
    public class KeyboardNavigationTestsFirefox : KeyboardNavigationTests
    {
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
    public class KeyboardNavigationTestsChrome : KeyboardNavigationTests
    {
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
}