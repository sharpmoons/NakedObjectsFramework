// Copyright � Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System;
using System.Collections.Generic;
using System.Linq;
using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Facets.AutoComplete;
using NakedObjects.Architecture.Facets.Objects.Aggregated;
using NakedObjects.Architecture.Facets.Properties.Access;
using NakedObjects.Architecture.Facets.Properties.Choices;
using NakedObjects.Architecture.Facets.Properties.Defaults;
using NakedObjects.Architecture.Facets.Properties.Enums;
using NakedObjects.Architecture.Facets.Properties.Modify;
using NakedObjects.Architecture.Facets.Propparam.Validate.Mandatory;
using NakedObjects.Architecture.Interactions;
using NakedObjects.Architecture.Reflect;
using NakedObjects.Architecture.Resolve;
using NakedObjects.Architecture.Spec;
using NakedObjects.Core.Context;
using NakedObjects.Core.Persist;
using NakedObjects.Core.Util;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.Spec {
    public class OneToOneAssociationImpl : NakedObjectAssociationAbstract, IOneToOneAssociation {
        private readonly INakedObjectAssociationPeer reflectiveAdapter;

        public OneToOneAssociationImpl(INakedObjectAssociationPeer association)
            : base(association.Identifier.MemberName, association.Specification, association) {
            reflectiveAdapter = association;
        }

        #region IOneToOneAssociation Members

        public override bool IsObject {
            get { return true; }
        }

        public override bool IsChoicesEnabled {
            get { return Specification.IsBoundedSet() || ContainsFacet<IPropertyChoicesFacet>() || ContainsFacet<IEnumFacet>(); }
        }

        public override bool IsMandatory {
            get {
                var mandatoryFacet = GetFacet<IMandatoryFacet>();
                return mandatoryFacet.IsMandatory;
            }
        }

        public override INakedObject GetNakedObject(INakedObject fromObject) {
            return GetAssociation(fromObject);
        }

        public override Tuple<string, INakedObjectSpecification>[] GetChoicesParameters() {
            var propertyChoicesFacet = GetFacet<IPropertyChoicesFacet>();
            return propertyChoicesFacet != null ? propertyChoicesFacet.ParameterNamesAndTypes : new Tuple<string, INakedObjectSpecification>[] {};
        }

        public override INakedObject[] GetChoices(INakedObject target, IDictionary<string, INakedObject> parameterNameValues) {
            var propertyChoicesFacet = GetFacet<IPropertyChoicesFacet>();
            var enumFacet = GetFacet<IEnumFacet>();

            object[] objectOptions = propertyChoicesFacet == null ? null : propertyChoicesFacet.GetChoices(target, parameterNameValues);
            if (objectOptions != null) {
                if (enumFacet == null) {
                    return PersistorUtils.GetCollectionOfAdaptedObjects(objectOptions).ToArray();
                }
                return PersistorUtils.GetCollectionOfAdaptedObjects(enumFacet.GetChoices(target, objectOptions)).ToArray();
            }

            objectOptions = enumFacet == null ? null : enumFacet.GetChoices(target);
            if (objectOptions != null) {
                return PersistorUtils.GetCollectionOfAdaptedObjects(objectOptions).ToArray();
            }

            if (Specification.IsBoundedSet()) {
                return PersistorUtils.GetCollectionOfAdaptedObjects(Specification.GetBoundedSet()).ToArray();
            }
            return null;
        }

        public override INakedObject[] GetCompletions(INakedObject target, string autoCompleteParm) {
            var propertyAutoCompleteFacet = GetFacet<IAutoCompleteFacet>();
            return propertyAutoCompleteFacet == null ? null : PersistorUtils.GetCollectionOfAdaptedObjects(propertyAutoCompleteFacet.GetCompletions(target, autoCompleteParm)).ToArray();
        }

        public virtual void InitAssociation(INakedObject inObject, INakedObject associate) {
            var initializerFacet = GetFacet<IPropertyInitializationFacet>();
            if (initializerFacet != null) {
                initializerFacet.InitProperty(inObject, associate);
            }
        }

        public virtual IConsent IsAssociationValid(INakedObject inObject, INakedObject reference) {
            if (reference != null && !reference.Specification.IsOfType(Specification)) {
                return GetConsent(string.Format(Resources.NakedObjects.TypeMismatchError, Specification.SingularName));
            }

            if (!inObject.ResolveState.IsNotPersistent()) {
                if (reference != null && !reference.Specification.IsParseable && reference.ResolveState.IsNotPersistent()) {
                    return GetConsent(Resources.NakedObjects.TransientFieldMessage);
                }
            }

            var buf = new InteractionBuffer();
            InteractionContext ic = InteractionContext.ModifyingPropParam(NakedObjectsContext.Session, false, inObject, Identifier, reference);
            InteractionUtils.IsValid(this, ic, buf);
            return InteractionUtils.IsValid(buf);
        }

        public override bool IsEmpty(INakedObject inObject) {
            return GetAssociation(inObject) == null;
        }

        public override bool IsInline {
            get { return Specification.ContainsFacet(typeof (IComplexTypeFacet)); }
        }

        public override INakedObject GetDefault(INakedObject fromObject) {
            return GetDefaultObject(fromObject).Item1;
        }

        public override TypeOfDefaultValue GetDefaultType(INakedObject fromObject) {
            return GetDefaultObject(fromObject).Item2;
        }

        public override void ToDefault(INakedObject inObject) {
            INakedObject defaultValue = GetDefault(inObject);
            if (defaultValue != null) {
                InitAssociation(inObject, defaultValue);
            }
        }

        public virtual void SetAssociation(INakedObject inObject, INakedObject associate) {
            INakedObject currentValue = GetAssociation(inObject);
            if (currentValue != associate) {
                if (associate == null && ContainsFacet<IPropertyClearFacet>()) {
                    GetFacet<IPropertyClearFacet>().ClearProperty(inObject);
                }
                else {
                    var setterFacet = GetFacet<IPropertySetterFacet>();
                    if (setterFacet != null) {
                        inObject.ResolveState.CheckCanAssociate(associate);
                        setterFacet.SetProperty(inObject, associate);
                    }
                }
            }
        }

        #endregion

        private INakedObject GetAssociation(INakedObject fromObject) {
            object obj = GetFacet<IPropertyAccessorFacet>().GetProperty(fromObject);
            if (obj == null) {
                return null;
            }
            INakedObjectSpecification specification = NakedObjectsContext.Reflector.LoadSpecification(obj.GetType());
            if (specification.ContainsFacet(typeof (IComplexTypeFacet))) {
                return PersistorUtils.CreateAggregatedAdapter(fromObject, this, obj);
            }
            return PersistorUtils.CreateAdapter(obj);
        }

        public virtual Tuple<INakedObject, TypeOfDefaultValue> GetDefaultObject(INakedObject fromObject) {
            var typeofDefaultValue = TypeOfDefaultValue.Explicit;

            // look for a default on the association ...
            var propertyDefaultFacet = GetFacet<IPropertyDefaultFacet>();
            // ... if none, attempt to find a default on the specification (eg an int should default to 0).
            if (propertyDefaultFacet == null || propertyDefaultFacet.IsNoOp) {
                typeofDefaultValue = TypeOfDefaultValue.Implicit;
                propertyDefaultFacet = Specification.GetFacet<IPropertyDefaultFacet>();
            }
            if (propertyDefaultFacet == null) {
                return new Tuple<INakedObject, TypeOfDefaultValue>(null, TypeOfDefaultValue.Implicit);
            }
            object obj = propertyDefaultFacet.GetDefault(fromObject);
            return new Tuple<INakedObject, TypeOfDefaultValue>(PersistorUtils.CreateAdapter(obj), typeofDefaultValue);
        }

        public override string ToString() {
            var str = new AsString(this);
            str.Append(base.ToString());
            str.AddComma();
            str.Append("persisted", IsPersisted);
            str.Append("type", Specification.ShortName);
            return str.ToString();
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}