namespace DTO;

public class BookingsNotificationDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string MattermostName { get; set; }
    public IEnumerable<BookingInfoDto> Devices { get; set; }
}