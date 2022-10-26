// LinqGen.Generator, Maxwell Keonwoo Kang <code.athei@gmail.com>, 2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cathei.LinqGen.Generator
{
    using static SyntaxFactory;
    using static CodeGenUtils;

    public sealed class SpecializeGeneration : CompilingGeneration
    {
        private TypeSyntax CallerEnumerableType { get; }
        private TypeSyntax CallerEnumeratorType { get; }

        public SpecializeGeneration(in LinqGenExpression expression, int id,
            INamedTypeSymbol enumerableSymbol) : base(expression, id)
        {
            // TODO prevent generic type element?
            ITypeSymbol? elementSymbol;

            if (enumerableSymbol.MetadataName == "IEnumerable`1")
            {
                // type is interface itself
                elementSymbol = enumerableSymbol.TypeArguments[0];
            }
            else
            {
                // find IEnumerable interface
                elementSymbol = enumerableSymbol.AllInterfaces
                    .FirstOrDefault(x => x.MetadataName == "IEnumerable`1")?
                    .TypeArguments[0];
            }

            // if element symbol is not found, use object type
            OutputElementType = elementSymbol != null ?
                ParseTypeName(elementSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)) : ObjectType;

            // find GetEnumerator with same rule as C# duck typing
            ITypeSymbol enumeratorSymbol = enumerableSymbol.GetMembers()
                .OfType<IMethodSymbol>()
                .First(x => x.DeclaredAccessibility == Accessibility.Public && x.Name == "GetEnumerator")
                .ReturnType;

            CallerEnumerableType =
                ParseTypeName(enumerableSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));

            CallerEnumeratorType =
                ParseTypeName(enumeratorSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }

        public override TypeSyntax OutputElementType { get; }

        protected override IEnumerable<MemberInfo> GetMemberInfos()
        {
            yield return new MemberInfo(
                MemberKind.Enumerable, CallerEnumerableType, SourceName);

            yield return new MemberInfo(
                MemberKind.Enumerator, CallerEnumeratorType, SourceName);
        }

        public override BlockSyntax RenderConstructorBody()
        {
            return Block(ExpressionStatement(SimpleAssignmentExpression(
                MemberAccessExpression(ThisExpression(), SourceName),
                InvocationExpression(ParentName, SourceName, GetEnumeratorName))));
        }

        public override BlockSyntax RenderMoveNextBody()
        {
            return Block(ReturnStatement(InvocationExpression(SourceName, MoveNextName)));
        }

        public override BlockSyntax RenderCurrentGetBody()
        {
            return Block(ReturnStatement(MemberAccessExpression(SourceName, CurrentName)));
        }

        public override BlockSyntax RenderDisposeBody()
        {
            return Block(ExpressionStatement(InvocationExpression(SourceName, DisposeName)));
        }
    }
}