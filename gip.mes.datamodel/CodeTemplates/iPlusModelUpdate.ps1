# Define variables for paths, connection strings, and model names
$databaseConnectionString = "data source=.\;Initial catalog=iPlusMESV5;Trusted_Connection=False;Encrypt=False;user id=gip;password=netspirit;"
$namespace = "gip.mes.datamodel"
$outputDir = "EFModels"
$compiledOutputDir = "EFCompiledModels"
$efBaseName = "iPlusMESV5";
# -------- 

$contextName = $efBaseName + "Context"
$entitiesName = $efBaseName + "_Entities"
$projectPath = "..\" + $namespace + ".csproj"
$contextFilePath = "..\" + $outputDir + "\" + $contextName + ".cs"
$modelName = $contextName + "Model"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$efPropsCandidates = @(
	(Join-Path $scriptDir "../../build/EntityFramework.Build.props"),
	(Join-Path $scriptDir "../../../iPlus/build/EntityFramework.Build.props")
)
$efPropsPath = $efPropsCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1
$originalEnableEFDesignTime = $null

if ($efPropsPath) {
	$propsContent = Get-Content $efPropsPath -Raw
	if ($propsContent -match '<EnableEFDesignTime>\s*([^<]+)\s*</EnableEFDesignTime>') {
		$originalEnableEFDesignTime = $matches[1].Trim()
		$propsContent = [Regex]::Replace($propsContent, '<EnableEFDesignTime>\s*[^<]+\s*</EnableEFDesignTime>', '<EnableEFDesignTime>True</EnableEFDesignTime>', 1)
		Set-Content $efPropsPath $propsContent
		Write-Host "Temporarily set EnableEFDesignTime=True in $efPropsPath"
	}
}

# -------- SCRIPT --------
try {
# Scaffold db models
Write-Host "Running the scaffold command to generate the models from the database"
Write-Host "Command: dotnet ef dbcontext scaffold $databaseConnectionString Microsoft.EntityFrameworkCore.SqlServer --output-dir $outputDir --use-database-names --context-namespace $namespace --namespace $namespace --force --project $projectPath"
dotnet ef dbcontext scaffold $databaseConnectionString Microsoft.EntityFrameworkCore.SqlServer --output-dir $outputDir --use-database-names --context-namespace $namespace --namespace $namespace --force --project $projectPath

# Modify context class
Write-Host "Commenting the connection string in the Context class"
Start-Sleep -Seconds 2
(Get-Content $contextFilePath) -replace ".UseModel(" + $modelName + ".Instance)", '//$&' | Set-Content $contextFilePath
Start-Sleep -Seconds 2
(Get-Content $contextFilePath) -replace '\.ConfigureWarnings\(warnings => warnings.Ignore\(CoreEventId.ManyServiceProvidersCreatedWarning\)\);', '.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning))' | Set-Content $contextFilePath
Start-Sleep -Seconds 2
(Get-Content $contextFilePath) -replace '//.*UseSqlServer.*', ".UseSqlServer(ConfigurationManager.ConnectionStrings[`"$entitiesName`"].ConnectionString);" | Set-Content $contextFilePath

# Optimize models
Write-Host "Running the command to generate compiled models"
Write-Host "Command: dotnet ef dbcontext optimize --output-dir $compiledOutputDir --namespace $namespace --project $projectPath --context $contextName"
dotnet ef dbcontext optimize --output-dir $compiledOutputDir --namespace $namespace --project $projectPath --context $contextName

# Revert changes in context class
Write-Host "Command finished, reverting the comments in the Context class"
Start-Sleep -Seconds 2
(Get-Content $contextFilePath) -replace "//.UseModel(" + $modelName + ".Instance)", ".UseModel(" + $modelName + ".Instance)" | Set-Content $contextFilePath
Start-Sleep -Seconds 2
(Get-Content $contextFilePath) -replace '\.ConfigureWarnings\(warnings => warnings.Ignore\(CoreEventId.ManyServiceProvidersCreatedWarning\)\)', '.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));' | Set-Content $contextFilePath
Start-Sleep -Seconds 2
(Get-Content $contextFilePath) -replace '^(.*)(\.UseSqlServer.*)' , "$1//.UseSqlServer(ConfigurationManager.ConnectionStrings[`"$entitiesName`"].ConnectionString);" | Set-Content $contextFilePath

Write-Host "The database update was completed successfully, closing in 2 seconds"
Start-Sleep -Seconds 2
}
finally {
	if ($efPropsPath -and $null -ne $originalEnableEFDesignTime) {
		$propsContent = Get-Content $efPropsPath -Raw
		$escapedOriginal = [Regex]::Escape($originalEnableEFDesignTime)
		$propsContent = [Regex]::Replace($propsContent, '<EnableEFDesignTime>\s*[^<]+\s*</EnableEFDesignTime>', "<EnableEFDesignTime>$escapedOriginal</EnableEFDesignTime>", 1)
		Set-Content $efPropsPath $propsContent
		Write-Host "Restored EnableEFDesignTime=$originalEnableEFDesignTime in $efPropsPath"
	}
}