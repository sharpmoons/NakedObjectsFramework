﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Spec;
using NakedObjects.Facade.Utility;
using NakedObjects.Util;

namespace NakedObjects.Facade.Impl.Utility {
    public class IdHelper : IIdHelper {
        private const string Sep = "-";
        private const string InputName = "Input";
        private const string SelectName = "Select";
        private const string ConcurrencyName = "Concurrency";
        private const string AutoCompleteName = "AutoComplete";

        #region IIdHelper Members

        public string GetDisplayFormatId(string id) {
            return MakeId(id, IdConstants.DisplayFormatFieldId);
        }

        public string GetCollectionItemId(IObjectFacade owner, IAssociationFacade assocFacade) {
            IAssociationSpec assoc = assocFacade.WrappedSpec();
            return GetObjectId(owner) + Sep + assoc.Id + Sep + "Item";
        }

        public string GetObjectId(IObjectFacade objectFacade) {
            INakedObjectAdapter owner = objectFacade.WrappedAdapter();
            string postFix = "";

            if (owner.Spec.IsCollection) {
                var elementFacet = owner.Spec.GetFacet<ITypeOfFacet>();
                var elementType = elementFacet.GetValue(owner);

                postFix = Sep + elementType.Name;
            }

            return owner.Spec.ShortName + postFix;
        }

        public string GetFieldId(IObjectFacade owner, IAssociationFacade assocFacade) {
            IAssociationSpec assoc = assocFacade.WrappedSpec();

            return GetObjectId(owner) + Sep + assoc.Id;
        }

        public string GetInlineFieldId(IAssociationFacade parentFacade, IObjectFacade owner, IAssociationFacade assocFacade) {
            IAssociationSpec parent = parentFacade.WrappedSpec();
            IAssociationSpec assoc = assocFacade.WrappedSpec();

            return parent.Id + Sep + GetObjectId(owner) + Sep + assoc.Id;
        }

        public string GetFieldInputId(IObjectFacade owner, IAssociationFacade assocFacade) {
            IAssociationSpec assoc = assocFacade.WrappedSpec();

            return GetFieldId(owner, assocFacade) + Sep + InputOrSelect(assoc.ReturnSpec);
        }

        public string GetFieldAutoCompleteId(string id, IObjectFacade owner, IAssociationFacade assocFacade) {
            IAssociationSpec assoc = assocFacade.WrappedSpec();

            return assoc.ReturnSpec.IsParseable ? id : id + Sep + AutoCompleteName;
        }

        public string GetInlineFieldInputId(IAssociationFacade parent, IObjectFacade owner, IAssociationFacade assocFacade) {
            IAssociationSpec assoc = assocFacade.WrappedSpec();

            return GetInlineFieldId(parent, owner, assocFacade) + Sep + InputOrSelect(assoc.ReturnSpec);
        }

        public string GetConcurrencyFieldInputId(IObjectFacade owner, IAssociationFacade assoc) {
            return GetFieldId(owner, assoc) + Sep + ConcurrencyName;
        }

        public string GetInlineConcurrencyFieldInputId(IAssociationFacade parent, IObjectFacade owner, IAssociationFacade assoc) {
            return GetInlineFieldId(parent, owner, assoc) + Sep + ConcurrencyName;
        }

        public string GetConcurrencyActionInputId(IObjectFacade owner, IActionFacade action, IAssociationFacade assocFacade) {
            IAssociationSpec assoc =  assocFacade.WrappedSpec();
            return GetActionId(owner, action) + Sep + assoc.Id + Sep + ConcurrencyName;
        }

        public string GetActionId(IObjectFacade owner, IActionFacade actionFacade) {
            IActionSpec action = actionFacade.WrappedSpec();
            return GetObjectId(owner) + Sep + action.Id;
        }

        public string GetActionId(string propertyName, IActionFacade actionContextActionFacade, IObjectFacade actionObjectFacade, IObjectFacade targetObjectFacade, IActionFacade targetActionFacade) {
            IActionSpec actionContextAction = actionContextActionFacade == null ? null :  actionContextActionFacade.WrappedSpec();
            INakedObjectAdapter actionContextTarget = actionObjectFacade == null ? null : actionObjectFacade.WrappedAdapter();
            IActionSpec targetActionContextAction = targetActionFacade == null ? null : targetActionFacade.WrappedSpec();
            INakedObjectAdapter targetActionContextTarget = targetObjectFacade == null ? null : targetObjectFacade.WrappedAdapter();

            string contextActionName = actionContextAction == null ? "" : actionContextAction.Id + Sep;
            string contextNakedObjectId = actionContextTarget == null || actionContextTarget == targetActionContextTarget ? "" : GetObjectId(actionObjectFacade) + Sep;
            string propertyId = string.IsNullOrEmpty(propertyName) ? "" : NameUtils.CapitalizeName(propertyName) + Sep;
            return contextNakedObjectId + contextActionName + propertyId + GetObjectId(targetObjectFacade) + Sep + targetActionContextAction.Id;
        }

