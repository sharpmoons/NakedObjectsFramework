// Copyright � Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using System.Collections.Generic;
using NakedObjects.Architecture.Adapter;

namespace NakedObjects.Architecture.Persist {
    /// <summary>
    ///     The NakedObjectLoader is responsible for managing the adapters and identities for each and every POCO that
    ///     is being used by the NOF. It provides a consistent set of adapters in memory, providing adapter for the
    ///     POCOs that are in use by the NOF and ensuring that the same object is not loaded twice into memory.
    /// </summary>
    /// <para>
    ///     Each POCO is given an adapter so that the NOF can work with the POCOs even though it does not understand
    ///     their types. Each POCO maps to an adapter and these are reused
    /// </para>
    /// <para>
    ///     Loading of an object refers to the initializing of state within each object as it is restored for
    ///     persistent storage.
    /// </para>
    public interface IIdentityMap : IEnumerable<INakedObject> {
        /// <summary>
        ///     Indicates to the component that it is to initialise itself as it will soon be receiving requests
        /// </summary>
        void Init();

        /// <summary>
        ///     Resets the loader to a known state
        /// </summary>
        void Reset();

        /// <summary>
        ///     Indicates to the component that no more requests will be made of it and it can safely release any
        ///     services it has hold of.
        /// </summary>
        void Shutdown();

        void AddAdapter(INakedObject nakedObject);

        /// <summary>
        ///     Marks the specified adapter as persistent (as opposed to to being transient) and sets the OID on the
        ///     adapter. The adapter is added to the identity-adapter map.
        /// </summary>
        void MadePersistent(INakedObject nakedObject);

        /// <summary>
        ///     Unloads the specified object from both the identity-adapter map, and the poco-adapter map. This
        ///     indicates that the object is no longer in use, and therefore that no objects exists within the system.
        /// </summary>
        void Unloaded(INakedObject nakedObject);

        /// <summary>
        ///     Retrieves an existing adapter, from the Poco-adapter map, for the specified object. If the object is
        ///     not in the map then null is returned.
        /// </summary>
        INakedObject GetAdapterFor(object domainObject);

        /// <summary>
        ///     Retrieves an existing adapter, from the identity-adapter map, for the specified object. If the OID is
        ///     not in the map then null is returned.
        /// </summary>
        INakedObject GetAdapterFor(IOid oid);

        /// <summary>
        ///     Returns true if the object for the specified OID exists, ie it is already loaded
        /// </summary>
        bool IsIdentityKnown(IOid oid);

        /// <summary>
        ///     Marks this object as having been replaced (presumably by a proxy) this allows us to catch
        ///     any incorrect uses of the original object.
        /// </summary>
        void Replaced(object domainObject);

        void UpdateViewModel(INakedObject adapter, string[] keys);
    }

    // Copyright (c) Naked Objects Group Ltd.
}