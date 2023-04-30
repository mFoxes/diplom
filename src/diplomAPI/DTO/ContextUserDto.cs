using Domain.Enums;

namespace DTO;

public class ContextUserDto
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MattermostName { get; set; } = string.Empty;
    public Guid? PhotoId { get; set; }
    public Roles Role { get; set; }
}