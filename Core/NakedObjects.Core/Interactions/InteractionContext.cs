// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using NakedObjects.Architecture.Adapter;
using NakedObjects.Architecture.Component;
using NakedObjects.Architecture.Facet;
using NakedObjects.Architecture.Interactions;

namespace NakedObjects.Core.Interactions {
    /// <summary>
    ///     Represents an interaction between the framework and (a <see cref="IFacet" /> of) the domain object.
    /// </summary>
    /// <para>
    ///     Effectively just wraps up a target object, parameters and a <see cref="ISession" />.
    ///     Defining this as a separate interface makes for a more stable API, however.
    /// </para>
    internal sealed class InteractionContext : IInteractionContext {
        private readonly IIdentifier id;
        private readonly InteractionType interactionType;
        private readonly bool programmatic;
        private readonly INakedObjectAdapter proposedArgument;
        private readonly INakedObjectAdapter[] proposedArguments;
        private readonly ISession session;
        private readonly INakedObjectAdapter target;

        private InteractionContext(InteractionType interactionType,
                                   ISession session,
                                   bool programmatic,
                                   INakedObjectAdapter target,
                                   IIdentifier id,
                                   INakedObjectAdapter proposedArgument,
                                   INakedObjectAdapter[] arguments) {
            this.interactionType = interactionType;
            this.programmatic = programmatic;
            this.id = id;
            this.session = session;
            this.target = target;
            this.proposedArgument = proposedArgument;
            proposedArguments = arguments;
        }

        #region IInteractionContext Members

        /// <summary>
        ///     The type of interaction
        /// </summary>
        /// <para>
        ///     Used by <see cref="IFacet" />s that apply only in certain conditions.  For
        ///     example, some facets for collections will care only when an object is
        ///     being added to the collection, but won't care when an object is being removed from
        ///     the collection.
        /// </para>
        /// <para>
        ///     Will be set for all interactions.
        /// </para>
        public InteractionType InteractionType {
            get { return interactionType; }
        }

        /// <summary>
        ///     The  user or role <see cref="ISession" /> that is performing this interaction.
        /// </summary>
        /// <para>
        ///     Will be set for all interactions.
        /// </para>
        public ISession Session {
            get { return session; }
        }

        /// <summary>
        ///     How the interaction was initiated
        /// </summary>
        public bool IsProgrammatic {
            get { return programmatic; }
        }

        /// <summary>
        ///     The target object that this interaction is with.
        /// </summary>
        /// <para>
        ///     Will be set for all interactions.
        /// </para>
        public INakedObjectAdapter Target {
            get { return target; }
        }

        /// <summary>
        ///     The identifier of the object or member that is being identified with.
        /// </summary>
        /// <para>
        ///     If the <see cref="InteractionType" /> type is <see cref="Architecture.Interactions.InteractionType.ObjectPersist" />,
        ///     will be the identifier of the <see cref="Target" /> object's specification.
        ///     Otherwise will be the identifier of the member.
        /// </para>
        /// <para>
        ///     Will be set for all interactions.
        /// </para>
        public IIdentifier Id {
            get { return id; }
        }

        /// <summary>
        ///     The proposed value for a property, or object being added/removed from a collection.
        /// </summary>
        /// <para>
        ///     Will be set if the <see cref="InteractionType" /> type is
        ///     <see
        ///         cref="Architecture.Interactions.InteractionType.PropertyParamModify" />
        ///     ,
        ///     <see cref="Architecture.Interactions.InteractionType.CollectionAddTo" /> or
        ///     <see
        ///         cref="Architecture.Interactions.InteractionType.CollectionRemoveFrom" />
        ///     ;
        ///     <c>null</c> otherwise.  In the case of the collection interactions, may be safely downcast
        ///     to <see cref="INakedObjectAdapter" />
        /// </para>
        public INakedObjectAdapter ProposedArgument {
            get { return proposedArgument; }
        }

        /// <summary>
        ///     The arguments for a proposed action invocation.
        /// </summary>
        /// <para>
        ///     Will be set if the <see cref="InteractionType" /> type is <see cref="Architecture.Interactions.InteractionType.ActionInvoke" />;
        ///     <c>null</c> otherwise.
        /// </para>
        public INakedObjectAdapter[] ProposedArguments {
            get { return proposedArguments; }
        }

        /// <summary>
        ///     Convenience to allow implementors of <see cref="IValidatingInteractionAdvisor" /> etc to determine
        ///     if the interaction's type applies.
        /// </summary>
        public bool TypeEquals(InteractionType other) {
            return InteractionType.Equals(other);
        }

        #endregion

        /// <summary>
        ///     Factory method to create an an <see cref="InteractionContext" /> to represent
        ///     <see cref="Architecture.Interactions.InteractionType.MemberAccess" />  reading a property.
        /// </summary>
        public static InteractionContext AccessMember(ISession session,
                                                      bool programmatic,
                                                      INakedObjectAdapter target,
                                                      IIdentifier memberIdentifier) {
            return new InteractionContext(InteractionType.MemberAccess,
                session,
                programmatic,
                target,
                memberIdentifier,
                null,
                null);
        }

        /// <summary>
        ///     Factory method to create an an <see cref="InteractionContext" /> to represent
        ///     <see cref="Architecture.Interactions.InteractionType.PropertyParamModify" />  modifying a property or parameter.
        /// </summary>
        public static InteractionContext ModifyingPropParam(ISession session,
                                                            bool programmatic,
                                                            INakedObjectAdapter target,
                                                            IIdentifier propertyIdentifier,
                                                            INakedObjectAdapter proposedArgument) {
            return new InteractionContext(InteractionType.PropertyParamModify,
                session,
                programmatic,
                target,
                propertyIdentifier,
                proposedArgument,
                null);
        }

        /// <summary>
        ///     Factory method to create an an <see cref="InteractionContext" /> to represent
        ///     <see cref="Architecture.Interactions.InteractionType.ActionInvoke" />  invoking an action.
        /// </summary>
        public static InteractionContext InvokingAction(ISession session,
                                                        bool programmatic,
                                                        INakedObjectAdapter target,
                                                        IIdentifier actionIdentifier,
                                                        INakedObjectAdapter[] arguments) {
            return new InteractionContext(InteractionType.ActionInvoke,
                session,
                programmatic,
                target,
                actionIdentifier,
                null,
                arguments);
        }
    }

    // Copyright (c) Naked Objects Group Ltd.
}