        public string GetActionDialogId(IObjectFacade owner, IActionFacade actionFacade) {
            IActionSpec action = actionFacade.WrappedSpec();

            return GetObjectId(owner) + Sep + action.Id + Sep + IdConstants.DialogName;
        }

        public string GetSubMenuId(IObjectFacade owner, IActionFacade actionFacade) {
            IActionSpec action =  actionFacade.WrappedSpec();

            return EnsureEndsWithColon(GetObjectId(owner) + Sep + action.Id.Split('.').Last());
        }

        public string GetSubMenuId(IObjectFacade owner, IObjectFacade serviceFacade) {
            INakedObjectAdapter service = serviceFacade.WrappedAdapter();
            return EnsureEndsWithColon(GetObjectId(owner) + Sep + service.Spec.ShortName);
        }

        public string GetFindMenuId(IObjectFacade objectFacade, IActionFacade actionFacade, string propertyName) {
            IActionSpec action = actionFacade == null ? null : actionFacade.WrappedSpec();
            string contextActionName = action == null ? "" : Sep + action.Id;
            return GetObjectId(objectFacade) + contextActionName + Sep + NameUtils.CapitalizeName(propertyName) + Sep + IdConstants.FindMenuName;
        }

        public string GetParameterId(IActionFacade actionFacade, IActionParameterFacade parameterFacade) {
            IActionParameterSpec parameter = parameterFacade.WrappedSpec();
            IActionSpec action =  actionFacade.WrappedSpec();
            return action.OnSpec.ShortName + Sep + action.Id + Sep + NameUtils.CapitalizeName(parameter.Id);
        }

        public string GetParameterInputId(IActionFacade action, IActionParameterFacade parameterFacade) {
            IActionParameterSpec parameter =  parameterFacade.WrappedSpec();
            return GetParameterId(action, parameterFacade) + Sep + InputOrSelect(parameter.Spec);
        }

        public string GetParameterAutoCompleteId(IActionFacade action, IActionParameterFacade parameterFacade) {
            IActionParameterSpec parameter = parameterFacade.WrappedSpec();
            var id = GetParameterInputId(action, parameterFacade);
            return parameter.Spec.IsParseable ? id : id + Sep + AutoCompleteName;
        }

        public string GetCollectionContainerId(IObjectFacade collectionFacade) {
            INakedObjectAdapter collection = collectionFacade.WrappedAdapter();

            return IdConstants.CollContainerName + Sep + collection.Spec.ShortName;
        }

        public string GetActionContainerId(IObjectFacade objectFacade) {
            return GetObjectId(objectFacade) + Sep + IdConstants.ActionsName;
        }

        public string GetServiceContainerId(IObjectFacade objectFacade) {
            return GetObjectId(objectFacade);
        }

        public string GetFieldContainerId(IObjectFacade objectFacade) {
            return GetObjectId(objectFacade) + Sep + IdConstants.PropertyListName;
        }

        public string GetParameterContainerId(IActionFacade actionFacade) {
            IActionSpec action =  actionFacade.WrappedSpec();

            return action.Id + Sep + IdConstants.ParamListName;
        }

        public string GetGenericActionId(IObjectFacade objectFacade, string type) {
            INakedObjectAdapter owner = objectFacade.WrappedAdapter();

            return IdConstants.ActionName + Sep + owner.Spec.ShortName + Sep + type;
        }

        public string GetActionLabel(IObjectFacade objectFacade) {
            return "Actions";
        }

        public string GetServiceLabel(IObjectFacade objectFacade) {
            INakedObjectAdapter nakedObject = objectFacade.WrappedAdapter();

            return nakedObject.TitleString();
        }

        public string GetMandatoryIndicatorClass() {
            return "nof-mandatory-field-indicator";
        }

        public string GetMandatoryIndicator() {
            return "*";
        }

        public string MakeId(params string[] ids) {
            return ids.Aggregate(string.Empty, (x, y) => (string.IsNullOrEmpty(x) ? string.Empty : x + Sep) + y);
        }

        public bool KeyPrefixIs(string key, string match) {
            return key.StartsWith(match + Sep);
        }

        public string GetAggregateFieldInputId(IObjectFacade objectFacade, IAssociationFacade propertyFacade) {
            string fieldId;
            INakedObjectAdapter nakedObject = objectFacade.WrappedAdapter();

            var aoid = nakedObject.Oid as IAggregateOid;
            if (aoid != null) {
                IAssociationSpec parent = ((IObjectSpec) aoid.ParentOid.Spec).Properties.SingleOrDefault(p => p.Id == aoid.FieldName);           
                fieldId = parent.Id + Sep + GetObjectId(objectFacade) + Sep + propertyFacade.Id + Sep + InputOrSelect(propertyFacade.WrappedSpec().ReturnSpec);
            }
            else {
                fieldId = GetFieldInputId(objectFacade, propertyFacade);
            }
            return fieldId;
        }

        #endregion

        private string InputOrSelect(ITypeSpec spec) {
            return (spec.IsParseable ? InputName : SelectName);
        }

        private string EnsureEndsWithColon(string id) {
            return id.EndsWith(":") ? id : id + ":";
        }
    }
}