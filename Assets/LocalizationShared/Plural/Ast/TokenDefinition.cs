using System;

namespace L10n.Plural.Ast
{
	/// <summary>
	/// A token definition that describes behavior of token with specified type.
	/// </summary>
    public class TokenDefinition
	{
		public delegate Token NullDenotationGetterDelegate(Token self);
		public delegate Token LeftDenotationGetterDelegate(Token self, Token left);

		/// <summary>
		/// Gets type of the token this definition associated with.
		/// </summary>
		public TokenType TokenType { get; protected set; }

		/// <summary>
		/// Gets or sets a left binding power.
		/// </summary>
		public int LeftBindingPower { get; set; }

		protected NullDenotationGetterDelegate NullDenotationGetter;
		protected LeftDenotationGetterDelegate LeftDenotationGetter;

		/// <summary>
		/// Initializes a new instance of the <see cref="TokenDefinition"/> class for given token type with given left binding power.
		/// </summary>
		/// <param name="tokenType"></param>
		/// <param name="leftBindingPower"></param>
		public TokenDefinition(TokenType tokenType, int leftBindingPower)
		{
			TokenType = tokenType;
			LeftBindingPower = leftBindingPower;
		}

		/// <summary>
		/// Sets a null denotation getter.
		/// </summary>
		/// <param name="nullDenotationGetter"></param>
		/// <returns></returns>
		public TokenDefinition SetNullDenotationGetter(NullDenotationGetterDelegate nullDenotationGetter)
		{
			NullDenotationGetter = nullDenotationGetter;
			return this;
		}

		/// <summary>
		/// Sets a left denotation getter.
		/// </summary>
		/// <param name="leftDenotationGetter"></param>
		/// <returns></returns>
		public TokenDefinition SetLeftDenotationGetter(LeftDenotationGetterDelegate leftDenotationGetter)
		{
			LeftDenotationGetter = leftDenotationGetter;
			return this;
		}

		/// <summary>
		/// Gets a null denotation token for given token using the null denotation getter.
		/// </summary>
		/// <param name="self"></param>
		/// <returns></returns>
		public Token GetNullDenotation(Token self)
		{
			if (NullDenotationGetter == null)
				throw new InvalidOperationException("Unable to invoke null denotation getter: getter is not set.");
			if (self.Type != TokenType)
				throw new ArgumentException("Unable to invoke null denotation getter: invalid self type.", "self");

			return NullDenotationGetter(self);
		}

		/// <summary>
		/// Gets a left denotation token for given token using the left denotation getter.
		/// </summary>
		/// <param name="self"></param>
		/// <param name="left"></param>
		/// <returns></returns>
		public Token GetLeftDenotation(Token self, Token left)
		{
			if (LeftDenotationGetter == null)
				throw new InvalidOperationException("Unable to invoke left denotation getter: getter is not set.");
			if (self.Type != TokenType)
				throw new ArgumentException("Unable to invoke null denotation getter: invalid self type.", "self");

			return LeftDenotationGetter(self, left);
		}
	}
}
