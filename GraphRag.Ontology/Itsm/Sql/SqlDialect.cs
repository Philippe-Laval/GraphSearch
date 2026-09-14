namespace GraphRag.Ontology.Itsm.Sql;

/// <summary>
/// Which SQL dialect a database uses. Text-to-SQL must adapt to dialect.
/// </summary>
public enum SqlDialect
{
    AnsiSql,
    TSql,
    PlSql,
    PostgreSql,
    MySql,
    Snowflake,
    BigQuery,
    Databricks,
    Sqlite,
}
