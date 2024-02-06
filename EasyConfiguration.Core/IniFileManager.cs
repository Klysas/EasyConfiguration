using IniParser;
using IniParser.Configuration;
using IniParser.Model;
using static IniParser.Configuration.IniFormattingConfiguration;


namespace EasyConfiguration.Core
{
	public sealed class IniFileManager
	{
		//========================================================
		//	Fields
		//========================================================

		private readonly IniDataParser _iniDataParser;
		private readonly IniDataFormatter _iniDataFormatter;

		private readonly IniFormattingConfiguration _iniFormattingConfiguration;

		private IniData _iniData = null!;

		//========================================================
		//	Constructors
		//========================================================

		public IniFileManager(string filePath)
		{
			FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
			if (!Path.IsPathFullyQualified(FilePath)) throw new ArgumentException("Ini file path is invalid. Provide full path to the file.");

			_iniDataParser = new IniDataParser();
			_iniDataParser.Scheme.CommentString = "#";

			_iniFormattingConfiguration = new IniFormattingConfiguration()
			{
				NewLineAfterProperty = true,
				NewLineAfterSection = true,
				NewLineBeforeSection = true,
				NewLineType = OperatingSystem.IsWindows() ? ENewLine.Windows : ENewLine.Unix_Mac
			};
			_iniDataFormatter = new IniDataFormatter();

			Reset();
		}

		//========================================================
		//	Properties
		//========================================================

		public string FilePath { get; init; }
		public bool IsEmpty
			=> _iniData.Sections.Count == 0 && _iniData.Global.Count == 0;

		//========================================================
		//	Methods
		//========================================================

		public object? GetValue(string key, string? section = null)
		{
			if (key is null) throw new ArgumentNullException(nameof(key));

			if (section == null)
				return _iniData.Global[key];
			else
				return _iniData[section][key];
		}

		public void Load()
		{
			if (!File.Exists(FilePath))
				throw new FileNotFoundException("No ini file.", FilePath);

			var iniString = File.ReadAllText(FilePath);

			if (string.IsNullOrWhiteSpace(iniString))
				throw new Exception("Ini file is empty.");

			_iniData = _iniDataParser.Parse(iniString);
		}

		public bool TryLoad()
		{
			try
			{
				Load();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void Reset()
		{
			_iniData = new IniData
			{
				CreateSectionsIfTheyDontExist = true,
				Scheme = new IniScheme() { CommentString = "#" }
			};
		}

		public void Save()
		{
			var output = IsEmpty ? string.Empty : _iniDataFormatter.Format(_iniData, _iniFormattingConfiguration);

			var dir = Path.GetDirectoryName(FilePath)!;
			if (!Directory.Exists(dir))
				Directory.CreateDirectory(dir);

			File.WriteAllText(FilePath, output);
		}

		public void SetValue(string key, object value, string? section = null, string? comment = null)
		{
			if (key is null) throw new ArgumentNullException(nameof(key));
			if (value is null) throw new ArgumentNullException(nameof(value));

			var property = new Property(key, value.ToString());
			if (comment != null) property.Comments.Add(comment);

			if (section == null)
				_iniData.Global.Add(property);
			else
				_iniData[section].Add(property);
		}
	}
}