namespace AuctionService.Entities;

public class Item
{
    public long Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
    public int Mileage { get; set; }
    public string ImageUrl { get; set; }
    public long AuctionId { get; set; }
    public Auction Auction { get; set; }
}