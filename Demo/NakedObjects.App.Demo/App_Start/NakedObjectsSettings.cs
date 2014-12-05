﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using AdventureWorksModel;
using NakedObjects.Core.Configuration;
using NakedObjects.Persistor.Entity.Configuration;
using NakedObjects.Web.Mvc.Helpers;
using NakedObjects.Web.Mvc.Models;

namespace NakedObjects.App.Demo {
    public static class NakedObjectsSettings {
        

        private static Type[] Types {
            get {
                return new[] {
                    typeof (EntityCollection<object>),
                    typeof (ObjectQuery<object>),
                    typeof (ActionResultModelQ<object>)
                };
            }
        }

        private static Type[] MenuServices {
            get {
                return new[] {
                    typeof (CustomerRepository),
                    typeof (OrderRepository),
                    typeof (ProductRepository),
                    typeof (EmployeeRepository),
                    typeof (SalesRepository),
                    typeof (SpecialOfferRepository),
                    typeof (ContactRepository),
                    typeof (VendorRepository),
                    typeof (PurchaseOrderRepository),
                    typeof (WorkOrderRepository)
                };
            }
        }

        private static Type[] ContributedActions {
            get {
                return new Type[] {
                    typeof (OrderContributedActions),
                    typeof (CustomerContributedActions)
                };
            }
        }

        private static Type[] SystemServices {
            get {
                return new[] {
                    typeof (SimpleEncryptDecrypt)
                };
            }
        }

        private static Type[] AssociatedTypes() {
            var allTypes = AppDomain.CurrentDomain.GetAssemblies().Single(a => a.GetName().Name == "AdventureWorksModel").GetTypes();
            return allTypes.Where(t => t.BaseType == typeof (AWDomainObject) && !t.IsAbstract).ToArray();
        }

        public static ReflectorConfiguration ReflectorConfig() {
            return new ReflectorConfiguration(Types, MenuServices, ContributedActions, SystemServices);
        }

        public static EntityObjectStoreConfiguration EntityObjectStoreConfig() {
            var config = new EntityObjectStoreConfiguration();
            config.UsingEdmxContext("Model").AssociateTypes(AssociatedTypes);
            config.SpecifyTypesNotAssociatedWithAnyContext(() => new[] {typeof (AWDomainObject)});
            return config;
        }
    }
}