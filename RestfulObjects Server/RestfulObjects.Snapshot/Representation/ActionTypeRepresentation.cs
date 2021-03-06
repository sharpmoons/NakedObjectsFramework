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
    public class ActionTypeRepresentation : Representation {
        protected ActionTypeRepresentation(IOidStrategy oidStrategy, HttpRequestMessage req, ActionTypeContextFacade actionTypeContext, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SelfRelType = new TypeMemberRelType(RelValues.Self, new UriMtHelper(oidStrategy ,req, actionTypeContext));
            SetScalars(actionTypeContext);
            SetLinks(req, actionTypeContext);
            SetParameters(req, actionTypeContext);
            SetExtensions();
            SetHeader();
        }

        [DataMember(Name = JsonPropertyNames.Id)]
        public string Id { get; set; }

        [DataMember(Name = JsonPropertyNames.FriendlyName)]
        public string FriendlyName { get; set; }

        [DataMember(Name = JsonPropertyNames.Description)]
        public string Description { get; set; }

        [DataMember(Name = JsonPropertyNames.HasParams)]
        public bool HasParams { get; set; }

        [DataMember(Name = JsonPropertyNames.MemberOrder)]
        public int MemberOrder { get; set; }

        [DataMember(Name = JsonPropertyNames.Links)]
        public LinkRepresentation[] Links { get; set; }

        [DataMember(Name = JsonPropertyNames.Parameters)]
        public LinkRepresentation[] Parameters { get; set; }

        [DataMember(Name = JsonPropertyNames.Extensions)]
        public MapRepresentation Extensions { get; set; }

        private void SetHeader() {
            caching = CacheType.NonExpiring;
        }

        private void SetExtensions() {
            Extensions = MapRepresentation.Create();
        }

        private void SetScalars(ActionTypeContextFacade actionTypeContext) {
            Id = actionTypeContext.ActionContext.Id;
            FriendlyName = actionTypeContext.ActionContext.Action.Name;
            Description = actionTypeContext.ActionContext.Action.Description;
            HasParams = actionTypeContext.ActionContext.VisibleParameters.Any();
            MemberOrder = actionTypeContext.ActionContext.Action.MemberOrder;
        }

        private void SetParameters(HttpRequestMessage req, ActionTypeContextFacade actionTypeContext) {
            IEnumerable<LinkRepresentation> parms = actionTypeContext.ActionContext.VisibleParameters.
                Select(p => LinkRepresentation.Create(OidStrategy,new ParamTypeRelType(new UriMtHelper(OidStrategy, req, new ParameterTypeContextFacade {
                    Action = actionTypeContext.ActionContext.Action,
                    OwningSpecification = actionTypeContext.OwningSpecification,
                    Parameter = p.Parameter
                })), Flags));
            Parameters = parms.ToArray();
        }

        private void SetLinks(HttpRequestMessage req, ActionTypeContextFacade actionTypeContext) {
            var domainTypeUri = new UriMtHelper(OidStrategy, req, actionTypeContext);
            var tempLinks = new List<LinkRepresentation> {
                LinkRepresentation.Create(OidStrategy, SelfRelType, Flags),
                LinkRepresentation.Create(OidStrategy ,new DomainTypeRelType(RelValues.Up, domainTypeUri), Flags),
                LinkRepresentation.Create(OidStrategy, new DomainTypeRelType(RelValues.ReturnType, new UriMtHelper(OidStrategy, req, actionTypeContext.ActionContext.Action.ReturnType)), Flags)
            };

            if (actionTypeContext.ActionContext.Action.ReturnType.IsCollection) {
                tempLinks.Add(LinkRepresentation.Create(OidStrategy ,new DomainTypeRelType(RelValues.ElementType, new UriMtHelper(OidStrategy, req, actionTypeContext.ActionContext.Action.ElementType)), Flags));
            }

            Links = tempLinks.ToArray();
        }


        public static ActionTypeRepresentation Create(IOidStrategy oidStrategy, HttpRequestMessage req, ActionTypeContextFacade actionTypeContext, RestControlFlags flags) {
            return new ActionTypeRepresentation(oidStrategy, req, actionTypeContext, flags);
        }
    }
}