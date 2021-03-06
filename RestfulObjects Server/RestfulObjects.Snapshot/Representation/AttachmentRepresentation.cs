﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using NakedObjects.Facade;
using NakedObjects.Facade.Contexts;
using RestfulObjects.Snapshot.Utility;

namespace RestfulObjects.Snapshot.Representations {
    public class AttachmentRepresentation : Representation {
        private MediaTypeHeaderValue contentType;

        public AttachmentRepresentation(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyContextFacade propertyContext, RestControlFlags flags)
            : base(oidStrategy, flags) {
            SetContentType(propertyContext);
            SetContentDisposition(propertyContext);
            SetStream(propertyContext);
            SetHeader(propertyContext.Target);
        }

        public ContentDispositionHeaderValue ContentDisposition { get; set; }

        public Stream AsStream { get; set; }

        public override HttpResponseMessage GetAsMessage(MediaTypeFormatter formatter, Tuple<int, int, int> cacheSettings) {
            var content = new StreamContent(AsStream);
            var msg = new HttpResponseMessage {Content = content};
            msg.Content.Headers.ContentDisposition = ContentDisposition;

            msg.Content.Headers.ContentType = GetContentType();

            SetCaching(msg, cacheSettings);
            return msg;
        }

        public override MediaTypeHeaderValue GetContentType() {
            return contentType;
        }

        private void SetHeader(IObjectFacade target) {
            caching = CacheType.Transactional;
            SetEtag(target);
        }

        private void SetContentType(PropertyContextFacade context) {
            IObjectFacade no = context.Property.GetValue(context.Target);
            Func<string> defaultMimeType = () =>  no == null ? AttachmentContextFacade.DefaultMimeType : no.GetAttachment().DefaultMimeType();
            string mtv = no == null || string.IsNullOrWhiteSpace(no.GetAttachment().MimeType) ? defaultMimeType() : no.GetAttachment().MimeType;
            contentType = new MediaTypeHeaderValue(mtv);
        }

        private void SetContentDisposition(PropertyContextFacade context) {
            IObjectFacade no = context.Property.GetValue(context.Target);
            string cd = no == null || string.IsNullOrWhiteSpace(no.GetAttachment().ContentDisposition) ? AttachmentContextFacade.DefaultContentDisposition : no.GetAttachment().ContentDisposition;
            string fn = no != null ? no.GetAttachment().FileName : AttachmentContextFacade.DefaultFileName;
            ContentDisposition = new ContentDispositionHeaderValue(cd) {FileName = fn};
        }

        private void SetStream(PropertyContextFacade context) {
            IObjectFacade no = context.Property.GetValue(context.Target);
            AsStream = no != null ? no.GetAttachment().Content : new MemoryStream();
        }


        public static Representation Create(IOidStrategy oidStrategy, HttpRequestMessage req, PropertyContextFacade propertyContext, RestControlFlags flags) {
            return new AttachmentRepresentation(oidStrategy,req, propertyContext, flags);
        }
    }
}