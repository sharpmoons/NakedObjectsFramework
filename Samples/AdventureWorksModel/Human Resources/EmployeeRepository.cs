// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NakedObjects;
using NakedObjects.Services;
using System;

namespace AdventureWorksModel {
    [DisplayName("Employees")]
    public class EmployeeRepository : AbstractFactoryAndRepository {
        #region Injected Services
        public PersonRepository ContactRepository { set; protected get; }
        #endregion

        #region FindRecentHires
        //This method exists for test purposes only, to test that a hidden Finder Action does not
        //show up in the Find Menu
        [Hidden(WhenTo.Always)]
        [FinderAction]
        public IQueryable<Employee> FindRecentHires() {
            throw new NotImplementedException(); //Deliberately not implemented
        }
        #endregion

        #region FindEmployeeByName

        [FinderAction]
        [TableView(true, "Current", "Title", "Manager")]
        public IQueryable<Employee> FindEmployeeByName([Optionally] string firstName, string lastName) {
            IQueryable<Person> matchingContacts = ContactRepository.FindContactByName(firstName, lastName);

            IQueryable<Employee> query = from emp in Instances<Employee>()
                from contact in matchingContacts
                where emp.PersonDetails.BusinessEntityID == contact.BusinessEntityID
                orderby emp.PersonDetails.LastName
                select emp;

            return query;
        }

        #endregion

        #region FindEmployeeByNationalIDNumber
        [FinderAction]
        [QueryOnly]
        public Employee FindEmployeeByNationalIDNumber(string nationalIDNumber) {
            IQueryable<Employee> query = from obj in Container.Instances<Employee>()
                where obj.NationalIDNumber == nationalIDNumber
                select obj;

            return SingleObjectWarnIfNoMatch(query);
        }

        #endregion

        public Employee CreateNewEmployeeFromContact([ContributedAction("Employees")] [FindMenu] Person contactDetails) {
            var _Employee = Container.NewTransientInstance<Employee>();
            _Employee.BusinessEntityID = contactDetails.BusinessEntityID;
            return _Employee;
        }

        [PageSize(20)]
        public IQueryable<Person> AutoComplete0CreateNewEmployeeFromContact([MinLength(2)] string name) {
            return Container.Instances<Person>().Where(p => p.LastName.ToUpper().StartsWith(name.ToUpper()));
        }

        [FinderAction]
        [Eagerly(EagerlyAttribute.Do.Rendering)]
        [TableView(true, "GroupName")]
        public IQueryable<Department> ListAllDepartments() {
            return Container.Instances<Department>();
        }

        [NakedObjectsIgnore]
        public virtual Employee CurrentUserAsEmployee() {
            IQueryable<Employee> query = from obj in Container.Instances<Employee>()
                where obj.LoginID == "adventure-works\\" + Principal.Identity.Name
                select obj;

            return query.FirstOrDefault();
        }

        [FinderAction]
        [QueryOnly]
        public Employee Me() {
            Employee currentUser = CurrentUserAsEmployee();
            if (currentUser == null) {
                WarnUser("No Employee for current user");
            }
            return currentUser;
        }

        #region RandomEmployee

        [FinderAction]
        [QueryOnly]
        public Employee RandomEmployee() {
            return Random<Employee>();
        }

        #endregion
    }
}