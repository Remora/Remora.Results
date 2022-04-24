//
//  Descriptors.cs
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

using Microsoft.CodeAnalysis;

namespace Remora.Results.Analyzers;

/// <summary>
/// Defines analyzer descriptors for various analyzers.
/// </summary>
internal static class Descriptors
{
    /// <summary>
    /// Holds the descriptor for redundant FromError calls.
    /// </summary>
    internal static readonly DiagnosticDescriptor REM0001RedundantFromErrorCall = new
    (
        id: "REM0001",
        title: "Redundant FromError call",
        messageFormat: "Use \"{0}\" directly, instead of creating an identical result from its error",
        category: DiagnosticCategories.Redundancies,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );

    /// <summary>
    /// Holds the descriptor for redundant ternary expressions.
    /// </summary>
    internal static readonly DiagnosticDescriptor REM0002RedundantConditionalExpressionOnResult = new
    (
        id: "REM0002",
        title: "Redundant ternary expression on result",
        messageFormat: "Use \"{0}\" directly, instead of creating results via a ternary expression",
        category: DiagnosticCategories.Redundancies,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true
    );
}
