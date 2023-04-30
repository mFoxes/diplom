namespace DTO;

public class BookingInfoDto
{
    public string Name { get; set; }
    public string InventoryNumber { get; set; }
    public DateTime TakeAt { get; set; }
    public DateTime ReturnAt { get; set; }
}