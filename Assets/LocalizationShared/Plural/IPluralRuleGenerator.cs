﻿using System.Globalization;

namespace L10n.Plural
{
	/// <summary>
	/// Plural rule generator.
	/// </summary>
	public interface IPluralRuleGenerator
	{
		/// <summary>
		/// Creates a plural rule for given culture.
		/// </summary>
		/// <param name="cultureInfo"></param>
		/// <returns></returns>
		IPluralRule CreateRule(CultureInfo cultureInfo);
	}
}