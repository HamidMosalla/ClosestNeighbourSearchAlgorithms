using CapillarySale.ViewModel.Admin.PathFinder;
using CapillarySale.ViewModel.Admin.PathFinderEdit;
using DAL;
using ERPCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using CapillarySale.BLL.Admin.ClosestNeighbourFinder;
using CapillarySale.BLL.Admin.ClosestNeighbourFinder.NewAlgorithm.Models;
using CapillarySale.BLL.Admin.ClosestNeighbourFinder.NewAlgorithm.Utilities;
using ClosestNeighbourSearchAlgorithms.Models;

namespace CapillarySale.BLL.Admin
{
    internal class PathFinderBll
    {


        internal List<PointsPerFaaliat> CalculateSubmittedPoints(int[] faaliatGroupIds, int idPlaceTree)
        {
            using (var db = new PigisEntities())
            {
                return db.FaaliatGroupLanguage
                    .Where(f => f.IdLanguage == SystemSetting.IdLanguage)
                    .Join(faaliatGroupIds, f => f.IdFaaliatGroup, fa => fa, (f, fa) => f)
                    .Select(f => new PointsPerFaaliat
                    {
                        FaaliatName = f.Description,
                        TotalSubmittedPoints = f.FaaliatGroup.FaaliatToFaaliatGroup
                            .Select(fa => fa.Faaliat)
                            .SelectMany(fa => fa.Eshterak)
                            .Count(e => e.PlaceTree.Id == idPlaceTree &&
                                       (!e.EshterakToRoad.Any(t => t.Road.Vaaziat == true) || !e.EshterakToRoad.Any()) &&
                                       (e.IdTypeVaziatEshterak == 2 || e.IdTypeVaziatEshterak == 3) &&
                                        e.Geo != null)
                    }).ToList();
            }
        }

        internal Coordinate[] GetCoordinatesByFaaliatGroup(int[] faaliatGroupIds, int idPlaceTree)
        {
            using (var db = new PigisEntities())
            {
                return db.FaaliatGroup.Join(faaliatGroupIds, f => f.Id, fi => fi, (f, fi) => f)
                                      .SelectMany(f => f.FaaliatToFaaliatGroup)
                                      .Select(f => f.Faaliat)
                                      .SelectMany(f => f.Eshterak)
                                      .Where(GetEshterakPredicate(idPlaceTree))
                                      .Select(c => new //(this select is made because linq to entity doesn't support struct(Coordinate) types)
                                      {
                                          CoordinateId = c.Id,
                                          PotansielAvalie = c.PotansielAvalie,
                                          Latitude = c.Geo.Latitude.Value,
                                          Longitude = c.Geo.Longitude.Value,
                                          IdPlaceTree = idPlaceTree,
                                          CoordinateName = c.EshterakLanguage.FirstOrDefault(f => f.IdLanguage == SystemSetting.IdLanguage).Name,
                                          CoordinateAddress = c.EshterakAddress.FirstOrDefault(aa => aa.IdEshterak == c.Id) == null ? "فاقد آدرس" : c.EshterakAddress.FirstOrDefault(aa => aa.IdEshterak == c.Id).EshterakAddressLanguage.FirstOrDefault(l => l.IdLanguage == SystemSetting.IdLanguage).Address,
                                          CoordinateFaaliatName = c.Faaliat.FaaliatLanguage.FirstOrDefault(f => f.IdLanguage == SystemSetting.IdLanguage) != null ? c.Faaliat.FaaliatLanguage.FirstOrDefault(f => f.IdLanguage == SystemSetting.IdLanguage).Description : "فاقد نام"
                                      })
                                      .ToList()
                                      .Select(c => new Coordinate
                                      {
                                          CoordinateId = c.CoordinateId,
                                          PotansielAvalie = c.PotansielAvalie,
                                          Latitude = c.Latitude,
                                          Longitude = c.Longitude,
                                          IdPlaceTree = idPlaceTree,
                                          CoordinateName = c.CoordinateName,
                                          CoordinateAddress = c.CoordinateAddress,
                                          CoordinateFaaliatName = c.CoordinateFaaliatName
                                      }).ToArray();
            }
        }

        private static Expression<Func<Eshterak, bool>> GetEshterakPredicate(int idPlaceTree)
        {
            return eshterak => (eshterak.PlaceTree.Id == idPlaceTree &&
                                (eshterak.EshterakToRoad.Any(t => t.Road.Vaaziat != true) || !eshterak.EshterakToRoad.Any()) &&
                                eshterak.Geo != null &&
                                (eshterak.IdTypeVaziatEshterak == 2 || eshterak.IdTypeVaziatEshterak == 3));
        }

