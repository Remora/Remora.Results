//
//  Result.cs
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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

#pragma warning disable SA1402

namespace Remora.Results;

/// <inheritdoc />
[PublicAPI]
public readonly struct Result : IResult
{
    /// <inheritdoc />
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => this.Error is null;

    /// <inheritdoc />
    public IResult? Inner { get; }

    /// <inheritdoc />
    public IResultError? Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> struct.
    /// </summary>
    /// <param name="error">The error, if any.</param>
    /// <param name="inner">The inner result, if any.</param>
    private Result(IResultError? error, IResult? inner)
    {
        this.Error = error ?? inner?.Error;
        this.Inner = inner;
    }

    /// <summary>
    /// Creates a new successful result.
    /// </summary>
    /// <returns>The successful result.</returns>
    public static Result FromSuccess()
        => new(default, default);

    /// <summary>
    /// Creates a new failed result.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>The failed result.</returns>
    public static Result FromError<TError>(TError error) where TError : IResultError
        => new(error, default);

    /// <summary>
    /// Creates a new failed result.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="error">The error.</param>
    /// <param name="inner">The inner error that caused this error, if any.</param>
    /// <returns>The failed result.</returns>
    public static Result FromError<TError>(TError error, IResult inner) where TError : IResultError
        => new(error, inner);

    /// <summary>
    /// Creates a new failed result from another result.
    /// </summary>
    /// <typeparam name="TEntity">The entity type of the base result.</typeparam>
    /// <param name="result">The error.</param>
    /// <returns>The failed result.</returns>
    public static Result FromError<TEntity>(Result<TEntity> result)
        => new(result.Error, result);

    /// <summary>
    /// Converts an error into a failed result.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>The failed result.</returns>
    public static implicit operator Result(ResultError error)
        => new(error, default);

    /// <summary>
    /// Converts an exception into a failed result.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>The failed result.</returns>
    public static implicit operator Result(Exception exception)
        => new(new ExceptionError(exception), default);
}

/// <inheritdoc />
[PublicAPI]
public readonly struct Result<TEntity> : IResult
{
    /// <summary>
    /// Gets the entity returned by the result.
    /// </summary>
    [AllowNull]
    public TEntity Entity { get; }

    /// <inheritdoc />
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => this.Error is null;

    /// <inheritdoc />
    public IResult? Inner { get; }

    /// <inheritdoc />
    public IResultError? Error { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TEntity}"/> struct.
    /// </summary>
    /// <param name="entity">The entity, if any.</param>
    /// <param name="error">The error, if any.</param>
    /// <param name="inner">The inner result, if any.</param>
    private Result(TEntity? entity, IResultError? error, IResult? inner)
    {
        this.Error = error ?? inner?.Error;
        this.Inner = inner;
        this.Entity = entity;
    }

    /// <summary>
    /// Determines whether the result contains a defined value; that is, it has a value, and the value is not null.
    /// </summary>
    /// <returns>true if the result contains a defined value; otherwise, false.</returns>
    [MemberNotNullWhen(true, nameof(Entity))]
    public bool IsDefined() => this.IsSuccess && this.Entity is not null;

    /// <summary>
    /// Determines whether the result contains a defined value; that is, it has a value, and the value is not null.
    /// </summary>
    /// <param name="entity">The entity, if it is defined.</param>
    /// <returns>true if the result contains a defined value; otherwise, false.</returns>
    [MemberNotNullWhen(true, nameof(Entity))]
    public bool IsDefined([NotNullWhen(true)] out TEntity? entity)
    {
        entity = default;

        if (!this.IsSuccess)
        {
            return false;
        }

        if (this.Entity is null)
        {
            return false;
        }

        entity = this.Entity;
        return true;
    }

    /// <summary>
    /// Creates a new successful result.
    /// </summary>
    /// <param name="entity">The returned entity.</param>
    /// <returns>The successful result.</returns>
    public static Result<TEntity> FromSuccess(TEntity entity) => new(entity, default, default);

    /// <summary>
    /// Creates a new failed result.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="error">The error.</param>
    /// <returns>The failed result.</returns>
    public static Result<TEntity> FromError<TError>(TError error) where TError : IResultError
        => new(default, error, default);

    /// <summary>
    /// Creates a new failed result.
    /// </summary>
    /// <typeparam name="TError">The error type.</typeparam>
    /// <param name="error">The error.</param>
    /// <param name="inner">The inner error that caused this error, if any.</param>
    /// <returns>The failed result.</returns>
    public static Result<TEntity> FromError<TError>(TError error, IResult inner) where TError : IResultError
        => new(default, error, inner);

    /// <summary>
    /// Creates a new failed result from another result.
    /// </summary>
    /// <typeparam name="TOtherEntity">The entity type of the base result.</typeparam>
    /// <param name="result">The error.</param>
    /// <returns>The failed result.</returns>
    public static Result<TEntity> FromError<TOtherEntity>(Result<TOtherEntity> result)
        => new(default, result.Error, result);

    /// <summary>
    /// Creates a new failed result from another result.
    /// </summary>
    /// <param name="result">The error.</param>
    /// <returns>The failed result.</returns>
    public static Result<TEntity> FromError(Result result)
        => new(default, result.Error, result);

    /// <summary>
    /// Explicitly converts a value result into a plain result.
    /// </summary>
    /// <remarks>This operator discards any contained entity.</remarks>
    /// <param name="result">The value result.</param>
    /// <returns>The plain result.</returns>
    public static explicit operator Result(Result<TEntity> result)
    {
        return result.IsSuccess ? Result.FromSuccess() : Result.FromError(result.Error);
    }

    /// <summary>
    /// Converts an entity into a successful result.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The successful result.</returns>
    public static implicit operator Result<TEntity>(TEntity? entity)
    {
        return new(entity, default, default);
    }

    /// <summary>
    /// Converts an error into a failed result.
    /// </summary>
    /// <param name="error">The error.</param>
    /// <returns>The failed result.</returns>
    public static implicit operator Result<TEntity>(ResultError error)
    {
        return new(default, error, default);
    }

    /// <summary>
    /// Converts an exception into a failed result.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <returns>The failed result.</returns>
    public static implicit operator Result<TEntity>(Exception exception)
    {
        return new(default, new ExceptionError(exception), default);
    }
}
