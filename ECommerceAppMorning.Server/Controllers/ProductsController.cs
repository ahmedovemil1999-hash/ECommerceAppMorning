using ECommerceAppMorning.Server.Data;
using ECommerceAppMorning.Server.DTOs.Product;
using ECommerceAppMorning.Server.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceAppMorning.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
    [FromQuery] ProductQueryDto query)
        {
            var productsQuery = _context.Products
                .AsNoTracking()
                .Include(x => x.Category)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                productsQuery = productsQuery.Where(x =>
                    x.Name.Contains(query.Search) ||
                    x.Description.Contains(query.Search));
            }

            if (query.CategoryId.HasValue)
            {
                productsQuery = productsQuery.Where(x =>
                    x.CategoryId == query.CategoryId.Value);
            }

            if (query.MinPrice.HasValue)
            {
                productsQuery = productsQuery.Where(x =>
                    x.Price >= query.MinPrice.Value);
            }

            if (query.MaxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(x =>
                    x.Price <= query.MaxPrice.Value);
            }

            productsQuery = query.Sort.ToLower() switch
            {
                "priceasc" =>
                    productsQuery.OrderBy(x => x.Price),

                "pricedesc" =>
                    productsQuery.OrderByDescending(x => x.Price),

                "nameasc" =>
                    productsQuery.OrderBy(x => x.Name),

                "namedesc" =>
                    productsQuery.OrderByDescending(x => x.Name),

                _ =>
                    productsQuery.OrderByDescending(x => x.CreatedAt)
            };

            var totalCount = await productsQuery.CountAsync();

            var pageSize = Math.Clamp(query.PageSize, 1, 50);

            var page = Math.Max(query.Page, 1);

            var products = await productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProductListDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    Stock = x.Stock,
                    ImageUrl = x.ImageUrl,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name
                })
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(
                totalCount / (double)pageSize);

            var result = new PagedResultDto<ProductListDto>
            {
                Items = products,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(x => x.Id == dto.CategoryId);

            if (!categoryExists)
                return BadRequest("Category does not exist.");

            var product = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId
            };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = product.Id },
                product
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateProductDto dto)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null)
                return NotFound();

            var categoryExists = await _context.Categories
                .AnyAsync(x => x.Id == dto.CategoryId);

            if (!categoryExists)
                return BadRequest("Category does not exist.");

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.ImageUrl = dto.ImageUrl;
            product.CategoryId = dto.CategoryId;

            await _context.SaveChangesAsync();

            return Ok(product);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product is null)
                return NotFound();

            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}