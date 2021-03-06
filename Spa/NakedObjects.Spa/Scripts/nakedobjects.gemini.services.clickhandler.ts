/// <reference path="typings/lodash/lodash.d.ts" />
/// <reference path="nakedobjects.models.ts" />

module NakedObjects.Angular.Gemini {

    export interface IClickHandler {
        pane(currentPane: number, right?: boolean): number;
    }

    app.service("clickHandler", function () {
        const clickHandler = <IClickHandler>this;

        function leftRightClickHandler(currentPane: number, right = false): number {
            return right ? 2 : 1;
        }

        function sameOtherClickHandler(currentPane: number, right = false): number {
            const otherPane = currentPane === 1 ? 2 : 1;
            return right ? otherPane : currentPane;
        }

        clickHandler.pane = sameOtherClickHandler;

    });
}