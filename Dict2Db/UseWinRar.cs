using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace Dict2Db
{
    class UseWinRar
    {
        private string rarExeFile = null;//WinRar.exe路径
        private bool useAble = false;//标志WinRar是否可用

        public UseWinRar()
        {
            rarExeFile = getRarExe();
            useAble = !string.IsNullOrEmpty(rarExeFile);//如果WinRar.exe路径不为空，说明可用
        }

        public static string getRarExe()
        {
            string rarExe = null;
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\WinRAR.exe");
            if (regKey == null)
            {
                return null;
            }
            rarExe = regKey.GetValue("").ToString();
            regKey.Close();//关闭注册表
            return rarExe;
        }

        public bool exeRarCmd(string cmd)
        {
            if (!useAble)
            {
                return false;
            }
            Process process = new Process();//新建一个过程
            ProcessStartInfo startInfo = new ProcessStartInfo(rarExeFile);//新建一个启动信息
            startInfo.Arguments = cmd;//设置启动信息的执行参数
            //startInfo.WorkingDirectory = workDirectory;//设置启动信息的工作目录
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;//设置程序后台运行
            process.StartInfo = startInfo;//设置过程的启动信息
            process.Start();//开始过程
            process.WaitForExit();
            if (process.ExitCode == 0)
            {
                return true;
            }
            return false;
        }

        public bool exeRarCmd(string cmd, string workDirectory)
        {
            if (!useAble)
            {
                return false;
            }
            Process process = new Process();//新建一个过程
            ProcessStartInfo startInfo = new ProcessStartInfo(rarExeFile);//新建一个启动信息
            startInfo.Arguments = cmd;//设置启动信息的执行参数
            startInfo.WorkingDirectory = workDirectory;//设置启动信息的工作目录
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;//设置程序后台运行
            process.StartInfo = startInfo;//设置过程的启动信息
            process.Start();//开始过程
            process.WaitForExit();
            if (process.ExitCode == 0)
            {
                return true;
            }
            return false;
        }

        public bool unZipFile(string zipFile, string targetDirectory)
        {
            if (!File.Exists(zipFile))
            {
                return false;
            }
            string zipCmd = "x " + zipFile + " " + targetDirectory + " -y -ibck";//后台解压压缩文件中全部文件到指定目录
            return exeRarCmd(zipCmd);//执行解压操作
        }

        public bool unZipFile(string zipFile)//将压缩文件解压到当前目录
        {
            if (!File.Exists(zipFile))
            {
                return false;
            }
            FileInfo fileInfo = new FileInfo(zipFile);
            string zipCmd = "x " + zipFile + " " + fileInfo.DirectoryName + " -y -ibck";//后台解压压缩文件中全部文件到压缩文件所在目录
            return exeRarCmd(zipCmd);//执行解压操作
        }

        public bool unZipWithExtension(string zipDirectory, string extension, string targetDirectory)
        {
            if (!Directory.Exists(zipDirectory))
            {
                return false;
            }
            string zipCmd = "x " + zipDirectory + "\\*." + extension + " " + targetDirectory + " -y -ibck";//后台解压压缩文件中全部文件到指定目录
            return exeRarCmd(zipCmd);//执行解压操作
        }

        public bool unZipWithExtension(string zipDirectory, string extension)
        {
            if (!Directory.Exists(zipDirectory))
            {
                return false;
            }
            string zipCmd = "x " + zipDirectory + "\\*." + extension + " " + zipDirectory + " -y -ibck";//后台解压压缩文件中全部文件到指定目录
            return exeRarCmd(zipCmd);//执行解压操作
        }
    }
}
