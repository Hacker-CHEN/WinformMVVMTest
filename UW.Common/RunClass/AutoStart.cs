using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using File = System.IO.File;

namespace UW.Common
{
    public class AutoStart
    {
        private string QuickName => Path.GetFileNameWithoutExtension(appAllPath);

        private string systemStartPath => Environment.GetFolderPath(Environment.SpecialFolder.Startup);

        private string appAllPath => Application.ExecutablePath;

        private string desktopPath => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

        public void SetMeAutoStart(bool onOff = true)
        {
            List<string> shortcutPaths;
            if (onOff)
            {
                shortcutPaths = GetQuickFromFolder(systemStartPath, appAllPath);
                if (shortcutPaths.Count >= 2)
                {
                    for (int i = 1; i < shortcutPaths.Count; i++)
                    {
                        DeleteFile(shortcutPaths[i]);
                    }
                }
                else if (shortcutPaths.Count < 1)
                {
                    CreateShortcut(systemStartPath, QuickName, appAllPath, "Lyric_SCADA");
                }
                return;
            }
            shortcutPaths = GetQuickFromFolder(systemStartPath, appAllPath);
            if (shortcutPaths.Count > 0)
            {
                for (int i = 0; i < shortcutPaths.Count; i++)
                {
                    DeleteFile(shortcutPaths[i]);
                }
            }
        }

        private bool CreateShortcut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string shortcutPath = Path.Combine(directory, $"{shortcutName}.lnk");
                WshShell shell = (WshShell)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")));
                IWshShortcut shortcut = (IWshShortcut)(dynamic)shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
                shortcut.WindowStyle = 1;
                shortcut.Description = description;
                shortcut.IconLocation = (string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation);
                shortcut.Save();
                return true;
            }
            catch (Exception ex)
            {
                string temp = ex.Message;
                temp = "";
            }
            return false;
        }

        private List<string> GetQuickFromFolder(string directory, string targetPath)
        {
            List<string> tempStrs = new List<string>();
            tempStrs.Clear();
            string tempStr = null;
            string[] files = Directory.GetFiles(directory, "*.lnk");
            if (files == null || files.Length < 1)
            {
                return tempStrs;
            }
            for (int i = 0; i < files.Length; i++)
            {
                tempStr = GetAppPathFromQuick(files[i]);
                if (tempStr == targetPath)
                {
                    tempStrs.Add(files[i]);
                }
            }
            return tempStrs;
        }

        private string GetAppPathFromQuick(string shortcutPath)
        {
            if (File.Exists(shortcutPath))
            {
                WshShell shell = (WshShell)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("72C24DD5-D70A-438B-8A42-98424B88AFB8")));
                IWshShortcut shortct = (IWshShortcut)(dynamic)shell.CreateShortcut(shortcutPath);
                return shortct.TargetPath;
            }
            return "";
        }

        private void DeleteFile(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if (attr == FileAttributes.Directory)
            {
                Directory.Delete(path, recursive: true);
            }
            else
            {
                File.Delete(path);
            }
        }

        public void CreateDesktopQuick(string desktopPath = "", string quickName = "", string appPath = "")
        {
            if (desktopPath == "")
            {
                desktopPath = this.desktopPath;
            }
            if (quickName == "")
            {
                quickName = QuickName;
            }
            if (appPath == "")
            {
                appPath = appAllPath;
            }
            List<string> shortcutPaths = GetQuickFromFolder(desktopPath, appPath);
            if (shortcutPaths.Count < 1)
            {
                CreateShortcut(desktopPath, quickName, appPath, "UW.Software");
            }
        }
    }
}
