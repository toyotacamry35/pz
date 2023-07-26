using System;
using System.Collections.Generic;
using System.Text;

namespace L10n.Plural.Ast
{
	/// <summary>
	/// Plural rule formula parser.
	/// Ported from the I18n component from Zend Framework (https://github.com/zendframework/zf2).
	/// </summary>
	public class AstTokenParser
    {
		protected readonly Dictionary<TokenType, TokenDefinition> TokenDefinitions = new Dictionary<TokenType, TokenDefinition>();

		protected string Input;

		protected int Position;

		protected Token CurrentToken;

		/// <summary>
		/// Initializes a new instance of the <see cref="AstTokenParser"/> class with default token definitions.
		/// </summary>
		public AstTokenParser()
		{
			// Ternary operators
			RegisterTokenDefinition(TokenType.TernaryIf, 20)
				.SetLeftDenotationGetter((self, left) => {
					self.Children[0] = left;
					self.Children[1] = ParseNextExpression();
					AdvancePosition(TokenType.TernaryElse);
					self.Children[2] = ParseNextExpression();
					return self;
				});
			RegisterTokenDefinition(TokenType.TernaryElse);

			// Boolean operators
			RegisterLeftInfixTokenDefinition(TokenType.Or, 30);
			RegisterLeftInfixTokenDefinition(TokenType.And, 40);

			// Equal operators
			RegisterLeftInfixTokenDefinition(TokenType.Equals, 50);
			RegisterLeftInfixTokenDefinition(TokenType.NotEquals, 50);

			// Compare operators
			RegisterLeftInfixTokenDefinition(TokenType.GreaterThan, 50);
			RegisterLeftInfixTokenDefinition(TokenType.LessThan, 50);
			RegisterLeftInfixTokenDefinition(TokenType.GreaterOrEquals, 50);
			RegisterLeftInfixTokenDefinition(TokenType.LessOrEquals, 50);

			// Add operators
			RegisterLeftInfixTokenDefinition(TokenType.Minus, 60);
			RegisterLeftInfixTokenDefinition(TokenType.Plus, 60);

			// Multiply operators
			RegisterLeftInfixTokenDefinition(TokenType.Multiply, 70);
			RegisterLeftInfixTokenDefinition(TokenType.Divide, 70);
			RegisterLeftInfixTokenDefinition(TokenType.Modulo, 70);

			// Not operator
			RegisterPrefixTokenDefinition(TokenType.Not, 80);

			// Literals
			RegisterTokenDefinition(TokenType.N)
				.SetNullDenotationGetter((self) => {
					return self;
				});
			RegisterTokenDefinition(TokenType.Number)
				.SetNullDenotationGetter((self) => {
					return self;
				});

			// Parentheses
			RegisterTokenDefinition(TokenType.LeftParenthesis)
				.SetNullDenotationGetter((self) => {
					var expression = ParseNextExpression();
					AdvancePosition(TokenType.RightParenthesis);
					return expression;
				});
			RegisterTokenDefinition(TokenType.RightParenthesis);

			// EOF
			RegisterTokenDefinition(TokenType.EOF);
		}

		protected TokenDefinition RegisterTokenDefinition(TokenType tokenType, int leftBindingPower = 0)
		{
			TokenDefinition definition;
			if (TokenDefinitions.TryGetValue(tokenType, out definition))
			{
				definition.LeftBindingPower = Math.Max(definition.LeftBindingPower, leftBindingPower);
			}
			else
			{
				definition = new TokenDefinition(tokenType, leftBindingPower);
				TokenDefinitions[tokenType] = definition;
			}

			return definition;
		}

		protected TokenDefinition RegisterLeftInfixTokenDefinition(TokenType tokenType, int leftBindingPower)
		{
			return RegisterTokenDefinition(tokenType, leftBindingPower)
				.SetLeftDenotationGetter((self, left) => {
					self.Children[0] = left;
					self.Children[1] = ParseNextExpression(leftBindingPower);
					return self;
				});
		}

		protected TokenDefinition RegisterRightInfixTokenDefinition(TokenType tokenType, int leftBindingPower)
		{
			return RegisterTokenDefinition(tokenType, leftBindingPower)
				.SetLeftDenotationGetter((self, left) => {
					self.Children[0] = left;
					self.Children[1] = ParseNextExpression(leftBindingPower - 1);
					return self;
				});
		}

