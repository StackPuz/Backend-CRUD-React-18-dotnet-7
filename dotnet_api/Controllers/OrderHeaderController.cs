using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using App.Models;
using ViewModel = App.ViewModels.OrderHeader;

namespace App.Controllers
{
    [Authorize(Roles = "ADMIN,USER")]
    [Route("orderHeaders")]
    public class OrderHeaderController : Controller
    {
        private readonly DataContext _context;

        public OrderHeaderController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            string column = Request.Query["sc"];
            if (Util.IsInvalidSearch(typeof(ViewModel.Index.OrderHeader), column)) {
                return Forbid();
            }
            int page = Request.Query["page"].Any() ? Convert.ToInt32(Request.Query["page"]) : 1;
            int size = Request.Query["size"].Any() ? Convert.ToInt32(Request.Query["size"]) : 10;
            string sort = Request.Query["sort"].Any() ? Request.Query["sort"].First() : "Id";
            bool sortDesc = Request.Query["sort"].Any() ? Request.Query["desc"].Any() : false;
            var query = _context.OrderHeader
            .SelectMany(
                orderHeader => _context.Customer.Where(customer => orderHeader.CustomerId == customer.Id).DefaultIfEmpty(),
                (orderHeader, customer) => new { orderHeader, customer }
            )
            .Select(e => new ViewModel.Index.OrderHeader {
                Id = e.orderHeader.Id,
                CustomerName = e.customer.Name,
                OrderDate = e.orderHeader.OrderDate
            });
            if (Request.Query["sw"].Any()) {
                string search = Request.Query["sw"];
                if (column == "OrderDate") {
                    search = Util.formatDateStr(search);
                }
                query = query.Where(column, Request.Query["so"], search);
            }
            query = query.OrderBy(sort, sortDesc);
            int count = await query.CountAsync();
            int last = (int)Math.Ceiling(count / (double)size);
            var orderHeaders = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            return Ok(new { orderHeaders, last });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(int? id)
        {
            var orderHeader = await _context.OrderHeader
            .SelectMany(
                orderHeader => _context.Customer.Where(customer => orderHeader.CustomerId == customer.Id).DefaultIfEmpty(),
                (orderHeader, customer) => new { orderHeader, customer }
            )
            .Select(e => new ViewModel.Detail.OrderHeader {
                Id = e.orderHeader.Id,
                CustomerName = e.customer.Name,
                OrderDate = e.orderHeader.OrderDate
            }).FirstOrDefaultAsync(e => e.Id == id);
            var orderHeaderOrderDetails = await _context.OrderHeader
            .Join(
                _context.OrderDetail,
                orderHeader => orderHeader.Id,
                orderDetail => orderDetail.OrderId,
                (orderHeader, orderDetail) => new { orderHeader, orderDetail }
            )
            .Join(
                _context.Product,
                combine => combine.orderDetail.ProductId,
                product => product.Id,
                (combined, product) => new { combined.orderHeader, combined.orderDetail, product }
            )
            .Where(e => e.orderHeader.Id == id)
            .Select(e => new ViewModel.Detail.OrderHeaderOrderDetail {
                No = e.orderDetail.No,
                ProductName = e.product.Name,
                Qty = e.orderDetail.Qty,
                OrderId = e.orderDetail.OrderId
            }).ToListAsync();
            return Ok(new { orderHeader, orderHeaderOrderDetails });
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            var customers = await _context.Customer
            .Select(e => new ViewModel.Create.Customer {
                Id = e.Id,
                Name = e.Name
            }).ToListAsync();
            return Ok(new { customers });
        }

        [HttpPost("")]
        public async Task<IActionResult> Create([FromBody] ViewModel.Create.OrderHeader model)
        {
            if (ModelState.IsValid)
            {
                var orderHeader = new OrderHeader();
                orderHeader.CustomerId = model.CustomerId;
                orderHeader.OrderDate = model.OrderDate;
                _context.Add(orderHeader);
                await _context.SaveChangesAsync();
                return Ok();
            }
            return BadRequest(Util.getError(ModelState));
        }

        [HttpGet("{id}/edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            var item = await _context.OrderHeader.FindAsync(id);
            var orderHeader = new ViewModel.Edit.OrderHeader {
                Id = item.Id,
                CustomerId = item.CustomerId,
                OrderDate = item.OrderDate
            };
            var customers = await _context.Customer
            .Select(e => new ViewModel.Edit.Customer {
                Id = e.Id,
                Name = e.Name
            }).ToListAsync();
            return Ok(new { orderHeader, customers });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, [FromBody] ViewModel.Edit.OrderHeader model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var orderHeader = await _context.OrderHeader.FirstOrDefaultAsync(e => e.Id == id);
                    orderHeader.CustomerId = model.CustomerId;
                    orderHeader.OrderDate = model.OrderDate;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.OrderHeader.Any(e => e.Id == id))
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
            var orderHeader = await _context.OrderHeader
            .SelectMany(
                orderHeader => _context.Customer.Where(customer => orderHeader.CustomerId == customer.Id).DefaultIfEmpty(),
                (orderHeader, customer) => new { orderHeader, customer }
            )
            .Select(e => new ViewModel.Delete.OrderHeader {
                Id = e.orderHeader.Id,
                CustomerName = e.customer.Name,
                OrderDate = e.orderHeader.OrderDate
            }).FirstOrDefaultAsync(e => e.Id == id);
            return Ok(new { orderHeader });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var orderHeader = await _context.OrderHeader.FindAsync(id);
            _context.OrderHeader.Remove(orderHeader);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}