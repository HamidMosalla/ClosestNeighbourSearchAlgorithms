using CapillarySale.BLL.Admin;
using CapillarySale.ViewModel.Admin.PathFinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CapillarySale.BLL.Admin.ClosestNeighbourFinder.NewAlgorithm.Models;
using CapillarySale.BLL.Admin.ClosestNeighbourFinder.NewAlgorithm.Utilities;
using ClosestNeighbourSearchAlgorithms;
using ClosestNeighbourSearchAlgorithms.Models;

namespace CapillarySale.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin, PathFinder")]
    public partial class PathFinderController : Controller
    {
        private PathFinderBll _pathFinderService;
        private FaaliatBLL _faaliatBLL = new FaaliatBLL();
        private PlaceTreeBLL _placeTreeBLL = new PlaceTreeBLL();

        public PathFinderController()
        {
            _pathFinderService = new PathFinderBll();
        }

        public virtual ViewResult Index()
        {
            var viewModel = new PathFinderIndexViewModel
            {
                //FaaliatSelectListItems = _faaliatBLL.GetAllFaaliat(), //_pathFinderService.GetFaaliatSelectListItems(),
                ProviceSelectListItems = _placeTreeBLL.GetProvinceSelectListItems()
            };

            return View(viewModel);
        }

        [HttpPost]
        public virtual JsonResult CalculateSubmittedPointsForFaaliat(int[] faaliatGroupIds, int idPlaceTree)
        {
            if (faaliatGroupIds == null || idPlaceTree == default(int)) return Json(new { status = "ParameterNotSupplied" });

            var submittedPoints = _pathFinderService.CalculateSubmittedPoints(faaliatGroupIds, idPlaceTree);

            return submittedPoints.Any(s => s.TotalSubmittedPoints != 0) ? Json(submittedPoints) : Json(new { status = "NoPointSubmitted" });
        }


        [HttpGet]
        public virtual JsonResult GetCityBasedOnProvince(int provinceId)
        {
            var citySelectData = _placeTreeBLL.GetCitySelectListItems(provinceId);

            return Json(citySelectData, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual JsonResult GetFaaliatGroupBasedOnCity(int cityId)
        {
            var citySelectData = _pathFinderService.GetFaaliatGroupSelectListItems(cityId);

            return Json(citySelectData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public virtual JsonResult GetCoordinates(PathFinderIndexViewModel viewModel)
        {
            if (!ModelState.IsValid) return Json(new { status = "FormIsNotValid" });

            var coordinates = _pathFinderService.GetCoordinatesByFaaliatGroup(viewModel.FaaliatGroupSelect, viewModel.CitySelect.Value);

            if (!coordinates.Any()) return Json(new { status = "NoCoordinateFound" });

            var pointClusters = new KDTree<double, Coordinate>(2, coordinates.ToArrayOfDoubles(), coordinates, KdTreeHelper.L2Norm_Squared_Double)
                                  .NearestNeighborClusterRadial(Radius.SuperSlowButAccurate, viewModel.PointsOnPath.Value, coordinates)
                                  .ToPointClosters();

            var pathGrid = _pathFinderService.GetListOfPathGrid(viewModel, pointClusters);

            return Json(new { pointClusters, pathGrid });
        }

        [HttpGet]
        public virtual JsonResult RefreshMap(int pointPerCluster)
        {
            var mapData = Session["MapData"] as List<PointCluster>;

            return mapData != null ? Json(mapData, JsonRequestBehavior.AllowGet) : Json(new { status = "NoPointFound" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public virtual JsonResult RefreshGrid(int pointPerCluster)
        {
            var gridData = Session["PathGridData"] as List<PathGrid>;

            var pathGrid = _pathFinderService.GetListOfPathGridForRefreshGrid(pointPerCluster, gridData);

            return pathGrid.Any() ?
                Json(new { pathGrid }, JsonRequestBehavior.AllowGet) :
                Json(new { status = "NoPointFound" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public void UpdateMapSessionForRefresh(IEnumerable<Guid> temporaryIds)
        {
            if (temporaryIds == null) return;

            var gridCachedData = Session["PathGridData"] as List<PathGrid>;

            _pathFinderService.UpdateCheckedStatusOfPathGrid(temporaryIds, gridCachedData);

            var updatedMap = _pathFinderService.UpdateSessionWithSelectedCoordinates(gridCachedData, temporaryIds);

            Session["PathGridData"] = gridCachedData;
            Session["MapData"] = updatedMap;
        }

        [HttpPost]
        public virtual PartialViewResult PathGrid(List<PathGrid> viewModel)
        {
            Session["PathGridData"] = viewModel;

            Session["MapData"] = _pathFinderService.GetNonEmptyAndCheckedPointClusters(viewModel);

            return PartialView("_PathGrid", viewModel);
        }

        [HttpGet]
        public virtual PartialViewResult PathGridEshterakDetail(Guid temporaryId)
        {
            var gridData = Session["PathGridData"] as List<PathGrid>;

            var eshteraks = _pathFinderService.GetEshterakForPath(temporaryId, gridData);

            return PartialView("_PathGridEshterakDetail", eshteraks);
        }

        [HttpPost]
        public virtual PartialViewResult ChangePathOfEshterakPartial(ChangePathOfEshterakViewModel viewModel)
        {
            if (!ModelState.IsValid) return PartialView("_ChangePathOfEshterak", viewModel);

            viewModel.PathSelectListItems = viewModel.PathSelectListItems;
            viewModel.PreviousPathName = viewModel.PreviousPathName;

            return PartialView("_ChangePathOfEshterak", viewModel);
        }

        [HttpPost]
        [ActionName("ChangePathOfEshterak")]
        public virtual JsonResult ChangePathOfEshterakSubmit(ChangePathOfEshterakViewModel viewModel)
        {
            if (!ModelState.IsValid) return Json(new { status = "FormIsNotValid" });

            var gridData = Session["PathGridData"] as List<PathGrid>;

            var updatedGridData = _pathFinderService.ChangePathOfEshterak(gridData, viewModel);

            Session["PathGridData"] = updatedGridData;

            Session["MapData"] = updatedGridData.Select(g => new PointCluster { PointClusterCoordinates = g.PointClusterCoordinates }).ToList();

            return Json(new { status = "Success" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual JsonResult RemoveEshterakFromPath(Guid temporaryPathId, Guid eshterakId)
        {
            if (temporaryPathId == default(Guid)) return Json(new { status = "temporaryPathId not supplied." });

            var gridData = Session["PathGridData"] as List<PathGrid>;

            _pathFinderService.RemoveEshterakFromPath(gridData, temporaryPathId, eshterakId);

            Session["PathGridData"] = gridData;

            return Json(new { status = "Success" });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual JsonResult SubmitPath(List<PathGrid> pathGrid, int tedadNoghatDarMasir)
        {
            //if (tedadNoghatDarMasir > 13)
            //{
            //    return Json(new { status = "CannotAcceptMoreThan13WayPoints" });
            //}

            var gridData = Session["PathGridData"] as List<PathGrid>;

            _pathFinderService.InsertPath(pathGrid, gridData, tedadNoghatDarMasir);

            return Json(new { status = "Success" });
        }

    }
}