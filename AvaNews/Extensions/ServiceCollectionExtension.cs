using AvaNews.Application.Contracts;
using AvaNews.Application.Options;
using AvaNews.Application.Services;
using AvaNews.Jobs;
using Microsoft.Extensions.Options;
using Quartz;

namespace AvaNews.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddQuartzJobs(this IServiceCollection services, IConfiguration cfg)
    {
        services.Configure<IngestionOptions>(cfg.GetSection("Ingestion"));

        services.AddScoped<INewsIngestionService, NewsIngestionService>();

        var sp = services.BuildServiceProvider();
        var ingestionEnabled = sp.GetRequiredService<IOptions<IngestionOptions>>().Value.Enabled;
        var intervalMinutes = Math.Max(1, sp.GetRequiredService<IOptions<IngestionOptions>>().Value.IntervalMinutes);

        if (!ingestionEnabled) return services;

        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            var jobKey = new JobKey("FetchNewsJob");
            q.AddJob<FetchNewsJob>(opts => opts.WithIdentity(jobKey));
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("FetchNewsJob-trigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromMinutes(intervalMinutes))
                    .RepeatForever()));
        });

        services.AddQuartzHostedService(o => o.WaitForJobsToComplete = true);
        return services;
    }
}