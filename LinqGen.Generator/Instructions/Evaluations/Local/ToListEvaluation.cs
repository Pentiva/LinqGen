// LinqGen.Generator, Maxwell Keonwoo Kang <code.athei@gmail.com>, 2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Cathei.LinqGen.Generator
{
    using static SyntaxFactory;
    using static CodeGenUtils;

    public sealed class ToListEvaluation : LocalEvaluation
    {
        public ToListEvaluation(in LinqGenExpression expression, int id) : base(expression, id)
        {
        }

        protected override TypeSyntax ReturnType =>
            GenericName(Identifier("List"), TypeArgumentList(Upstream.OutputElementType));

        protected override IEnumerable<StatementSyntax> RenderInitialization()
        {
            ExpressionSyntax countExpression = Upstream.SupportCount
                ? CountProperty
                : LiteralExpression(0);

            yield return UsingLocalDeclarationStatement(VarName("list").Identifier, ObjectCreationExpression(
                PooledListType(Upstream.OutputElementType), ArgumentList(countExpression), null));
        }

        protected override IEnumerable<StatementSyntax> RenderAccumulation()
        {
            yield return ExpressionStatement(InvocationExpression(
                MemberAccessExpression(VarName("list"), AddMethod), ArgumentList(CurrentPlaceholder)));
        }

        protected override ExpressionSyntax RenderResult()
        {
            return InvocationExpression(VarName("list"), IdentifierName("ToList"));
        }
    }
}
