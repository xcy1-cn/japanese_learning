using JapaneseLearningApi.Utils;
using Microsoft.Extensions.Caching.Memory;

namespace JapaneseLearningApi.Services;

public class PublicCacheInvalidationService
{
    private readonly IMemoryCache _cache;

    public PublicCacheInvalidationService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void InvalidateArticles()
    {
        var currentVersion = _cache.GetOrCreate(PublicCacheKeys.ArticlesVersion, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);
            return 1;
        });

        _cache.Set(PublicCacheKeys.ArticlesVersion, currentVersion + 1, TimeSpan.FromHours(12));
    }

    public void InvalidateQuestions()
    {
        var currentVersion = _cache.GetOrCreate(PublicCacheKeys.QuestionsVersion, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);
            return 1;
        });

        _cache.Set(PublicCacheKeys.QuestionsVersion, currentVersion + 1, TimeSpan.FromHours(12));
    }

    public void InvalidateAll()
    {
        InvalidateArticles();
        InvalidateQuestions();
    }
}