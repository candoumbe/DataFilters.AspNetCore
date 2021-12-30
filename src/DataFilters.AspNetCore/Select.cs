namespace DataFilters.AspNetCore
{
#if STRING_SEGMENT
using Microsoft.Extensions.Primitives;
#endif

    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Allows to define a <see cref="Select"/> expression.
    /// </summary>
    public sealed class Select : IEquatable<Select>, IEqualityComparer<Select>
    {
        /// <summary>
        /// The string representation of properties to be selected.
        /// </summary>
        public string Expression { get; }

        /// <summary>
        /// Builds a new <see cref="Select"/> instance based on the provided <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">The string representation of properties to be selected.</param>
        /// <exception cref="ArgumentOutOfRangeException">if <paramref name="expression"/> is <c>null</c> or does not match <see cref="Constants.ValidFieldNamePattern"/> pattern</exception>
        public Select(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentOutOfRangeException(nameof(expression), expression, $"{nameof(expression)} cannot be null or whitespace only.");
            }

            if (!Regex.IsMatch(expression, $"^{Constants.ValidFieldNamePattern}$", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1)))
            {
                throw new ArgumentOutOfRangeException(nameof(expression), expression, $"{nameof(expression)} should match {Constants.ValidFieldNamePattern} pattern");
            }

            Expression = expression;
        }

        /// <inheritdoc/>
        public bool Equals(Select other) => other is not null && Expression == other.Expression;

        /// <inheritdoc/>
        public override bool Equals(object obj) => Equals(obj as Select);

        /// <inheritdoc/>
        public override int GetHashCode() => Expression.GetHashCode();

        /// <inheritdoc/>
        public bool IsEquivalentTo(Select other) => Equals(other);

        /// <inheritdoc/>
        public override string ToString() => this.Jsonify();

        /// <inheritdoc/>
        public bool Equals(Select x, Select y) => x.Equals(y);

        /// <inheritdoc/>
        public int GetHashCode([DisallowNull] Select obj) => obj.GetHashCode();
    }
}