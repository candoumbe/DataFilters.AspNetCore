// "Copyright (c) Cyrille NDOUMBE.
// Licenced under Apache, version 2.0"

using System;
using System.ComponentModel;
using System.Linq;

using Nuke.Common.Tooling;

namespace DataFilters.AspNetCore.ContinuousIntegration
{
    [TypeConverter(typeof(TypeConverter<Configuration>))]
    public class Configuration : Enumeration
    {
        public static Configuration Debug = new Configuration { Value = nameof(Debug) };
        public static Configuration Release = new Configuration { Value = nameof(Release) };

        public static implicit operator string(Configuration configuration)
        {
            return configuration.Value;
        }
    }
}