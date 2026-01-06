using Microsoft.AspNetCore.Mvc;
using VirtualBasket.Api.Models;
using VirtualBasket.Api.Services;
using VirtualBasket.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace VirtualBasket.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PricingController : ControllerBase
{
    private readonly PricingEngine _pricingEngine;
    private readonly ApplicationDbContext _context;

    public PricingController(PricingEngine pricingEngine, ApplicationDbContext context)
    {
        _pricingEngine = pricingEngine;
        _context = context;
    }

    [HttpPost("calculate")]
    public async Task<ActionResult<PricingResponse>> Calculate([FromBody] PricingRequest request)
    {
        var result = await _pricingEngine.CalculatePricing(request);
        return Ok(result);
    }

    [HttpGet("products")]
    public async Task<ActionResult<List<Product>>> GetProducts()
    {
        var products = await _context.Products.ToListAsync();
        return Ok(products);
    }

    [HttpGet("discounts")]
    public async Task<ActionResult<List<Discount>>> GetDiscounts()
    {
        var discounts = await _context.DiscountRules
            .Include(d => d.DiscountProducts)
            .ToListAsync();
        return Ok(discounts);
    }
}
