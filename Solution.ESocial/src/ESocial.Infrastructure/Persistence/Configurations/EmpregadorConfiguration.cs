using ESocial.Domain.Entities;
using ESocial.Domain.Enums;
using ESocial.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ESocial.Infrastructure.Persistence.Configurations;

public class EmpregadorConfiguration : IEntityTypeConfiguration<Empregador>
{
    public void Configure(EntityTypeBuilder<Empregador> builder)
    {
        builder.ToTable("empregadores");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id").ValueGeneratedNever();

        builder.Property(e => e.NomeRazaoSocial)
            .HasColumnName("nome_razao_social")
            .HasMaxLength(200)
            .IsRequired();

        builder.OwnsOne(e => e.Inscricao, inscricao =>
        {
            inscricao.Property(i => i.Tipo)
                .HasColumnName("tipo_inscricao")
                .HasConversion<int>()
                .IsRequired();

            inscricao.Property(i => i.Numero)
                .HasColumnName("nr_inscricao")
                .HasMaxLength(14)
                .IsRequired();
        });

        builder.HasIndex("Inscricao_NrInscricao").IsUnique();
    }
}
