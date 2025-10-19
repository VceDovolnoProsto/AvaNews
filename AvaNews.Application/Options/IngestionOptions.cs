namespace AvaNews.Application.Options;

public sealed class IngestionOptions
{
    public bool Enabled { get; init; } = true;

    public int IntervalMinutes { get; init; } = 60;

    public int PageLimit { get; init; } = 100;

    public int MaxPagesPerRun { get; init; } = 10;

    public int LookbackOverlapMinutes { get; init; } = 1;

    public int InitialLookbackMinutes { get; init; } = 180;
}