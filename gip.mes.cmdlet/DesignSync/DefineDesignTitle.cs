using System.Text.RegularExpressions;

namespace gip.mes.cmdlet.DesignSync
{
	public class DefineDesignTitle
    {
		const string enPattern = @"en\{'(.*?)'\}";
		const string dePattern = @"de\{'(.*?)'\}";

		public string ACIdentifier { get; set; }
		public string EnTitle { get; set; }
		public string DeTitle { get; set; }

		public string CaptionTuple
		{
			get
			{
				return string.Format(@"en{{'{0}'}}de{{'{1}'}}", EnTitle, DeTitle);
			}
			set
			{
				EnTitle = null;
				DeTitle = null;
				if (value != null)
				{
					Match enMatch = Regex.Match(value, enPattern);
					Match deMatch = Regex.Match(value, dePattern);
					if (enMatch.Success)
						EnTitle = enMatch.Groups[enMatch.Groups.Count - 1].Value;
					if (deMatch.Success)
						DeTitle = deMatch.Groups[enMatch.Groups.Count - 1].Value;
				}

			}
		}

		public override string ToString()
		{
			return CaptionTuple;
		}
	}
}
