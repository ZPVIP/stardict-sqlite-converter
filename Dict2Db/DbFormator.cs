using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Dict2Db
{
    class DbFormator
    {
        private string idxFile, dictFile, synFile;//表示idx文件，dict文件，syn文件
        private SqliteHelper helper = null;
        private bool isSyned = false;//表示是否有同义词
        private bool isFileOK = true;

        public DbFormator(string dictDirectory)
        {
            try
            {
                idxFile = Directory.GetFiles(dictDirectory, "*.idx")[0];//获取idx文件
                dictFile = Directory.GetFiles(dictDirectory, "*.dict")[0];//获取dict文件
            }
            catch (System.Exception ex)
            {
                isFileOK = false;
                return;
            }
            string[] syns = Directory.GetFiles(dictDirectory, "*.syn");//获取syn文件
            if (syns.Length == 1)//如果存在syn文件，说明字典有同义词
            {
                isSyned = true;
                synFile = syns[0];
            }
            FileInfo idxInfo = new FileInfo(idxFile);//索引文件info
            string dbTableName = idxInfo.Name.Substring(0, idxInfo.Name.Length - 4);//获取表名
            string dbName = idxInfo.DirectoryName + "\\" + dbTableName + ".db";//获取数据库名
            helper = new SqliteHelper(dbName, dbTableName);//创建sqlitehelper对象
        }

        public void start()//将信息写入数据库
        {
            if (!isFileOK)
            {
                return;
            }
            helper.createDataTable();
            helper.beginTrans();
            using (FileStream idxStream = File.OpenRead(idxFile))
            {
                byte temp;
                int index = 0;
                byte[] buffer = new byte[1024];
                UInt32 offset, length;//索引指明的内容偏移和长度
                bool start = true;

                FileStream dictStream = File.OpenRead(dictFile);
                BinaryReader idxReader = new BinaryReader(idxStream);
                BinaryReader dictReader = new BinaryReader(dictStream);
                while (idxStream.Position < idxStream.Length)
                {
                    temp = idxReader.ReadByte();
                    buffer[index++] = temp;

                    if (temp == 0 && !start)
                    {
                        offset = htonl(idxReader.ReadUInt32());
                        length = htonl(idxReader.ReadUInt32());

                        dictStream.Seek(offset, SeekOrigin.Begin);
                        byte[] dictBytes = dictReader.ReadBytes((int)length);

                        byte[] idxBytes = new byte[--index];
                        for (int i = 0; i < index; i++)
                        {
                            idxBytes[i] = buffer[i];
                        }
                        helper.putValue(Encoding.UTF8.GetString(idxBytes), Encoding.UTF8.GetString(dictBytes));
                        index = 0;
                        start = true;
                    }
                    else if (temp != 0)
                    {
                        start = false;
                    }
                }
            }
            helper.commit();
            if (isSyned)//如果存在同义词
            {
                helper.createSynTable();
                helper.beginTrans();
                using (FileStream synStream = File.OpenRead(synFile))//把同意词文件打开
                {
                    UInt32 synIndex;//同义词所对应的索引
                    byte temp;
                    int index = 0;
                    byte[] buffer = new byte[1024];

                    BinaryReader synReader = new BinaryReader(synStream);//读文件的Reader
                    while (synStream.Position < synStream.Length)
                    {
                        temp = synReader.ReadByte();//读取一个同义词
                        buffer[index++] = temp;

                        if (temp == 0)
                        {
                            synIndex = htonl(synReader.ReadUInt32());//读取同义词索引
                            byte[] synBytes = new byte[--index];
                            for (int i = 0; i < index; i++ )
                            {
                                synBytes[i] = buffer[i];
                            }

                            helper.putSyn(Encoding.UTF8.GetString(synBytes), synIndex);
                            index = 0;
                        }
                    }
                }
                helper.commit();
            }
            helper.Close();
        }

        private UInt32 htonl(UInt32 src)//大小端转换
        {
            byte[] bytes = BitConverter.GetBytes(src);
            byte temp = bytes[0];
            bytes[0] = bytes[3];
            bytes[3] = temp;

            temp = bytes[1];
            bytes[1] = bytes[2];
            bytes[2] = temp;
            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}
