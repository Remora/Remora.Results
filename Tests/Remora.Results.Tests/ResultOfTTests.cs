//
//  ResultOfTTests.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Affero General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Affero General Public License for more details.
//
//  You should have received a copy of the GNU Affero General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using Xunit;

#pragma warning disable CS0618

namespace Remora.Results.Tests
{
    /// <summary>
    /// Tests the <see cref="Result{T}"/> struct.
    /// </summary>
    public static class ResultOfTTests
    {
        /// <summary>
        /// Tests the <see cref="Result{T}.Entity"/> property.
        /// </summary>
        public class Entity
        {
            /// <summary>
            /// Tests whether <see cref="Result{T}.Entity"/> returns the default value for the type if the result is
            /// unsuccessful.
            /// </summary>
            [Fact]
            public void ReturnsDefaultIfResultIsUnsuccessful()
            {
                var result = Result<int>.FromError(new GenericError("Dummy"));
                Assert.Equal(default, result.Entity);
            }

            /// <summary>
            /// Tests whether <see cref="Result{T}.Entity"/> returns the default value for the type if the result is
            /// unsuccessful.
            /// </summary>
            [Fact]
            public void ReturnsCorrectObjectIfResultIsSuccessful()
            {
                var result = Result<int>.FromSuccess(0);
                Assert.Equal(0, result.Entity);
            }
        }

        /// <summary>
        /// Tests the <see cref="Result{T}.IsSuccess"/> property.
        /// </summary>
        public class IsSuccess
        {
            /// <summary>
            /// Tests whether <see cref="Result{T}.IsSuccess"/> returns true on a successful result.
            /// </summary>
            [Fact]
            public void ReturnsTrueForSuccessfulResult()
            {
                var successful = Result.FromSuccess();
                Assert.True(successful.IsSuccess);
            }

            /// <summary>
            /// Tests whether <see cref="Result{T}.IsSuccess"/> returns false on an unsuccessful result.
            /// </summary>
            [Fact]
            public void ReturnsFalseForUnsuccessfulResult()
            {
                var unsuccessful = Result<int>.FromError(new GenericError("Dummy error"));
                Assert.False(unsuccessful.IsSuccess);
            }
        }

        /// <summary>
        /// Tests the <see cref="Result{T}.Inner"/> property.
        /// </summary>
        public class Inner
        {
            /// <summary>
            /// Tests whether <see cref="Result{T}.Inner"/> returns null if no wrapped result exists.
            /// </summary>
            [Fact]
            public void ReturnsNullIfNoWrappedResultExists()
            {
                var plainResult = Result<int>.FromError(new GenericError("Dummy error"));
                Assert.Null(plainResult.Inner);
            }

            /// <summary>
            /// Tests whether <see cref="Result{T}.Inner"/> returns a valid object if a wrapped result exists.
            /// </summary>
            [Fact]
            public void ReturnsObjectIfWrappedResultExists()
            {
                var wrapped = Result<int>.FromError(new GenericError("Dummy wrapped"));
                var plainResult = Result<int>.FromError(new GenericError("Wrapping"), wrapped);

                Assert.NotNull(plainResult.Inner);
            }

            /// <summary>
            /// Tests whether <see cref="Result{T}.Inner"/> returns the correct object if a wrapped result exists.
            /// </summary>
            [Fact]
            public void ReturnsCorrectObjectIfWrappedResultExists()
            {
                var wrapped = Result<int>.FromError(new GenericError("Dummy wrapped"));
                var plainResult = Result<int>.FromError(new GenericError("Wrapping"), wrapped);

                Assert.Equal(wrapped, plainResult.Inner);
            }
        }

        /// <summary>
        /// Tests the <see cref="Result{T}.Error"/> property.
        /// </summary>
        public class Error
        {
            /// <summary>
            /// Tests whether <see cref="Result{T}.Error"/> returns null if the result is successful.
            /// </summary>
            [Fact]
            public void ReturnsNullIfResultIsSuccessful()
            {
                var successful = Result<int>.FromSuccess(0);
                Assert.Null(successful.Error);
            }

            /// <summary>
            /// Tests whether <see cref="Result{T}.Error"/> returns an object if the result is unsuccessful.
            /// </summary>
            [Fact]
            public void ReturnsObjectIfResultIsUnsuccessful()
            {
                var unsuccessful = Result<int>.FromError(new GenericError("Dummy error"));
                Assert.NotNull(unsuccessful.Error);
            }

