/// <reference path="typings/angularjs/angular.d.ts" />
/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />


module NakedObjects.Angular.Gemini {

    export interface IViewModelFactory {
        toolBarViewModel(): ToolBarViewModel;
        errorViewModel(errorRep: ErrorRepresentation): ErrorViewModel;
        actionViewModel(actionRep: ActionMember, routedata: PaneRouteData): ActionViewModel;
        collectionViewModel(collectionRep: CollectionMember, routeData: PaneRouteData): CollectionViewModel;
        listPlaceholderViewModel(routeData : PaneRouteData): CollectionPlaceholderViewModel;
        servicesViewModel(servicesRep: DomainServicesRepresentation): ServicesViewModel;
        serviceViewModel(serviceRep: DomainObjectRepresentation, routeData: PaneRouteData): ServiceViewModel;
        tableRowViewModel(objectRep: DomainObjectRepresentation, routedata: PaneRouteData): TableRowViewModel;
        parameterViewModel(parmRep: Parameter, previousValue: Value, paneId: number): ParameterViewModel;
        propertyViewModel(propertyRep: PropertyMember, id: string, previousValue: Value, paneId: number): PropertyViewModel;
        ciceroViewModel(): CiceroViewModel;
        handleErrorResponse(err: ErrorMap, vm: MessageViewModel, vms: ValueViewModel[]);
        getItems(links: Link[], populateItems: boolean, routeData: PaneRouteData, collectionViewModel: CollectionViewModel | ListViewModel );
        linkViewModel(linkRep: Link, paneId: number): LinkViewModel;
    }

    interface IViewModelFactoryInternal extends IViewModelFactory {
        itemViewModel(linkRep: Link, paneId: number, selected: boolean): ItemViewModel;
    }

