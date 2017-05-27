"use strict";

define("PathGridModule", ["MapModule"], function (mapModule) {

    function showEshterakButtonClickEventHandler() {

        $("body").on("click", ".show-eshterak-button", function (e) {
            e.preventDefault();

            var $this = $(this);
            var temporaryId = $this.data("temporary-in-memory-id");
            var Pathname = $this.data("path-name");

            $.get("/Admin/PathFinder/PathGridEshterakDetail",
                { temporaryId: temporaryId },
                function (data, textStatus, jqXHR) {

                    $("#path-eshterak-div-container").slideUp(500);
                    $("#path-eshterak-div").html(data);
                    $(".path-eshterak-div-container-widget-caption").html("جدول اشتراک مسیر : " + Pathname);
                    $("#path-eshterak-div-container").slideDown(500);

                    
                }, "HTML");

        });


    }

    function showChangeEshterakPathModalButtonClickEventHandler() {

        $("body").on("click", ".change-eshterak-path-button", function (e) {
            e.preventDefault();

            var $this = $(this);

            var FormData = new function () {
                this.EshterakId = $this.data("eshterak-id");
                this.TemporaryPathId = $this.data("temporary-path-id");

                this.PathSelectListItems = $("#path-grid-table-body tr")
                    .map((index, element) => {
                        return {
                            Text: $(element).find("#path-name").val(), Value: $(element)
                                .find("#path-checkbox").val()
                        };
                    }).toArray();

                this.PreviousPathName = $("#path-grid-table-body tr").filter("tr[data-temporary-path-id='" + this.TemporaryPathId + "']").find("#path-name").val();
            };

            $.post("/Admin/PathFinder/ChangePathOfEshterakPartial", FormData,
                function (data, textStatus, jqXHR) {

                    $("#change-eshterak-path-modal-body").html(data);
                    $("#PathSelect").select2();
                    $("#change-eshterak-path-modal").modal("show");

                }, "HTML"
            );

        });


    }

    function submitChangePathButtonClickHandler() {

        $("body").on("click", "#submit-change-path-button", function (e) {
            e.preventDefault();

            var $formData = $("#change-path-form :input").serialize();

            $.post("/Admin/PathFinder/ChangePathOfEshterak", $formData,
                function (data, textStatus, jqXHR) {

                    if (data.status === "Success") {
                        pNotifyModule.successNotice("تغییر موفق", "تغییر مسیر موفق آمیز بود.");
                        $("#change-eshterak-path-modal").modal("hide");
                        $("#path-eshterak-div-container").slideUp(500);

                        mapModule.reloadMap();
                        reloadPathGrid();
                    }

                },
                "json"
            );


        });


    }

    function removeEshterakFromPath() {

        $("body").on("click", ".remove-eshterak-path-button", function (e) {
            e.preventDefault();

            pNotifyModule.confirm(() => {

                var $this = $(this);
                var temporaryPathId = $this.data("temporary-path-id");
                var eshterakId = $this.data("eshterak-id");
                var antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

                $.post("/Admin/PathFinder/RemoveEshterakFromPath", { temporaryPathId: temporaryPathId, eshterakId: eshterakId, __RequestVerificationToken: antiForgeryToken },
                    function (data, textStatus, jqXHR) {

                        if (data.status === "Success") {
                            pNotifyModule.successNotice("حذف موفق", "اشتراک با موفقیت از مسیر حذف شد.");
                            $(`a[data-temporary-in-memory-id='${temporaryPathId}']`).click();
                        }

                    }, "json"
                );

            }, "تایید حذف", "آیا از حذف اشتراک از این مسیر اطمینان دارید؟");
        });
    }

    function reloadPathGrid() {

        var pointPerCluster = $("#PointsOnPath").val();

        $.get("/Admin/PathFinder/RefreshGrid",
            { pointPerCluster: pointPerCluster },
            function (data, textStatus, jqXHR) {

                mapModule.buildPathGrid(data);

            },
            "json");
    }

    function updateMapWithSelectedPathCheckboxEventHanlder() {

        $("body").on("change", ".path-checkbox", c => {

            var $this = $(c.target);

            var checkedPathIds = $(".path-checkbox:checked").map((i, el) => el.value).toArray();

            if (checkedPathIds.length === 0) {
                $("#map").html("");
                return;
            }


            $.post("/Admin/PathFinder/UpdateMapSessionForRefresh", { temporaryIds: checkedPathIds }, "json").complete(c => mapModule.reloadMap());
        });
    }

    return {
        addShowEshterakButtonClickEventHandler: showEshterakButtonClickEventHandler,
        addshowChangeEshterakPathModalButtonClickEventHandler: showChangeEshterakPathModalButtonClickEventHandler,
        addSubmitChangePathButtonClickHandler: submitChangePathButtonClickHandler,
        addUpdateMapWithSelectedPathCheckboxEventHanlder: updateMapWithSelectedPathCheckboxEventHanlder,
        removeEshterakFromPath: removeEshterakFromPath
    };

});