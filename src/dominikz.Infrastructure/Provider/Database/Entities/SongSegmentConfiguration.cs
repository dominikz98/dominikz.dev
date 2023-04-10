using dominikz.Domain.Models;
using dominikz.Infrastructure.Provider.Database.Converter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace dominikz.Infrastructure.Provider.Database.Entities;

public class SongSegmentConfiguration : IEntityTypeConfiguration<SongSegment>
{
    public void Configure(EntityTypeBuilder<SongSegment> builder)
    {
        builder.ToTable("songs_segments");
        builder.HasKey(x => new { x.Index, x.SongId });
        builder.Property(x => x.SongId).HasConversion(new GuidToStringConverter());
        builder.Property(x => x.TopNotes).HasConversion<NoteCollectionConverter>();
        builder.Property(x => x.BottomNotes).HasConversion<NoteCollectionConverter>();
    }
}