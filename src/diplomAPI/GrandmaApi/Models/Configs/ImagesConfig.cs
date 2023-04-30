namespace GrandmaApi.Models.Configs;

public class ImagesConfig
{
    public string Path { get; set; }
    public string[] AllowedExtensions { get; set; }
    public long MaxSize { get; set; }
}