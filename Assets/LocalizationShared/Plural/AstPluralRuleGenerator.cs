﻿using System;
using System.Globalization;
using System.Text.RegularExpressions;
using L10n.Plural.Ast;

namespace L10n.Plural
{
    /// <summary>
    /// Plural rule generator that can parse a string that contains a plural rule and generate an AstPluralRule from it.
    /// </summary>
    public class AstPluralRuleGenerator : DefaultPluralRuleGenerator, IPluralRuleTextParser
    {
        private static readonly Regex NPluralsRegex = new Regex(@"(nplurals=(?<nplurals>\d+))",
            RegexOptions.IgnoreCase //| RegexOptions.Compiled
        );

        private static readonly Regex PluralRegex = new Regex(@"(plural=(?<plural>[^;\n]+))",
            RegexOptions.IgnoreCase //| RegexOptions.Compiled
        );


        /// <summary>
        /// Gets a plural rule text.
        /// </summary>
        protected string PluralRuleText { get; private set; }

        /// <summary>
        /// An instance of the <see cref="AstTokenParser"/> class that will be used to parse a plural rule string into an abstract syntax tree.
        /// </summary>
        public AstTokenParser Parser { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AstPluralRuleGenerator"/> class with no plural rule text using default AstTokenParser.
        /// </summary>
        public AstPluralRuleGenerator()
            : this(new AstTokenParser())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AstPluralRuleGenerator"/> class with no plural rule text using given AstTokenParser.
        /// </summary>
        public AstPluralRuleGenerator(AstTokenParser parser)
        {
            Parser = parser;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AstPluralRuleGenerator"/> class using default AstTokenParser and sets a plural rule text.
        /// </summary>
        /// <param name="pluralRuleText"></param>
        public AstPluralRuleGenerator(string pluralRuleText)
            : this()
        {
            SetPluralRuleText(pluralRuleText);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AstPluralRuleGenerator"/> class using given AstTokenParser and sets a plural rule text.
        /// </summary>
        /// <param name="pluralRuleText"></param>
        /// <param name="parser"></param>
        public AstPluralRuleGenerator(string pluralRuleText, AstTokenParser parser)
            : this(parser)
        {
            SetPluralRuleText(pluralRuleText);
        }

        /// <summary>
        /// Sets a plural rule text representation that needs to be parsed.
        /// </summary>
        /// <param name="pluralRuleText">Plural rule text representation.</param>
        public void SetPluralRuleText(string pluralRuleText)
        {
            PluralRuleText = pluralRuleText;
        }

        /// <summary>
        /// Creates a plural rule for given culture.
        /// </summary>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override IPluralRule CreateRule(CultureInfo cultureInfo)
        {
            if (PluralRuleText != null)
            {
                var numPlurals = ParseNumPlurals(PluralRuleText);
                var plural = ParsePluralFormulaText(PluralRuleText);
                var astRoot = Parser.Parse(plural);

                return new AstPluralRule(numPlurals, astRoot);
            }

            return base.CreateRule(cultureInfo);
        }

        /// <summary>
        /// Parses value of the 'nplurals' parameter from the plural rule text.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public int ParseNumPlurals(string input)
        {
            var match = NPluralsRegex.Match(input);
            if (!match.Success)
                throw new FormatException("Failed to parse 'nplurals' parameter from the plural rule text: invalid format");

            return int.Parse(match.Groups["nplurals"].Value);
        }

        /// <summary>
        /// Parses value of the 'plural' parameter from the plural rule text.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string ParsePluralFormulaText(string input)
        {
            var match = PluralRegex.Match(input);
            if (!match.Success)
                throw new FormatException("Failed to parse 'plural' parameter from the plural rule text: invalid format");

            return match.Groups["plural"].Value;
        }
    }
}