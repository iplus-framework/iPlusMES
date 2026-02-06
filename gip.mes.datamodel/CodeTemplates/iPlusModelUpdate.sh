#!/bin/bash

# Define variables for paths, connection strings, and model names
# Note: You might need to update the Data Source for Linux context (e.g., escape backslashes or use a different host)
databaseConnectionString='data source=gipDLVmSQL1.incus;Initial catalog=iPlusMESV5;Trusted_Connection=False;Encrypt=False;user id=gip;password=netspirit;'
namespace="gip.mes.datamodel"
outputDir="EFModels"
compiledOutputDir="EFCompiledModels"
efBaseName="iPlusMESV5"

# -------- 

contextName="${efBaseName}Context"
entitiesName="${efBaseName}_Entities"
projectPath="../${namespace}.csproj"
contextFilePath="../${outputDir}/${contextName}.cs"
modelName="${contextName}Model"

# -------- SCRIPT --------
# Scaffold db models
echo "Running the scaffold command to generate the models from the database"
echo "Command: dotnet ef dbcontext scaffold \"$databaseConnectionString\" Microsoft.EntityFrameworkCore.SqlServer --output-dir $outputDir --use-database-names --context-namespace $namespace --namespace $namespace --force --project $projectPath"

dotnet ef dbcontext scaffold "$databaseConnectionString" Microsoft.EntityFrameworkCore.SqlServer --output-dir "$outputDir" --use-database-names --context-namespace "$namespace" --namespace "$namespace" --force --project "$projectPath"

# Modify context class
echo "Commenting the connection string in the Context class"
sleep 2

# 1. Comment out UseModel
# PowerShell: (Get-Content $contextFilePath) -replace ".UseModel(" + $modelName + ".Instance)", '//$&' | Set-Content $contextFilePath
sed -i "s/\.UseModel($modelName\.Instance)/\/\/&/" "$contextFilePath"

sleep 2

# 2. Update ConfigureWarnings
# PowerShell: -replace '\.ConfigureWarnings\(warnings => warnings.Ignore\(CoreEventId.ManyServiceProvidersCreatedWarning\)\);', '.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning))'
sed -i "s|\.ConfigureWarnings(warnings => warnings\.Ignore(CoreEventId\.ManyServiceProvidersCreatedWarning));|.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning))|" "$contextFilePath"

sleep 2

# 3. Replace commented UseSqlServer with ConfigurationManager call
# PowerShell: -replace '//.*UseSqlServer.*', ".UseSqlServer(ConfigurationManager.ConnectionStrings[`"$entitiesName`"].ConnectionString);"
sed -i "s|//.*UseSqlServer.*|.UseSqlServer(ConfigurationManager.ConnectionStrings[\"$entitiesName\"].ConnectionString);|" "$contextFilePath"

# Optimize models
echo "Running the command to generate compiled models"
echo "Command: dotnet ef dbcontext optimize --output-dir $compiledOutputDir --namespace $namespace --project $projectPath --context $contextName"

dotnet ef dbcontext optimize --output-dir "$compiledOutputDir" --namespace "$namespace" --project "$projectPath" --context "$contextName"

# Revert changes in context class
echo "Command finished, reverting the comments in the Context class"
sleep 2

# 1. Revert UseModel (uncomment)
# PowerShell: -replace "//.UseModel(" + $modelName + ".Instance)", ".UseModel(" + $modelName + ".Instance)"
sed -i "s|//.UseModel($modelName\.Instance)|.UseModel($modelName.Instance)|" "$contextFilePath"

sleep 2

# 2. Revert ConfigureWarnings (add back semicolon)
# PowerShell: -replace '\.ConfigureWarnings\(warnings => warnings.Ignore\(CoreEventId.ManyServiceProvidersCreatedWarning\)\)', '.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));'
sed -i "s|\.ConfigureWarnings(warnings => warnings\.Ignore(CoreEventId\.ManyServiceProvidersCreatedWarning))|.ConfigureWarnings(warnings => warnings.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning));|" "$contextFilePath"

sleep 2

# 3. Re-comment UseSqlServer
# PowerShell: -replace '^(.*)(\.UseSqlServer.*)' , "$1//.UseSqlServer(ConfigurationManager.ConnectionStrings[`"$entitiesName`"].ConnectionString);"
sed -i "s/^\(.*\)\.UseSqlServer.*/\1\/\/.UseSqlServer(ConfigurationManager.ConnectionStrings[\"$entitiesName\"].ConnectionString);/" "$contextFilePath"

echo "The database update was completed successfully, closing in 2 seconds"
sleep 2
