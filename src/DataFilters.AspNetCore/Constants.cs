﻿namespace DataFilters.AspNetCore
{
    /// <summary>
    /// Constants used throughout the package
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Regex pattern that a field name should respect.
        /// </summary>
        public const string ValidFieldNamePattern = @"[a-zA-Z_]+((\[""[a-zA-Z0-9_]+""]|(\.[a-zA-Z0-9_]+))*)";
    }
}