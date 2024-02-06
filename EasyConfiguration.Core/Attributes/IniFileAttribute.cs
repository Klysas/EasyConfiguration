namespace EasyConfiguration.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class IniFileAttribute : Attribute
	{
		public IniFileAttribute(string? key = null, string? section = null, string? comment = null)
		{
			Key = key;
			Section = section;
			Comment = comment;
		}

		public string? Comment { get; private set; }
		public string? Key { get; private set; }
		public string? Section { get; private set; }
	}
}