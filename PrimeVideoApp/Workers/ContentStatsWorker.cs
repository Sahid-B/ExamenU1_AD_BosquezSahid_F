using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using PrimeVideoApp.Data;

namespace PrimeVideoApp.Workers;

public class ContentStatsWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMemoryCache _cache;

    public ContentStatsWorker(IServiceProvider serviceProvider, IMemoryCache cache)
    {
        _serviceProvider = serviceProvider;
        _cache = cache;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<PrimeVideoContext>();

                var genreStats = context.Genres
                    .Select(g => new { g.Name, Count = g.Contents.Count })
                    .OrderByDescending(x => x.Count)
                    .ToList();

                var yearStats = context.Contents
                    .GroupBy(c => c.ReleaseYear)
                    .Select(g => new { Year = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Year)
                    .ToList();

                var movieCount = context.Contents.Count(c => c.ContentType == "MOVIE");
                var tvShowCount = context.Contents.Count(c => c.ContentType == "TV_SHOW");

                var stats = new
                {
                    MovieCount = movieCount,
                    TvShowCount = tvShowCount,
                    GenreStats = genreStats,
                    YearStats = yearStats
                };

                _cache.Set("ContentStats", stats, TimeSpan.FromHours(1));
            }

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
