using Antlr4.Runtime;
using Rubberduck.Parsing.ComReflection;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.VBA.Extensions;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.Symbols
{
    public class ParameterDeclaration : Declaration
    {
        /// <summary>
        /// Creates a new built-in parameter declaration.
        /// </summary>
        public ParameterDeclaration(QualifiedMemberName qualifiedName,
            Declaration parentDeclaration,
            string asTypeName,
            VBAParser.AsTypeClauseContext asTypeContext,
            string typeHint,
            bool isOptional,
            bool isByRef,
            bool isArray = false,
            bool isParamArray = false,
            string defaultValue = "")
            : base(
                  qualifiedName,
                  parentDeclaration,
                  parentDeclaration,
                  asTypeName,
                  typeHint,
                  false,
                  false,
                  Accessibility.Implicit,
                  DeclarationType.Parameter,
                  null,
                  null,
                  Selection.Home,
                  isArray,
                  asTypeContext,
                  false)
        {
            IsOptional = isOptional;
            IsByRef = isByRef;
            IsImplicitByRef = false;
            IsParamArray = isParamArray;
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Creates a new user declaration for a parameter.
        /// </summary>
        public ParameterDeclaration(QualifiedMemberName qualifiedName,
            Declaration parentDeclaration,
            ParserRuleContext context,
            Selection selection,
            string asTypeName,
            VBAParser.AsTypeClauseContext asTypeContext,
            string typeHint,
            bool isOptional,
            bool isByRef,
            bool isArray = false,
            bool isParamArray = false,
            bool isUserDefined = true)
            : base(
                  qualifiedName,
                  parentDeclaration,
                  parentDeclaration,
                  asTypeName,
                  typeHint,
                  false,
                  false,
                  Accessibility.Implicit,
                  DeclarationType.Parameter,
                  context,
                  null,
                  selection,
                  isArray,
                  asTypeContext,
                  isUserDefined)
        {
            var argContext = context as VBAParser.ArgContext;
            IsOptional = isOptional;
            IsByRef = isByRef;
            IsImplicitByRef = isByRef && argContext?.BYREF() == null;
            IsParamArray = isParamArray;

            if (!isOptional || argContext?.argDefaultValue() == null)
            {
                return;
            }

            DefaultValue = argContext.argDefaultValue().expression()?.GetText() ?? string.Empty;
        }

        public ParameterDeclaration(ComParameter parameter, Declaration parent, QualifiedModuleName module)
            : this(
                module.QualifyMemberName(parameter.Name),
                parent,
                parameter.TypeName,
                null,
                null,
                parameter.IsOptional,
                parameter.IsByRef,
                parameter.IsArray,
                parameter.IsParamArray)
        {
            if (!string.IsNullOrEmpty(parameter.DefaultAsEnum))
            {
                DefaultValue = parameter.DefaultAsEnum;
            }
            else if (parameter.HasDefaultValue)
            {
                DefaultValue = parameter.DefaultValue is string defaultValue ? defaultValue.ToVbExpression(false) : parameter.DefaultValue.ToString();
            }
        }

        public bool IsOptional { get; }
        public bool IsByRef { get; }
        public bool IsImplicitByRef { get; }
        public bool IsParamArray { get; set; }
        public string DefaultValue { get; set; } = string.Empty;
    }
}
