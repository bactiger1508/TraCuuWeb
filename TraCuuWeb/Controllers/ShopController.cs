using Microsoft.AspNetCore.Mvc;
using TraCuuWeb.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace TraCuuWeb.Controllers
{
    public class ShopController : Controller
    {
        private readonly TraCuuGoiCuocContext _context;

        public ShopController(TraCuuGoiCuocContext context)
        {
            _context = context;
        }

        public IActionResult Index(string sortOrder, string searchString, int? selectedNhomLon, List<int> diDongIds, List<int> bangRongIds, string minPrice, string maxPrice, string searchType = "TenGoi", int page = 1)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["PriceSortParm"] = string.IsNullOrEmpty(sortOrder) ? "price_desc" : "";
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                ViewData["CurrentFilter"] = searchString;
            }
            else
            {
                ViewData["CurrentFilter"] = "";
            }
            ViewData["SelectedNhomLon"] = selectedNhomLon;
            ViewData["SelectedDiDongIds"] = diDongIds;
            ViewData["SelectedBangRongIds"] = bangRongIds;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;
            ViewData["SearchType"] = searchType;

            decimal? ParsePrice(string input)
            {
                if (string.IsNullOrWhiteSpace(input)) return null;
                var cleaned = input.Replace(".", "").Replace(",", "").Trim();
                if (decimal.TryParse(cleaned, out var result)) return result;
                return null;
            }

            var nhomLonsWithChildren = _context.NhomLons
               .Include(nl => nl.NhomGoiCuocDiDongs)
               .Include(nl => nl.NhomGoiCuocBangRongs)
               .ToList();
            ViewBag.NhomLonsWithChildren = nhomLonsWithChildren;

            var goiCuocs = _context.GoiCuocs
                                   .Include(g => g.GiaCuocs)
                                   .Include(g => g.IdNhomBangRongNavigation).ThenInclude(n => n.IdNhomLonNavigation)
                                   .Include(g => g.IdNhomDiDongNavigation).ThenInclude(n => n.IdNhomLonNavigation)
                                   .AsQueryable();

            // Tìm kiếm nâng cao
            if (!string.IsNullOrEmpty(searchString))
            {
                switch (searchType)
                {
                    case "TenGoi":
                        goiCuocs = goiCuocs.Where(s => s.TenGoi.Contains(searchString));
                        break;
                    case "TenDichVu":
                        goiCuocs = goiCuocs.Where(s => s.DichVuBundle != null && s.DichVuBundle.Contains(searchString));
                        break;
                    case "NoiDung":
                        goiCuocs = goiCuocs.Where(s => s.NoiDung != null && s.NoiDung.Contains(searchString));
                        break;
                }
            }

            var bangRongIdsNotNull = bangRongIds != null && bangRongIds.Any();
            var diDongIdsNotNull = diDongIds != null && diDongIds.Any();

            if (bangRongIdsNotNull || diDongIdsNotNull)
            {
                goiCuocs = goiCuocs.Where(g =>
                    (bangRongIdsNotNull && g.IdNhomBangRong.HasValue && bangRongIds.Contains(g.IdNhomBangRong.Value)) ||
                    (diDongIdsNotNull && g.IdNhomDiDong.HasValue && diDongIds.Contains(g.IdNhomDiDong.Value))
                );
            }
            else if (selectedNhomLon.HasValue && selectedNhomLon.Value > 0)
            {
                 goiCuocs = goiCuocs.Where(g =>
                    (g.IdNhomBangRongNavigation != null && g.IdNhomBangRongNavigation.IdNhomLon == selectedNhomLon.Value) ||
                    (g.IdNhomDiDongNavigation != null && g.IdNhomDiDongNavigation.IdNhomLon == selectedNhomLon.Value));
            }

            // Helper to get min price as decimal
            decimal? GetMinPrice(GoiCuoc g)
            {
                var validPrices = g.GiaCuocs
                    .Select(gc => ParsePrice(gc.Gia))
                    .Where(x => x.HasValue)
                    .Select(x => x.Value)
                    .ToList();
                return validPrices.Any() ? validPrices.Min() : (decimal?)null;
            }

            var allGoiCuocs = goiCuocs.ToList(); // Execute DB query and bring data to memory

            // In-memory price filtering
            var minPriceVal = ParsePrice(minPrice);
            var maxPriceVal = ParsePrice(maxPrice);
            if (minPriceVal.HasValue || maxPriceVal.HasValue)
            {
                allGoiCuocs = allGoiCuocs.Where(g => {
                    var gia = GetMinPrice(g);
                    if (!gia.HasValue) return false;
                    bool minOk = !minPriceVal.HasValue || gia.Value >= minPriceVal.Value;
                    bool maxOk = !maxPriceVal.HasValue || gia.Value <= maxPriceVal.Value;
                    return minOk && maxOk;
                }).ToList();
            }

            // Sắp xếp chỉ theo giá
            switch (sortOrder)
            {
                case "price_desc":
                    allGoiCuocs = allGoiCuocs.OrderByDescending(g => GetMinPrice(g) ?? decimal.MaxValue).ToList();
                    break;
                default:
                    allGoiCuocs = allGoiCuocs.OrderBy(g => GetMinPrice(g) ?? decimal.MaxValue).ToList();
                    break;
            }

            int pageSize = 6;
            int totalItems = allGoiCuocs.Count();
            var pagedGoiCuocs = allGoiCuocs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var allGoiCuocList = _context.GoiCuocs.ToList();
            var allNhomDiDongList = _context.NhomGoiCuocDiDongs.ToList();
            var allNhomBangRongList = _context.NhomGoiCuocBangRongs.ToList();
            // Đếm số lượng sản phẩm cho từng nhóm lớn
            var nhomLonCounts = nhomLonsWithChildren.ToDictionary(
                nl => nl.Id,
                nl => allGoiCuocList.Count(g =>
                    (g.IdNhomBangRongNavigation != null && g.IdNhomBangRongNavigation.IdNhomLon == nl.Id) ||
                    (g.IdNhomDiDongNavigation != null && g.IdNhomDiDongNavigation.IdNhomLon == nl.Id))
            );
            // Đếm số lượng sản phẩm cho từng nhóm nhỏ
            var nhomDiDongCounts = allNhomDiDongList.ToDictionary(
                n => n.Id,
                n => allGoiCuocList.Count(g => g.IdNhomDiDong == n.Id)
            );
            var nhomBangRongCounts = allNhomBangRongList.ToDictionary(
                n => n.Id,
                n => allGoiCuocList.Count(g => g.IdNhomBangRong == n.Id)
            );
            ViewBag.NhomLonCounts = nhomLonCounts;
            ViewBag.NhomDiDongCounts = nhomDiDongCounts;
            ViewBag.NhomBangRongCounts = nhomBangRongCounts;

            return View(pagedGoiCuocs);
        }

        public IActionResult Detail(int id)
        {
            var goi = _context.GoiCuocs
                .Include(g => g.GiaCuocs)
                .Include(g => g.IdNhomBangRongNavigation).ThenInclude(n => n.IdNhomLonNavigation)
                .Include(g => g.IdNhomDiDongNavigation).ThenInclude(n => n.IdNhomLonNavigation)
                .FirstOrDefault(g => g.Id == id);
            if (goi == null) return NotFound();

            // Lấy 3 sản phẩm bán chạy nhất dựa vào lịch sử đăng ký
            var bestSellerIds = _context.LichSuDangKies
                .GroupBy(l => l.IdGoiCuoc)
                .Select(g => new { IdGoiCuoc = g.Key, Count = g.Count() })
                .OrderByDescending(g => g.Count)
                .Take(3)
                .Select(g => g.IdGoiCuoc)
                .ToList();
            var bestSellers = _context.GoiCuocs
                .Include(g => g.GiaCuocs)
                .Where(g => bestSellerIds.Contains(g.Id))
                .ToList();
            ViewBag.BestSellers = bestSellers;

            // Lấy 8 gói cước liên quan cùng nhóm lớn, random, khác gói hiện tại
            int? nhomLonId = null;
            if (goi.IdNhomDiDongNavigation?.IdNhomLonNavigation != null)
                nhomLonId = goi.IdNhomDiDongNavigation.IdNhomLonNavigation.Id;
            else if (goi.IdNhomBangRongNavigation?.IdNhomLonNavigation != null)
                nhomLonId = goi.IdNhomBangRongNavigation.IdNhomLonNavigation.Id;
            if (nhomLonId != null)
            {
                var related = _context.GoiCuocs
                    .Include(g => g.GiaCuocs)
                    .Include(g => g.IdNhomBangRongNavigation).ThenInclude(n => n.IdNhomLonNavigation)
                    .Include(g => g.IdNhomDiDongNavigation).ThenInclude(n => n.IdNhomLonNavigation)
                    .Where(g => g.Id != goi.Id &&
                        ((g.IdNhomDiDongNavigation != null && g.IdNhomDiDongNavigation.IdNhomLon == nhomLonId) ||
                         (g.IdNhomBangRongNavigation != null && g.IdNhomBangRongNavigation.IdNhomLon == nhomLonId)))
                    .ToList();
                var rnd = new Random();
                var relatedRandom = related.OrderBy(x => rnd.Next()).Take(8).ToList();
                ViewBag.RelatedPackages = relatedRandom;
            }
            else
            {
                ViewBag.RelatedPackages = new List<TraCuuWeb.Data.GoiCuoc>();
            }

            return View(goi);
        }
    }
} 