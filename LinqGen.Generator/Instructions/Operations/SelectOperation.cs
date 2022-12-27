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

    public class SelectOperation : Operation
    {
        private TypeSyntax SelectorType { get; }
        private bool WithIndex { get; }
        private bool WithStruct { get; }

        public SelectOperation(in LinqGenExpression expression, int id, bool withIndex, bool withStruct)
            : base(expression, id)
        {
            var parameterType = expression.GetNamedParameterType(0);
            SelectorType = ParseTypeName(parameterType);

            // Func<TIn, TOut> or IStructFunction<TIn, TOut>
            // Func<TIn, int, TOut> or IStructFunction<TIn, int, TOut>
            var elementSymbol = parameterType.TypeArguments[withIndex ? 2 : 1];

            OutputElementSymbol = elementSymbol;
            OutputElementType = ParseTypeName(elementSymbol);

            WithIndex = withIndex;
            WithStruct = withStruct;
        }

        public override ITypeSymbol OutputElementSymbol { get; }
        public override TypeSyntax OutputElementType { get; }

        public override TypeSyntax? DummyParameterType
        {
            get
            {
                if (!WithStruct)
                    return null;

                if (WithIndex)
                    return TupleType(SeparatedList(new[] { TupleElement(OutputElementType), TupleElement(BoolType) }));

                return OutputElementType;
            }
        }

        protected override IEnumerable<TypeParameterInfo> GetTypeParameterInfos()
        {
            if (WithStruct)
            {
                yield return new TypeParameterInfo(TypeName("Selector"), SelectorType);
            }
        }

        protected override IEnumerable<MemberInfo> GetMemberInfos(bool isLocal)
        {
            yield return new MemberInfo(MemberKind.Both,
                WithStruct ? TypeName("Selector") : SelectorType, LocalName("selector"));

            if (WithIndex)
                yield return new MemberInfo(MemberKind.Enumerator, IntType, LocalName("index"));
        }

        public override IEnumerable<StatementSyntax> RenderInitialization(
            bool isLocal, ExpressionSyntax? skipVar, ExpressionSyntax? takeVar)
        {
            if (WithIndex)
            {
                ExpressionSyntax initialIndex = LiteralExpression(-1);

                if (Upstream.SupportPartition && skipVar != null)
                    initialIndex = SubtractExpression(skipVar, LiteralExpression(1));

                yield return ExpressionStatement(SimpleAssignmentExpression(Iterator("index"), initialIndex));
            }

            foreach (var statement in base.RenderInitialization(isLocal, skipVar, takeVar))
                yield return statement;
        }

        public override ExpressionSyntax? RenderCount()
        {
            return Upstream.RenderCount();
        }

        protected override ExpressionSyntax RenderCurrent()
        {
            return InvocationExpression(
                MemberAccessExpression(Member("selector"), InvokeMethod),
                ArgumentList(WithIndex
                    ? new ExpressionSyntax[] { CurrentPlaceholder, PreIncrementExpression(Iterator("index")) }
                    : new ExpressionSyntax[] { CurrentPlaceholder }));
        }
    }
}