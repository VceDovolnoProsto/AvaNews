using AvaNews.Application.Models;

namespace AvaNews.Application.Contracts;

public interface INewsService
{
    Task<List<NewsItemDto>> GetAllNewsAsync(CancellationToken ct);

    Task<List<NewsItemDto>> GetAllNewsWithGivingDayAsync(int days, CancellationToken ct);

    Task<List<NewsItemDto>> GetAllNewsPerInstrumentAsync(string ticker, int limit, CancellationToken ct);

    Task<List<NewsItemDto>> SearchAsync(string query, CancellationToken ct);

    Task<List<NewsItemDto>> GetLatestNewsAsync(int limit, CancellationToken ct);
}