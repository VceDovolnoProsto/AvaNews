using AvaNews.Application.Contracts;
using Quartz;

namespace AvaNews.Jobs;

public sealed class FetchNewsJob : IJob
{
    private readonly ILogger<FetchNewsJob> _logger;
    private readonly INewsIngestionService _newsIngestionService;

    public FetchNewsJob(INewsIngestionService newsIngestionService, ILogger<FetchNewsJob> logger)
    {
        _newsIngestionService = newsIngestionService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var ct = context.CancellationToken;
        try
        {
            var added = await _newsIngestionService.IngestOnceAsync(ct);

            _logger.LogInformation("FetchNewsJob: ingested {Count} items", added);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("FetchNewsJob cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "FetchNewsJob failed");
        }
    }
}