﻿// Copyright Naked Objects Group Ltd, 45 Station Road, Henley on Thames, UK, RG9 1AT
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0.
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and limitations under the License.
module DomainTypeActionInvoke26

open NUnit.Framework
open RestfulObjects.Mvc
open System.Net
open System.Net.Http.Headers
open Newtonsoft.Json.Linq
open System.Web
open RestfulObjects.Snapshot.Constants
open System.Web.Http
open System.Linq
open RestTestFunctions

let VerifyResult result resultValue oType oRel ooType ooRel = 
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    let rurl = sprintf "http://localhost/domain-types/%s" ooType
    let args = TObjectJson([ TProperty(ooRel, TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(rurl)) ])) ])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(oRel))
          TProperty(JsonPropertyNames.Value, TObjectVal(resultValue))
          
          TProperty
              (JsonPropertyNames.Links, 
               
               TArray
                   ([ TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) 
                           :: makeGetLinkProp RelValues.Self (sprintf "domain-types/%s/type-actions/%s/invoke" oType oRel) RepresentationTypes.TypeActionResult 
                                  "") ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
    Assert.AreEqual(new typeType(RepresentationTypes.TypeActionResult), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult

let VerifyFilterResult result resultValue oType oRel argsHref ooRel = 
    let jsonResult = readSnapshotToJson result
    let parsedResult = JObject.Parse(jsonResult)
    
    let args = TObjectJson([ TProperty(ooRel, TArray( argsHref ) )])
    
    let expected = 
        [ TProperty(JsonPropertyNames.Id, TObjectVal(oRel))
          TProperty(JsonPropertyNames.Value, TArray(resultValue))
          
          TProperty
              (JsonPropertyNames.Links,            
               TArray
                   ([ TObjectJson
                          (TProperty(JsonPropertyNames.Arguments, args) 
                           :: makeGetLinkProp RelValues.Self (sprintf "domain-types/%s/type-actions/%s/invoke" oType oRel) RepresentationTypes.TypeActionFilterResult 
                                  "") ]))
          TProperty(JsonPropertyNames.Extensions, TObjectJson([])) ]
    Assert.AreEqual(HttpStatusCode.OK, result.StatusCode, jsonResult)
    Assert.AreEqual(new typeType(RepresentationTypes.TypeActionFilterResult), result.Content.Headers.ContentType)
    assertNonExpiringCache result
    compareObject expected parsedResult


let GetIsSubTypeOfReturnFalseSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let resultValue = false
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel

let GetIsSuperTypeOfReturnFalseSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let resultValue = false
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel

let GetIsSubTypeOfReturnTrueSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let resultValue = true
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel

let GetIsSuperTypeOfReturnTrueSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let resultValue = true
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel



let GetIsSubTypeOfReturnFalseFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let resultValue = false
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel (parms.ToString())
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel






let GetIsSuperTypeOfReturnFalseFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let resultValue = false
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel (parms.ToString())
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel

let GetIsSubTypeOfReturnTrueFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let resultValue = true
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel (parms.ToString())
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel

let GetIsSuperTypeOfReturnTrueFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType = ttc "RestfulObjects.Test.Data.WithActionObject"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let resultValue = true
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel (parms.ToString())
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    VerifyResult result resultValue oType oRel ooType ooRel

// filters 

let GetFilterSubTypesFromReturnEmptyFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "filterSubtypesFrom"
    let ooRel = JsonPropertyNames.SubTypes
    let resultValue = []
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JArray(new JObject(new JProperty(JsonPropertyNames.Href, oourl)))))))
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel (parms.ToString())
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)

    let rurl = sprintf "http://localhost/domain-types/%s" ooType
    let args = [ TObjectJson([TProperty(JsonPropertyNames.Href, TObjectVal(rurl)) ])]

    VerifyFilterResult result resultValue oType oRel args ooRel

let GetFilterSubTypesFromReturnSingleFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "filterSubtypesFrom"
    let ooRel = JsonPropertyNames.SubTypes
    let resultValue = [ TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" oType) RepresentationTypes.DomainType "") ]
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JArray(new JObject(new JProperty(JsonPropertyNames.Href, oourl)))))))
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel (parms.ToString())
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)

    let rurl = sprintf "http://localhost/domain-types/%s" ooType
    let args = [ TObjectJson([TProperty(JsonPropertyNames.Href, TObjectVal(rurl)) ])]

    VerifyFilterResult result resultValue oType oRel args ooRel

let GetFilterSuperTypesFromReturnSingleFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "filterSupertypesFrom"
    let ooRel = JsonPropertyNames.SuperTypes
    let resultValue = [ TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" oType) RepresentationTypes.DomainType "") ]
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JArray(new JObject(new JProperty(JsonPropertyNames.Href, oourl)))))))
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel (parms.ToString())
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)

    let rurl = sprintf "http://localhost/domain-types/%s" ooType
    let args = [ TObjectJson([TProperty(JsonPropertyNames.Href, TObjectVal(rurl)) ])]

    VerifyFilterResult result resultValue oType oRel args ooRel

let GetFilterSubTypesFromMultipleReturnSingleFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
   
    let ooType1 = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType2 = ttc "RestfulObjects.Test.Data.WithActionObject"
    let ooType3 = ttc "RestfulObjects.Test.Data.MostSimple"


    let oRel = "filterSubtypesFrom"
    let ooRel = JsonPropertyNames.SubTypes
    let resultValue = [ TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" ooType1) RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" ooType2) RepresentationTypes.DomainType "") ]
    
    let oourl1 = sprintf "http://localhost/domain-types/%s" ooType1
    let oourl2 = sprintf "http://localhost/domain-types/%s" ooType2
    let oourl3 = sprintf "http://localhost/domain-types/%s" ooType3 
    
    let o1 = new JObject(new JProperty(JsonPropertyNames.Href, oourl1))
    let o2 = new JObject(new JProperty(JsonPropertyNames.Href, oourl2))
    let o3 = new JObject(new JProperty(JsonPropertyNames.Href, oourl3))


    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JArray([o1;o2;o3])))))
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel (parms.ToString())
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)

    let rurl1 = sprintf "http://localhost/domain-types/%s" ooType1
    let rurl2 = sprintf "http://localhost/domain-types/%s" ooType2
    let rurl3 = sprintf "http://localhost/domain-types/%s" ooType3

    let args = 
        [ TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(rurl1)) ])
          TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(rurl2)) ])
          TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(rurl3)) ])  ]

    VerifyFilterResult result resultValue oType oRel args ooRel


let GetFilterSuperTypesFromMultipleReturnSingleFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithActionObject"
   
    let ooType1 = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType2 = ttc "RestfulObjects.Test.Data.WithActionObject"
    let ooType3 = ttc "RestfulObjects.Test.Data.MostSimple"


    let oRel = "filterSupertypesFrom"
    let ooRel = JsonPropertyNames.SuperTypes
    let resultValue = [ TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" ooType1) RepresentationTypes.DomainType "");
                        TObjectJson(makeGetLinkProp RelValues.DomainType (sprintf "domain-types/%s" ooType2) RepresentationTypes.DomainType "") ]
    
    let oourl1 = sprintf "http://localhost/domain-types/%s" ooType1
    let oourl2 = sprintf "http://localhost/domain-types/%s" ooType2
    let oourl3 = sprintf "http://localhost/domain-types/%s" ooType3 
    
    let o1 = new JObject(new JProperty(JsonPropertyNames.Href, oourl1))
    let o2 = new JObject(new JProperty(JsonPropertyNames.Href, oourl2))
    let o3 = new JObject(new JProperty(JsonPropertyNames.Href, oourl3))


    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JArray([o1;o2;o3])))))
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel (parms.ToString())
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)

    let rurl1 = sprintf "http://localhost/domain-types/%s" ooType1
    let rurl2 = sprintf "http://localhost/domain-types/%s" ooType2
    let rurl3 = sprintf "http://localhost/domain-types/%s" ooType3

    let args = 
        [ TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(rurl1)) ])
          TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(rurl2)) ])
          TObjectJson([ TProperty(JsonPropertyNames.Href, TObjectVal(rurl3)) ])  ]

    VerifyFilterResult result resultValue oType oRel args ooRel



