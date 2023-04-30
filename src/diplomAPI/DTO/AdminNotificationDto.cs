namespace DTO;

public class AdminNotificationDto
{
    public string Name { get; set; }
    public IEnumerable<BookingInfoDto> Devices { get; set; }
}