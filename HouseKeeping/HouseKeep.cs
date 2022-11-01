using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using log4net;
using MOH.BatchJobs.HouseKeeping.Models;
using Newtonsoft.Json;

namespace MOH.BatchJobs.HouseKeeping
{
    class HouseKeep
    {
        public static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void HouseKeeping()
        {
            _log.Info("MOH.BatchJobs.HouseKeeping START");

            using (StreamReader r = new StreamReader("HouseKeepFolderPath.json"))
            {
                string json = r.ReadToEnd();
                List<LoadJson> items = JsonConvert.DeserializeObject<List<LoadJson>>(json);
                int houseKeepday = -1 * Convert.ToInt32(ConfigurationManager.AppSettings["HouseKeepDay"]);
                foreach (LoadJson folderpath in items)
                {
                    if (Directory.Exists(folderpath.path) && folderpath.enable)
                    {
                        string[] fileEntries = Directory.GetFiles(folderpath.path);
                        string folderName = folderpath.path;
                        if (Directory.Exists(folderpath.path))
                        {

                            DirectoryInfo Dictiontory = new DirectoryInfo(folderpath.path);
                            DirectoryInfo[] FolderDir = Dictiontory.GetDirectories();
                            if (FolderDir.Length > 0) //if sub folder , warning about sub folder and won't delete any files under sub folder.
                            {
                                foreach (DirectoryInfo subDir in FolderDir)
                                {
                                    _log.Warn("There is sub folder " + subDir.Name + " under " + folderName);
                                    if (subDir.LastWriteTime.Date >= DateTime.Now.AddDays(houseKeepday) && subDir.CreationTime >= DateTime.Now.AddDays(-1) && folderpath.subdir)
                                    {
                                        _log.Info(subDir + " is deleted");
                                        subDir.Delete();
                                    }
                                }
                            }
                            if (fileEntries.Length > 0)
                            {
                                //Delete the file
                                foreach (string filePath in fileEntries)
                                {
                                    //if (File.GetCreationTime(filePath).Date <= DateTime.Now.AddDays(houseKeepday).Date && File.GetLastWriteTime(filePath).Date <= DateTime.Now.AddDays(houseKeepday).Date)  //check files that is created or modified less than housekeepday
                                    if (File.GetLastWriteTime(filePath).Date <= DateTime.Now.AddDays(houseKeepday).Date)//to use upper condition
                                    {
                                        _log.Info(filePath.ToString() + " is deleted");
                                        System.IO.File.Delete(filePath);
                                    }
                                }

                            }
                        }
                        else
                        {
                            _log.Info("No files found for" + folderName);
                            Environment.ExitCode = 0;
                        }
                    }
                    else
                    {
                        _log.Info("No folder path: " + folderpath.path);
                    }
                }
            }
        }
    }
}
