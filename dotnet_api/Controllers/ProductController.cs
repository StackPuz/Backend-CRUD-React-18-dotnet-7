using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using App.Models;
using ViewModel = App.ViewModels.Product;

namespace App.Controllers
{
    [Authorize(Roles = "ADMIN,USER")]
    [Route("products")]
    public class ProductController : Controller
    {
        private readonly DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            string column = Request.Query["sc"];
            if (Util.IsInvalidSearch(typeof(ViewModel.Index.Product), column)) {
                return Forbid();
            }
            int page = Request.Query["page"].Any() ? Convert.ToInt32(Request.Query["page"]) : 1;
            int size = Request.Query["size"].Any() ? Convert.ToInt32(Request.Query["size"]) : 10;
            string sort = Request.Query["sort"].Any() ? Request.Query["sort"].First() : "Id";
            bool sortDesc = Request.Query["sort"].Any() ? Request.Query["desc"].Any() : false;
            var query = _context.Product
            .SelectMany(
                product => _context.Brand.Where(brand => product.BrandId == brand.Id).DefaultIfEmpty(),
                (product, brand) => new { product, brand }
            )
            .SelectMany(
                combine => _context.UserAccount.Where(userAccount => combine.product.CreateUser == userAccount.Id).DefaultIfEmpty(),
                (combined, userAccount) => new { combined.product, combined.brand, userAccount }
            )
            .Select(e => new ViewModel.Index.Product {
                Id = e.product.Id,
                Image = e.product.Image,
                Name = e.product.Name,
                Price = e.product.Price,
                BrandName = e.brand.Name,
                UserAccountName = e.userAccount.Name
            });
            if (Request.Query["sw"].Any()) {
                string search = Request.Query["sw"];
                query = query.Where(column, Request.Query["so"], search);
            }
            query = query.OrderBy(sort, sortDesc);
            int count = await query.CountAsync();
            int last = (int)Math.Ceiling(count / (double)size);
            var products = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            return Ok(new { products, last });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(int? id)
        {
            var product = await _context.Product
            .SelectMany(
                product => _context.Brand.Where(brand => product.BrandId == brand.Id).DefaultIfEmpty(),
                (product, brand) => new { product, brand }
            )
            .SelectMany(
                combine => _context.UserAccount.Where(userAccount => combine.product.CreateUser == userAccount.Id).DefaultIfEmpty(),
                (combined, userAccount) => new { combined.product, combined.brand, userAccount }
            )
            .Select(e => new ViewModel.Detail.Product {
                Id = e.product.Id,
                Name = e.product.Name,
                Price = e.product.Price,
                BrandName = e.brand.Name,
                UserAccountName = e.userAccount.Name,
                Image = e.product.Image
            }).FirstOrDefaultAsync(e => e.Id == id);
            return Ok(new { product });
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            var brands = await _context.Brand
            .Select(e => new ViewModel.Create.Brand {
                Id = e.Id,
                Name = e.Name
            }).ToListAsync();
            return Ok(new { brands });
        }

        [HttpPost("")]
        public async Task<IActionResult> Create([FromForm] ViewModel.Create.Product model)
        {
            if (ModelState.IsValid)
            {
                var product = new Product();
                product.Name = model.Name;
                product.Price = model.Price;
                product.BrandId = model.BrandId;
                product.Image = Util.getFile("products", model.ImageFile);
                product.CreateUser = Convert.ToInt32(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
                product.CreateDate = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(Util.getError(ModelState));
        }

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            var item = await _context.Product.FindAsync(id);
            var product = new ViewModel.Edit.Product {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                BrandId = item.BrandId,
                Image = item.Image
            };
            var brands = await _context.Brand
            .Select(e => new ViewModel.Edit.Brand {
                Id = e.Id,
                Name = e.Name
            }).ToListAsync();
            return Ok(new { product, brands });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromForm] ViewModel.Edit.Product model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = await _context.Product.FirstOrDefaultAsync(e => e.Id == id);
                    product.Name = model.Name;
                    product.Price = model.Price;
                    product.BrandId = model.BrandId;
                    product.Image = Util.getFile("products", model.ImageFile) ?? product.Image;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Product.Any(e => e.Id == id))
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
            var product = await _context.Product
            .SelectMany(
                product => _context.Brand.Where(brand => product.BrandId == brand.Id).DefaultIfEmpty(),
                (product, brand) => new { product, brand }
            )
            .SelectMany(
                combine => _context.UserAccount.Where(userAccount => combine.product.CreateUser == userAccount.Id).DefaultIfEmpty(),
                (combined, userAccount) => new { combined.product, combined.brand, userAccount }
            )
            .Select(e => new ViewModel.Delete.Product {
                Id = e.product.Id,
                Name = e.product.Name,
                Price = e.product.Price,
                BrandName = e.brand.Name,
                UserAccountName = e.userAccount.Name,
                Image = e.product.Image
            }).FirstOrDefaultAsync(e => e.Id == id);
            return Ok(new { product });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}