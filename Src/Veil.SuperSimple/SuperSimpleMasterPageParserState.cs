using System;
using System.Collections.Generic;
using Veil.Parser;

namespace Veil.SuperSimple
{
    internal class SuperSimpleMasterPageParserState
    {
        private readonly Dictionary<string, SyntaxTreeNode> sections = new Dictionary<string, SyntaxTreeNode>();

        private readonly List<SuperSimpleToken> tokensInCurrentSection = new List<SuperSimpleToken>();

        public Type ModelType { get; private set; }

        public string MasterPageName { get; set; }

        public string CurrentSectionName { get; private set; }

        public bool IsProcessingASection { get; set; }

        public IDictionary<string, SyntaxTreeNode> Sections { get { return this.sections; } }

        public SuperSimpleMasterPageParserState(Type modelType)
        {
            this.ModelType = modelType;
        }

        internal void AddTokenToCurrentSection(SuperSimpleToken token)
        {
            this.tokensInCurrentSection.Add(token);
        }

        internal void StartSection(string name)
        {
            this.CurrentSectionName = name;
            this.IsProcessingASection = true;
        }

        internal void FinalizeCurrentSection()
        {
            if (!this.IsProcessingASection) throw new VeilParserException("Found token @EndSection without a matching @Section");

            var node = SuperSimpleTemplateParser.Parse(this.tokensInCurrentSection, this.ModelType);
            this.sections.Add(this.CurrentSectionName, node);
            this.IsProcessingASection = false;
            this.CurrentSectionName = "";
            this.tokensInCurrentSection.Clear();
        }
    }
}