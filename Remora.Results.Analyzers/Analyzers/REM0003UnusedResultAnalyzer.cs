//
//  REM0003UnusedResultAnalyzer.cs
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

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Remora.Results.Analyzers;

/// <summary>
/// Detects and flags unused result returning method calls.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class REM0003UnusedResultAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc />
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Descriptors.REM0003UnusedResult);

    /// <inheritdoc />
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis
        (
            GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics
        );
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation ||
            !context.Node.IsKind(SyntaxKind.InvocationExpression))
        {
            return;
        }

        if (context.SemanticModel.GetSymbolInfo(invocation).Symbol is not IMethodSymbol methodSymbol)
        {
            return;
        }

        // Filter out everything except Result and Result<T> return types
        if (methodSymbol?.ReturnType.Name != "Result")
        {
            return;
        }

        var parentSyntax = invocation.Parent;

        if (parentSyntax
            is AssignmentExpressionSyntax
            or EqualsValueClauseSyntax
            or ReturnStatementSyntax
            or ArrowExpressionClauseSyntax)
        {
            return;
        }

        // Bad !
        var diagnostic = Diagnostic.Create(
            descriptor: Descriptors.REM0003UnusedResult,
            location: invocation.GetLocation(),
            messageArgs: methodSymbol.Name);
        context.ReportDiagnostic(diagnostic);
    }
}
