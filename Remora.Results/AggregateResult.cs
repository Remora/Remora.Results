using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Remora.Results;

/// <inheritdoc />
[PublicAPI]
public readonly struct AggregateResult : IResult
{
    /// <summary>
    /// Gets a value indicating whether all the contained results were successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets an <see cref="AggregateError"/> containing the failed results.
    /// </summary>
    public IResultError? Error => new AggregateError(_lookup[false].ToArray());

    /// <inheritdoc />
    public IResult? Inner { get; } = null;

    /// <summary>
    /// Gets a collection of successful results.
    /// </summary>
    public IEnumerable<IResult> SuccessfulResults => _lookup[true];

    /// <summary>
    /// Gets a collection of failed results.
    /// </summary>
    public IEnumerable<IResult> FailedResults => _lookup[false];

    /// <summary>
    /// Gets a readonly collection of the results contained in this collection.
    /// </summary>
    public IReadOnlyCollection<IResult> Results => _results;

    private readonly IResult[] _results;
    private readonly ILookup<bool, IResult> _lookup;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateResult"/> struct.
    /// </summary>
    /// <param name="results">The results to use.</param>
    public AggregateResult(params IResult[] results)
        : this(results.All(it => it.IsSuccess), results)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateResult"/> struct.
    /// </summary>
    /// <param name="isSuccess">A value indicating whether all the results are successful.</param>
    /// <param name="results">The results.</param>
    internal AggregateResult(bool isSuccess, IResult[] results)
    {
        this.IsSuccess = isSuccess;
        _results = results;
        _lookup = this.Results.ToLookup(it => it.IsSuccess);
    }
}
