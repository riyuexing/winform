using System;
using System.Text;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using JCodes.Framework.CommonControl.Framework;
using JCodes.Framework.Common;
using JCodes.Framework.Entity;
using JCodes.Framework.BLL;
using JCodes.Framework.AddIn.Basic;
using JCodes.Framework.jCodesenum.BaseEnum;
using JCodes.Framework.CommonControl;
using JCodes.Framework.Common.Framework;
using JCodes.Framework.CommonControl.Other;
using JCodes.Framework.CommonControl.PlugInInterface;
using System.Threading;
using DevExpress.Utils;
using System.Diagnostics;
using JCodes.Framework.Common.Office;

namespace JCodes.Framework.AddIn.Basic
{
    /// <summary>
    /// ��̬����RibbonPage��������İ�ť��Ŀ������
    /// </summary>
    public class RibbonPageHelper
    {
        private RibbonControl control;
        public MainForm mainForm;

        public RibbonPageHelper(MainForm mainForm, ref RibbonControl control)
        {
            this.mainForm = mainForm;
            this.control = control;
        }

        public void AddPages()
        {
            // Լ���˵�����3������һ��Ϊ�����𣬵ڶ���ΪСģ����飬������Ϊ����Ĳ˵� 
            // Portal.gc.SystemType = WareMis
            List<MenuNodeInfo> menuList = BLLFactory<Menus>.Instance.GetTree(Portal.gc.SystemType);
            if (menuList.Count == 0) return;

            int i = 0;
            foreach(MenuNodeInfo firstInfo in menuList)
            {
                //���û�в˵���Ȩ�ޣ�������
                if (!Portal.gc.HasFunction(firstInfo.FunctionId)) continue;

                //����ҳ�棨һ���˵���
                RibbonPage page = new RibbonPage();
                page.Text = firstInfo.Name;
                page.Name = firstInfo.ID;
                this.control.Pages.Insert(i++, page);
                
                if(firstInfo.Children.Count == 0) continue;
                foreach(MenuNodeInfo secondInfo in firstInfo.Children)
                {
                    //���û�в˵���Ȩ�ޣ�������
                    if (!Portal.gc.HasFunction(secondInfo.FunctionId)) continue;

                    //����RibbonPageGroup�������˵���
                    RibbonPageGroup group = new RibbonPageGroup();
                    group.Text = secondInfo.Name;
                    group.Name = secondInfo.ID;
                    //group.Glyph = LoadIcon(secondInfo.Icon);
                    //group.ImageIndex = 5;
                    page.Groups.Add(group);                

                    if(secondInfo.Children.Count == 0) continue;
                    foreach (MenuNodeInfo thirdInfo in secondInfo.Children)
                    {
                        //���û�в˵���Ȩ�ޣ�������
                        if (!Portal.gc.HasFunction(thirdInfo.FunctionId)) continue;

                        // �ж� WinformType ����� RgbiSkins ���ʾƤ��
                        if (thirdInfo.WinformType == Const.RgbiSkins)
                        {
                            RibbonGalleryBarItem rgbi = new RibbonGalleryBarItem();
                            var galleryItemGroup1 = new GalleryItemGroup();
                            rgbi.Name = thirdInfo.ID;
                            rgbi.Caption = thirdInfo.Name;
                            rgbi.Gallery.Groups.AddRange(new DevExpress.XtraBars.Ribbon.GalleryItemGroup[] {
            galleryItemGroup1});
                            group.ItemLinks.Add(rgbi);
                            DevExpress.XtraBars.Helpers.SkinHelper.InitSkinGallery(rgbi, true);
                        }
                        else {
                            //���ӹ��ܰ�ť�������˵���
                            BarButtonItem button = new BarButtonItem();
                            button.PaintStyle = BarItemPaintStyle.CaptionGlyph;
                            button.LargeGlyph = LoadIcon(thirdInfo.Icon);
                            button.Glyph = LoadIcon(thirdInfo.Icon);

                            button.Name = thirdInfo.ID;
                            button.Caption = thirdInfo.Name;
                            button.Tag = thirdInfo.WinformType;
                            button.ItemClick += (sender, e) =>
                            {
                                if (button.Tag != null && !string.IsNullOrEmpty(button.Tag.ToString()))
                                {
                                    Portal.gc._waitBeforeLogin = new WaitDialogForm("������� "+button.Caption + " ������...", "���ش���");
                                    LoadPlugInForm(button.Tag.ToString());
                                    if (Portal.gc._waitBeforeLogin != null)
                                    {
                                        Portal.gc._waitBeforeLogin.Invoke((EventHandler)delegate
                                        {
                                            if (Portal.gc._waitBeforeLogin != null)
                                            {
                                                Portal.gc._waitBeforeLogin.Close(); Portal.gc._waitBeforeLogin = null;
                                            }
                                        });
                                    }
                                }
                                else
                                {
                                    MessageDxUtil.ShowTips(button.Caption);
                                }
                            };
                            if (thirdInfo.WinformType.Contains(Const.BeginGroup))
                            {
                                group.ItemLinks.Add(button, true);
                            }
                            else
                            {
                                group.ItemLinks.Add(button);
                            }
                            
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ����ͼ�꣬������ز��ɹ�����ôʹ��Ĭ��ͼ��
        /// </summary>
        /// <param name="iconPath"></param>
        /// <returns></returns>
        private Image LoadIcon(string iconPath)
        {
            // 20170512 wjm ��ʱ�޸�
            Image result = Properties.Resources.favicon.ToBitmap();
            try
            {
                if (!string.IsNullOrEmpty(iconPath))
                {
                    string path = Path.Combine(Application.StartupPath, iconPath);
                    if (File.Exists(path))
                    {
                        result = Image.FromFile(path);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogLevel.LOG_LEVEL_CRIT, ex, typeof(RibbonPageHelper));
                MessageDxUtil.ShowError(ex.Message);
            }

            return result;
        }

        /// <summary>
        /// ���ز������
        /// </summary>
        private void LoadPlugInForm(string typeName)
        {
            try
            {
                string[] itemArray = typeName.Split(new char[]{',',';'});

                string type = itemArray[0].Trim();
                string filePath = itemArray[1].Trim();//���������·��

                // �ж��Ƿ��Ǵ�����
                // ����� ����ҳ��ĵ�������
                if (Const.BtnLink == type)
                {
                    Process.Start(filePath);
                    return;
                }

                //�ж��Ƿ���������ʾģʽ��Ĭ�ϴ���ΪShow��ģʽ��ʾ
                string showDialog = (itemArray.Length > 2) ? itemArray[2].ToLower() : "";
                bool isShowDialog = (showDialog == "1") || (showDialog == "dialog");

                if (isShowDialog)
                {
                    if (Portal.gc._waitBeforeLogin != null)
                    {
                        Portal.gc._waitBeforeLogin.Invoke((EventHandler)delegate
                        {
                            if (Portal.gc._waitBeforeLogin != null)
                            {
                                Portal.gc._waitBeforeLogin.Close(); Portal.gc._waitBeforeLogin = null;
                            }
                        });
                    }
                }

                string dllFullPath = Path.Combine(Application.StartupPath, filePath);
                Assembly tempAssembly = System.Reflection.Assembly.LoadFrom(dllFullPath);
                if (tempAssembly != null)
                {
                    Type objType = tempAssembly.GetType(type);
                    if (objType != null)
                    {
                        LoadMdiForm(this.mainForm, objType, isShowDialog);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogLevel.LOG_LEVEL_CRIT, ex, typeof(RibbonPageHelper));
                MessageDxUtil.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Ψһ����ĳ�����͵Ĵ��壬�����������ʾ�����򴴽���
        /// </summary>
        /// <param name="mainDialog">���������</param>
        /// <param name="formType">����ʾ�Ĵ�������</param>
        /// <returns></returns>
        public static Form LoadMdiForm(Form mainDialog, Type formType, bool isShowDialog)
        {
            Form tableForm = null;
            bool bFound = false;
            if (!isShowDialog) //�����ģ̬���ڣ�����
            {
                foreach (Form form in mainDialog.MdiChildren)
                {
                    if (form.GetType() == formType)
                    {
                        bFound = true;
                        tableForm = form;
                        break;
                    }
                }
            }

            //û���ڶ��ĵ����ҵ�������ģ̬���ڣ���Ҫ��ʼ������
            if (!bFound || isShowDialog)
            {
                tableForm = (Form)Activator.CreateInstance(formType);

                //������弯����IFunction�ӿ�(��һ�δ�����Ҫ����)
                IFunction function = tableForm as IFunction;
                if (function != null)
                {
                    // 20170610 �ĳɴӻ����ж�ȡ
                    var cacheDict = Cache.Instance["FunctionDict"] as Dictionary<string, string>;
                    var loginUserInfo = Cache.Instance["LoginUserInfo"] as LoginUserInfo;
                    //��ʼ��Ȩ�޿�����Ϣ
                    function.InitFunction(loginUserInfo, cacheDict);

                    //��¼����������Ϣ
                    function.AppInfo = new AppInfo(Portal.gc.AppUnit, Portal.gc.AppName, Portal.gc.AppWholeName, Portal.gc.SystemType);
                }

            }

            if (isShowDialog)
            {
                tableForm.ShowDialog();
            }
            else
            {
                tableForm.MdiParent = mainDialog;
                tableForm.Show();
            }
            tableForm.BringToFront();
            tableForm.Activate();

            return tableForm;
        }
    }
}