            /// <summary>
            /// Tests whether <see cref="Result{T}.Error"/> returns the correct object if the result is unsuccessful.
            /// </summary>
            [Fact]
            public void ReturnsCorrectObjectIfResultIsUnsuccessful()
            {
                var expected = new GenericError("Dummy error");
                var unsuccessful = Result<int>.FromError(expected);

                Assert.Same(expected, unsuccessful.Error);
            }
        }

        /// <summary>
        /// Tests the <see cref="Result{T}.FromSuccess"/> method.
        /// </summary>
        public class FromSuccess
        {
            /// <summary>
            /// Tests whether <see cref="Result{T}.FromSuccess"/> creates a successful result.
            /// </summary>
            [Fact]
            public void CreatesASuccessfulResult()
            {
                var successful = Result<int>.FromSuccess(0);
                Assert.True(successful.IsSuccess);
            }
        }

        /// <summary>
        /// Tests the <see cref="Result{T}.FromError"/> method and its overloads.
        /// </summary>
        public class FromError
        {
            /// <summary>
            /// Tests whether <see cref="Result{T}.FromSuccess"/> creates an unsuccessful result from a plain error
            /// instance.
            /// </summary>
            [Fact]
            public void CreatesAnUnsuccessfulResultFromAnErrorInstance()
            {
                var result = Result<int>.FromError(new GenericError("Dummy error"));
                Assert.False(result.IsSuccess);
            }

            /// <summary>
            /// Tests whether <see cref="Result{T}.FromSuccess"/> creates an unsuccessful result from a plain error
            /// instance and a wrapped result.
            /// </summary>
            [Fact]
            public void CreatesAnUnsuccessfulResultFromAnErrorInstanceAndAWrappedResult()
            {
                var wrapped = Result<int>.FromError(new GenericError("Dummy error."));
                var result = Result<int>.FromError(new GenericError("Dummy error"), wrapped);

                Assert.False(result.IsSuccess);
                Assert.NotNull(result.Inner);
            }

            /// <summary>
            /// Tests whether <see cref="Result{T}.FromSuccess"/> creates an unsuccessful result from another result
            /// type.
            /// </summary>
            [Fact]
            public void CreatesAnUnsuccessfulResultFromAnotherResult()
            {
                var wrapped = Result.FromError(new GenericError("Dummy error."));
                var result = Result<int>.FromError(wrapped);

                Assert.False(result.IsSuccess);
                Assert.NotNull(result.Inner);
                Assert.IsType<GenericError>(result.Error);
            }

            /// <summary>
            /// Tests whether <see cref="Result{T}.FromSuccess"/> creates an unsuccessful result from another result
            /// type.
            /// </summary>
            [Fact]
            public void CreatesAnUnsuccessfulResultFromAnotherResultOfT()
            {
                var wrapped = Result<ulong>.FromError(new GenericError("Dummy error."));
                var result = Result<int>.FromError(wrapped);

                Assert.False(result.IsSuccess);
                Assert.NotNull(result.Inner);
                Assert.IsType<GenericError>(result.Error);
            }
        }

        /// <summary>
        /// Tests the <see cref="Result{T}.op_Implicit(T)"/> operator.
        /// </summary>
        public class EntityErrorOperator
        {
            /// <summary>
            /// Tests whether <see cref="Result{T}.op_Implicit(ResultError)"/> creates a successful result from an
            /// entity.
            /// </summary>
            [Fact]
            public void CreatesAnUnsuccessfulResultFromAnErrorInstance()
            {
                Result<int> result = 0;
                Assert.True(result.IsSuccess);
            }
        }

        /// <summary>
        /// Tests the <see cref="Result{T}.op_Implicit(ResultError)"/> operator.
        /// </summary>
        public class ResultErrorOperator
        {
            /// <summary>
            /// Tests whether <see cref="Result{T}.op_Implicit(ResultError)"/> creates an unsuccessful result from a plain
            /// error instance.
            /// </summary>
            [Fact]
            public void CreatesAnUnsuccessfulResultFromAnErrorInstance()
            {
                Result<int> result = new GenericError("Dummy error");
                Assert.False(result.IsSuccess);
            }
        }

        /// <summary>
        /// Tests the <see cref="Result{T}.op_Implicit(Exception)"/> operator.
        /// </summary>
        public class ExceptionOperator
        {
            /// <summary>
            /// Tests whether <see cref="Result{T}.op_Implicit(Exception)"/> creates an unsuccessful result from an
            /// exception instance.
            /// </summary>
            [Fact]
            public void CreatesAnUnsuccessfulResultFromAnExceptionInstance()
            {
                Result<int> result = new Exception("Dummy error");

                Assert.False(result.IsSuccess);
                Assert.IsType<ExceptionError>(result.Error);
            }
        }
    }
}
