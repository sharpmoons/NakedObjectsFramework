﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NakedObjects.Mvc.Selenium.Test.Helper;
using OpenQA.Selenium;

namespace NakedObjects.Mvc.Selenium.Test {
    public abstract class TabbedHistoryAndClearTests : AWWebTest {
        public abstract void CumulativeHistory();

        private void CustomerByAccountNumber(string accountNumber) {
            var f = wait.ClickAndWait("#CustomerRepository-FindCustomerByAccountNumber button", "#CustomerRepository-FindCustomerByAccountNumber-AccountNumber-Input");

            f.TypeText(accountNumber + Keys.Tab);

            wait.ClickAndWait(".nof-ok", ".nof-objectview");
        }

        public void DoCumulativeHistory() {
            Login();

            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab a")).Text);

            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            // 3rd object
            wait.ClickAndWait("#SalesPerson-SalesTerritory a", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 3);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:nth-of-type(2) a")).Text);
            Assert.AreEqual("Canada", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            // collection 
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 4);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:nth-of-type(2) a")).Text);
            Assert.AreEqual("Canada", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:nth-of-type(3) a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            //Go back to second object
            wait.ClickAndWait(".nof-tab:nth-of-type(2) a", wd => wd.Title == "José Saraiva");

            Assert.AreEqual(4, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:nth-of-type(2) a")).Text);
            Assert.AreEqual("Canada", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:nth-of-type(3) a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            //Go back to first object
            wait.ClickAndWait(".nof-tab:first-of-type a", wd => wd.Title == "Metro Manufacturing, AW00000065");

            Assert.AreEqual(4, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:nth-of-type(2) a")).Text);
            Assert.AreEqual("Canada", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:nth-of-type(3) a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            // same link so just wait
            wait.ClickAndWait(".nof-tab:first-of-type a", wd => {
                Thread.Sleep(1000);
                return true;
            });

            Assert.AreEqual(4, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:nth-of-type(2) a")).Text);
            Assert.AreEqual("Canada", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:nth-of-type(3) a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            //Go to collection
            wait.ClickAndWait(".nof-tab:last-of-type a", wd => wd.Title == "20 Sales Orders");
            Assert.AreEqual(4, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:nth-of-type(2) a")).Text);
            Assert.AreEqual("Canada", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:nth-of-type(3) a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickAndWait(".nof-tab:nth-of-type(3) a", wd => wd.Title == "Canada");

            Assert.AreEqual(4, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:nth-of-type(2) a")).Text);
            Assert.AreEqual("Canada", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:nth-of-type(3) a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);
        }

        public void DoClearSingleItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab a")).Text);

            wait.ClickClearItem(0);
            br.AssertElementDoesNotExist(By.CssSelector(".nof-tabbed-history"));
            Assert.AreEqual("Home Page", br.Title);
        }

        public void DoClearSingleCollectionItem() {
            Login();

            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", ".nof-tabbed-history");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            wait.ClickClearItem(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            Assert.AreEqual("Home Page", br.Title);
        }

        public void DoClearActiveItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab a")).Text);

            // 2nd object
            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickClearItem(1);
            wait.Until(wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 1);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab a")).Text);

            Assert.AreEqual("Metro Manufacturing, AW00000065", br.Title);
        }

        public void DoCollectionKeepsPage() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            // 2nd collection
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            var pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 1 of 1574", pageNo.Text);

            br.FindElement(By.CssSelector("button[title=Last]")).Click();

            wait.ClickAndWait("button[title=Last]", wd => wd.FindElement(By.CssSelector(".nof-page-number")).Text == "Page 1574 of 1574");

            Assert.AreEqual(2, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("5 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickAndWait(".nof-tabbed-history .nof-tab:first-of-type a", wd => wd.Title == "Metro Manufacturing, AW00000065");

            Assert.AreEqual(2, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("5 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickAndWait(".nof-tabbed-history  .nof-tab:last-of-type a", wd => wd.Title == "5 Sales Orders");

            Assert.AreEqual(2, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("5 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            pageNo = br.FindElement(By.ClassName("nof-page-number"));
            Assert.AreEqual("Page 1574 of 1574", pageNo.Text);

            Assert.AreEqual("5 Sales Orders", br.Title);
        }

        public void DoCollectionKeepsFormat() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history a")).Text);

            // 2nd collection
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history a")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickAndWait("button[title=Table]", ".nof-collection-table");

            wait.ClickAndWait(".nof-tabbed-history .nof-tab:first-of-type a", wd => wd.Title == "Metro Manufacturing, AW00000065");

            Assert.AreEqual(2, br.FindElements(By.CssSelector(".nof-tabbed-history a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickAndWait(".nof-tabbed-history .nof-tab:last-of-type a", wd => wd.Title == "20 Sales Orders");

            br.FindElement(By.ClassName("nof-collection-table"));
            Assert.AreEqual(2, br.FindElements(By.CssSelector(".nof-tabbed-history a")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            Assert.AreEqual("20 Sales Orders", br.Title);
        }

        public void DoClearActiveCollectionItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            // 2nd collection
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickClearItem(1);

            wait.Until(wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 1);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            Assert.AreEqual("Metro Manufacturing, AW00000065", br.Title);
        }

        public void DoClearActiveMultipleCollectionItems() {
            Login();

            // 1st collection
            wait.ClickAndWait("#ContactRepository-ValidCountries button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 1);
            Assert.AreEqual("12 Country Regions", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            // 2nd collection
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("12 Country Regions", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickClearItem(1);
            wait.Until(wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 1);
            Assert.AreEqual("12 Country Regions", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            Assert.AreEqual("12 Country Regions", br.Title);
        }

        public void DoClearInActiveItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            // 2nd object
            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickClearItem(0);
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            Assert.AreEqual("José Saraiva", br.Title);
        }

        public void DoClearInActiveCollectionItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            // 2nd object
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickClearItem(0);
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            Assert.AreEqual("20 Sales Orders", br.Title);
        }

        public void DoClearInActiveCollectionMultipleItems() {
            Login();

            wait.ClickAndWait("#ContactRepository-ValidCountries button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 1);

            // 1st collection 
            Assert.AreEqual("12 Country Regions", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            // 2nd collection
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("12 Country Regions", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickClearItem(0);
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            Assert.AreEqual("20 Sales Orders", br.Title);
        }

        public void DoClearOthersSingleItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            wait.ClickClearOthers(0);
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab a")).Text);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.Title);
        }

        public void DoClearOthersSingleCollectionItem() {
            Login();
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 1);

            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            wait.ClickClearOthers(0);
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab a")).Text);
            Assert.AreEqual("20 Sales Orders", br.Title);
        }

        public void DoClearOthersActiveItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            // 2nd object
            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickClearOthers(1);
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            Assert.AreEqual("José Saraiva", br.Title);
        }

        public void DoClearOthersActiveCollectionItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            // 2nd object
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickClearOthers(1);
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            Assert.AreEqual("20 Sales Orders", br.Title);
        }

        public void DoClearOthersInActiveItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            // 2nd object
            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickClearOthers(0);
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            Assert.AreEqual("Metro Manufacturing, AW00000065", br.Title);
        }

        public void DoClearOthersInActiveCollectionItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            // 2nd object
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickClearOthers(0);
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            Assert.AreEqual("Metro Manufacturing, AW00000065", br.Title);
        }

        public void DoClearAllSingleItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            wait.ClickClearAll(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            Assert.AreEqual("Home Page", br.Title);
        }

        public void DoClearAllSingleCollectionItem() {
            Login();
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 1);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            wait.ClickClearAll(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            Assert.AreEqual("Home Page", br.Title);
        }

        public void DoClearAllActiveItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            // 2nd object
            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickClearAll(1);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            Assert.AreEqual("Home Page", br.Title);
        }

        public void DoClearAllActiveCollectionItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            // 2nd object
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickClearAll(1);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            Assert.AreEqual("Home Page", br.Title);
        }

        public void DoClearAllInActiveItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            // 2nd object
            wait.ClickAndWait("#Store-SalesPerson a", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("José Saraiva", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickClearAll(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            Assert.AreEqual("Home Page", br.Title);
        }

        public void DoClearAllInActiveCollectionItem() {
            Login();
            CustomerByAccountNumber("AW00000065");

            // 1st object
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);

            // 2nd object
            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type a")).Text);
            Assert.AreEqual("20 Sales Orders", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type a")).Text);

            wait.ClickClearAll(0);
            br.AssertElementDoesNotExist(By.ClassName("nof-tabbed-history"));
            Assert.AreEqual("Home Page", br.Title);
        }

        public void DoTransientObjectsDoNotShowUpInHistory() {
            Login();
            CustomerByAccountNumber("AW00000065");

            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history a")).Count);

            wait.ClickAndWait("#CustomerRepository-CreateNewStoreCustomer button", ".nof-objectedit");

            IWebElement elem = br.FindElement(By.CssSelector(".nof-objectedit"));
            var cls = elem.GetAttribute("class");
            Assert.IsTrue(cls.Contains("nof-objectedit") && cls.Contains("nof-transient") && cls.Replace("nof-transient", "").Replace("nof-objectedit", "").Trim().Length == 0);

            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history a")).Count);

            br.FindElement(By.CssSelector("#Store-Name-Input")).TypeText("Foo Bar");

            wait.ClickAndWait(".nof-save", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history a")).Count == 2);

            Assert.AreEqual("Metro Manufacturing, AW00000065", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:first-of-type  a")).Text);
            Assert.AreEqual("Foo Bar, AW00029484", br.FindElement(By.CssSelector(".nof-tabbed-history .nof-tab:last-of-type  a")).Text);
        }

        public void DoCollectionsShowUpInHistory() {
            Login();
            CustomerByAccountNumber("AW00000065");
            Assert.AreEqual(1, br.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count);

            wait.ClickAndWait("#OrderRepository-HighestValueOrders button", wd => wd.FindElements(By.CssSelector(".nof-tabbed-history .nof-tab")).Count == 2);
            Assert.AreEqual("20 Sales Orders", br.Title);
        }

        #region abstract

        public abstract void ClearSingleItem();
        public abstract void ClearSingleCollectionItem();
        public abstract void ClearActiveItem();
        public abstract void CollectionsShowUpInHistory();
        public abstract void TransientObjectsDoNotShowUpInHistory();
        public abstract void ClearAllInActiveCollectionItem();
        public abstract void ClearAllInActiveItem();
        public abstract void ClearAllActiveCollectionItem();
        public abstract void ClearAllActiveItem();
        public abstract void ClearAllSingleCollectionItem();
        public abstract void ClearAllSingleItem();
        public abstract void ClearOthersInActiveCollectionItem();
        public abstract void ClearOthersInActiveItem();
        public abstract void ClearOthersActiveCollectionItem();
        public abstract void ClearOthersActiveItem();
        public abstract void ClearOthersSingleCollectionItem();
        public abstract void ClearOthersSingleItem();
        public abstract void ClearInActiveCollectionMultipleItems();
        public abstract void ClearInActiveCollectionItem();
        public abstract void ClearInActiveItem();
        public abstract void ClearActiveMultipleCollectionItems();
        public abstract void ClearActiveCollectionItem();
        public abstract void CollectionKeepsFormat();
        public abstract void CollectionKeepsPage();

        #endregion
    }
}