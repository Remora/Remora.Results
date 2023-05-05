//
//  IResult.cs
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

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace Remora.Results;

#pragma warning disable SA1402

/// <summary>
/// Represents the public API of a result.
/// </summary>
[PublicAPI]
public interface IResult
{
    /// <summary>
    /// Gets a value indicating whether the result was successful.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Error))]
    bool IsSuccess { get; }

    /// <summary>
    /// Gets the error, if any.
    /// </summary>
    IResultError? Error { get; }

    /// <summary>
    /// Gets the inner result, if any.
    /// </summary>
    IResult? Inner { get; }
}

/// <summary>
/// Represents the public API of a result with a contained value.
/// </summary>
/// <typeparam name="TEntity">The type of the contained value.</typeparam>
[PublicAPI]
public interface IResult<out TEntity> : IResult
{
    /// <summary>
    /// Gets the entity returned by the result.
    /// </summary>
    [AllowNull]
    TEntity Entity { get; }
}