let GetFilterSuperTypesFromReturnEmptyFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "filterSupertypesFrom"
    let ooRel = JsonPropertyNames.SuperTypes
    let resultValue = []
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JArray(new JObject(new JProperty(JsonPropertyNames.Href, oourl)))))))
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel (parms.ToString())
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)

    let rurl = sprintf "http://localhost/domain-types/%s" ooType
    let args = [ TObjectJson([TProperty(JsonPropertyNames.Href, TObjectVal(rurl)) ])]


    VerifyFilterResult result resultValue oType oRel args ooRel





let NotFoundTypeIsSubTypeOfSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" oType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundTypeIsSuperTypeOfSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" oType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundTypeIsSubTypeOfFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel parmsEncoded
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" oType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundTypeIsSuperTypeOfFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel parmsEncoded
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" oType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundActionSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "noSuchAction"
    let ooRel = JsonPropertyNames.SuperType
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type action noSuchAction in domain type %s\"" oType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundActionFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "noSuchAction"
    let ooRel = JsonPropertyNames.SuperType
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel parmsEncoded
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type action noSuchAction in domain type %s\"" oType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundSuperTypeIsSubTypeOfSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" ooType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundSubTypeIsSuperTypeOfSimpleParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" ooType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundSuperTypeIsSubTypeOfFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel parmsEncoded
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" ooType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

let NotFoundSubTypeIsSuperTypeOfFormalParms(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.WithAction"
    let ooType = ttc "RestfulObjects.Test.Data.NoSuchType"
    let oRel = "isSupertypeOf"
    let ooRel = JsonPropertyNames.SubType
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel parmsEncoded
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.NotFound, result.StatusCode, jsonResult)
    Assert.AreEqual(sprintf "199 RestfulObjects \"No such domain type %s\"" ooType, result.Headers.Warning.ToString())
    Assert.AreEqual("", jsonResult)

// not acceptable 
let MissingParmsIsSubTypeOf(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let oRel = "isSubtypeOf"
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke" oType oRel
    let args = CreateArgMapFromUrl ""
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
    Assert.AreEqual("199 RestfulObjects \"Missing arguments\"", result.Headers.Warning.First().ToString())
    Assert.AreEqual("", jsonResult)

let MalformedSimpleParmsIsSubTypeOf(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = "nosuchtype"
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    let args = CreateArgMapFromUrl qs
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.First().ToString())
    Assert.AreEqual("", jsonResult)

let MalformedFormalParmsIsSubTypeOf(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = "nosuchtype"
    let oourl = sprintf "http://localhost/domain-types/%s" ooType
    let parms = 
        new JObject(new JProperty(ooRel, new JObject(new JProperty(JsonPropertyNames.Value, new JObject(new JProperty(JsonPropertyNames.Href, oourl))))))
    let parmsEncoded = HttpUtility.UrlEncode(parms.ToString())
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel parmsEncoded
    let args = CreateArgMap parms
    api.Request <- jsonGetMsg (url)
    let result = api.GetInvokeTypeActions(oType, oRel, args)
    let jsonResult = readSnapshotToJson result
    Assert.AreEqual(HttpStatusCode.BadRequest, result.StatusCode, jsonResult)
    Assert.AreEqual("199 RestfulObjects \"Malformed arguments\"", result.Headers.Warning.First().ToString())
    Assert.AreEqual("", jsonResult)

let NotAcceptableIsSubTypeOf(api : RestfulObjectsControllerBase) = 
    let oType = ttc "RestfulObjects.Test.Data.MostSimple"
    let ooType = ttc "RestfulObjects.Test.Data.WithAction"
    let oRel = "isSubtypeOf"
    let ooRel = JsonPropertyNames.SuperType
    let qs = sprintf "%s=%s" ooRel ooType
    let url = sprintf "http://localhost/domain-types/%s/type-actions/%s/invoke?%s" oType oRel qs
    try 
        let args = CreateArgMapFromUrl qs
        let msg = jsonGetMsg (url)
        msg.Headers.Accept.Single().Parameters.Add(new NameValueHeaderValue("profile", (makeProfile RepresentationTypes.ActionDescription)))
        api.Request <- msg
        let result = api.GetInvokeTypeActions(oType, oRel, args)
        readSnapshotToJson result |> ignore
        Assert.Fail("expect exception")
    with :? HttpResponseException as ex -> Assert.AreEqual(HttpStatusCode.NotAcceptable, ex.Response.StatusCode)