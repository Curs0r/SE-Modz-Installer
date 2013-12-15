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
        public bool valid, uda;
        System.Timers.Timer tmrUpdate = new System.Timers.Timer(3600000);
        string strTempDir = Application.UserAppDataPath + "\\Temp\\";
        StreamWriter log = new StreamWriter(Application.UserAppDataPath + "semi-log" + DateTime.Today.ToFileTimeUtc() + ".txt", true);
        private void BadFile()
        {
            pnlDrop.BackgroundImage = pnlDrop.BackgroundImage = SE_Modz_Installer.Properties.Resources.invfile;
            lblStatus.Text = "The file appears to be incompatible with this installer.";
        }

        private void BadFolder()
        {
            pnlDrop.BackgroundImage = pnlDrop.BackgroundImage = SE_Modz_Installer.Properties.Resources.invpath;
            lblStatus.Text = "It seems you have selected an invalid folder. Please try again";
        }

        private void FMove(string ze)
        {
            /*
             * First we need the path and filename string of the file's destination.
             * We concatenate the game's path, with the Content subfolder, and then
             * add the archive's path structure after stripping off the archive name.
             */
            string f = strGamePath + "\\Content\\" + ze.Substring(ze.IndexOf("/") + 1).Replace("/", "\\");
            /*
             * Now we need to check if that file exists, and delete the old version
             * if it does.
             */
            if (File.Exists(f))
            {
                File.Delete(f);
            }
            else
            {
                /*
                 * Before moving the file, we must ensure the destination directory exists!
                 * As in theory, we have already validated the game's path, we need only
                 * verify subdirectories of the game's Content directory. If the file had
                 * hit for the exists check then we would know the folders already exist.
                */
                string d = strGamePath + "\\Content\\";
                foreach (string s in ze.Substring(ze.IndexOf("/") + 1, ze.LastIndexOf("/") - 1).Split('/'))
                {
                    d += s;
                    if (!Directory.Exists(d))
                    {
                        Directory.CreateDirectory(d);
                    }
                    d += "\\";
                }
            }
            /*
             * Now we can move the file to its destination, and update the status bar.
             */
            File.Move(strTempDir + ze.Replace("/", "\\"), f);
            lbxContents.Items.Add(ze.Substring(ze.LastIndexOf("/") + 1) + " copied to " + ze.Substring(ze.IndexOf("/") + 1).Replace("/", "\\") + ".");
        }

        private void CheckPath()
        {
            if (!Directory.Exists(strGamePath + "\\Content"))
            {
                BadFolder();
            }
            else
            {
                pnlDrop.Enabled = true;
                pnlDrop.BackgroundImage = pnlDrop.BackgroundImage = SE_Modz_Installer.Properties.Resources.draghere;
                lblStatus.Text = "Drag a zipped block file to the colored area.";
            }
        }

        private void UpdateCheck()
        {
            UpdateCheckInfo info = null;
            ApplicationDeployment ad;
            try
            {
                ad = ApplicationDeployment.CurrentDeployment;
            }
            catch (InvalidDeploymentException ide)
            {
                log.WriteLine(DateTime.Now.ToLocalTime().ToString() + ": Unable to auto-update. " + ide.Message);
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
                    log.WriteLine(DateTime.Now.ToLocalTime().ToString() + ": There was an error attempting to retrieve the file. \n\nPlease check your network connection, or try again later. Error: " + dde.Message);
                    return;
                }
                catch (InvalidDeploymentException ide)
                {
                    log.WriteLine(DateTime.Now.ToLocalTime().ToString() + ": Please reinstall. Error: " + ide.Message);
                    return;
                }
                catch (InvalidOperationException ioe)
                {
                    log.WriteLine(DateTime.Now.ToLocalTime().ToString() + ": The application cannot be updated." + ioe.Message);
                    return;
                }
                if (info.UpdateAvailable)
                {
                    uda = true;
                    if (!info.IsUpdateRequired)
                    {
                        DialogResult dr = MessageBox.Show("A new update is availabe. Do you wish to update?", "New Update", MessageBoxButtons.OKCancel);
                        if (dr == DialogResult.Cancel)
                        {
                            uda = false;
                            tmrUpdate.Enabled = false;
                        }
                        else
                        {
                            UpdateMe();
                        }
                    }
                    else
                    {
                        MessageBox.Show("A very important update has been released that will convert you from the installed " +
                            "version to version " + info.MinimumRequiredVersion.ToString() +
                            ". The application will update and restart.",
                            "Important Update Available", MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        UpdateMe();
                    }
                }
            }
        }

        private void UpdateMe()
        {
            try
            {
                ApplicationDeployment.CurrentDeployment.UpdateAsync();
                ApplicationDeployment.CurrentDeployment.UpdateCompleted += new System.ComponentModel.AsyncCompletedEventHandler(CurrentDeployment_UpdateCompleted);
            }
            catch (DeploymentDownloadException dde)
            {
                MessageBox.Show("Unable to update. \n\nPlease check your network connection, or try again later. Error: " + dde);
                return;
            }
        }

        void CurrentDeployment_UpdateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            MessageBox.Show("Update complete. The Application will now restart.");
            Application.Restart();
        }

        public frmMain()
        {
            InitializeComponent();
            log.AutoFlush = true;
            valid = false;
            tmrUpdate.Elapsed += new System.Timers.ElapsedEventHandler(tmrUpdate_Elapsed);
            tmrUpdate.AutoReset = true;
            strGamePath = Properties.Settings.Default.Path;
            txtGamePath.Text = strGamePath;
            CheckPath();
            if (Properties.Settings.Default.AutoUpdate)
            {
                UpdateCheck();
                if (!uda)
                {
                    tmrUpdate.Enabled = true;
                }
            }
        }

        void tmrUpdate_Elapsed(object sender, EventArgs e)
        {
            UpdateCheck();
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
                if (zf.Info != null)
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
                    BadFile();
                }
            }
            else
            {
                BadFile();
            }
            if (valid)
            {
                pnlDrop.BackgroundImage = pnlDrop.BackgroundImage = SE_Modz_Installer.Properties.Resources.okfile;
                lblStatus.Text = zf.Info;
            }
            else
            {
                BadFile();
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
                BadFolder();
            }
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
            if (ckbUpdate.Checked)
            {
                UpdateCheck();
                tmrUpdate.Enabled = true;
            }
            else
            {
                tmrUpdate.Enabled = false;
            }
        }
        void frmMain_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            log.Close();
            Properties.Settings.Default.Save();
        }

        private void pnlDrop_DragDrop(object sender, DragEventArgs e)
        {
            System.Timers.Timer tmrReset = new System.Timers.Timer(2500);
            tmrReset.Elapsed += new System.Timers.ElapsedEventHandler(tmrReset_Elapsed);
            tmrReset.AutoReset = false;
            if (valid)
            {
                if (zf.Info != "")
                {
                    lbxContents.Items.Clear();
                    lblStatus.Text = zf.Info;
                    string s = zf.Name.Substring(zf.Name.LastIndexOf("\\") + 1);
                    s = s.Substring(0, s.Length - 4);
                    zf.ExtractAll(strTempDir, ExtractExistingFileAction.OverwriteSilently);
                    foreach (string ze in zf.EntryFileNames)
                    {
                        if (ze.ToLower().Contains("definition.xml"))
                        {
                            XmlDocument xmdDesc = new XmlDocument();
                            xmdDesc.Load(strTempDir + ze.Replace("/", "\\"));
                            XmlNode xndDef = xmdDesc.GetElementsByTagName("Definition").Item(0);
                            XmlDocument xmdCubeBlocks = new XmlDocument();
                            // CubeBlocks Backup
                            DateTime dtmNow = DateTime.Now;
                            File.Copy(strGamePath + "\\Content\\Data\\CubeBlocks.sbc", strGamePath + "\\Content\\Data\\CubeBlocks_backup" + dtmNow.ToFileTimeUtc() + ".sbc", true);
                            lbxContents.Items.Add("CubeBlocks.sbc backed up to " + "Data\\CubeBlocks_backup" + dtmNow.ToFileTimeUtc() + ".sbc");
                            //

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
                                lbxContents.Items.Add("CubeBlocks.sbc Modified.");
                            }
                            else
                            {
                                xmdCubeBlocks.Save(strGamePath + "\\Content\\Data\\CubeBlocks.sbc");
                                lbxContents.Items.Add("CubeBlocks.sbc updated.");
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
                    if (Directory.Exists(strTempDir + zf[0].FileName.Replace("/", "")))
                    {
                        Directory.Delete(strTempDir + zf[0].FileName.Replace("/", ""), true);
                        lbxContents.Items.Add("Temporary files removed.");
                    }
                }
                lbxContents.Items.Add("Installation complete.");
                lblStatus.Text = "Installed " + zf.Info;
                tmrReset.Enabled = true;
            }
            else
            {
                lblStatus.Text = "Unable to install, the archive is invalid.";
                tmrReset.Enabled = true;
            }
            valid = false;
        }

        void tmrReset_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            CheckPath();
        }

        private void pnlDrop_DragLeave(object sender, EventArgs e)
        {
            CheckPath();
            valid = false;
        }

        private void lnkAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("SE-Modz Block Installer by Curs0r\n\nThis software is distrubuted under the terms of the GPL v3 license." +
            "\n\nArtwork by Suge." +
            "\n\nVisit http://curs0r.github.io/SE-Modz-Installer to view the source and license for this program.", "About SE-Modz Block Installer");
        }

        private void pbxIcon_MouseEnter(object sender, EventArgs e)
        {
            lblStatus.Text = "Click to open http://www.se-modz.com in your browser.";
        }

        private void pbxIcon_MouseLeave(object sender, EventArgs e)
        {
            CheckPath();
        }

        private void lnkSEMForum_MouseEnter(object sender, EventArgs e)
        {
            lblStatus.Text = "Click to open http://www.se-modz.com/forum in your browser.";
        }

        private void lnkSEMForum_MouseLeave(object sender, EventArgs e)
        {
            CheckPath();
        }

        private void txtGamePath_MouseEnter(object sender, EventArgs e)
        {
            lblStatus.Text = "Click to set the path to your Space Engineers game directory.";
        }

        private void txtGamePath_MouseLeave(object sender, EventArgs e)
        {
            CheckPath();
        }

        private void lnkChangeLog_MouseEnter(object sender, EventArgs e)
        {
            lblStatus.Text = "Click to view this program's change log in your browser.";
        }

        private void lnkChangeLog_MouseLeave(object sender, EventArgs e)
        {
            CheckPath();
        }

        private void lnkAbout_MouseEnter(object sender, EventArgs e)
        {
            lblStatus.Text = "Click to learn more about his program.";
        }

        private void lnkAbout_MouseLeave(object sender, EventArgs e)
        {
            CheckPath();
        }

        private void ckbUpdate_MouseEnter(object sender, EventArgs e)
        {
            lblStatus.Text = "Click to toggle automatic checks for updates.";
        }

        private void ckbUpdate_MouseLeave(object sender, EventArgs e)
        {
            CheckPath();
        }

    }
}
