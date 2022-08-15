//
//  AggregateError.cs
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

using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Remora.Results;

/// <summary>
/// Represents a set of errors produced by an operation.
/// </summary>
/// <param name="Errors">The errors.</param>
/// <param name="Message">The custom error message, if any.</param>
/// <remarks>Used in place of <see cref="AggregateException"/>.</remarks>
[PublicAPI]
public record AggregateError
(
    IReadOnlyCollection<IResult> Errors,
    string Message = "One or more errors occurred."
) : ResultError(BuildMessage(Message, Errors))
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateError"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errors">The errors.</param>
    public AggregateError(string message, params IResult[] errors)
        : this(errors, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateError"/> class.
    /// </summary>
    /// <param name="errors">The errors.</param>
    public AggregateError(params IResult[] errors)
        : this((IReadOnlyCollection<IResult>)errors)
    {
    }

    private static string BuildMessage(string message, IReadOnlyCollection<IResult> errors)
    {
        var sb = new StringBuilder(message);
        sb.AppendLine();

        var index = 0;
        foreach (var error in errors)
        {
            if (error.IsSuccess)
            {
                continue;
            }

            sb.Append($"[{index}]: ");
            var errorLines = (error.Error.ToString() ?? "Unknown").Split('\n');
            foreach (var errorLine in errorLines)
            {
                sb.Append("\t");
                sb.AppendLine(errorLine);
            }

            ++index;
        }

        return sb.ToString();
    }
}
