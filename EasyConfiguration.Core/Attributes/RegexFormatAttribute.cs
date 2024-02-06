namespace EasyConfiguration.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class RegexFormatAttribute : Attribute
	{
		/// <summary>
		/// Does value validation according to provided <paramref name="regex"/> pattern.
		/// On failure throws exception with provided <paramref name="messageOnFailedValidation"/> text.
		/// 
		/// NOTE: You can include non-compliant value in <paramref name="messageOnFailedValidation"/> by adding "{0}" text.
		/// </summary>
		/// <param name="regex">Regex string patter.</param>
		/// <param name="messageOnFailedValidation">Failure message to be used in exception.</param>
		public RegexFormatAttribute(string regex, string messageOnFailedValidation)
		{
			MessageOnFailedValidation = messageOnFailedValidation;
			Regex = regex;
		}

		public string MessageOnFailedValidation { get; private set; }
		public string Regex { get; private set; }
	}
}