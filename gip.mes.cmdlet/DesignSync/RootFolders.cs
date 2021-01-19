using System.IO;

namespace gip.mes.cmdlet.DesignSync
{
	public class RootFolders
    {
		private  string solutionRoot = @"c:\Aleksandar\gipSoft\Source\iPlusMES\V4\XAMLEditing\";
		public  string SolutionRoot
		{
			get
			{
				return solutionRoot;
			}
		}

		public  string VariobatchRoot
		{
			get
			{
				return Path.Combine(SolutionRoot, @"XAMLDesigns\Items\iPlusMES\Businessobjects");
			}
		}

		// Define root folder
		public  string ElgRoot
		{
			get
			{
				return Path.Combine(SolutionRoot, @"XAMLDesigns\1. Projects\ELGV3\BSO");
			}
		}
		public  string AldiRoot
		{
			get
			{
				return Path.Combine(SolutionRoot, @"XAMLDesigns\1. Projects\Aldi");
			}
		}
		public  string AldiMRoot
		{
			get
			{
				return @"C:\Aleksandar\gipSoft\Source\iPlusMES\V4\XAMLEditing\XAMLDesigns\1. Projects\AldiM\Items\Businessobjects";
			}
		}
		public  string VarioLibararyFolder
		{
			get
			{
				return Path.Combine(SolutionRoot, @"XAMLDesigns\Items\Variolibrary");
			}
		}
	}
}