        public IEnumerable<PathGrid> GetListOfPathGrid(PathFinderIndexViewModel viewModel, List<PointCluster> pointClusters)
        {
            return pointClusters.Select((p, index) =>
                new PathGrid
                {
                    RowNumber = ++index,
                    PathName = $"مسیر شماره - {index}",
                    IdPlaceTree = p.IdPlaceTree,
                    TemporaryId = Guid.NewGuid(),
                    Checked = false,
                    ToziZamaniPishfarz = viewModel.DefaultTimeDistribution,
                    BazeZamaniTour = viewModel.TourTimeSpan,
                    PointClusterCoordinates = p.PointClusterCoordinates,
                    PointsPerCluster = p.PointClusterCoordinates.Count,
                    GradeAvaliyeBazar = p.PointClusterCoordinates.Average(a => a.PotansielAvalie),
                    Enheraf = viewModel.PointsOnPath - p.PointClusterCoordinates.Count,
                    OriginDestinationDistance = Math.Round((KdTreeHelper.GetDistanceCoordinate(p.PointClusterCoordinates.First(), p.PointClusterCoordinates.Last()) / 1000), 2)//convert to kilometer
                });
        }

        public List<PathGrid> GetListOfPathGridForRefreshGrid(int pointPerCluster, List<PathGrid> gridData)
        {
            return gridData.Select((p, index) =>
                new PathGrid
                {
                    RowNumber = ++index,
                    PathName = $"مسیر شماره - {index}",
                    TemporaryId = Guid.NewGuid(),
                    Checked = p.Checked,
                    ToziZamaniPishfarz = p.ToziZamaniPishfarz,
                    BazeZamaniTour = p.BazeZamaniTour,
                    PointClusterCoordinates = p.PointClusterCoordinates,
                    PointsPerCluster = p.PointClusterCoordinates.Count,
                    GradeAvaliyeBazar = p.PointClusterCoordinates.Average(a => a.PotansielAvalie),
                    Enheraf = pointPerCluster - p.PointClusterCoordinates.Count,
                    OriginDestinationDistance = Math.Round((KdTreeHelper.GetDistanceCoordinate(p.PointClusterCoordinates.First(), p.PointClusterCoordinates.Last()) / 1000), 2)//convert to kilometer
                }).ToList();
        }

        public List<PointCluster> GetNonEmptyAndCheckedPointClusters(List<PathGrid> viewModel)
        {
            return viewModel.Where(v => v.Checked && v.PointClusterCoordinates.Any())
                .Select(g => new PointCluster { PointClusterCoordinates = g.PointClusterCoordinates })
                .ToList();
        }

        public void UpdateCheckedStatusOfPathGrid(IEnumerable<Guid> temporaryIds, List<PathGrid> gridCachedData)
        {
            gridCachedData?.ForEach(g => g.Checked = temporaryIds.Any(t => t == g.TemporaryId));
        }

        internal List<PathsEshterakGridModel> GetEshterakForPath(Guid temporaryId, List<PathGrid> gridData)
        {
            if (gridData == null) throw new ArgumentNullException(nameof(gridData), "gridData Argument cannot be null.");

            using (var db = new PigisEntities())
            {
                var eshterakIds = gridData.Single(g => g.TemporaryId == temporaryId).PointClusterCoordinates.Select(p => p.CoordinateId);

                return
                    db.Eshterak.Join(eshterakIds, e => e.Id, es => es, (e, es) => e).ToList()
                        .Select(
                            (e, index) =>
                                new PathsEshterakGridModel
                                {
                                    EshterakId = e.Id,
                                    PathTemporaryId = temporaryId,
                                    RowNumber = ++index,
                                    GeoLocation = e.Geo,
                                    Faaliat = e.Faaliat.FaaliatLanguage.FirstOrDefault(f => f.IdLanguage == SystemSetting.IdLanguage)?.Description,
                                    CustomerCode = 0,
                                    CustomerName = e.EshterakLanguage.FirstOrDefault(f => f.IdLanguage == SystemSetting.IdLanguage)?.Name,
                                    SalePotential = 0,
                                    Address = e.EshterakAddress.FirstOrDefault(aa => aa.IdEshterak == e.Id) != null ?
                                    e.EshterakAddress.FirstOrDefault(aa => aa.IdEshterak == e.Id).EshterakAddressLanguage.FirstOrDefault(l => l.IdLanguage == SystemSetting.IdLanguage).Address : "",//e.EshterakLanguage.FirstOrDefault(f => f.IdLanguage == SystemSetting.IdLanguage)?.Address,
                                    GradeAvaliyeBazar = e.PotansielAvalie,
                                    RialiGrade = 0
                                }).ToList();
            }
        }

        internal List<SelectListItem> GetPathSelectListItems(List<PathGrid> gridData)
        {
            if (gridData == null) throw new ArgumentNullException(nameof(gridData), "gridData Argument cannot be null.");

            return gridData.Select(g => new SelectListItem { Text = g.PathName, Value = g.TemporaryId.ToString() }).ToList();
        }

