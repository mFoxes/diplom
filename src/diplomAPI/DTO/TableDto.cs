namespace DTO;

public class TableDto<TItem>
{
    public int TotalItems { get; set; }
    public int TotalItemsFiltered { get; set; }
    public IEnumerable<TItem> Items { get; set; }
}