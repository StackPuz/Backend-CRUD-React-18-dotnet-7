using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using App.Models;
using ViewModel = App.ViewModels.Brand;

namespace App.Controllers
{
    [Authorize(Roles = "ADMIN,USER")]
    [Route("brands")]
    public class BrandController : Controller
    {
        private readonly DataContext _context;

        public BrandController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            string column = Request.Query["sc"];
            if (Util.IsInvalidSearch(typeof(ViewModel.Index.Brand), column)) {
                return Forbid();
            }
            int page = Request.Query["page"].Any() ? Convert.ToInt32(Request.Query["page"]) : 1;
            int size = Request.Query["size"].Any() ? Convert.ToInt32(Request.Query["size"]) : 10;
            string sort = Request.Query["sort"].Any() ? Request.Query["sort"].First() : "Id";
            bool sortDesc = Request.Query["sort"].Any() ? Request.Query["desc"].Any() : false;
            var query = _context.Brand
            .Select(e => new ViewModel.Index.Brand {
                Id = e.Id,
                Name = e.Name
            });
            if (Request.Query["sw"].Any()) {
                string search = Request.Query["sw"];
                query = query.Where(column, Request.Query["so"], search);
            }
            query = query.OrderBy(sort, sortDesc);
            int count = await query.CountAsync();
            int last = (int)Math.Ceiling(count / (double)size);
            var brands = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            return Ok(new { brands, last });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(int? id)
        {
            var brand = await _context.Brand
            .Select(e => new ViewModel.Detail.Brand {
                Id = e.Id,
                Name = e.Name
            }).FirstOrDefaultAsync(e => e.Id == id);
            var brandProducts = await _context.Brand
            .Join(
                _context.Product,
                brand => brand.Id,
                product => product.BrandId,
                (brand, product) => new { brand, product }
            )
            .Where(e => e.brand.Id == id)
            .Select(e => new ViewModel.Detail.BrandProduct {
                Name = e.product.Name,
                Price = e.product.Price,
                Id = e.product.Id
            }).ToListAsync();
            return Ok(new { brand, brandProducts });
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return Ok();
        }

        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] ViewModel.Create.Brand model)
        {
            if (ModelState.IsValid)
            {
                var brand = new Brand();
                brand.Name = model.Name;
                _context.Add(brand);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(Util.getError(ModelState));
        }

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            var item = await _context.Brand.FindAsync(id);
            var brand = new ViewModel.Edit.Brand {
                Id = item.Id,
                Name = item.Name
            };
            return Ok(new { brand });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] ViewModel.Edit.Brand model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var brand = await _context.Brand.FirstOrDefaultAsync(e => e.Id == id);
                    brand.Name = model.Name;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Brand.Any(e => e.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok();
            }
            return BadRequest(Util.getError(ModelState));
        }

        [HttpGet("{id}/delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            var brand = await _context.Brand
            .Select(e => new ViewModel.Delete.Brand {
                Id = e.Id,
                Name = e.Name
            }).FirstOrDefaultAsync(e => e.Id == id);
            return Ok(new { brand });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var brand = await _context.Brand.FindAsync(id);
            _context.Brand.Remove(brand);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}