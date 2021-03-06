// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.Net.Http.Headers;
using RestfulObjects.Snapshot.Constants;

namespace RestfulObjects.Snapshot.Utility {
    public class MemberRelType : RelType {
        public MemberRelType(UriMtHelper helper) : base(RelValues.Details, helper) {}
        public MemberRelType(string name, UriMtHelper helper) : base(name, helper) {}

        public override string Name {
            get { return base.Name + (HasRelParameter ? helper.GetRelParameters() : ""); }
        }

        public override Uri GetUri() {
            return helper.GetDetailsUri();
        }

        public override MediaTypeHeaderValue GetMediaType(RestControlFlags flags) {
            MediaTypeHeaderValue mediaType = UriMtHelper.GetJsonMediaType(helper.GetMemberMediaType());
            helper.AddObjectCollectionRepresentationParameter(mediaType, flags);
            return mediaType;
        }
    }
}