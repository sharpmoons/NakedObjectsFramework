// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using RestfulObjects.Snapshot.Constants;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    [DataContract]
    public class CollectionTypeRepresentation : MemberTypeRepresentation {
        protected CollectionTypeRepresentation(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyTypeContextFacade propertyContext, RestControlFlags flags)
            : base(oidStrategy, req, propertyContext, flags) {
            SetScalars(propertyContext);
            SetLinks(req, propertyContext);
        }

        [DataMember(Name = JsonPropertyNames.PluralName)]
        public string PluralName { get; set; }

        private void SetScalars(PropertyTypeContextFacade propertyContext) {
            PluralName = propertyContext.Property.ElementSpecification.PluralName;
        }

        private void SetLinks(HttpRequestMessage req, PropertyTypeContextFacade propertyContext) {
            IList<LinkRepresentation> tempLinks = CreateLinks(req, propertyContext);
            tempLinks.Add(LinkRepresentation.Create(OidStrategy ,new DomainTypeRelType(RelValues.ReturnType, new UriMtHelper(OidStrategy ,req, propertyContext.Property)), Flags));
            tempLinks.Add(LinkRepresentation.Create(OidStrategy ,new DomainTypeRelType(RelValues.ElementType, new UriMtHelper(OidStrategy ,req, propertyContext.Property.ElementSpecification)), Flags));
            Links = tempLinks.ToArray();
        }

        public new static CollectionTypeRepresentation Create(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyTypeContextFacade propertyContext, RestControlFlags flags) {
            return new CollectionTypeRepresentation(oidStrategy ,req, propertyContext, flags);
        }
    }
}