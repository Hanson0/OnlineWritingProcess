using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.Skins;
using DevExpress.LookAndFeel;
using DevExpress.UserSkins;
using OnlineWritingProcess.AllForms;
using System.Threading;
using Production;


namespace OnlineWritingProcess
{
    public partial class MainForm : XtraForm
    {
        AutoWriteProcess autoWp;
        private bool openAndClose;
        public MainForm()
        {
            InitializeComponent();
            //InitTreeListControl();
            InitTreeViewControl();
            ConfigInfo.Init();
        }
        void InitTreeViewControl()
        {

            TreeNode treeNode1 = new TreeNode("手动写程");
            treeNode1.Name = "ManualWriPro";
            //treeNode1.Text = "手动写程";
            TreeNode treeNode2 = new TreeNode("自动写程");
            treeNode2.Name = "AutoWriPro";
            TreeNode treeNode3 = new TreeNode("写程", new TreeNode[] {
            treeNode1,
            treeNode2});
            treeNode3.Name = "WriPro";

            TreeNode treeNode4 = new TreeNode("节点4");
            treeNode4.Name = "node4";
            TreeNode treeNode5 = new TreeNode("节点5");
            treeNode5.Name = "node5";
            TreeNode treeNode6 = new TreeNode("节点3", new TreeNode[] {
            treeNode4,
            treeNode5});
            treeNode6.Name = "node6";

            //添加节点集合
            this.treeView1.Nodes.AddRange(new TreeNode[] {
            treeNode3,
            treeNode6});
            treeView1.ExpandAll();
            //添加子节点
            //TreeNode fnode = treeView1.Nodes[0];
            //fnode.Nodes.Add("子节点");
            //添加根节点
            //treeView1.Nodes.Add(treeNode6);
        }


        void InitTreeListControl()
        {
            Projects projects = InitData();
            DataBinding(projects);
        }
        Projects InitData()
        {
            Projects projects = new Projects();
            projects.Add(new Project("Project A", false));
            projects.Add(new Project("Project B", false));
            projects[0].Projects.Add(new Project("Task 1", true));
            projects[0].Projects.Add(new Project("Task 2", true));
            projects[0].Projects.Add(new Project("Task 3", true));
            projects[0].Projects.Add(new Project("Task 4", true));
            projects[1].Projects.Add(new Project("Task 1", true));
            projects[1].Projects.Add(new Project("Task 2", true));
            return projects;
        }
        void DataBinding(Projects projects)
        {
            //treeList.ExpandAll();
            //treeList.DataSource = projects;
            //treeList.BestFitColumns();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView1.SelectedNode.Name == "AutoWriPro")
            {
#if tabControl
                //try
                //{
                //    string testFormName = typeof(AutoWriteProcess).ToString();
                //    creatForm.AddTabpage(xtraTabControl1, "TabPageAuto", "自动写程", testFormName);
                //}
                //catch (Exception ex)
                //{

                //    XtraMessageBox.Show(ex.Message);
                //}
#else
                panel1.Controls.Clear();

                autoWp = new AutoWriteProcess();
                autoWp.TopLevel = false;
                autoWp.Dock = DockStyle.Fill;
                autoWp.FormBorderStyle = FormBorderStyle.None;
                autoWp.TopLevel = false;

                panel1.Controls.Add(autoWp);
                autoWp.Show();
#endif
            }
            else if (treeView1.SelectedNode.Name == "ManualWriPro")
            {
                DialogResult dr = XtraMessageBox.Show("打开手动写程后将关闭主界面\r\n是否关闭主程序", "警告", MessageBoxButtons.YesNo,
                                    MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.Yes)
                {
                    Login login = new Login();
                    login.ShowDialog();

                    if (!login.LoginOk)//登录成功才打开
                    {
                        XtraMessageBox.Show("手动写程请先登录");
                        return;
                    }
                        string exePath = System.Environment.CurrentDirectory + "\\" + AutoWriteProcess.ToolFolder + "\\SerialDownloader_cpp.exe";
                        System.Diagnostics.Process.Start(exePath);
                        Thread.Sleep(1000);
                        openAndClose = true;
                        Application.Exit();

                }


            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (openAndClose)
            {
                return;
            }

            if (autoWp != null)//自动写号界面打开过
            {
                DialogResult dr = XtraMessageBox.Show("是否保存设置", "确认", MessageBoxButtons.YesNo,
   MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.Yes)
                {
                    splashScreenManager1.ShowWaitForm();
                    splashScreenManager1.SetWaitFormCaption("请稍后");
                    splashScreenManager1.SetWaitFormDescription("保存中...");
                    //关闭设备搜索
                    autoWp.StartSearchDev = false;
                    //存储并设置
                    autoWp.SaveConfigValue(true);
                    splashScreenManager1.CloseWaitForm();
                }


            }
        }
    }



}