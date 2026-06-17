namespace JapaneseLearningApi.Utils;

public static class PublicCacheKeys
{
    public const string ArticlesVersion = "public:articles:version";
    public const string QuestionsVersion = "public:questions:version";

    public static string ArticleList(
        string? keyword,
        string? level,
        string? category,
        int page,
        int pageSize,
        int version
    )
    {
        return $"public:articles:v={version}:keyword={keyword ?? ""}:level={level ?? ""}:category={category ?? ""}:page={page}:pageSize={pageSize}";
    }

    public static string ArticleDetail(int id, int version)
    {
        return $"public:articles:v={version}:detail:{id}";
    }

    public static string ArticleSentences(int articleId, int version)
    {
        return $"public:articles:v={version}:{articleId}:sentences";
    }

    public static string QuestionList(
        string? type,
        string? keyword,
        int page,
        int pageSize,
        int version
    )
    {
        return $"public:questions:v={version}:type={type ?? ""}:keyword={keyword ?? ""}:page={page}:pageSize={pageSize}";
    }

    public static string QuestionDetail(int id, int version)
    {
        return $"public:questions:v={version}:detail:{id}";
    }
}