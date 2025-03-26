//
//  AggregateResult.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

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
    public IResultError Error => new AggregateError(_lookup[false].ToArray());

    /// <inheritdoc />
    /// <remarks>
    /// Always returns null.
    /// </remarks>
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
