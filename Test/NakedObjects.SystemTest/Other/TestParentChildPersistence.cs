// Copyright � Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 
using NakedObjects.Boot;
using NakedObjects.Core.NakedObjectsSystem;
using NakedObjects.Services;
using NakedObjects.Xat;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.Entity;

namespace NakedObjects.SystemTest.ParentChild {
    namespace ParentChild {
        [TestClass, Ignore]
        public class TestParentChildPersistence : AbstractSystemTest<ParentChildDbContext> {

            #region Setup/Teardown
            [ClassInitialize]
            public static void ClassInitialize(TestContext tc)
            {
                InitializeNakedObjectsFramework(new TestParentChildPersistence());
            }

            [ClassCleanup]
            public static void ClassCleanup()
            {
                CleanupNakedObjectsFramework(new TestParentChildPersistence());
                Database.Delete(ParentChildDbContext.DatabaseName);
            }

            [TestInitialize()]
            public void TestInitialize()
            {
                StartTest();
            }

            [TestCleanup()]
            public void TestCleanup()
            {
            }

            #endregion

            protected override IServicesInstaller MenuServices {
                get {
                    return new ServicesInstaller(new object[] {
                                                                  new SimpleRepository<Parent>(),
                                                                  new SimpleRepository<Parent2>(), 
                                                                  new SimpleRepository<Child>()
                                                              });
                }
            }

            [TestMethod]
            public virtual void CannotSaveParentIfChildHasMandatoryFieldsMissing() {

                var parent = GetTestService("Parents").GetAction("New Instance").InvokeReturnObject();
                var parent0 = parent.GetPropertyByName("Prop0").AssertIsMandatory().AssertIsEmpty();
                parent.AssertCannotBeSaved();
                parent0.SetValue("Foo");
                parent.AssertCannotBeSaved();

                var childProp = parent.GetPropertyByName("Child");
                var child = childProp.ContentAsObject;
                child.AssertIsType(typeof(Child));

                var child0 = child.GetPropertyByName("Prop0").AssertIsMandatory().AssertIsEmpty();
                child.AssertCannotBeSaved();
                child0.SetValue("Bar");
                child.AssertCanBeSaved();
                child.Save();

                parent.AssertCanBeSaved();
                parent.Save();
            }
        }

        #region Classes used in tests

        public class ParentChildDbContext : DbContext
        {
            public const string DatabaseName = "TestParentChild";
            public ParentChildDbContext() : base(DatabaseName) { }

            public DbSet<Parent> Parents { get; set; }
            public DbSet<Parent2> Parent2s { get; set; }
            public DbSet<Child> Children { get; set; }

        }

        public class Parent {
            [NakedObjectsIgnore]
            public virtual int Id { get; set; }

            public Parent()
            {
                Child = new Child();
            }

            public virtual string Prop0 { get; set; }

            public virtual  Child Child { get; set; }
           
        }

        public class Parent2
        {
            [NakedObjectsIgnore]
            public virtual int Id { get; set; }

            public virtual string Prop0 { get; set; }

            #region Children (collection)
            private ICollection<Child> myChildren = new List<Child>();

            public virtual ICollection<Child> Children
            {
                get
                {
                    return myChildren;
                }
                set
                {
                    myChildren = value;
                }
            }

            public virtual void AddToChildren(Child value)
            {
                if (!(myChildren.Contains(value)))
                {
                    myChildren.Add(value);
                }
            }

            public virtual void RemoveFromChildren(Child value)
            {
                if (myChildren.Contains(value))
                {
                    myChildren.Remove(value);
                }
            }
            #endregion


        }

        public class Child
        {
            [NakedObjectsIgnore]
            public virtual int Id { get; set; }

            public virtual string Prop0 { get; set; }

        }
#endregion
    }
} 