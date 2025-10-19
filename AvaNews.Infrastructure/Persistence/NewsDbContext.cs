using System.Text.Json;
using AvaNews.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AvaNews.Infrastructure.Persistence;

public sealed class NewsDbContext : DbContext
{
    public NewsDbContext(DbContextOptions<NewsDbContext> options) : base(options)
    {
    }

    public DbSet<NewsItem> News => Set<NewsItem>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new NewsItemConfiguration());
        modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());
    }
}

file sealed class NewsItemConfiguration : IEntityTypeConfiguration<NewsItem>
{
    public void Configure(EntityTypeBuilder<NewsItem> b)
    {
        b.ToTable("news");
        b.HasKey(x => x.Id);
        b.Property(x => x.ProviderId).HasColumnName("provider_id").IsRequired();
        b.HasIndex(x => x.ProviderId).IsUnique();

        b.Property(x => x.Title).HasColumnName("title").IsRequired();
        b.Property(x => x.Summary).HasColumnName("summary");
        b.Property(x => x.Url).HasColumnName("url").IsRequired();
        b.Property(x => x.PublishedUtc).HasColumnName("published_utc").IsRequired();

        b.Property(x => x.ImageUrl).HasColumnName("image_url");

        b.OwnsOne(x => x.Publisher, navigationBuilder =>
        {
            navigationBuilder.Property(p => p.Name).HasColumnName("publisher_name").HasMaxLength(256);
            navigationBuilder.Property(p => p.LogoUrl).HasColumnName("publisher_logo_url");
            navigationBuilder.Property(p => p.HomepageUrl).HasColumnName("publisher_homepage_url");
        });

        b.OwnsOne(x => x.Enrichment, navigationBuilder =>
        {
            navigationBuilder.Property(p => p.PriceNow).HasColumnName("enr_price_now").HasColumnType("numeric(18,4)");
            navigationBuilder.Property(p => p.PrevClose).HasColumnName("enr_prev_close").HasColumnType("numeric(18,4)");
            navigationBuilder.Property(p => p.PriceChangePct).HasColumnName("enr_change_pct");
            navigationBuilder.Property(p => p.Sentiment).HasColumnName("enr_sentiment");
        });

        b.Property(x => x.Keywords)
            .HasColumnType("text[]")
            .HasColumnName("keywords");

        b.Property(x => x.Tickers)
            .HasColumnType("text[]")
            .HasColumnName("tickers");

        b.Property<DateTime>("CreatedAt").HasColumnName("created_at");
        b.Property<DateTime?>("UpdatedAt").HasColumnName("updated_at");

        b.HasIndex(x => x.PublishedUtc).HasDatabaseName("ix_news_published_desc");
        b.HasIndex(x => x.Tickers).HasMethod("gin");
    }
}

file sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> b)
    {
        b.ToTable("subscriptions");
        b.HasKey(x => x.Id);
        b.Property(x => x.UserId).HasColumnName("user_id").IsRequired();
        b.Property(x => x.Channel).HasColumnName("channel").IsRequired();
        b.Property(x => x.Target).HasColumnName("target");
        b.Property(x => x.QueryText).HasColumnName("query_text");
        b.Property(x => x.IsActive).HasColumnName("is_active").HasDefaultValue(true);

        b.Property(x => x.Tickers)
            .HasConversion(
                v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => v == null
                    ? null
                    : JsonSerializer.Deserialize<IReadOnlyList<string>>(v, (JsonSerializerOptions?)null)!
            )
            .HasColumnType("jsonb")
            .HasColumnName("tickers");

        b.Property<DateTime>("CreatedAt").HasColumnName("created_at");
        b.Property<DateTime?>("UpdatedAt").HasColumnName("updated_at");

        b.HasIndex(x => x.UserId);
    }
}