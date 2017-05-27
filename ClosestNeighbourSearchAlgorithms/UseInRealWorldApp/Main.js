"use strict";

define("Main", ["PathFinderModule", "MapModule", "PathGridModule"], function (pathFinderModule, mapModule, pathGridModule) {

    pathFinderModule.calculateSubmittedPoints();
    pathFinderModule.addSubmitPathButtonClickEventHandler();
    pathFinderModule.loadCitySelectByOstan();
    pathFinderModule.loadFaaliatGroupByCity();
    mapModule.checkPathAndDrawMap();
    mapModule.warnClientIfMaximumWaysPointsExceeded();
    mapModule.incrementPointPerPathIfIncompatible();



    pathGridModule.addShowEshterakButtonClickEventHandler();
    pathGridModule.addshowChangeEshterakPathModalButtonClickEventHandler();
    pathGridModule.addSubmitChangePathButtonClickHandler();
    pathGridModule.addUpdateMapWithSelectedPathCheckboxEventHanlder();

    pathGridModule.removeEshterakFromPath();

});