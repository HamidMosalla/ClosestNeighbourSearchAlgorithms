"use strict";

define("PathFinderModule", function () {

    function faaliatSelectChangeEventHandler() {

        $("#FaaliatGroupSelect").on("change", function (e) {

            var idPlaceTree = $("#CitySelect").val();

            if (idPlaceTree === "") {
                pNotifyModule.warningNotice("انتخاب شهر", "لطفا ابتدا شهر انتخاب کنید.");
                return;
            }

            var $this = $(this);
            var faaliatGroupIds = $this.val();
            var spinnerElement = startAjaxSpinner("#path-finder-form-container", "Navy");

            $.post("/Admin/PathFinder/CalculateSubmittedPointsForFaaliat",
                { faaliatGroupIds: faaliatGroupIds, idPlaceTree: idPlaceTree },
                function (data, textStatus, jqXHR) {

                    if (data.status === "ParameterNotSupplied") {
                        pNotifyModule.warningNotice("فرم نامعتبر", "فرم معتبر نیست.");
                        return;
                    }

                    $("#submitted-points-label").slideUp(500);
                    if (data.status === "NoPointSubmitted") {
                        $("#submitted-point-table").slideUp(500);
                        $("#submitted-points-label").slideDown(500);
                        return;
                    }

                    var submittedPointsHtmlString = "";

                    data.forEach(function (value, index) {

                        submittedPointsHtmlString += `<tr><td>${value.FaaliatName}</td><td>${value.TotalSubmittedPoints}</td></tr>`;

                    });

                    var totalPointsCount = data.map(x=> x.TotalSubmittedPoints).reduce((a, b) => a + b, 0);

                    $("#submitted-point-table-body").html(submittedPointsHtmlString);
                    $("#total-point").text(totalPointsCount);
                    $("#submitted-point-table").slideDown(500);

                },
                "json"
            ).complete(function () {
                stopAjaxSpinner(spinnerElement);
            });

        });

    }

    function loadCitySelectByOstan() {

        $("#ProvinceSelect").on("change", function (e) {
            e.preventDefault();

            var $this = $(this);

            if ($this.val() === "") return;

            var $provinceId = $this.val();

            $.get("/Admin/PathFinder/GetCityBasedOnProvince",
                { provinceId: $provinceId },
                function (data, textStatus, jqXHR) {

                    var htmlString = '<option value="">لطفا یک مقدار انتخاب کنید</option>';

                    data.forEach(function (value, index) {

                        htmlString += '<option value="' + value.Value + '">' + value.Text + '</option>';
                    });

                    $("#CitySelect").html(htmlString);

                },
                "json"
            );

        });
    }

    function loadFaaliatGroupByCity() {

        $("#CitySelect").on("change", function (e) {
            e.preventDefault();

            var $this = $(this);

            if ($this.val() === "") return;

            var $cityId = $this.val();

            $.get("/Admin/PathFinder/GetFaaliatGroupBasedOnCity",
                { cityId: $cityId },
                function (data, textStatus, jqXHR) {

                    var htmlString = '<option value="">لطفا یک مقدار انتخاب کنید</option>';

                    data.forEach(function (value, index) {

                        htmlString += '<option value="' + value.Value + '">' + value.Text + '</option>';
                    });

                    $("#FaaliatGroupSelect").html(htmlString);

                },
                "json"
            );

        });
    }


    function submitPathButtonClickEventHandler() {

        $("#submit-paths-button").on("click", function (e) {
            e.preventDefault();

            $("#submit-paths-button").prop("disabled", true);
            $("#submit-paths-button").text("در حال پردازش ...");


            var antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();
            var tedadNoghatDarMasir = $("#PointsOnPath").val();

            var $formData = $("#path-grid-table-body tr").map(function () {
                var $thisTr = $(this);
                return {
                    TemporaryId: $thisTr.data("temporary-path-id"),
                    PathName: $thisTr.find("#path-name").val(),
                    ToziZamaniPishfarz: $thisTr.find("#default-time-distribution").val(),
                    BazeZamaniTour: $thisTr.find("#tour-time-span").val(),
                    PointsPerCluster: $thisTr.find("#point-per-cluster").text(),
                    TarikhEtebarTo: $("#TarikhEtebarTo").val(),
                    TarikhEtebarFrom: $("#TarikhEtebarFrom").val(),
                    Enheraf: $thisTr.find("#enheraf").text(),
                    PotansiyelForoosh: $thisTr.find("#sale-potential").text(),
                    GradeRiyali: $thisTr.find("#grade-riyali").text(),
                    GradeAvaliyeBazar: $thisTr.find("#grade-avaliye-bazar").text(),
                    OriginDestinationDistance: $thisTr.find("#origin-destination-distance").text(),
                    IdPlaceTree: $("#CitySelect").val()
                }
            }).toArray();

            $.post("/Admin/PathFinder/SubmitPath",
            { __RequestVerificationToken: antiForgeryToken, pathGrid: $formData, tedadNoghatDarMasir: tedadNoghatDarMasir },
            function (data, textStatus, jqXHR) {

                if (data.status === "Success") {
                    pNotifyModule.successNotice("ثبت موفق", "مسیرها با موفقیت ثبت شدند.");

                    $("#path-eshterak-div-container").slideUp(1000, function () {
                        $("#submit-paths-button").slideUp(500);
                        $("#path-grid-div-container").slideUp(500, function () {

                            $("#map-container").slideUp(200);
                            document.querySelector("#path-finder-form").reset();
                            $("#submitted-point-table").slideUp(500);
                            $("#FaaliatGroupSelect").select2();
                            $("#ProvinceSelect").select2();
                            $("#CitySelect").select2();

                            $("#submit-paths-button").prop("disabled", false);
                            $("#submit-paths-button").text("ثبت نهایی");
                        });

                    });

                }
            },
            "json"
        );

            
        });

    }

    return {
        calculateSubmittedPoints: faaliatSelectChangeEventHandler,
        addSubmitPathButtonClickEventHandler: submitPathButtonClickEventHandler,
        loadCitySelectByOstan: loadCitySelectByOstan,
        loadFaaliatGroupByCity: loadFaaliatGroupByCity
    };

});