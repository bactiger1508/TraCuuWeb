using Microsoft.AspNetCore.Mvc;
using TraCuuWeb.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;

namespace TraCuuWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly TraCuuGoiCuocContext _context;
        
        public AdminController(TraCuuGoiCuocContext context)
        {
            _context = context;
        }
        
        public IActionResult Index(string sortOrder, string searchString, int? selectedNhomLon, List<int> diDongIds, List<int> bangRongIds, string minPrice, string maxPrice, string searchType = "TenGoi", int page = 1)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["SearchType"] = searchType;
            ViewData["CurrentFilter"] = searchString ?? "";
            ViewData["SelectedNhomLon"] = selectedNhomLon;
            ViewData["SelectedDiDongIds"] = diDongIds;
            ViewData["SelectedBangRongIds"] = bangRongIds;
            ViewData["MinPrice"] = minPrice;
            ViewData["MaxPrice"] = maxPrice;

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

            decimal? GetMinPrice(GoiCuoc g)
            {
                var validPrices = g.GiaCuocs
                    .Select(gc => ParsePrice(gc.Gia))
                    .Where(x => x.HasValue)
                    .Select(x => x.Value)
                    .ToList();
                return validPrices.Any() ? validPrices.Min() : (decimal?)null;
            }

            var allGoiCuocs = goiCuocs.ToList();

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

            // Sắp xếp theo ID tăng dần
            allGoiCuocs = allGoiCuocs.OrderBy(g => g.Id).ToList();

            int pageSize = 10;
            int totalItems = allGoiCuocs.Count();
            var pagedGoiCuocs = allGoiCuocs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            return View(pagedGoiCuocs);
        }
        
        public IActionResult Create()
        {
            ViewBag.NhomBangRongs = _context.NhomGoiCuocBangRongs.ToList();
            ViewBag.NhomDiDongs = _context.NhomGoiCuocDiDongs.ToList();
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(GoiCuoc goiCuoc)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.GoiCuocs.Add(goiCuoc);
                    _context.SaveChanges();
                    TempData["Success"] = "Gói cước đã được thêm thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Có lỗi xảy ra khi thêm gói cước: " + ex.Message;
                }
            }
            
            ViewBag.NhomBangRongs = _context.NhomGoiCuocBangRongs.ToList();
            ViewBag.NhomDiDongs = _context.NhomGoiCuocDiDongs.ToList();
            return View(goiCuoc);
        }
        
        public IActionResult Edit(int id)
        {
            var goiCuoc = _context.GoiCuocs.Find(id);
            if (goiCuoc == null)
            {
                return NotFound();
            }
            
            ViewBag.NhomBangRongs = _context.NhomGoiCuocBangRongs.ToList();
            ViewBag.NhomDiDongs = _context.NhomGoiCuocDiDongs.ToList();
            return View(goiCuoc);
        }
        
        [HttpPost]
        public IActionResult Edit(int id, GoiCuoc goiCuoc)
        {
            if (id != goiCuoc.Id)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(goiCuoc);
                    _context.SaveChanges();
                    TempData["Success"] = "Gói cước đã được cập nhật thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GoiCuocExists(goiCuoc.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["Error"] = "Có lỗi xảy ra khi cập nhật gói cước.";
                        throw;
                    }
                }
            }
            
            ViewBag.NhomBangRongs = _context.NhomGoiCuocBangRongs.ToList();
            ViewBag.NhomDiDongs = _context.NhomGoiCuocDiDongs.ToList();
            return View(goiCuoc);
        }
        
        public IActionResult Delete(int id)
        {
            var goiCuoc = _context.GoiCuocs
                .Include(g => g.GiaCuocs)
                .Include(g => g.IdNhomBangRongNavigation).ThenInclude(n => n.IdNhomLonNavigation)
                .Include(g => g.IdNhomDiDongNavigation).ThenInclude(n => n.IdNhomLonNavigation)
                .FirstOrDefault(g => g.Id == id);
                
            if (goiCuoc == null)
            {
                return NotFound();
            }
            
            return View(goiCuoc);
        }
        
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var goiCuoc = _context.GoiCuocs.Find(id);
            if (goiCuoc != null)
            {
                try
                {
                    _context.GoiCuocs.Remove(goiCuoc);
                    _context.SaveChanges();
                    TempData["Success"] = "Gói cước đã được xóa thành công!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Có lỗi xảy ra khi xóa gói cước: " + ex.Message;
                }
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        private bool GoiCuocExists(int id)
        {
            return _context.GoiCuocs.Any(e => e.Id == id);
        }
    }
} 