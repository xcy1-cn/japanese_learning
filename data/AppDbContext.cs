using JapaneseLearningApi.Models;
using Microsoft.EntityFrameworkCore;

namespace JapaneseLearningApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();

    public DbSet<Article> Articles => Set<Article>();

    public DbSet<Sentence> Sentences => Set<Sentence>();

    public DbSet<Vocabulary> Vocabularies => Set<Vocabulary>();

    public DbSet<GrammarPoint> GrammarPoints => Set<GrammarPoint>();

    public DbSet<Question> Questions => Set<Question>();

    public DbSet<SentenceVocabulary> SentenceVocabularies => Set<SentenceVocabulary>();

    public DbSet<SentenceGrammarPoint> SentenceGrammarPoints => Set<SentenceGrammarPoint>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SentenceVocabulary>()
            .HasKey(sv => new { sv.SentenceId, sv.VocabularyId });

        modelBuilder.Entity<SentenceVocabulary>()
            .HasOne(sv => sv.Sentence)
            .WithMany(s => s.SentenceVocabularies)
            .HasForeignKey(sv => sv.SentenceId);

        modelBuilder.Entity<SentenceVocabulary>()
            .HasOne(sv => sv.Vocabulary)
            .WithMany(v => v.SentenceVocabularies)
            .HasForeignKey(sv => sv.VocabularyId);

        modelBuilder.Entity<SentenceGrammarPoint>()
            .HasKey(sg => new { sg.SentenceId, sg.GrammarPointId });

        modelBuilder.Entity<SentenceGrammarPoint>()
            .HasOne(sg => sg.Sentence)
            .WithMany(s => s.SentenceGrammarPoints)
            .HasForeignKey(sg => sg.SentenceId);

        modelBuilder.Entity<SentenceGrammarPoint>()
            .HasOne(sg => sg.GrammarPoint)
            .WithMany(g => g.SentenceGrammarPoints)
            .HasForeignKey(sg => sg.GrammarPointId);

        modelBuilder.Entity<Article>()
            .HasMany(a => a.Sentences)
            .WithOne(s => s.Article)
            .HasForeignKey(s => s.ArticleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Article)
            .WithMany()
            .HasForeignKey(q => q.ArticleId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Sentence)
            .WithMany(s => s.Questions)
            .HasForeignKey(q => q.SentenceId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}