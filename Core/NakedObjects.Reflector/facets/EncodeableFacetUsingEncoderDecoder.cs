// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Capabilities;
using NakedObjects.Metamodel.Facet;

namespace NakedObjects.Reflector.DotNet.Facets.Objects.Encodeable {
    public class EncodeableFacetUsingEncoderDecoder<T> : FacetAbstract, IEncodeableFacet {
        public static string ENCODED_NULL = "NULL";
        private readonly IEncoderDecoder<T> encoderDecoder;

        public EncodeableFacetUsingEncoderDecoder(IEncoderDecoder<T> encoderDecoder, ISpecification holder)
            : base(typeof (IEncodeableFacet), holder) {
            this.encoderDecoder = encoderDecoder;
        }

        #region IEncodeableFacet Members

        public bool IsValid {
            get { return encoderDecoder != null; }
        }

        public INakedObject FromEncodedString(string encodedData, INakedObjectManager manager) {
            //Assert.assertNotNull(encodedData);
            if (ENCODED_NULL.Equals(encodedData)) {
                return null;
            }
            return manager.CreateAdapter(encoderDecoder.FromEncodedString(encodedData), null, null);
        }

        public string ToEncodedString(INakedObject nakedObject) {
            return nakedObject == null ? ENCODED_NULL : encoderDecoder.ToEncodedString(nakedObject.GetDomainObject<T>());
        }

        #endregion

        // TODO: is this safe? really?

        protected override string ToStringValues() {
            return encoderDecoder.ToString();
        }
    }
}