		protected TokenDefinition RegisterPrefixTokenDefinition(TokenType tokenType, int leftBindingPower)
		{
			return RegisterTokenDefinition(tokenType, leftBindingPower)
				.SetNullDenotationGetter((self) => {
					self.Children[0] = ParseNextExpression(leftBindingPower);
					self.Children[1] = null;
					return self;
				});
		}

		protected TokenDefinition GetDefinition(TokenType tokenType)
		{
			TokenDefinition tokenDefinition;
			if (!TokenDefinitions.TryGetValue(tokenType, out tokenDefinition))
			{
				throw new ParserException(String.Format("Can not find token definition for \"\" token type.", tokenType));
			}
			return tokenDefinition;
		}

		/// <summary>
		/// Parses the input string that contains a plural rule formula and generates an abstract syntax tree.
		/// </summary>
		/// <param name="input">Input string.</param>
		/// <returns>Root node of the abstract syntax tree.</returns>
		public Token Parse(string input)
		{
			Input = input + "\0";
			Position = 0;
			CurrentToken = GetNextToken();

			return ParseNextExpression();
		}

		protected Token ParseNextExpression(int rightBindingPower = 0)
		{
			var token = CurrentToken;
			CurrentToken = GetNextToken();
			var left = GetDefinition(token.Type).GetNullDenotation(token);

			while (rightBindingPower < GetDefinition(CurrentToken.Type).LeftBindingPower)
			{
				token = CurrentToken;
				CurrentToken = GetNextToken();
				left = GetDefinition(token.Type).GetLeftDenotation(token, left);
			}

			return left;
		}

		protected void AdvancePosition()
		{
			CurrentToken = GetNextToken();
		}

		protected void AdvancePosition(TokenType expectedTokenType)
		{
			if (CurrentToken.Type != expectedTokenType)
			{
				throw new ParserException(String.Format("Expected token \"{0}\" but received \"{1}\"", expectedTokenType, CurrentToken.Type));
			}
			AdvancePosition();
		}

		protected Token GetNextToken()
		{
			while (Input[Position] == ' ' || Input[Position] == '\t') {
				Position++;
			}

			var character = Input[Position++];
			var tokenType = TokenType.None;
			var value = 0L;

			switch (character)
			{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					var sb = new StringBuilder();
					sb.Append(character);
					while (Char.IsNumber(Input[Position]))
					{
						sb.Append(Input[Position++]);
					}
					tokenType = TokenType.Number;
					value = long.Parse(sb.ToString());
					break;

				case '=':
				case '&':
				case '|':
					if (Input[Position] == character)
					{
						Position++;
						switch (character)
						{
							case '=': tokenType = TokenType.Equals; break;
							case '&': tokenType = TokenType.And; break;
							case '|': tokenType = TokenType.Or; break;
						}
					}
					else
					{
						throw new ParserException(String.Format("Found invalid character \"{0}\" after character \"{1}\" in input stream.", Input[Position], character));
					}
					break;

				case '!':
					if (Input[Position] == '=')
					{
						Position++;
						tokenType = TokenType.NotEquals;
					}
					else
					{
						tokenType = TokenType.Not;
					}
					break;

				case '<':
					if (Input[Position] == '=')
					{
						Position++;
						tokenType = TokenType.LessOrEquals;
					}
					else
					{
						tokenType = TokenType.LessThan;
					}
					break;

				case '>':
					if (Input[Position] == '=')
					{
						Position++;
						tokenType = TokenType.GreaterOrEquals;
					}
					else
					{
						tokenType = TokenType.GreaterThan;
					}
					break;

				case '*': tokenType = TokenType.Multiply; break;
				case '/': tokenType = TokenType.Divide; break;
				case '%': tokenType = TokenType.Modulo; break;
				case '+': tokenType = TokenType.Plus; break;
				case '-': tokenType = TokenType.Minus; break;
				case 'n': tokenType = TokenType.N; break;
				case '?': tokenType = TokenType.TernaryIf; break;
				case ':': tokenType = TokenType.TernaryElse; break;
				case '(': tokenType = TokenType.LeftParenthesis; break;
				case ')': tokenType = TokenType.RightParenthesis; break;

				case ';':
				case '\n':
				case '\0':
					tokenType = TokenType.EOF;
					Position--;
					break;

				default:
					throw new ParserException(String.Format("Found invalid character \"{0}\" in input stream at position {1}.", character, Position));
			}

			return new Token(tokenType, value);
		}
	}
}