        internal List<SelectListItem> GetFaaliatGroupSelectListItems(int cityId)
        {
            using (var db = new PigisEntities())
            {
                return db.FaaliatGroup
                    .Where(p => p.IdPlaceTree == cityId)
                    .Select(
                        p =>
                            new SelectListItem
                            {
                                Text = p.FaaliatGroupLanguage.FirstOrDefault(t => t.IdLanguage == SystemSetting.IdLanguage).Description,
                                Value = p.Id.ToString()
                            })
                    .ToList();
            }
        }




        public string GetPreviousPathName(List<PathGrid> gridData, Guid? pathTemporaryId)
        {
            if (gridData == null) throw new ArgumentNullException(nameof(gridData), "gridData Argument cannot be null.");

            if (pathTemporaryId == null) throw new ArgumentNullException(nameof(pathTemporaryId), "pathTemporaryId Argument cannot be null.");

            return gridData.Single(g => g.TemporaryId == pathTemporaryId).PathName;
        }

        public List<PathGrid> ChangePathOfEshterak(List<PathGrid> gridData, ChangePathOfEshterakViewModel viewModel)
        {
            if (gridData == null) throw new ArgumentNullException(nameof(gridData), "gridData Argument cannot be null.");

            var pathOrigin = gridData.Single(g => g.TemporaryId == viewModel.TemporaryPathId);
            var eshterak = pathOrigin.PointClusterCoordinates.Single(p => p.CoordinateId == viewModel.EshterakId);
            pathOrigin.PointClusterCoordinates.Remove(eshterak);

            var pathDestination = gridData.Single(g => g.TemporaryId == viewModel.PathSelect);
            pathDestination.PointClusterCoordinates.Add(eshterak);

            // if path origin doesn't have any point left, delete the path (since it doesn't have any coordinate)
            if (!pathOrigin.PointClusterCoordinates.Any()) gridData.Remove(pathOrigin);

            return gridData;
        }

        public void InsertPath(List<PathGrid> pathGrid, List<PathGrid> gridData, int tedadNoghatDarMasir)
        {
            if (gridData == null) throw new ArgumentNullException(nameof(gridData), "gridData Argument cannot be null.");

            using (var db = new PigisEntities())
            {
                pathGrid.ForEach(p => p.PointClusterCoordinates.AddRange(gridData.Single(g => g.TemporaryId == p.TemporaryId).PointClusterCoordinates));

                var roads =
                    pathGrid.Select(
                        p =>
                            new Road
                            {
                                TedadNoghatDarMasir = p.PointClusterCoordinates.Count,
                                BazeZamaniToor = p.BazeZamaniTour,
                                BazeZamaniTozie = p.ToziZamaniPishfarz,
                                RoadLanguage = new List<RoadLanguage> { new RoadLanguage { IdLanguage = SystemSetting.IdLanguage, Name = p.PathName } },
                                TarikhMiladiEtebar = p.TarikhEtebarFrom.ToShortMiladiDateTime(),
                                TarikhMiladiPayanEtebar = p.TarikhEtebarTo.ToShortMiladiDateTime(),
                                TarikhShamsiEtebar = p.TarikhEtebarFrom.ArabicDigitToWestern(),
                                TarikhPayanEtebar = p.TarikhEtebarTo.ArabicDigitToWestern(),
                                IdPlaceTree = p.IdPlaceTree,
                                EshterakToRoad = p.PointClusterCoordinates.Select(c => new EshterakToRoad { IdEshterak = c.CoordinateId, Id = Guid.NewGuid() }).ToList(),
                                //PersonelToRoad = new List<PersonelToRoad> { new PersonelToRoad { IdPersonel = SM.General.IdPersonel.Value, IsActive = true } },
                                TedadNoghte = tedadNoghatDarMasir,
                                Vaaziat = true
                            }).ToList();

                db.Road.AddRange(roads);
                db.SaveChanges();
            }
        }

        public List<PointCluster> UpdateSessionWithSelectedCoordinates(List<PathGrid> gridCachedData, IEnumerable<Guid> temporaryIds)
        {
            if (gridCachedData == null) throw new ArgumentNullException(nameof(gridCachedData), "gridData Argument cannot be null.");

            return gridCachedData.Join(temporaryIds, g => g.TemporaryId, t => t, (g, t) => g).Select(g => new PointCluster { PointClusterCoordinates = g.PointClusterCoordinates }).ToList();
        }


        public void RemoveEshterakFromPath(List<PathGrid> gridData, Guid temporaryPathId, Guid eshterakId) =>
            gridData.Single(g => g.TemporaryId == temporaryPathId)
                .PointClusterCoordinates
                .RemoveAll(p => p.CoordinateId == eshterakId);
    }
}