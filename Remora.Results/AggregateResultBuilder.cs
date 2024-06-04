using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Remora.Results;

/// <summary>
/// Allows for the construction of an AggregateResult.
/// </summary>
/// <remarks>
/// This is intended for use within a foreach loop or in situations where results are added on their own
/// rather than as a group. If you have all of the results available together, use <see cref="AggregateResult.New()"/>
/// </remarks>
[PublicAPI]
public class AggregateResultBuilder
{
    private List<IResult> _results = new();

    /// <summary>
    /// Adds the result to the collection.
    /// </summary>
    /// <param name="result">The result to add.</param>
    public void Add(IResult result) => _results.Add(result);

    /// <summary>
    /// Builds the <see cref="AggregateResult"/>.
    /// </summary>
    /// <returns>A new AggregateResult.</returns>
    /// <exception cref="NotImplementedException"></exception>
    public AggregateResult Build()
    {
        throw new NotImplementedException();
    }
}
