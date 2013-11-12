using System.Drawing;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using Ionic.Zip;
using Microsoft.Win32;
using System.Deployment.Application;
using System;

namespace SE_Modz_Installer
{
    public partial class frmMain : Form
    {
        public string strGamePath;
        const string userRoot = "HKEY_CURRENT_USER";
        const string subkey = "Software\\SE-Modz Installer";
        const string keyName = userRoot + "\\" + subkey;
        const string strForumURL = "http://se-modz.com/?q=forum";
        const string strSiteURL = "http://se-modz.com";
        private void FMove(string ze)
        {
            string f = strGamePath + "\\Content\\" + ze.Substring(ze.IndexOf("/") + 1).Replace("/", "\\");
            if (System.IO.File.Exists(f))
            {
                System.IO.File.Delete(f);
            }
            System.IO.File.Move("C:\\Temp\\" + ze.Replace("/", "\\"), f);
        }

        private void LaunchSite(string s)
        {
            switch (s)
            {
                case "forum":
                    System.Diagnostics.Process.Start(strForumURL);
                    break;
                case "site":
                    System.Diagnostics.Process.Start(strSiteURL);
                    break;
                default:
                    break;
            }
            
        }

        public frmMain()
        {
            InitializeComponent();
            strGamePath = Registry.GetValue(keyName, "Game Path", "Click here to locate your game directory") as string;
            txtGamePath.Text = strGamePath;
            if (strGamePath != "Click here to locate your game directory")
            {
                pnlDrop.Enabled = true;
                pnlDrop.BackgroundImage = pnlDrop.BackgroundImage = SE_Modz_Installer.Properties.Resources.Drag_Zip_File_Here;
                lblStatus.Text = "Install path set. Drag a zipped block file to the colored area.";
            }
            if (Registry.GetValue(keyName, "Auto Update", "True") as string == "True")
            {
                ckbUpdate.Checked = true;
            }
            if (ckbUpdate.Checked)
            {
                UpdateCheckInfo info = null;
                ApplicationDeployment ad;
                try
                {
                    ad = ApplicationDeployment.CurrentDeployment;
                }
                catch (InvalidDeploymentException ide)
                {
                    lblStatus.Text = "Unable to auto-update";
                    ad = null;
                }
                if (ad != null)
                {
                    try
                    {
                        info = ad.CheckForDetailedUpdate();

                    }
                    catch (DeploymentDownloadException dde)
                    {
                        MessageBox.Show("There was an error attempting to retrieve the file. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                        return;
                    }
                    catch (InvalidDeploymentException ide)
                    {
                        MessageBox.Show("Please reinstall. Error: " + ide.Message);
                        return;
                    }
                    catch (InvalidOperationException ioe)
                    {
                        MessageBox.Show("The application cannot be updated." + ioe.Message);
                        return;
                    }
                    if (info.UpdateAvailable)
                    {
                        Boolean doUpdate = true;

                        if (!info.IsUpdateRequired)
                        {
                            DialogResult dr = MessageBox.Show("A new update is availabe. Do you wish to update?", "New Update", MessageBoxButtons.OKCancel);
                            if (!(DialogResult.OK == dr))
                            {
                                doUpdate = false;
                            }
                        }
                        else
                        {
                            // Display a message that the app MUST reboot. Display the minimum required version.
                            MessageBox.Show("A very important update has been released that will convert you from the installed " +
                                "version to version " + info.MinimumRequiredVersion.ToString() +
                                ". The application will update and restart.",
                                "Important Update Available", MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }

                        if (doUpdate)
                        {
                            try
                            {
                                ad.Update();
                                MessageBox.Show("Update complete. The program will now restart.");
                                Application.Restart();
                            }
                            catch (DeploymentDownloadException dde)
                            {
                                MessageBox.Show("Unable to update. \n\nPlease check your network connection, or try again later. Error: " + dde);
                                return;
                            }
                        }
                    }
                }
            }
        }

        private void pnlDrop_DragEnter(object sender, DragEventArgs e)
        {
            lbxContents.Items.Clear();
            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            ZipFile zf = new ZipFile(FileList[0]);
            if (zf.Info != "")
            {
                zf.ExtractAll("C:\\Temp",ExtractExistingFileAction.OverwriteSilently);
                lblStatus.Text = zf.Info;
                string s = zf.Name.Substring(zf.Name.LastIndexOf("\\") + 1);
                s = s.Substring(0, s.Length - 4);
                foreach (string ze in zf.EntryFileNames)
                {
                    lbxContents.Items.Add(ze);
                    if (ze.ToLower().Contains("definition.xml"))
                    {
                        XmlDocument xmdDesc = new XmlDocument();
                        xmdDesc.Load("C:\\Temp\\" + ze.Replace("/", "\\"));
                        XmlNode xndDef = xmdDesc.GetElementsByTagName("Definition").Item(0);
                        XmlDocument xmdCubeBlocks = new XmlDocument();
                        System.IO.File.Copy(strGamePath + "\\Content\\Data\\CubeBlocks.sbc", strGamePath + "\\Content\\Data\\CubeBlocks_backup"+ System.DateTime.Now.ToFileTimeUtc() +".sbc",true);
                        xmdCubeBlocks.Load(strGamePath + "\\Content\\Data\\CubeBlocks.sbc");
                        XmlNode xndImport = xmdCubeBlocks.ImportNode(xndDef,true);
                        bool exists = false;
                        foreach (XmlNode xndN in xmdCubeBlocks.GetElementsByTagName("Definitions").Item(0).ChildNodes)
                        {
                            if (xndN.ChildNodes.Item(1).InnerText == xndDef.ChildNodes.Item(1).InnerText &&
                                xndN.FirstChild.ChildNodes.Item(1).InnerText == xndDef.FirstChild.ChildNodes.Item(1).InnerText)
                            {
                                exists = true;
                                xmdCubeBlocks.GetElementsByTagName("Definitions").Item(0).ReplaceChild(xndImport, xndN);
                            }
                        }
                        if (!exists)
                        {
                            xmdCubeBlocks.GetElementsByTagName("Definitions").Item(0).AppendChild(xndImport);
                            xmdCubeBlocks.Save(strGamePath + "\\Content\\Data\\CubeBlocks.sbc");
                            lblStatus.Text = "CubeBlocks.sbc Modified.";
                        }
                        else
                        {
                            xmdCubeBlocks.Save(strGamePath + "\\Content\\Data\\CubeBlocks.sbc");
                            lblStatus.Text = "CubeBlocks.sbc updated.";
                        }
                    }
                    else if (ze.ToLower().Contains("textures") && ze.ToLower().EndsWith(".dds"))
                    {
                        FMove(ze);
                    }
                    else if (ze.ToLower().Contains("models") && ze.ToLower().EndsWith(".mwm"))
                    {
                        FMove(ze);
                    }
                }
            }
        }

        private void txtGamePath_MouseClick(object sender, MouseEventArgs e)
        {
            FolderBrowserDialog fbdGamePath = new FolderBrowserDialog();
            fbdGamePath.RootFolder = System.Environment.SpecialFolder.DesktopDirectory;
            fbdGamePath.ShowDialog();
            if (fbdGamePath.SelectedPath != "" && System.IO.Directory.Exists(fbdGamePath.SelectedPath + "\\Content"))
            {
                pnlDrop.Enabled = true;
                pnlDrop.BackgroundImage = SE_Modz_Installer.Properties.Resources.Drag_Zip_File_Here;
                txtGamePath.Text = fbdGamePath.SelectedPath;
                strGamePath = txtGamePath.Text;
                lblStatus.Text = "Install path set. Drag a zipped block file to the colored area.";
                Registry.SetValue(keyName, "Game Path", strGamePath);
            }
            else if (fbdGamePath.SelectedPath != "" && !System.IO.Directory.Exists(fbdGamePath.SelectedPath + "\\Content"))
            {
                lblStatus.Text = "It seems you have selected an invalid folder. Please try again";
            }
        }

        private void lnkSEMForum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LaunchSite("forum");
        }

        private void pbxIcon_MouseClick(object sender, MouseEventArgs e)
        {
            LaunchSite("site");
        }

        private void ckbUpdate_CheckedChanged(object sender, EventArgs e)
        {
            Registry.SetValue(keyName, "Auto Update", ckbUpdate.Checked.ToString());
        }
    }
}
