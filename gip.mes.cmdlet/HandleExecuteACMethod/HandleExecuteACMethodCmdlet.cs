using gip.core.datamodel;
using gip.mes.cmdlet.Settings;
using System;
using System.Linq;
using System.IO;
using System.Management.Automation;
using System.Text.RegularExpressions;
using gip.bso.iplus;

namespace gip.mes.cmdlet.HandleExecuteACMethod
{
    [Cmdlet(VerbsCommon.Set, nameof(HandleExecuteACMethodCmdlet))]
    public class HandleExecuteACMethodCmdlet : Cmdlet
    {
        #region const
        public const string RegexPatternClass = @"public class (\w*)";
        public const string RegexPatternMehtod = @"protected override bool HandleExecuteACMethod";
        #endregion

        #region Configuration
        public string VarioData { get; set; } = VBPowerShellSettings.VarioDataDefault;

        #endregion

        #region Parameters

        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public string FileName { get; set; }

        #endregion

        protected override void ProcessRecord()
        {
            VBPowerShellSettings iPlusCmdLetSettings = FactorySettings.Factory(VarioData);
            if (File.Exists(FileName))
            {
                using (Database database = new Database())
                {
                    string tempFileName = Path.GetFileName(FileName);
                    tempFileName = tempFileName + ".temp";
                    tempFileName = Path.Combine(Path.GetDirectoryName(FileName), tempFileName);

                    ACClass iPlusClass = null;
                    bool isInMethod = false;
                    int? bracetCount = null;
                    if (File.Exists(FileName))
                    {
                        using (FileStream inputFileStream = new FileStream(FileName, FileMode.Open))
                        using (FileStream outputFileStream = new FileStream(tempFileName, FileMode.OpenOrCreate))
                        {
                            using (StreamReader sr = new StreamReader(inputFileStream))
                            using (StreamWriter sw = new StreamWriter(outputFileStream))
                            {
                                string line = "";

                                while ((line = sr.ReadLine()) != null)
                                {

                                    if (!string.IsNullOrEmpty(line))
                                    {
                                        if (iPlusClass == null)
                                        {
                                            iPlusClass = GetACClass(database, line);
                                        }

                                        if(iPlusClass != null)
                                        {
                                            if(Regex.IsMatch(line, RegexPatternMehtod))
                                            {
                                                isInMethod = true;
                                                bracetCount = -1; // avoid fail on first line where method is defined
                                                string newMethodContent = BSOiPlusStudio.GenerateExecuteHandlerInternal(iPlusClass, null);
                                                sw.WriteLine(newMethodContent);
                                            }
                                        }

                                        if(isInMethod && bracetCount != null)
                                        {
                                            if(Regex.IsMatch(line, "{"))
                                            {
                                                if(bracetCount == -1)
                                                {
                                                    bracetCount = 0;
                                                }
                                                bracetCount++;
                                            }
                                            if (Regex.IsMatch(line, "}"))
                                            {
                                                bracetCount--;
                                            }

                                            if(bracetCount == 0)
                                            {
                                                isInMethod = false;
                                            }
                                        }
                                    }

                                    if (!isInMethod)
                                    {
                                        sw.WriteLine(line);
                                    }
                                }
                            }

                            File.Delete(FileName);
                            File.Copy(tempFileName, FileName);
                            File.Delete(tempFileName);
                        }
                    }

                }
            }
            else
            {
                Console.WriteLine($"Unable to find file: {FileName}");
            }
        }

        private static ACClass GetACClass(Database database, string line)
        {
            ACClass iPlusClass = null;
            if (Regex.IsMatch(line, RegexPatternClass))
            {
                Match match = Regex.Match(line, RegexPatternClass);
                if (match.Success && match.Groups.Count > 1)
                {
                    string className = match.Groups[1].Value;
                    iPlusClass = database.ACClass.Where(c => c.ACIdentifier == className).FirstOrDefault();
                }
            }

            return iPlusClass;
        }
    }
}
