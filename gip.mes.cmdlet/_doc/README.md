Setup cmdlet for use

### (1) Setup VBPowerShellSettings.VarioDataDefault path

### (2) Copy and configure VBPowerShellSettings.json on this path

### (3) Import in powershell
Import-Module "c:\Devel\iPlusV4\trunk\iPlusMES\bin\Debug\gip.mes.cmdlet.dll" -Force

### (4) Set-HandleExecuteACMethod

#### Desc
Read a .cs file, extract and  search ACClass and then call method BSOIplusStudio for generate HandleExecuteACMethod() method

```ps
Set-HandleExecuteACMethodCmdlet -FileName "c:\Devel\kajbum.com\projects\Schaefers\schaefers.solution.project\BSO\BSOPartsListSchaefers.cs"
```


### Use alex.text.powershell

#### (1) Importing module

`Import-Module .\alex.text.powershell.dll -Force`

#### (2) Cakk texxt replace
```ps
Get-ChildItem -Path "c:\Users\aagincic\My Drive\Inbox\" -File -Filter "*BSOInOrder*.cs" | Format-TextToolReplace -JsonFile "C:\Users\aagincic\My Drive\gipSoft\Dev\regex-base\replace.json" -IsRegex $True
```