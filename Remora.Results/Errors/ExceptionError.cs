//
//  ExceptionError.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
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
using JetBrains.Annotations;

namespace Remora.Results;

/// <summary>
/// Represents an error caused by an exception.
/// </summary>
[PublicAPI]
public sealed record ExceptionError : ResultError
{
    /// <summary>
    /// Gets the exception that caused the error.
    /// </summary>
    public Exception Exception { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionError"/> class.
    /// </summary>
    /// <param name="exception">The exception that caused the error.</param>
    public ExceptionError(Exception exception)
        : base(exception.Message)
    {
        this.Exception = exception;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionError"/> class.
    /// </summary>
    /// <param name="exception">The exception that caused the error.</param>
    /// <param name="message">The custom human-readable error message.</param>
    public ExceptionError(Exception exception, string message)
        : base(message)
    {
        this.Exception = exception;
    }
}
