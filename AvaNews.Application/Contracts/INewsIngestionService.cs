namespace AvaNews.Application.Contracts;

public interface INewsIngestionService
{
    Task<int> IngestOnceAsync(CancellationToken ct);
}