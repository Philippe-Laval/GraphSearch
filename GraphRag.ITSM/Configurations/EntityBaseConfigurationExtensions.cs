using GraphRag.ITSM.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphRag.ITSM.Configurations;

/// <summary>
/// Shared configuration conventions for any entity deriving from <see cref="EntityBase"/>:
/// row-version column and ExternalId size + index.
/// </summary>
internal static class EntityBaseConfigurationExtensions
{
    /// <summary>
    /// The audit conventions (RowVersion, ExternalId max length, ExternalId index) 
    /// are now applied via ConfigureEntityBase() in each EntityBase-derived configuration 
    /// (once per TPH root — shared table means one index).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static EntityTypeBuilder<T> ConfigureEntityBase<T>(this EntityTypeBuilder<T> builder) where T : EntityBase
    {
        builder.Property(e => e.RowVersion).IsRowVersion();

        builder.Property(e => e.ExternalId).HasMaxLength(64);

        var tableName = builder.Metadata.GetTableName() ?? typeof(T).Name;
        builder.HasIndex(e => e.ExternalId).HasDatabaseName($"IX_{tableName}_ExternalId");

        return builder;
    }
}
