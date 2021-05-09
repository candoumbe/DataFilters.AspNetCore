// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

namespace DataFilters.AspNetCore.ContinuousIntegration
{
    using Nuke.Common.Tooling;

    using System.ComponentModel;

    [TypeConverter(typeof(TypeConverter<Configuration>))]
    public class Configuration : Enumeration
    {
        public static readonly Configuration Debug = new() { Value = nameof(Debug) };
        public static readonly Configuration Release = new() { Value = nameof(Release) };

        public static implicit operator string(Configuration configuration)
        {
            return configuration.Value;
        }
    }
}