using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;

namespace Dict2Db
{
    class SqliteHelper
    {
        SQLiteConnection conn = null;
        SQLiteTransaction tran = null;
        string tableName = null;

        public SqliteHelper(string DbName, string tableName)
        {
            try
            {
                conn = new SQLiteConnection("Data Source=" + DbName);
                conn.Open();
            }
            catch (System.Exception ex)
            {

            }
            this.tableName = "db_" + tableName.Replace('-', '_').Replace('.', '_');//替换掉特殊字符
        }

        public void beginTrans()//开始一个事务处理
        {
            try
            {
                tran = conn.BeginTransaction();
            }
            catch (System.Exception ex)
            {

            }
        }

        public void createDataTable()
        {
            string sql = "CREATE TABLE IF NOT EXISTS " + tableName + "(dIndex TEXT,dContent TEXT);";//创建数据表
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {

            }
        }

        public void createSynTable()
        {
            string sql = "CREATE TABLE IF NOT EXISTS " + tableName + "Syn(dIndex TEXT,dSynIndex INTEGER);";//创建同步表
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {

            }
        }

        public void putValue(string index, string content)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.Transaction = tran;
            cmd.CommandText = "INSERT INTO " + tableName + "(dIndex,dContent)VALUES(@index,@content);";//插入数据
            cmd.Parameters.AddRange(new[]{
                new SQLiteParameter("@index", index),
                new SQLiteParameter("@content", content)
            });
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {

            }
        }

        public void putSyn(string index, UInt32 syn)
        {
            SQLiteCommand cmd = new SQLiteCommand(conn);
            cmd.Transaction = tran;
            cmd.CommandText = "INSERT INTO " + tableName + "Syn(dIndex,dSynIndex)VALUES(@index,@syn);";//插入索引
            cmd.Parameters.AddRange(new[]{
                new SQLiteParameter("@index", index),
                new SQLiteParameter("@syn", syn + 1)
            });
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (System.Exception ex)
            {

            }
        }

        public void commit()//提交操作
        {
            try
            {
                tran.Commit();
            }
            catch (System.Exception ex)
            {

            }
        }

        public void Close()//关闭数据连接
        {
            try
            {
                conn.Close();
            }
            catch (System.Exception ex)
            {

            }
        }
    }
}
