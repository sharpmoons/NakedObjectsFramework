// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System;
using NakedObjects.Architecture.Reflect;

namespace NakedObjects.Architecture.Adapter {
    public interface IIdentifier : IComparable {
        string ClassName { get; }

        string MemberName { get; }

        string[] MemberParameterTypeNames { get; }

        string[] MemberParameterNames { get; }

        IObjectSpecImmutable[] MemberParameterSpecifications { get; }

        /// <summary>
        ///     Returns <c>true</c> if the member is for a property or collection; <c>false</c> if for an action
        /// </summary>
        bool IsField { get; }

        string ToIdentityString(IdentifierDepth depth);

        string ToIdentityStringWithCheckType(IdentifierDepth depth, CheckType checkType);
    }

    // Copyright (c) Naked Objects Group Ltd.
}