// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.

using System.Linq;
using NakedObjects.Architecture.Spec;
using NakedObjects.Facade.Contexts;

namespace NakedObjects.Facade.Impl.Contexts {
    public class ActionContext : Context {
        private ParameterContext[] parameters;
        public IActionSpec Action { get; set; }

        public override string Id {
            get { return Action.Id; }
        }

        public override ITypeSpec Specification {
            get { return Action.ReturnSpec; }
        }

        public ParameterContext[] VisibleParameters {
            get { return parameters ?? new ParameterContext[] {}; }
            set { parameters = value; }
        }

        public string OverloadedUniqueId { get; set; }

        public ActionContextFacade ToActionContextFacade(IFrameworkFacade facade, INakedObjectsFramework framework) {
            var ac = new ActionContextFacade {
                Action = new ActionFacade(Action, facade, framework, OverloadedUniqueId ?? ""),
                VisibleParameters = VisibleParameters.Select(p => p.ToParameterContextFacade(facade, framework)).ToArray()
            };
            return ToContextFacade(ac, facade, framework);
        }
    }
}