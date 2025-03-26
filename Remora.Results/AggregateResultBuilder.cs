//
//  AggregateResultBuilder.cs
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
using JetBrains.Annotations;

namespace Remora.Results;

/// <summary>
/// Allows for the construction of an AggregateResult.
/// </summary>
/// <remarks>
/// This is intended for use within a foreach loop or in situations where results are added on their own
/// rather than as a group. If you have all the results available together, use <see cref="AggregateResult"/>.
/// </remarks>
[PublicAPI]
public class AggregateResultBuilder
{
    private List<IResult> _results = new();
    private bool _allSuccessful = true;

    /// <summary>
    /// Adds the result to the collection.
    /// </summary>
    /// <param name="result">The result to add.</param>
    public void Add(IResult result)
    {
        if (!result.IsSuccess)
        {
            _allSuccessful = false;
        }

        _results.Add(result);
    }

    /// <summary>
    /// Builds the <see cref="AggregateResult"/>.
    /// </summary>
    /// <returns>A new AggregateResult.</returns>
    public AggregateResult Build()
    {
        return new AggregateResult(_allSuccessful, _results.ToArray());
    }
}
