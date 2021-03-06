using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShop.Backend.Data;
using MyShop.Backend.Models;
using MyShop.Share;


namespace MyShop.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Bearer")]
    public class BrandController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BrandController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BrandVm>>> GetBrands()
        {
            return await _context.Brands
                .Select(x => new BrandVm { Id = x.Id, Name = x.Name })
                .ToListAsync();
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BrandVm>> GetBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            var brandVm = new BrandVm
            {
                Id = brand.Id,
                Name = brand.Name
            };

            return brandVm;
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> PutBrand(int id, BrandCreateRequest brandCreateRequest)
        {
            var brand = await _context.Brands.FindAsync(id);

            if (brand == null)
            {
                return NotFound();
            }

            brand.Name = brandCreateRequest.Name;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<BrandVm>> PostBrand(BrandCreateRequest brandCreateRequest)
        {
            if (brandCreateRequest.Name == "")
            {
                return NotFound();
            }
            
            var brand = new Brand
            {
                Name = brandCreateRequest.Name
            };

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBrand", new { id = brand.Id }, new BrandVm { Id = brand.Id, Name = brand.Name });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}