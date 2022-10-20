<Query Kind="Statements">
  <Output>DataGrids</Output>
</Query>

// 

string sourceFolder = @"c:\Aleksandar\gipSoft\Source\iPlusMES\V4\trunk\iPlusMES\gip.variobatch.sql\Programmability";
string targetFolder = @"c:\Aleksandar\gipSoft\Source\iPlusMES\V4\trunk\iPlusMES\gip.variobatch.sql\build";

string[] filterNames = new string[] 
{
"ProdOrderInwardsView.sql",
"ProdOrderOutwardsView.sql",
"ProdOrderConnectionsDetailView.sql",
"ProdOrderConnectionsView.sql"
};


string buildFile = string.Format(@"build-file-{0}.sql", DateTime.Now.ToString("yyyy-MM-dd_hh-mm"));


List<SQLScriptGroup> groups = new List<UserQuery.SQLScriptGroup>()
{
	new SQLScriptGroup()
	{
		GroupName = "udw",
		FilterNames = filterNames
	},
	new SQLScriptGroup()
	{
		GroupName = "udf",
		FilterNames = new string[]{}
	},
	new SQLScriptGroup()
	{
		GroupName = "udp",
		FilterNames = new string[]{}
	}
};

foreach (var group in groups)
{
	if(group.FilterNames == null)
	{
		group.Files =
		new DirectoryInfo(sourceFolder + "\\" + group.GroupName)
		.GetFiles("*.sql")
		.Where(c => !filterNames.Any() || filterNames.Contains(c.Name))
		.OrderBy(c => c.Name)
		.Select(c => c.FullName)
		.ToList();
	}
	else
	{
		group.Files = group.FilterNames.Select(c => Path.Combine(sourceFolder, group.GroupName, c)).ToList();
	}

}


string tempLine = "";
using (FileStream outStream = new FileStream(targetFolder + @"\" + buildFile, FileMode.OpenOrCreate))
{
	using (StreamWriter sw = new StreamWriter(outStream))
	{
		foreach (var group in groups)
		{
			sw.WriteLine("-- " + group.GroupName);
			foreach (var file in group.Files)
			{
				sw.WriteLine("");
				sw.WriteLine(string.Format(@"-- {0}", Path.GetFileName(file)));
				using (FileStream readUdw = new FileStream(file, FileMode.Open))
				{
					using (StreamReader reader = new StreamReader(readUdw))
					{
						while ((tempLine = reader.ReadLine()) != null)
						{
							sw.WriteLine(tempLine);
						}
					}
					sw.WriteLine("GO");
					sw.WriteLine("");
				}
			}
		}
	}
}

}
public class SQLScriptGroup
{
	public string GroupName { get; set; }
	public List<string> Files { get; set; }
	
	public string[] FilterNames{get;set;}