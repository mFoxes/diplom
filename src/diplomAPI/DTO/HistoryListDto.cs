namespace DTO;

public class HistoryListDto
{
    public HistoryListDto(string name, string inventoryNumber, List<HistoryDto> history)
    {
        Name = name;
        InventoryNumber = inventoryNumber;
        History = history;
    }
    public string Name { get; set; }
    public string InventoryNumber { get; set; }
    public List<HistoryDto> History { get; set; } 
}