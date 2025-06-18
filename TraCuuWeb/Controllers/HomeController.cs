using Microsoft.AspNetCore.Mvc;
using TraCuuWeb.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TraCuuWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly TraCuuGoiCuocContext _context;
        public HomeController(TraCuuGoiCuocContext context)
        {
            _context = context;
        }
        public IActionResult Index(int? categoryId, int page = 1)
        {
            int pageSize = 8;
            var nhomLons = _context.NhomLons.ToList();
            var goiCuocQuery = _context.GoiCuocs
                .Include(g => g.GiaCuocs)
                .Include(g => g.IdNhomBangRongNavigation).ThenInclude(n => n.IdNhomLonNavigation)
                .Include(g => g.IdNhomDiDongNavigation).ThenInclude(n => n.IdNhomLonNavigation)
                .AsQueryable();

            if (categoryId.HasValue && categoryId.Value != 0)
            {
                goiCuocQuery = goiCuocQuery.Where(g =>
                    (g.IdNhomBangRongNavigation != null && g.IdNhomBangRongNavigation.IdNhomLon == categoryId) ||
                    (g.IdNhomDiDongNavigation != null && g.IdNhomDiDongNavigation.IdNhomLon == categoryId)
                );
            }

            var totalItems = goiCuocQuery.Count();
            var goiCuocs = goiCuocQuery
                .OrderByDescending(g => g.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.NhomLons = nhomLons;
            ViewBag.CurrentCategory = categoryId ?? 0;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(goiCuocs);
        }
    }
} 