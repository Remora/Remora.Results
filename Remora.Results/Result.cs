//
//  Result.cs
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
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

#pragma warning disable SA1402

namespace Remora.Results;

/// <inheritdoc />
[PublicAPI]
public readonly struct Result : IResult
{
    /// <summary>
    /// Gets a successful result.
    /// </summary>
    public static Result Success { get; } = FromSuccess();

    /// <summary>
    /// Gets a successful result.
    /// </summary>
    /// <remarks>
    /// The returned instance is a boxed singleton.
    /// </remarks>
    public static IResult BoxedSuccess { get; } = FromSuccess();

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
    /// Maps the result to a value result using the provided value. Any error information will be passed through
    /// unaltered.
    /// </summary>
    /// <typeparam name="TOut">The output entity type.</typeparam>
    /// <param name="value">The value to use if the result is successful.</param>
    /// <returns>The mapped result.</returns>
    public Result<TOut> Map<TOut>(TOut value)
    {
        return this.IsSuccess
            ? value
            : Result<TOut>.FromError(this.Error, this.Inner);
    }

    /// <summary>
    /// Creates a fallback value if the result is unsuccessful or returns the user-provided default value.
    /// </summary>
    /// <param name="value">The value to use if the result is successful.</param>
    /// <param name="fallback">The function to use if the result is unsuccessful.</param>
    /// <typeparam name="TOut">The output entity type.</typeparam>
    /// <returns>The mapped result.</returns>
    public TOut MapOrElse<TOut>(TOut value, Func<IResultError, IResult?, TOut> fallback)
    {
        return this.IsSuccess
            ? value
            : fallback(this.Error, this.Inner);
    }

    /// <summary>
    /// Maps the result from one error type to another using a user-provided conversion.
    /// </summary>
    /// <typeparam name="TError">The new error type.</typeparam>
    /// <param name="conversion">The conversion function.</param>
    /// <returns>The mapped result.</returns>
    public Result MapError<TError>(Func<IResultError, IResult?, TError> conversion)
        where TError : IResultError
    {
        return this.IsSuccess
            ? this
            : new(conversion(this.Error, this.Inner), this.Inner);
    }

    /// <summary>
    /// Maps the result from one error type to another using a user-provided conversion.
    /// </summary>
    /// <typeparam name="TError">The new error type.</typeparam>
    /// <param name="conversion">The conversion function.</param>
    /// <returns>The mapped result.</returns>
    public Result MapError<TError>(Func<IResultError, IResult?, (TError Error, IResult? Inner)> conversion)
        where TError : IResultError
    {
        if (this.IsSuccess)
        {
            return this;
        }

        var (error, inner) = conversion(this.Error, this.Inner);
        return new(error, inner);
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
    public static Result FromError<TError>(TError error, IResult? inner) where TError : IResultError
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
public readonly struct Result<TEntity> : IResult<TEntity>
{
    /// <summary>
    /// Gets a successful result.
    /// </summary>
    public static Result<TEntity> Success { get; } = new(default, default, default);

    /// <summary>
    /// Gets a successful result.
    /// </summary>
    /// <remarks>
    /// The returned instance is a boxed singleton.
    /// </remarks>
    public static IResult BoxedSuccess { get; } = new Result<TEntity>(default, default, default);

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
    /// Maps the result from one entity type to another using a user-provided conversion.
    /// </summary>
    /// <param name="conversion">The conversion function.</param>
    /// <typeparam name="TOut">The output entity type.</typeparam>
    /// <returns>The mapped result.</returns>
    public Result<TOut> Map<TOut>(Func<TEntity, TOut> conversion)
    {
        return this.IsSuccess
            ? new Result<TOut>(conversion(this.Entity), default, default)
            : new Result<TOut>(default, this.Error, this.Inner);
    }

    /// <summary>
    /// Returns the provided default if the result is unsuccessful or applies a user-provided conversion if the result
    /// contains an entity.
    /// </summary>
    /// <param name="conversion">The conversion function.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <typeparam name="TOut">The output entity type.</typeparam>
    /// <returns>The mapped result.</returns>
    public TOut MapOr<TOut>(Func<TEntity, TOut> conversion, TOut defaultValue)
        => MapOrElse(conversion, (_, _) => defaultValue);

    /// <summary>
    /// Creates a fallback value if the result is unsuccessful or applies a user-provided conversion if the result
    /// contains an entity.
    /// </summary>
    /// <param name="conversion">The function to use if the result is successful.</param>
    /// <param name="fallback">The function to use if the result is unsuccessful.</param>
    /// <typeparam name="TOut">The output entity type.</typeparam>
    /// <returns>The mapped result.</returns>
    public TOut MapOrElse<TOut>(Func<TEntity, TOut> conversion, Func<IResultError, IResult?, TOut> fallback)
    {
        return this.IsSuccess
            ? conversion(this.Entity)
            : fallback(this.Error, this.Inner);
    }

    /// <summary>
    /// Maps the result from one error type to another using a user-provided conversion.
    /// </summary>
    /// <remarks>
    /// The user-provided conversion is only called if the result is unsuccessful.
    /// </remarks>
    /// <typeparam name="TError">The new error type.</typeparam>
    /// <param name="conversion">The conversion function.</param>
    /// <returns>The mapped result.</returns>
    public Result<TEntity> MapError<TError>(Func<IResultError, IResult?, TError> conversion)
        where TError : IResultError
    {
        return this.IsSuccess
            ? this
            : new Result<TEntity>(default, conversion(this.Error, this.Inner), this.Inner);
    }

    /// <summary>
    /// Maps the result from one error type to another using a user-provided conversion.
    /// </summary>
    /// <remarks>
    /// The user-provided conversion is only called if the result is unsuccessful.
    /// </remarks>
    /// <typeparam name="TError">The new error type.</typeparam>
    /// <param name="conversion">The conversion function.</param>
    /// <returns>The mapped result.</returns>
    public Result<TEntity> MapError<TError>(Func<IResultError, IResult?, (TError Error, IResult? Inner)> conversion)
        where TError : IResultError
    {
        if (this.IsSuccess)
        {
            return this;
        }

        var (error, inner) = conversion(this.Error, this.Inner);
        return new Result<TEntity>(default, error, inner);
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
    public static Result<TEntity> FromError<TError>(TError error, IResult? inner) where TError : IResultError
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
