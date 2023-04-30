namespace DTO;

public class ErrorDto
{
    public string FieldName { get; set; }
    public string Message { get; set; }
}

public class ErrorListDto
{
    public List<ErrorDto> Errors { get; set; } = new ();
}