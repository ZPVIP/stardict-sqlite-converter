using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Dict2Db
{
    class UnZipWork
    {
        UseWinRar rar = null;

        public UnZipWork()
        {
            rar = new UseWinRar();
        }
        
        /// <summary>
        /// 解压压缩字典.dict.dz文件
        /// </summary>
        /// <param name="fileDirectory">压缩字典所在目录的父目录</param>
        /// <returns>解压结果</returns>
        public bool startUnZipDictdz(string fileDirectory)
        {
            bool succeed = true;
            
            string[] dictDirectorys = Directory.GetDirectories(fileDirectory);
            foreach (string dictDirectory in dictDirectorys)
            {
                if (!rar.unZipWithExtension(dictDirectory, "dz"))
                {
                    succeed = false;
                }
            }
            return succeed;
        }

        /// <summary>
        /// 解压字典包.tar.bz2文件
        /// </summary>
        /// <param name="fileDirectory">字典包所在目录</param>
        /// <returns></returns>
        public bool startUnZipTarbz2(string fileDirectory)
        {
            UseWinRar rar = new UseWinRar();
            bool result = rar.unZipWithExtension(fileDirectory, "bz2");
            return result;
        }
    }
}
