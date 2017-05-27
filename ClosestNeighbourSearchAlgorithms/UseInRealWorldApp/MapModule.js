"use strict";

define("MapModule", ["../../Helpers/UtilityFunctionsModule"], function (utilityFunctionsModule) {

    function buildPathGrid(data) {

        var spinnerElement = startAjaxSpinner("#path-grid-div", "#color");

        $.post("/Admin/PathFinder/PathGrid",
            { viewModel: data.pathGrid },
            function (innerData, textStatus, jqXHR) {

                $("#path-grid-div-container").slideUp(500);
                $("#path-grid-div").html(innerData);
                $("#path-grid-table").DataTable({ "bPaginate": false });
                $("#path-grid-div-container").slideDown(500);

            },
            "HTML"
        ).complete(function () {
            stopAjaxSpinner(spinnerElement);
        });
    }

    function checkPathAndDrawMapButtonClickEventHandler() {

        $("#check-path").on("click", function (e) {
            e.preventDefault();

            var $form = $("#path-finder-form");
            $("#map-container").slideUp(500);
            $("#path-eshterak-div-container").slideUp(500);

            if (!$form.valid()) return;

            var spinnerElement = startAjaxSpinner("#path-finder-form-div", "Blue");

            $.post("/Admin/PathFinder/GetCoordinates", $form.serialize(),
                function (data, textStatus, jqXHR) {

                    if (data.status === "NoCoordinateFound") {
                        pNotifyModule.warningNotice("نقطه یافت نشد.", "نقطه ای برای نمایش یافت نشد.");
                        return;
                    }

                    if (data.status === "CannotAcceptMoreThan13WayPoints") {
                        pNotifyModule.warningNotice("تعداد نقاط در مسیر نمی تواندبیشتر از 13 باشد.");
                        return;
                    }

                    //drawMap(data.pointClusters);

                },
                "json").success(function (data) {

                    if (data.status === "NoCoordinateFound") return;

                    buildPathGrid(data);
                    $("#submit-paths-button-container").slideDown(500);

                }).complete(() => stopAjaxSpinner(spinnerElement));

        });

    }

    function warnClientIfMaximumWaysPointsExceeded() {

        let warningAlreadyShown = false;

        $("#PointsOnPath").on("input", function () {

            var $value = +$(this).val();

            if ($value > 23 && !warningAlreadyShown) {
                warningAlreadyShown = true;
                pNotifyModule.warningNotice("هشدار", "ماکزیمم محدودیت ترسیم نقطه بر روی نقشه به ازای هر مسیر 23 نقطه است، نقطه های بیشتر صرفا جهت نمایش به مسیرهای یکپارچه 23 تایی تقسیم می شوند.");
            }

        });

    }

    function incrementPointPerPathIfIncompatible() {

        $("#PointsOnPath").on("focusout", function () {

            var $value = +$(this).val();

            var remainder = $value % 23;

            if (remainder === 1) {
                $(this).val($value + 1);
            }

        });
    }

    function getArrayOfWaypoints(value) {

        var wayPoints = [];

        $.each(value.PointClusterCoordinates, function (innerIndex, innerValue) {

            //if the element was the first or the last element which have been already added, we skip the loop with returning true
            if (innerIndex === 0 || innerIndex === value.PointClusterCoordinates.length - 1) return true;

            wayPoints.push({ location: new google.maps.LatLng(innerValue.Longitude, innerValue.Latitude), stopover: true });

        });

        return wayPoints;
    }

    function drawInfoWindows(pointClusters, map) {

        var flattenPoints = [].concat.apply([], pointClusters.map(x => x.PointClusterCoordinates));

        flattenPoints.forEach(f => {

            var infoWindow = new google.maps.InfoWindow({ content: `<div style="width:60px; height:20px; text-align:center;">${f.CoordinateName} </div>` });
            var coordinateLatLog = new google.maps.LatLng(f.Longitude, f.Latitude);
            infoWindow.setPosition(coordinateLatLog);
            infoWindow.open(map);

        });

    }

    function getMarkerText(index) {

        const lowerLimit = "A".charCodeAt(0);
        const upperLimit = "Z".charCodeAt(0);

        const limitBoundary = upperLimit - lowerLimit;
        const ratio = Math.round(index % limitBoundary);

        var currentChar = lowerLimit + ratio;

        return index < limitBoundary ? String.fromCharCode(currentChar) : (String.fromCharCode(currentChar) + String.fromCharCode(currentChar + 1));
    }

    function setMapMarkersWithInfoWindow(pointClusters, map) {

        var flattenPoints = [].concat.apply([], pointClusters.map(x => x.PointClusterCoordinates));

        flattenPoints.forEach((f, i) => {

            var infoWindow = new google.maps.InfoWindow({
                content: `<div style="direction: rtl;" class="panel panel-default">
                                <div class="panel-heading">مشخصات اشتراک</div>
                                <div class="panel-body">
                                    <table class="table table-hover table-striped table-responsive">

                                        <tbody>

                                            <tr>
                                                <td>نام اشتراک</td>
                                                <td>${f.CoordinateName}</td>
                                            </tr>

                                            <tr>
                                                <td>فعالیت اشتراک</td>
                                                <td>${f.CoordinateFaaliatName}</td>
                                            </tr>

                                            <tr>
                                                <td>آدرس اشتراک</td>
                                                <td>${f.CoordinateAddress}</td>
                                            </tr>

                                        </tbody>

                                    </table>
                                </div>
                            </div>`
            });

            var coordinateLatLog = new google.maps.LatLng(f.Longitude, f.Latitude);

            var marker = new google.maps.Marker({
                position: coordinateLatLog,
                map: map,
                title: f.CoordinateName,
                zIndex: 99999999,
                label: {
                    text: getMarkerText(i),
                    fontSize:"10px",
                    fontWeight: "bold"
                },
            });

            google.maps.event.addListener(marker, 'click', (function () {
                infoWindow.open(map, marker);
            }));

        });

        //var originMarker = new google.maps.Marker({
        //    position: origin,
        //    title: originCoordinate.CoordinateName,
        //    icon: {
        //        url: "http://maps.google.com/mapfiles/ms/icons/red-dot.png",
        //        labelOrigin: new google.maps.Point(75, 32),
        //        size: new google.maps.Size(32, 32),
        //        anchor: new google.maps.Point(16, 32)
        //    },
        //    label: {
        //        text: "5409 Madison St",
        //        color: "#C70E20",
        //        fontWeight: "bold"
        //    },
        //    map: map
        //});

        //var destinationMarker = new google.maps.Marker({
        //    position: destination,
        //    title: destinationCoordinate.CoordinateName,
        //    label: "Z",
        //    map: map
        //});

    }

    function mapApiWaypointsLimitExceeded(value) {
        return value.PointClusterCoordinates.length > 23;
    }

    function drawChunkedClusters(value, map, directionsService) {

        if (typeof Array.prototype.chunk === 'undefined') {

            var ceil = Math.ceil;
            Object.defineProperty(Array.prototype, 'chunk', {
                value: function (n) {
                    return Array.from(Array(ceil(this.length / n)), (_, i) => this.slice(i * n, i * n + n));
                }
            });

        }

        var chunkedArray = value.PointClusterCoordinates.chunk(23);

        var chunkedPathsColor = utilityFunctionsModule.getRandomColor();

        for (var i = 0; i < chunkedArray.length; i++) {

            let directionsRendererChunk = new google.maps.DirectionsRenderer({ suppressMarkers: true, polylineOptions: { strokeColor: chunkedPathsColor, strokeWeight: 6 } });
            directionsRendererChunk.setMap(map);

            let valueChunk = { PointClusterCoordinates: chunkedArray[i] };

            //set first element of every collection as origin
            let originChunk = new google.maps.LatLng(valueChunk.PointClusterCoordinates[0].Longitude, valueChunk.PointClusterCoordinates[0].Latitude);

            //set the last element of every collection as destination
            let destinationChunk = new google.maps.LatLng(
                valueChunk.PointClusterCoordinates[valueChunk.PointClusterCoordinates.length - 1].Longitude,
                valueChunk.PointClusterCoordinates[valueChunk.PointClusterCoordinates.length - 1].Latitude);

            let request = {
                origin: originChunk,
                destination: destinationChunk,
                optimizeWaypoints: true,
                waypoints: getArrayOfWaypoints(valueChunk),
                travelMode: google.maps.TravelMode['DRIVING']
            };

            directionsService.route(request, function (response, status) {
                if (status == 'OK') {
                    directionsRendererChunk.setDirections(response);
                }
            });
        }
    }

    function drawPathClusters(data, map, directionsService) {

        $.each(data, function (index, value) {

            if (mapApiWaypointsLimitExceeded(value)) {
                drawChunkedClusters(value, map, directionsService);
                return;
            }

            var directionsRenderer = new google.maps.DirectionsRenderer({ suppressMarkers: true, polylineOptions: { strokeColor: utilityFunctionsModule.getRandomColor(), strokeWeight: 6 } });
            directionsRenderer.setMap(map);

            //set first element of every collection as origin
            var originCoordinate = value.PointClusterCoordinates[0];
            var origin = new google.maps.LatLng(originCoordinate.Longitude, originCoordinate.Latitude);

            //set the last element of every collection as destination
            var destinationCoordinate = value.PointClusterCoordinates[value.PointClusterCoordinates.length - 1];
            var destination = new google.maps.LatLng(destinationCoordinate.Longitude, destinationCoordinate.Latitude);

            var request = {
                origin: origin,
                destination: destination,
                optimizeWaypoints: false,
                waypoints: getArrayOfWaypoints(value),
                travelMode: google.maps.TravelMode['DRIVING']
            };

            directionsService.route(request, function (response, status) {
                if (status == 'OK') {
                    directionsRenderer.setDirections(response);
                }
            });

        });

    }

    function drawMap(data) {

        var spinnerElement = startAjaxSpinner("#map", "Thistle");

        var mapOptions = { zoom: 20, center: null }

        var map = new google.maps.Map(document.getElementById('map'), mapOptions);

        var directionsService = new google.maps.DirectionsService();

        drawPathClusters(data, map, directionsService);
        //drawInfoWindows(data, map);
        setMapMarkersWithInfoWindow(data, map);

        $("#map-container").slideDown(500);
        $("#submit-paths-button-container").slideDown(500);

        stopAjaxSpinner(spinnerElement);
    }

    function reloadMap() {

        var pointPerCluster = $("#PointsOnPath").val();

        $.get("/Admin/PathFinder/RefreshMap",
            { pointPerCluster: pointPerCluster },
            function (data, textStatus, jqXHR) {

                drawMap(data);

            }, "json");
    }

    return {
        checkPathAndDrawMap: checkPathAndDrawMapButtonClickEventHandler,
        drawMap: drawMap,
        reloadMap: reloadMap,
        buildPathGrid: buildPathGrid,
        warnClientIfMaximumWaysPointsExceeded: warnClientIfMaximumWaysPointsExceeded,
        incrementPointPerPathIfIncompatible: incrementPointPerPathIfIncompatible
    };

});