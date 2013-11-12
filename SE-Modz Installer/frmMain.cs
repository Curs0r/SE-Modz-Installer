using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Ionic.Zip;

namespace SE_Modz_Installer
{
    public partial class frmMain : Form
    {
        public string strGamePath;
        public ZipFile zf;
        public bool valid;
        private void FMove(string ze)
        {
            string f = strGamePath + "\\Content\\" + ze.Substring(ze.IndexOf("/") + 1).Replace("/", "\\");
            if (File.Exists(f))
            {
                File.Delete(f);
            }
            File.Move("C:\\Temp\\" + ze.Replace("/", "\\"), f);
        }

        public frmMain()
        {
            InitializeComponent();
            valid = false;
            strGamePath = Properties.Settings.Default.Path;
            txtGamePath.Text = strGamePath;
            if (!Directory.Exists(strGamePath + "\\Content"))
            {
                pnlDrop.BackgroundImage = pnlDrop.BackgroundImage = SE_Modz_Installer.Properties.Resources.disabled;
                lblStatus.Text = "It seems you have selected an invalid folder. Please try again";
            }
            else
            {
                pnlDrop.Enabled = true;
                pnlDrop.BackgroundImage = pnlDrop.BackgroundImage = SE_Modz_Installer.Properties.Resources.draghere;
                lblStatus.Text = "Install path set. Drag a zipped block file to the colored area.";
            }
            ckbUpdate.Checked = Properties.Settings.Default.AutoUpdate;
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
                    lblStatus.Text = "Unable to auto-update. " + ide.Message;
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
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            string[] FileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            try
            {
                zf = new ZipFile(FileList[0]);
            }
            catch
            {
                zf = null;
            }
            if (zf != null)
            {
                foreach (string ze in zf.EntryFileNames)
                {
                    if (ze.ToLower().Contains("definition.xml"))
                    {
                        valid = true;
                    }
                }
            }
            else
            {
                lblStatus.Text = "The file appears to be incompatible with this installer.";
            }
            if (valid)
            {
                lblStatus.Text = zf.Info;
            }
            else
            {
                lblStatus.Text = "The file appears to be incompatible with this installer.";
            }
        }

        private void txtGamePath_MouseClick(object sender, MouseEventArgs e)
        {
            FolderBrowserDialog fbdGamePath = new FolderBrowserDialog();
            fbdGamePath.RootFolder = System.Environment.SpecialFolder.DesktopDirectory;
            fbdGamePath.ShowDialog();
            if (fbdGamePath.SelectedPath != "" && Directory.Exists(fbdGamePath.SelectedPath + "\\Content"))
            {
                pnlDrop.Enabled = true;
                pnlDrop.BackgroundImage = SE_Modz_Installer.Properties.Resources.draghere;
                txtGamePath.Text = fbdGamePath.SelectedPath;
                strGamePath = txtGamePath.Text;
                lblStatus.Text = "Install path set. Drag a zipped block file to the colored area.";
                Properties.Settings.Default.Path = strGamePath;
            }
            else if (fbdGamePath.SelectedPath != "" && !Directory.Exists(fbdGamePath.SelectedPath + "\\Content"))
            {
                lblStatus.Text = "It seems you have selected an invalid folder. Please try again";
            }
        }

        private void lnkSEMForum_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.se-modz.com/forum");
        }

        private void pbxIcon_MouseClick(object sender, MouseEventArgs e)
        {
            Process.Start("http://www.se-modz.com");
        }

        private void lnkChangeLog_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.se-modz.com/semi/install/Changelog.txt");
        }

        private void ckbUpdate_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.AutoUpdate = ckbUpdate.Checked;
        }
        void frmMain_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void pnlDrop_DragDrop(object sender, DragEventArgs e)
        {
            if (valid)
            {
                if (zf.Info != "")
                {
                    lbxContents.Items.Clear();
                    lblStatus.Text = zf.Info;
                    string s = zf.Name.Substring(zf.Name.LastIndexOf("\\") + 1);
                    s = s.Substring(0, s.Length - 4);
                    zf.ExtractAll("C:\\Temp", ExtractExistingFileAction.OverwriteSilently);
                    foreach (string ze in zf.EntryFileNames)
                    {
                        lbxContents.Items.Add(ze);
                        if (ze.ToLower().Contains("definition.xml"))
                        {
                            XmlDocument xmdDesc = new XmlDocument();
                            xmdDesc.Load("C:\\Temp\\" + ze.Replace("/", "\\"));
                            XmlNode xndDef = xmdDesc.GetElementsByTagName("Definition").Item(0);
                            XmlDocument xmdCubeBlocks = new XmlDocument();
                            System.IO.File.Copy(strGamePath + "\\Content\\Data\\CubeBlocks.sbc", strGamePath + "\\Content\\Data\\CubeBlocks_backup" + System.DateTime.Now.ToFileTimeUtc() + ".sbc", true);
                            xmdCubeBlocks.Load(strGamePath + "\\Content\\Data\\CubeBlocks.sbc");
                            XmlNode xndImport = xmdCubeBlocks.ImportNode(xndDef, true);
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
                    if (Directory.Exists("C:\\Temp\\" + zf[0].FileName.Replace("/", "")))
                    {
                        Directory.Delete("C:\\Temp\\" + zf[0].FileName.Replace("/", ""), true);
                    }
                }
                lblStatus.Text = zf.Info;
            }
            else
            {
                lblStatus.Text = "Unable to install, the archive is invalid.";
            }
            valid = false;
        }

    }
}
