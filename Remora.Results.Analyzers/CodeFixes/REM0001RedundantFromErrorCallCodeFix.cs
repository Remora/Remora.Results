//
//  REM0001RedundantFromErrorCallCodeFix.cs
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

using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Remora.Results.Analyzers.CodeFixes;

/// <summary>
/// Provides a code fix for instances of REM0001.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp), Shared]
public class REM0001RedundantFromErrorCallCodeFix : CodeFixProvider
{
    /// <inheritdoc />
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(Descriptors.REM0001RedundantFromErrorCall.Id);

    /// <inheritdoc />
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync();
        if (root is null)
        {
            return;
        }

        var node = root.FindNode(context.Span);

        if (node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        if (invocation.ArgumentList.Arguments[0].Expression is not MemberAccessExpressionSyntax argumentMemberAccess)
        {
            return;
        }

        if (argumentMemberAccess.Expression is not SimpleNameSyntax argumentMemberAccessName)
        {
            return;
        }

        var fixTitle = $"Replace with \"{argumentMemberAccessName.Identifier.Text}\"";
        context.RegisterCodeFix
        (
            CodeAction.Create
            (
                title: fixTitle,
                createChangedDocument: _ =>
                {
                    var newRoot = root.ReplaceNode(node, argumentMemberAccessName.WithTriviaFrom(node));
                    return Task.FromResult(context.Document.WithSyntaxRoot(newRoot));
                },
                equivalenceKey: fixTitle
            ),
            context.Diagnostics
        );
    }

    /// <inheritdoc />
    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;
}
