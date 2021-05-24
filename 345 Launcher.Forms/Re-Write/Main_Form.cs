﻿using _345_Launcher.Re_Write.SettingsForms;
using CmlLib.Core;
using CmlLib.Core.Version;
using System;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace _345_Launcher.Re_Write
{
    public partial class Main_Form : Form
    {
        public Main_Form()
        {
            InitializeComponent();
            cbVersion.DropDownHeight = 200;
        }

        private const int cGrip = 16;
        private const int cCaption = 32;
        MinecraftPath MinecraftPath;
        MVersionCollection Versions;

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int conn, int val);

        public string LabelText
        {
            get
            {
                //Loling textbox value is comes here
                return this.username_lbl.Text;
            }
            set
            {
                this.username_lbl.Text = value;
            }
        }

        protected override void WndProc(ref Message m)
        {
            // For draggeble form
            if (m.Msg == 0x84)
            {
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;
                    return;
                }
                if (pos.X > this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17;
                    return;
                }
            }
            base.WndProc(ref m);
        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void refreshVersions(string showVersion)
        {
            // Gets Minecraft Versions

            cbVersion.Items.Clear();
            int Out;
            if (InternetGetConnectedState(out Out, 0) == true)
            {
                //if user have internet connections
                var th = new Thread(new ThreadStart(delegate
                {
                    Versions = new MVersionLoader().GetVersionMetadatas(MinecraftPath);

                    Invoke(new Action(() =>
                    {
                        bool showVersionExist = false;
                        //Snapshot checkbox
                        if (snapbox.Checked == true)
                        {
                            foreach (var item in Versions)
                            {
                                if (item.IsLocalVersion || item.MType == MVersionType.Snapshot)
                                {
                                    showVersionExist = true;
                                    cbVersion.Items.Add(item.Name);
                                }
                                if (showVersion != null || showVersionExist)
                                    cbVersion.Text = showVersion;

                            }
                        }
                        else
                        {
                            foreach (var item in Versions)
                            {
                                if (item.IsLocalVersion || item.MType == MVersionType.Release)
                                {
                                    showVersionExist = true;
                                    cbVersion.Items.Add(item.Name);
                                }
                                if (showVersion != null || showVersionExist)
                                    cbVersion.Text = showVersion;

                            }
                        }

                    }));
                }));
                th.Start();
            }
            else
            {
                //if user don't have a internet connection

                var th = new Thread(new ThreadStart(delegate
                {
                    Versions = new MVersionLoader().GetVersionMetadatasFromLocal(MinecraftPath);

                    Invoke(new Action(() =>
                    {
                        bool showVersionExist = false;

                        foreach (var item in Versions)
                        {
                            if (item.IsLocalVersion)
                            {
                                MVersion version = Versions.GetVersion(item);
                                showVersionExist = true;
                                cbVersion.Items.Add(item.Name);
                            }
                            if (showVersion != null || showVersionExist)
                                cbVersion.Text = showVersion;
                        }
                    }));
                }));
                th.Start();
            }

        }

        private void InitializeLauncher(MinecraftPath path)
        {
            //initilizes mc path
            MinecraftPath = path;
            refreshVersions(null);
        }

        private void Main_Form_Load(object sender, EventArgs e)
        {
            int Out;
            if (InternetGetConnectedState(out Out, 0) == true)
            {
                webBrowser1.Navigate("https://launcher.mehmetali345.xyz/launcher.html");
                webBrowser1.ScriptErrorsSuppressed = true;
                webBrowser1.IsWebBrowserContextMenuEnabled = false;
                UserHead();
            }
            else
            {
                userhead.Image = Properties.Resources.steve;
                noint_picturebox.Visible = true;
                noint_picturebox.Image = Properties.Resources.nointbg;
            }
            if (Properties.Settings.Default.McPath != null)
            {
                var custompath = new MinecraftPath(Properties.Settings.Default.McPath);
                InitializeLauncher(custompath);
            }
            else
            {
                var defaultPath = new MinecraftPath(MinecraftPath.GetOSDefaultPath());
                InitializeLauncher(defaultPath);
            }
        }

        private void snapbox_CheckedChanged(object sender, EventArgs e)
        {
            refreshVersions(null);
        }

        private void UserHead()
        {
            var request = WebRequest.Create("https://minotar.net/helm/" + username_lbl.Text);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                userhead.Image = Bitmap.FromStream(stream);
            }
        }

        private void set_button_Click(object sender, EventArgs e)
        {
            if (pnl_settings.Visible == false)
                guna2Transition1.ShowSync(pnl_settings, false);
            else
                pnl_settings.Visible = false;
        }

        private void info_button_Click(object sender, EventArgs e)
        {
            if (pnl_settings_show.Visible == false)
            {
                Settings_Info frm = new Settings_Info() { TopLevel = false, TopMost = true };

                this.pnl_settings_show.Controls.Add(frm);

                frm.Show();

                webBrowser1.Width = 737;

                guna2Transition1.ShowSync(pnl_settings_show, false);

            }
            else
            {
                pnl_settings_show.Visible = false;

                pnl_settings_show.Controls.Clear();

            }


        }

        private void btn_Launch_Click(object sender, EventArgs e)
        {

        }
    }
}