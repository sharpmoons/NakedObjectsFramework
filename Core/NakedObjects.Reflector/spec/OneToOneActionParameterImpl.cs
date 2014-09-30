// Copyright � Naked Objects Group Ltd ( http://www.nakedobjects.net). 
// All Rights Reserved. This code released under the terms of the 
// Microsoft Public License (MS-PL) ( http://opensource.org/licenses/ms-pl.html) 

using NakedObjects.Architecture.Reflect;
using NakedObjects.Reflector.Peer;

namespace NakedObjects.Reflector.Spec {
    public class OneToOneActionParameterImpl : NakedObjectActionParameterAbstract, IOneToOneFeature {
        public OneToOneActionParameterImpl(int index, INakedObjectAction actionImpl, INakedObjectActionParamPeer peer)
            : base(index, actionImpl, peer) {}

        public override bool IsObject {
            get { return true; }
        }
    }
}