using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuctionsController(AuctionDbContext context, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAuctions()
    {
        var auctions = await context.Auctions
            .Include(x => x.Item)
            .OrderBy(x => x.Item.Make).ToListAsync();
        var dto = mapper.Map<List<AuctionDto>>(auctions);
        return Ok(dto);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetAuctionById(long id)
    {
        var auction = await context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null) return NotFound();

        var dto = mapper.Map<AuctionDto>(auction);
        return Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAuction([FromBody] CreateAuctionDto createAuctionDto)
    {
        var auction = mapper.Map<Auction>(createAuctionDto);
        // TODO: set current user as seller
        auction.Seller = "Test User";
        context.Auctions.Add(auction);
        var result = await context.SaveChangesAsync() > 0;
        if (!result) return BadRequest("Something went wrong when saving the auction.");
        var dto = mapper.Map<AuctionDto>(auction);
        return CreatedAtAction(nameof(GetAuctionById), new { dto.Id }, dto);
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> UpdateAuction(long id, [FromBody] UpdateAuctionDto updateAuctionDto)
    {
        var auction = await context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (auction == null) return NotFound();
        // TODO: check seller == username
        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;
        var result = await context.SaveChangesAsync() > 0;

        if (result) return NoContent();

        return BadRequest("Something went wrong when saving the auction.");
    }

    [HttpDelete("{id:long}")] // Will be removed later
    public async Task<IActionResult> DeleteAuction(long id)
    {
        var auction = await context.Auctions.FindAsync(id);
        if (auction == null) return NotFound();
        // TODO: check seller == username
        context.Auctions.Remove(auction);
        var result = await context.SaveChangesAsync() > 0;

        if (result) return NoContent();

        return BadRequest("Something went wrong when removing the auction.");
    }
}