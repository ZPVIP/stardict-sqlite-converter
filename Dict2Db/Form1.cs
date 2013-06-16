using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Media;

namespace Dict2Db
{
    public partial class Form1 : Form
    {
        string dictDirectory;

        public Form1()
        {
            InitializeComponent();
            dictDirectory = textBoxDictDir.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);//桌面作为默认路径
            CheckForIllegalCrossThreadCalls = false;
            toolStripStatusLabelInfo.Text = "字典目录:" + dictDirectory;
        }

        private void buttonOpen_Click(object sender, EventArgs e)//选择路径按钮事件
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                dictDirectory = textBoxDictDir.Text = folderBrowserDialog1.SelectedPath;
                textBoxDictDir.Focus();
                textBoxDictDir.SelectAll();
                toolStripStatusLabelInfo.Text = "字典目录:" + dictDirectory;
            }
        }

        private void buttonUnZip_Click(object sender, EventArgs e)//解压按钮事件
        {
            pictureBoxProcess.Visible = true;
            toolStripStatusLabelInfo.Text = "正在解压*.tar.bz2,请稍候...";
            Thread th = new Thread(unZipProcess);
            th.Start();
        }

        private void unZipProcess()
        {
            startProcess();
            UnZipWork work = new UnZipWork();
            work.startUnZipTarbz2(dictDirectory);
            toolStripStatusLabelInfo.Text = "tar.bz2解压完成,正在解压*.dict.dz,请稍候...";
            work.startUnZipDictdz(dictDirectory);
            toolStripStatusLabelInfo.Text = "解压工作已全部完成。";
            endProcess();
        }

        private void buttonWrite_Click(object sender, EventArgs e)//转入SQLite按钮事件
        {
            pictureBoxProcess.Visible = true;
            toolStripStatusLabelInfo.Text = "开始转换数据,请不要关闭...";
            Thread th = new Thread(writeDbProcess);
            th.Start();
        }

        private void writeDbProcess()
        {
            startProcess();
            string[] subDirectorys = Directory.GetDirectories(dictDirectory);
            foreach (string subDirectory in subDirectorys)
            {
                toolStripStatusLabelInfo.Text = subDirectory;
                new DbFormator(subDirectory).start();
            }
            toolStripStatusLabelInfo.Text = "数据转换完成。";
            endProcess();
        }

        private void startProcess()
        {
            textBoxDictDir.Enabled = false;
            buttonOpen.Enabled = false;
            buttonUnZip.Enabled = false;
            buttonWrite.Enabled = false;
        }

        private void endProcess()
        {
            new SoundPlayer(Dict2Db.sounds.finish).Play();//播放声音
            textBoxDictDir.Enabled = true;
            buttonOpen.Enabled = true;
            buttonUnZip.Enabled = true;
            buttonWrite.Enabled = true;
            pictureBoxProcess.Visible = false;
        }
    }
}
