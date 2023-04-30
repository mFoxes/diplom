namespace DTO;

public class HistoryDto
{
    public string TakedBy { get; set; }
    public DateTime TakeAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
}