    app.service('viewModelFactory', function ($q: ng.IQService,
        $timeout: ng.ITimeoutService,
        $location: ng.ILocationService,
        $filter: ng.IFilterService,
        $cacheFactory: ng.ICacheFactoryService,
        repLoader: IRepLoader,
        color: IColor,
        context: IContext,
        mask: IMask,
        urlManager: IUrlManager,
        focusManager: IFocusManager,
        navigation: INavigation,
        clickHandler: IClickHandler,
        commandFactory: ICommandFactory,
        $rootScope: ng.IRootScopeService,
        $route ) {

        var viewModelFactory = <IViewModelFactoryInternal>this;

        viewModelFactory.errorViewModel = (errorRep: ErrorRepresentation) => {
            const errorViewModel = new ErrorViewModel();
            errorViewModel.message = errorRep.message() || "An Error occurred";
            const stackTrace = errorRep.stackTrace();
            errorViewModel.stackTrace = !stackTrace || stackTrace.length === 0 ? ["Empty"] : stackTrace;
            return errorViewModel;
        };

        function initLinkViewModel(linkViewModel: LinkViewModel, linkRep: Link) {
            linkViewModel.title = linkRep.title();
            linkViewModel.color = color.toColorFromHref(linkRep.href());
            linkViewModel.link = linkRep;

            linkViewModel.domainType = linkRep.type().domainType;
            linkViewModel.draggableType = linkViewModel.domainType;

            // for dropping 
            const value = new Value(linkRep);

            linkViewModel.value = value.toString();
            linkViewModel.reference = value.toValueString();
            linkViewModel.choice = ChoiceViewModel.create(value, "");

            linkViewModel.canDropOn = (targetType: string) => context.isSubTypeOf(targetType, linkViewModel.domainType);
        }


        viewModelFactory.linkViewModel = (linkRep: Link, paneId: number) => {
            const linkViewModel = new LinkViewModel();
            linkViewModel.doClick = () => {
                // because may be clicking on menu already open so want to reset focus             
                urlManager.setMenu(linkRep.rel().parms[0].value, paneId);
                focusManager.focusOverrideOff();
                focusManager.focusOn(FocusTarget.SubAction, 0, paneId);
            };
            initLinkViewModel(linkViewModel, linkRep);
            return linkViewModel;
        };

        viewModelFactory.itemViewModel = (linkRep: Link, paneId: number, selected: boolean) => {
            const itemViewModel = new ItemViewModel();
            itemViewModel.doClick = (right?: boolean) => urlManager.setItem(linkRep, clickHandler.pane(paneId, right));
            initLinkViewModel(itemViewModel, linkRep);

            itemViewModel.selected = selected;

            itemViewModel.checkboxChange = (index) => {
                urlManager.setListItem(paneId, index, itemViewModel.selected);
                focusManager.focusOverrideOn(FocusTarget.CheckBox, index + 1, paneId);
            }


            return itemViewModel;
        };

        function addAutoAutoComplete(valueViewModel: ValueViewModel, currentChoice: ChoiceViewModel, id: string, currentValue: Value) {
            valueViewModel.hasAutoAutoComplete = true;

            const cache = $cacheFactory.get("recentlyViewed");

            valueViewModel.choice = currentChoice;

            // make sure current value is cached so can be recovered ! 

            const { returnType: key, reference: subKey } = valueViewModel;
            const dict = cache.get(key) || {}; // todo fix type !
            dict[subKey] = { value: currentValue, name: currentValue.toString() };
            cache.put(key, dict);

            // bind in autoautocomplete into prompt 

            valueViewModel.prompt = (st: string) => {
                const defer = $q.defer<ChoiceViewModel[]>();
                const filtered = _.filter(dict, (i: { value: Value, name: string }) =>
                    i.name.toString().toLowerCase().indexOf(st.toLowerCase()) > -1);
                const ccs = _.map(filtered, (i: { value: Value, name: string }) => ChoiceViewModel.create(i.value, id, i.name));

                defer.resolve(ccs);

                return defer.promise;
            };
        }


        viewModelFactory.parameterViewModel = (parmRep: Parameter, previousValue: Value, paneId: number) => {
            var parmViewModel = new ParameterViewModel();

            parmViewModel.parameterRep = parmRep;
            parmViewModel.type = parmRep.isScalar() ? "scalar" : "ref";
            parmViewModel.dflt = parmRep.default().toValueString();
            parmViewModel.optional = parmRep.extensions().optional();
            var required = "";
            if (!parmViewModel.optional) {
                required = "* ";
            }
            parmViewModel.description = required + parmRep.extensions().description();
            parmViewModel.message = "";
            parmViewModel.id = parmRep.parameterId();
            parmViewModel.argId = `${parmViewModel.id.toLowerCase() }`;
            parmViewModel.paneArgId = `${parmViewModel.argId}${paneId}`;
            parmViewModel.reference = "";

            parmViewModel.mask = parmRep.extensions().mask();
            parmViewModel.title = parmRep.extensions().friendlyName();
            parmViewModel.returnType = parmRep.extensions().returnType();
            parmViewModel.format = parmRep.extensions().format();
            parmViewModel.isCollectionContributed = parmRep.isCollectionContributed();
            parmViewModel.onPaneId = paneId;

            parmViewModel.drop = (newValue: IDraggableViewModel) => {
                context.isSubTypeOf(newValue.draggableType, parmViewModel.returnType).
                    then((canDrop: boolean) => {
                        if (canDrop) {
                            parmViewModel.setNewValue(newValue);
                        }
                    }
                    );
            };

            parmViewModel.choices = _.map(parmRep.choices(), (v, n) => ChoiceViewModel.create(v, parmRep.parameterId(), n));
            parmViewModel.hasChoices = parmViewModel.choices.length > 0;
            parmViewModel.hasPrompt = !!parmRep.promptLink() && !!parmRep.promptLink().arguments()["x-ro-searchTerm"];
            parmViewModel.hasConditionalChoices = !!parmRep.promptLink() && !parmViewModel.hasPrompt;
            parmViewModel.isMultipleChoices = (parmViewModel.hasChoices || parmViewModel.hasConditionalChoices) && parmRep.extensions().returnType() === "list";

            if (parmViewModel.hasPrompt || parmViewModel.hasConditionalChoices) {

                const promptRep = parmRep.getPrompts();
                if (parmViewModel.hasPrompt) {
                    parmViewModel.prompt = _.partial(context.prompt, promptRep, parmViewModel.id);
                    parmViewModel.minLength = parmRep.promptLink().extensions().minLength();
                }

                if (parmViewModel.hasConditionalChoices) {
                    parmViewModel.conditionalChoices = _.partial(context.conditionalChoices, promptRep, parmViewModel.id);
                    parmViewModel.arguments = _.object<_.Dictionary<Value>>(_.map(parmRep.promptLink().arguments(), (v: any, key) => [key, new Value(v.value)]));
                }
            }

            if (parmViewModel.hasChoices || parmViewModel.hasPrompt || parmViewModel.hasConditionalChoices || parmViewModel.isCollectionContributed) {

                function setCurrentChoices(vals: Value) {

                    const choicesToSet = _.map(vals.list(), val => ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null));

                    if (parmViewModel.hasPrompt || parmViewModel.hasConditionalChoices || parmViewModel.isCollectionContributed) {
                        parmViewModel.multiChoices = choicesToSet;
                    } else {
                        parmViewModel.multiChoices = _.filter(parmViewModel.choices, c => _.any(choicesToSet, choiceToSet => c.match(choiceToSet)));
                    }
                }

                function setCurrentChoice(val: Value) {
                    const choiceToSet = ChoiceViewModel.create(val, parmViewModel.id, val.link() ? val.link().title() : null);

                    if (parmViewModel.hasPrompt || parmViewModel.hasConditionalChoices) {
                        parmViewModel.choice = choiceToSet;
                    } else {
                        parmViewModel.choice = _.find(parmViewModel.choices, c => c.match(choiceToSet));
                    }
                }
                if (previousValue || parmViewModel.dflt) {
                    const toSet = previousValue || parmRep.default();
                    if (parmViewModel.isMultipleChoices || parmViewModel.isCollectionContributed) {
                        setCurrentChoices(toSet);
                    } else {
                        setCurrentChoice(toSet);
                    }
                }
            } else {
                if (parmRep.extensions().returnType() === "boolean") {
                    parmViewModel.value = previousValue ? previousValue.toString().toLowerCase() === "true" : parmRep.default().scalar();
                } else if (parmRep.extensions().returnType() === "string" && parmRep.extensions().format() === "date-time" ) {
                    const rawValue = (previousValue ? previousValue.toString() : "") || parmViewModel.dflt || "";
                    const dateValue = Date.parse(rawValue);
                    parmViewModel.value = dateValue ? new Date(rawValue) : null; 
                } else {
                    parmViewModel.value = (previousValue ? previousValue.toString() : null) || parmViewModel.dflt || "";
                }
            }

            var remoteMask = parmRep.extensions().mask();

            if (remoteMask && parmRep.isScalar()) {
                const localFilter = mask.toLocalFilter(remoteMask);
                if (localFilter) {
                    // todo formatting will have to happen in directive - at lesat for dates - value is now date in that case
                    //parmViewModel.value = $filter(localFilter.name)(parmViewModel.value, localFilter.mask);
                    parmViewModel.formattedValue = parmViewModel.value ? $filter(localFilter.name)(parmViewModel.value.toString(), localFilter.mask) : "";

                }
            }

            if (parmViewModel.type === "ref" && !parmViewModel.hasPrompt && !parmViewModel.hasChoices && !parmViewModel.hasConditionalChoices) {

                let currentChoice: ChoiceViewModel = null;

                if (previousValue) {
                    currentChoice = ChoiceViewModel.create(previousValue, parmViewModel.id, previousValue.link() ? previousValue.link().title() : null);
                }
                else if (parmViewModel.dflt) {
                    let dflt = parmRep.default();
                    currentChoice = ChoiceViewModel.create(dflt, parmViewModel.id, dflt.link().title());
                }

                const currentValue = new Value(currentChoice ? { href: currentChoice.value, title: currentChoice.name } : "");

                addAutoAutoComplete(parmViewModel, currentChoice, parmViewModel.id, currentValue);
            }

            parmViewModel.color = parmViewModel.value ? color.toColorFromType(parmViewModel.returnType) : "";

            return parmViewModel;
        };

        viewModelFactory.actionViewModel = (actionRep: ActionMember, routeData: PaneRouteData) => {
            var actionViewModel = new ActionViewModel();

            const parms = routeData.actionParams;
            const paneId = routeData.paneId;

            actionViewModel.actionRep = actionRep;
            actionViewModel.title = actionRep.extensions().friendlyName();
            actionViewModel.menuPath = actionRep.extensions().menuPath() || "";
            actionViewModel.disabled = () => !!actionRep.disabledReason();

            if (actionViewModel.disabled()) {
                actionViewModel.description = actionRep.disabledReason();
            } else {
                actionViewModel.description = actionRep.extensions().description();
            }

            const parameters = _.pick(actionRep.parameters(), p => !p.isCollectionContributed()) as _.Dictionary<Parameter>;
            actionViewModel.parameters = _.map(parameters, parm => viewModelFactory.parameterViewModel(parm, parms[parm.parameterId()], paneId));

            actionViewModel.executeInvoke = (pps: ParameterViewModel[], right?: boolean) => {
                const parmMap = _.zipObject(_.map(pps, p => p.id), _.map(pps, p => p.getValue())) as _.Dictionary<Value>;
                _.forEach(pps, p => urlManager.setParameterValue(actionRep.actionId(), p.parameterRep, p.getValue(), paneId, false));
                return context.invokeAction(actionRep, clickHandler.pane(paneId, right), parmMap);
            }

            // open dialog on current pane always - invoke action goes to pane indicated by click
            actionViewModel.doInvoke = actionRep.extensions().hasParams() ?
                (right?: boolean) => {
                    focusManager.focusOverrideOff();
                    urlManager.setDialog(actionRep.actionId(), paneId);
                } :
                (right?: boolean) => {
                    focusManager.focusOverrideOff();
                    const pps = actionViewModel.parameters;
                    actionViewModel.executeInvoke(pps, right);
                    // todo display error if fails on parent ?
                };

            return actionViewModel;
        };


        viewModelFactory.handleErrorResponse = (err: ErrorMap, vm: MessageViewModel, vms: ValueViewModel[]) => {

            let requiredFieldsMissing = false; // only show warning message if we have nothing else 
            let fieldValidationErrors = false;

            _.each(vms, vmi => {
                const errorValue = err.valuesMap()[vmi.id];

                if (errorValue) {
                    vmi.value = errorValue.value.toValueString();

                    const reason = errorValue.invalidReason;
                    if (reason) {
                        if (reason === "Mandatory") {
                            const r = "REQUIRED";
                            requiredFieldsMissing = true;
                            vmi.description = vmi.description.indexOf(r) === 0 ? vmi.description : `${r} ${vmi.description}`;
                        } else {
                            vmi.message = reason;
                            fieldValidationErrors = true;
                        }
                    }
                }
            });

            let msg = "";
            if (err.invalidReason()) msg += err.invalidReason();
            if (requiredFieldsMissing) msg += "Please complete REQUIRED fields. ";
            if (fieldValidationErrors) msg += "See field validation message(s). ";
            if (!msg) msg = err.warningMessage;
            vm.message = msg;

        }

     
        viewModelFactory.propertyViewModel = (propertyRep: PropertyMember, id: string, previousValue: Value, paneId: number) => {
            const propertyViewModel = new PropertyViewModel();


            propertyViewModel.title = propertyRep.extensions().friendlyName();
            propertyViewModel.optional = propertyRep.extensions().optional();
            propertyViewModel.onPaneId = paneId;
            propertyViewModel.propertyRep = propertyRep;

            const required = propertyViewModel.optional ? "" : "* ";

            propertyViewModel.description = required + propertyRep.extensions().description();

            const value = previousValue || propertyRep.value();

            if (propertyRep.extensions().returnType() === "string" && propertyRep.extensions().format() === "date-time") {
                const rawValue = value ? value.toString() : "";
                const dateValue = Date.parse(rawValue);
                propertyViewModel.value = dateValue ? new Date(rawValue) : null;
            } else {
                propertyViewModel.value = propertyRep.isScalar() ? value.scalar() : value.isNull() ? propertyViewModel.description : value.toString();
            }


            propertyViewModel.type = propertyRep.isScalar() ? "scalar" : "ref";
            propertyViewModel.returnType = propertyRep.extensions().returnType();
            propertyViewModel.format = propertyRep.extensions().format();
            propertyViewModel.reference = propertyRep.isScalar() || value.isNull() ? "" : value.link().href();
            propertyViewModel.draggableType = propertyViewModel.returnType;

            propertyViewModel.canDropOn = (targetType: string) => context.isSubTypeOf(propertyViewModel.returnType, targetType);

            propertyViewModel.drop = (newValue: IDraggableViewModel) => {
                context.isSubTypeOf(newValue.draggableType, propertyViewModel.returnType).
                    then((canDrop: boolean) => {
                        if (canDrop) {
                            propertyViewModel.setNewValue(newValue);
                        }
                    });
            };

            propertyViewModel.doClick = (right?: boolean) => urlManager.setProperty(propertyRep, clickHandler.pane(paneId, right));
            if (propertyRep.attachmentLink() != null) {
                propertyViewModel.attachment = AttachmentViewModel.create(propertyRep.attachmentLink().href(),
                    propertyRep.attachmentLink().type().asString,
                    propertyRep.attachmentLink().title());
            }

            // only set color if has value 
            propertyViewModel.color = propertyViewModel.value ? color.toColorFromType(propertyRep.extensions().returnType()) : "";

            propertyViewModel.id = id;
            propertyViewModel.argId = `${id.toLowerCase() }`;
            propertyViewModel.paneArgId = `${propertyViewModel.argId}${paneId}`;
            propertyViewModel.isEditable = !propertyRep.disabledReason();
            propertyViewModel.choices = [];
            propertyViewModel.hasPrompt = propertyRep.hasPrompt();

            if (propertyRep.hasChoices()) {

                const choices = propertyRep.choices();

                if (choices) {
                    propertyViewModel.choices = _.map(choices, (v, n) => ChoiceViewModel.create(v, id, n));
                }
            }

            propertyViewModel.hasChoices = propertyViewModel.choices.length > 0;
            propertyViewModel.hasPrompt = !!propertyRep.promptLink() && !!propertyRep.promptLink().arguments()["x-ro-searchTerm"];
            propertyViewModel.hasConditionalChoices = !!propertyRep.promptLink() && !propertyViewModel.hasPrompt;

            if (propertyViewModel.hasPrompt || propertyViewModel.hasConditionalChoices) {
                const promptRep: PromptRepresentation = propertyRep.getPrompts();

                if (propertyViewModel.hasPrompt) {
                    propertyViewModel.prompt = _.partial(context.prompt, promptRep, id);
                    propertyViewModel.minLength = propertyRep.promptLink().extensions().minLength();
                }

                if (propertyViewModel.hasConditionalChoices) {
                    propertyViewModel.conditionalChoices = _.partial(context.conditionalChoices, promptRep, id);
                    propertyViewModel.arguments = _.object<_.Dictionary<Value>>(_.map(propertyRep.promptLink().arguments(), (v: any, key) => [key, new Value(v.value)]));
                }
            }

            if (propertyViewModel.hasChoices || propertyViewModel.hasPrompt || propertyViewModel.hasConditionalChoices) {

                var currentChoice: ChoiceViewModel = ChoiceViewModel.create(value, id);

                if (propertyViewModel.hasPrompt || propertyViewModel.hasConditionalChoices) {
                    propertyViewModel.choice = currentChoice;
                } else {
                    propertyViewModel.choice = _.find(propertyViewModel.choices, (c: ChoiceViewModel) => c.match(currentChoice));
                }
            }

            if (propertyRep.isScalar()) {
                const remoteMask = propertyRep.extensions().mask();
                const localFilter = mask.toLocalFilter(remoteMask) || mask.defaultLocalFilter(propertyRep.extensions().format());
                if (localFilter) {
                    propertyViewModel.formattedValue = $filter(localFilter.name)(propertyViewModel.value, localFilter.mask);
                } else {
                    propertyViewModel.formattedValue = propertyViewModel.value ? propertyViewModel.value.toString() : "";
                }
            }

            // if a reference and no way to set (ie not choices or autocomplete) use autoautocomplete
            if (propertyViewModel.type === "ref" && !propertyViewModel.hasPrompt && !propertyViewModel.hasChoices && !propertyViewModel.hasConditionalChoices) {
                addAutoAutoComplete(propertyViewModel, ChoiceViewModel.create(value, id), id, value);
            }

            return propertyViewModel;
        };

        viewModelFactory.getItems = (links: Link[], populateItems: boolean, routeData: PaneRouteData, listViewModel : ListViewModel | CollectionViewModel) => {
            const selectedItems = routeData.selectedItems;

            const items = _.map(links, (link, i) => viewModelFactory.itemViewModel(link, routeData.paneId, selectedItems[i]));

            if (populateItems) {

                _.forEach(items, itemViewModel => {
                    const tempTgt = itemViewModel.link.getTarget() as DomainObjectRepresentation;

                    context.getObject(routeData.paneId, tempTgt.getDtId().dt, tempTgt.getDtId().id).
                        then((obj: DomainObjectRepresentation) => {

                            itemViewModel.target = viewModelFactory.tableRowViewModel(obj, routeData);

                            if (!listViewModel.header) {
                                listViewModel.header = _.map(itemViewModel.target.properties, property => property.title);
                                focusManager.focusOverrideOff();
                                focusManager.focusOn(FocusTarget.TableItem, 0, urlManager.currentpane());
                            }

                        });
                });
            }

            return items;
        }

        viewModelFactory.collectionViewModel = (collectionRep: CollectionMember, routeData: PaneRouteData) => {
            const collectionViewModel = new CollectionViewModel();

            const links = collectionRep.value();
            const paneId = routeData.paneId;
            const state = routeData.collections[collectionRep.collectionId()];

            collectionViewModel.collectionRep = collectionRep;
            collectionViewModel.onPaneId = paneId;

            collectionViewModel.title = collectionRep.extensions().friendlyName();
            collectionViewModel.size = links.length;
            collectionViewModel.pluralName = collectionRep.extensions().pluralName();
            collectionViewModel.color = color.toColorFromType(collectionRep.extensions().elementType());

            collectionViewModel.items = viewModelFactory.getItems(links, state === CollectionViewState.Table, routeData, collectionViewModel);

            switch (state) {
                case CollectionViewState.List:
                    collectionViewModel.template = collectionListTemplate;
                    break;
                case CollectionViewState.Table:
                    collectionViewModel.template = collectionTableTemplate;
                    break;
                default:
                    collectionViewModel.template = collectionSummaryTemplate;
            }

            const setState = _.partial(urlManager.setCollectionMemberState, paneId, collectionRep.collectionId());
            collectionViewModel.doSummary = () => setState(CollectionViewState.Summary);
            collectionViewModel.doList = () => setState(CollectionViewState.List);
            collectionViewModel.doTable = () => setState(CollectionViewState.Table);

            return collectionViewModel;
        };

      
        viewModelFactory.listPlaceholderViewModel = (routeData : PaneRouteData) => {
            const collectionPlaceholderViewModel = new CollectionPlaceholderViewModel();

            collectionPlaceholderViewModel.description = () => `Page ${routeData.page}`;

            const recreate = () =>
                routeData.objectId ?
                    context.getListFromObject(routeData.paneId, routeData.objectId, routeData.actionId, routeData.actionParams, routeData.page, routeData.pageSize) :
                    context.getListFromMenu(routeData.paneId, routeData.menuId, routeData.actionId, routeData.actionParams, routeData.page, routeData.pageSize);


            collectionPlaceholderViewModel.reload = () =>
                recreate().then(() => {
                $route.reload();
            });
            return collectionPlaceholderViewModel;
        }

        viewModelFactory.servicesViewModel = (servicesRep: DomainServicesRepresentation) => {
            const servicesViewModel = new ServicesViewModel();

            // filter out contributed action services 
            const links = _.filter(servicesRep.value(), m => {
                const sid = m.rel().parms[0].value;
                return sid.indexOf("ContributedActions") === -1;
            });

            servicesViewModel.title = "Services";
            servicesViewModel.color = "bg-color-darkBlue";
            servicesViewModel.items = _.map(links, link => viewModelFactory.linkViewModel(link, 1));
            return servicesViewModel;
        };

        viewModelFactory.serviceViewModel = (serviceRep: DomainObjectRepresentation, routeData: PaneRouteData) => {
            const serviceViewModel = new ServiceViewModel();
            const actions = serviceRep.actionMembers();
            serviceViewModel.serviceId = serviceRep.serviceId();
            serviceViewModel.title = serviceRep.title();
            serviceViewModel.actions = _.map(actions, action => viewModelFactory.actionViewModel( action, routeData));
            serviceViewModel.color = color.toColorFromType(serviceRep.serviceId());

            return serviceViewModel;
        };
  
        
        viewModelFactory.tableRowViewModel = (objectRep: DomainObjectRepresentation, routeData: PaneRouteData): TableRowViewModel => {
            const tableRowViewModel = new TableRowViewModel();
            const properties = objectRep.propertyMembers();
            tableRowViewModel.properties = _.map(properties, (property, id) => viewModelFactory.propertyViewModel(property, id, null, routeData.paneId));

            return tableRowViewModel;
        };

        let cachedToolBarViewModel: ToolBarViewModel;

        function getToolBarViewModel() {
            if (!cachedToolBarViewModel) {
                const tvm = new ToolBarViewModel();
                tvm.goHome = (right?: boolean) => {
                    focusManager.focusOverrideOff();
                    urlManager.setHome(clickHandler.pane(1, right));
                };
                tvm.goBack = () => {
                    focusManager.focusOverrideOff();
                    navigation.back();
                };
                tvm.goForward = () => {
                    focusManager.focusOverrideOff();
                    navigation.forward();
                };
                tvm.swapPanes = () => urlManager.swapPanes();
                tvm.singlePane = (right?: boolean) => {
                    urlManager.singlePane(clickHandler.pane(1, right));
                    focusManager.refresh(1);
                };
                tvm.cicero = () => {
                    urlManager.singlePane(clickHandler.pane(1));
                    urlManager.cicero();
                }
                tvm.template = appBarTemplate;
                tvm.footerTemplate = footerTemplate;

                $rootScope.$on("ajax-change", (event, count) =>
                    tvm.loading = count > 0 ? "Loading..." : "");
                $rootScope.$on("back", () => {
                    focusManager.focusOverrideOff();
                    navigation.back();
                });
                $rootScope.$on("forward", () => {
                    focusManager.focusOverrideOff();
                    navigation.forward();
                });

                cachedToolBarViewModel = tvm;
            }
            return cachedToolBarViewModel;
        }

        viewModelFactory.toolBarViewModel = () => getToolBarViewModel();

        let cvm: CiceroViewModel = null;

        viewModelFactory.ciceroViewModel = () => {
            if (cvm == null) {
                cvm = new CiceroViewModel();
                cvm.parseInput = (input: string) => {
                    cvm.previousInput = input;
                    commandFactory.parseInput(input, cvm);
                };
                cvm.renderHome = (routeData: PaneRouteData) => {
                    //TODO: Could put this in a function passed into a render method on CVM
                    if (cvm.message) {
                        cvm.outputMessageThenClearIt();
                    } else {
                        if (routeData.menuId) {
                            context.getMenu(routeData.menuId)
                                .then((menu: MenuRepresentation) => {
                                    let output = "";
                                    output += menu.title() + " menu" + ". ";
                                    output += renderActionDialogIfOpen(menu, routeData);
                                    cvm.clearInput();
                                    cvm.output = output;
                                });
                        }
                        else {
                            cvm.clearInput();
                            cvm.output = "Welcome to Cicero";
                        }
                    }
                };
                cvm.renderObject = (routeData: PaneRouteData) => {
                    if (cvm.message) {
                        cvm.outputMessageThenClearIt();
                    } else {
                        const [domainType, ...id] = routeData.objectId.split("-");
                        context.getObject(1, domainType, id) //TODO: move following code out into a ICireroRenderers service with methods for rendering each context type
                            .then((obj: DomainObjectRepresentation) => {
                                let output = "";
                                const openCollIds = openCollectionIds(routeData);
                                if (_.any(openCollIds)) {
                                    const id = openCollIds[0];
                                    const coll = obj.collectionMember(id);
                                    output += `Collection: ${coll.extensions().friendlyName() } on ${Helpers.typePlusTitle(obj) },  `;
                                    switch (coll.size()) {
                                        case 0:
                                            output += "empty";
                                            break;
                                        case 1:
                                            output += "1 item";
                                            break;
                                        default:
                                            output += `${coll.size() } items`;
                                    }
                                } else {
                                    if (routeData.edit) {
                                        output += "Editing ";
                                    }
                                    output += Helpers.typePlusTitle(obj) + ". ";
                                    output += renderActionDialogIfOpen(obj, routeData);
                                }
                                cvm.clearInput();
                                cvm.output = output;
                            });
                    }
                };
                cvm.renderList = (routeData: PaneRouteData) => {
                    if (cvm.message) {
                        cvm.outputMessageThenClearIt();
                    } else {
                        const listPromise = context.getListFromMenu(1, routeData.menuId, routeData.actionId, routeData.actionParams, routeData.page, routeData.pageSize);
                        listPromise.then((list: ListRepresentation) => {
                            context.getMenu(routeData.menuId).then((menu: MenuRepresentation) => {
                                const count = list.value().length;
                                const numPages = list.pagination().numPages;
                                let description: string;
                                if (numPages > 1) {
                                    const page = list.pagination().page;
                                    const totalCount = list.pagination().totalCount;
                                    description = `Page ${page} of ${numPages} containing ${count} of ${totalCount} items`;
                                } else {
                                    description = `${count} items`;
                                }
                                const actionMember = menu.actionMember(routeData.actionId);
                                const actionName = actionMember.extensions().friendlyName();
                                cvm.clearInput();
                                cvm.output = `Result from ${actionName}: ${description}`;
                            });
                        });
                    }
                };
                cvm.renderError = () => {
                    const err = context.getError();
                    cvm.clearInput();
                    cvm.output = `Sorry, an application error has occurred. ${err.message() }`;
                };
            }
            return cvm;
        };
    });

    //Returns collection Ids for any collections on an object that are currently in List or Table mode
    export function openCollectionIds(routeData: PaneRouteData ): string[] {
        return _.filter(_.keys(routeData.collections), k => routeData.collections[k] !== CollectionViewState.Summary);
    }

    function renderActionDialogIfOpen(
        repWithActions: IHasActions,
        routeData: PaneRouteData): string {
        let output = "";
        if (routeData.dialogId) {
            const actionMember = repWithActions.actionMember(routeData.dialogId);
            const actionName = actionMember.extensions().friendlyName();
            output += `Action dialog: ${actionName}. `;
            _.forEach(routeData.dialogFields, (value, key) => {
                output += Helpers.friendlyNameForParam(actionMember, key) + ": ";
                output += value.toString() || "empty";
                output += ", ";
            });
        }
        return output;
    }

}