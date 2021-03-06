using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyShop.Backend.Data;
using MyShop.Backend.Models;
using MyShop.Backend.Services;
using MyShop.Share;


namespace MyShop.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IConfiguration _config;

        public ProductController(ApplicationDbContext context, IStorageService storageService, IConfiguration config)
        {
            _context = context;
            _storageService = storageService;
            _config = config;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductVm>>> GetProducts()
        {
            return await _context.Products
                .Select(x => new ProductVm
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    Description = x.Description,
                    Rating = x.Rating,
                    ThumbnailImageUrl = x.ImageFileName == null ? "" : Path.Combine($"{_config["Host"]}/images", x.ImageFileName),
                    CreateDate = x.CreateDate.ToString("dd'/'MM'/'yyyy HH:mm:ss"),
                    ModifyDate = x.ModifyDate == DateTime.MinValue ? "null" : x.ModifyDate.ToString("dd'/'MM'/'yyyy HH:mm:ss")
                })
                .ToListAsync();
        }

        [HttpGet("Category/{CategoryId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductVm>>> GetProductByCategory(int CategoryId)
        {
            return await _context.ProductCategory
                .Include(p => p.Product)
                .Where(p => p.CategoryId == CategoryId)
                .Select(x => new ProductVm
                {
                    Id = x.Product.Id,
                    Name = x.Product.Name,
                    Price = x.Product.Price,
                    CategoryId = CategoryId,
                    Description = x.Product.Description,
                    Rating = x.Product.Rating,
                    ThumbnailImageUrl = x.Product.ImageFileName == null ? "" : Path.Combine($"{_config["Host"]}/images", x.Product.ImageFileName),
                    CreateDate = x.Product.CreateDate.ToString("dd'/'MM'/'yyyy HH:mm:ss"),
                    ModifyDate = x.Product.ModifyDate == DateTime.MinValue ? "null" : x.Product.ModifyDate.ToString("dd'/'MM'/'yyyy HH:mm:ss")
                })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductVm>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            var productVm = new ProductVm
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                Rating = product.Rating,
                RatingCount = product.RatingCount,
                ThumbnailImageUrl = product.ImageFileName == null ? "" : Path.Combine($"{_config["Host"]}/images", product.ImageFileName),
                CreateDate = product.CreateDate.ToString("dd'/'MM'/'yyyy HH:mm:ss"),
                ModifyDate = product.ModifyDate == DateTime.MinValue ? "null" : product.ModifyDate.ToString("dd'/'MM'/'yyyy HH:mm:ss")
            };

            return productVm;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutProduct(int id, [FromForm] ProductCreateRequest productCreateRequest)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            product.Name = productCreateRequest.Name;
            product.Price = productCreateRequest.Price;
            product.Description = productCreateRequest.Description;
            product.BrandId = productCreateRequest.BrandId;
            product.ModifyDate = DateTime.Now;

            if (productCreateRequest.ThumbnailImageUrl != null)
            {
                await _storageService.DeleteFileAsync(product.ImageFileName);
                product.ImageFileName = await SaveFile(productCreateRequest.ThumbnailImageUrl);
            }
            else
            {
                product.ImageFileName = product.ImageFileName;
            }

            _context.ProductCategory.RemoveRange(
                await _context.ProductCategory.Where(i => i.ProductId.Equals(id))
                .ToListAsync()
                );

            foreach (var Id in productCreateRequest.CategoryId)
            {
                _context.ProductCategory.Add(
                    new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = Id
                    }
                );
            }

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<ProductVm>> PostProduct([FromForm] ProductCreateRequest productCreateRequest)
        {

            var product = new Product
            {
                Name = productCreateRequest.Name,
                Price = productCreateRequest.Price,
                Description = productCreateRequest.Description,
                BrandId = productCreateRequest.BrandId,
                CreateDate = DateTime.Now,
            };

            if (productCreateRequest.ThumbnailImageUrl != null)
            {
                product.ImageFileName = await SaveFile(productCreateRequest.ThumbnailImageUrl);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            foreach (var Id in productCreateRequest.CategoryId)
            {
                _context.ProductCategory.Add(
                    new ProductCategory
                    {
                        ProductId = product.Id,
                        CategoryId = Id
                    }
                );
            }
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            await _storageService.DeleteFileAsync(product.ImageFileName);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file, fileName);
            return fileName;
        }
    }
}