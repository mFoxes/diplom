namespace DTO;

public class EmbeddingsResultDto
{
    public Guid Id { get; set; }
    public double[] Embeddings { get; set; }
}