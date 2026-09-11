using System;
using System.Collections.Generic;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GraphRag.Indexing.SerializerFactories;

public static class YamlSerializerFactory
{
    /// <summary>
    /// Creates a new instance of a YAML serializer with a predefined configuration.
    /// A best practice is to centralize the serializer's configuration.
    /// This ensures that all parts of the application use the same configuration and makes maintenance easier.
    /// </summary>
    /// <returns>A configured YAML serializer.</returns>
    public static ISerializer Create()
    {
        return new SerializerBuilder()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .WithIndentedSequences()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }
}