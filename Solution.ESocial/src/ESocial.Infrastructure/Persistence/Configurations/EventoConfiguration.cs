using ESocial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESocial.Infrastructure.Persistence.Configurations;

public class EventoConfiguration : IEntityTypeConfiguration<Evento>
{
    public void Configure(EntityTypeBuilder<Evento> builder)
    {
        builder.ToTable("eventos");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(e => e.TipoEvento)
            .HasColumnName("tipo_evento")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.XmlContent)
            .HasColumnName("xml_content")
            .HasColumnType("longtext")
            .IsRequired();

        builder.Property(e => e.CriadoEm).HasColumnName("criado_em").IsRequired();

        builder.OwnsOne(e => e.Retorno, retorno =>
        {
            retorno.Property(r => r.CdResposta).HasColumnName("cd_resposta").HasMaxLength(10);
            retorno.Property(r => r.DescResposta).HasColumnName("desc_resposta").HasMaxLength(500);
        });
    }
}
