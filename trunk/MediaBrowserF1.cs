﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using XBMControl.Language;
using XBMControl.Properties;

namespace XBMControl
{
    public partial class MediaBrowserF1 : Form
    {
        MainForm parent;
        TreeNode sharesNode;
        string currentViewMode = "files";

        public MediaBrowserF1(MainForm parentForm)
        {
            parent = parentForm;
            InitializeComponent();
            this.PopulateShareBrowser();
            this.Owner = parent;
        }

        public void PopulateShareBrowser()
        {
            this.TestConnectivity();

            string[] aShares     = parent.XBMC.Media.GetShares(cbShareType.Text);
            string[] aSharePaths = parent.XBMC.Media.GetShares(cbShareType.Text, true);
            tvMediaShares.Nodes.Clear();

            if (aShares != null)
            {
                for (int x = 1; x < aShares.Length; x++)
                {
                    if (aShares[x] != null)
                    {
                        sharesNode = new TreeNode();
                        sharesNode.Name = aShares[x];
                        sharesNode.Text = aShares[x];
                        sharesNode.ToolTipText = aSharePaths[x];
                        tvMediaShares.Nodes.Add(sharesNode);
                        tvMediaShares.Nodes[x - 1].ImageIndex = 0;
                    }
                }
            }

        }

        private void ExpandSharedDirectory()
        {
            this.TestConnectivity();

            if (tvMediaShares.SelectedNode.GetNodeCount(false) == 0)
            {
                string[] aDirectoryContentPaths = parent.XBMC.Media.GetDirectoryContentPaths(tvMediaShares.SelectedNode.ToolTipText, "/");
                string[] aDirectoryContentNames = parent.XBMC.Media.GetDirectoryContentNames(tvMediaShares.SelectedNode.ToolTipText, "/");

                if (aDirectoryContentPaths != null)
                {
                    for (int x = 1; x < aDirectoryContentPaths.Length; x++)
                    {
                        if (aDirectoryContentPaths[x] != null)
                        {
                            sharesNode = new TreeNode();
                            sharesNode.Name = aDirectoryContentNames[x];
                            sharesNode.Text = aDirectoryContentNames[x];
                            sharesNode.ToolTipText = aDirectoryContentPaths[x];
                            tvMediaShares.SelectedNode.Nodes.Add(sharesNode);
                            tvMediaShares.SelectedNode.Nodes[x - 1].ImageIndex = 0;
                        }
                    }

                    tvMediaShares.SelectedNode.Expand();
                }
            }
        }

        private void ExpandArtistDirectory(string artist)
        {
            this.TestConnectivity();

            if (tvMediaShares.SelectedNode.GetNodeCount(false) == 0)
            { 
                string[] aAlbums = parent.XBMC.Database.GetAlbumsByArtist(artist);

                if (aAlbums != null)
                {
                    if (aAlbums.Length == 1)
                        this.PopulateSongBrowser();
                    else
                    {
                        for (int x = 1; x < aAlbums.Length; x++)
                        {
                            if (aAlbums.Length > 1)
                            {
                                sharesNode = new TreeNode();
                                sharesNode.Name = aAlbums[x];
                                sharesNode.Text = aAlbums[x];
                                tvMediaShares.SelectedNode.Nodes.Add(sharesNode);
                                tvMediaShares.SelectedNode.Nodes[x - 1].ImageIndex = 0;

                                tvMediaShares.SelectedNode.Expand();
                            }
                        }
                    }
                }
            }
        }

        private void PopulateFileBrowser()
        {
            this.TestConnectivity();
            this.lvDirectoryContent.Items.Clear();

            string[] aDirectoryContentPaths = parent.XBMC.Media.GetDirectoryContentPaths(tvMediaShares.SelectedNode.ToolTipText, "[" + cbShareType.Text + "]");
            string[] aDirectoryContentNames = parent.XBMC.Media.GetDirectoryContentNames(tvMediaShares.SelectedNode.ToolTipText, "[" + cbShareType.Text + "]");
            int imgIndex = 0;

            if (cbShareType.Text == "music")
                imgIndex = 1;
            else if (cbShareType.Text == "pictures")
                imgIndex = 2;
            else if (cbShareType.Text == "video")
                imgIndex = 3;
            else
                imgIndex = 4;

            if (aDirectoryContentNames != null)
            {
                for (int x = 1; x < aDirectoryContentNames.Length; x++)
                {
                    if (aDirectoryContentNames[x] != null)
                    {
                        lvDirectoryContent.Items.Add(aDirectoryContentNames[x]);
                        lvDirectoryContent.Items[x - 1].ImageIndex = imgIndex;
                        lvDirectoryContent.Items[x - 1].ToolTipText = aDirectoryContentPaths[x];
                    }
                }
            }
        }

        private void PopulateSongBrowser()
        {
            this.TestConnectivity();
            this.lvDirectoryContent.Items.Clear();
            string[] aTitlesByAlbum = null;
            string[] aPathsByAlbum = null;
            string[] aTitlesByArtist = null;
            string[] aPathsByArtist = null;

            if (tvMediaShares.SelectedNode.Parent != null)
            {
                aTitlesByAlbum = parent.XBMC.Database.GetTitlesByAlbum(tvMediaShares.SelectedNode.Parent.Text.ToString(), tvMediaShares.SelectedNode.Text.ToString());
                aPathsByAlbum = parent.XBMC.Database.GetPathsByAlbum(tvMediaShares.SelectedNode.Parent.Text.ToString(), tvMediaShares.SelectedNode.Text.ToString());
            }
            else
            {
                aTitlesByAlbum = parent.XBMC.Database.GetTitlesByAlbum(tvMediaShares.SelectedNode.Text.ToString());
                aPathsByAlbum = parent.XBMC.Database.GetPathsByAlbum(tvMediaShares.SelectedNode.Text.ToString());
            }

            if (aTitlesByAlbum != null)
            {
                for (int x = 1; x < aTitlesByAlbum.Length; x++)
                {
                    if (aTitlesByAlbum[x] != null)
                    {
                        lvDirectoryContent.Items.Add(aTitlesByAlbum[x]);
                        lvDirectoryContent.Items[x - 1].ImageIndex = 1;
                        lvDirectoryContent.Items[x - 1].ToolTipText = aPathsByAlbum[x];
                    }
                }
            }
            else
            {
                aTitlesByArtist = parent.XBMC.Database.GetTitlesByArtist(tvMediaShares.SelectedNode.Text.ToString());

                for (int x = 1; x < aTitlesByArtist.Length; x++)
                {
                    lvDirectoryContent.Items.Add(aTitlesByArtist[x]);
                    lvDirectoryContent.Items[x - 1].ImageIndex = 1;
                }
            }
        }

        private void TestConnectivity()
        {
            if (!parent.XBMC.Status.IsConnected())
                this.Dispose();
        }

        private void AddDirectoryContentToPlaylist(bool play, bool enqueue, bool recursive)
        {
            this.TestConnectivity();
            if (play) parent.XBMC.Playlist.Clear();
            parent.XBMC.Playlist.AddDirectoryContent(tvMediaShares.SelectedNode.ToolTipText, cbShareType.Text, recursive);
            if (play) parent.XBMC.Playlist.PlaySong(0);
            if (Settings.Default.playlistOpened) parent.Playlist.RefreshPlaylist();
        }

        private void AddFilesToPlaylist(bool play)
        {
            this.TestConnectivity();
            if (play) parent.XBMC.Playlist.Clear();

            foreach (ListViewItem item in lvDirectoryContent.SelectedItems)
                parent.XBMC.Playlist.AddFilesToPlaylist(item.ToolTipText);

            if (play) parent.XBMC.Playlist.PlaySong(0);
            if (Settings.Default.playlistOpened) parent.Playlist.RefreshPlaylist();
        }

        private void AddFilesToPlaylist()
        {
            this.AddFilesToPlaylist(false);
        }

        private void ChangeBrowserView(int browserView)
        {
            if (browserView == 0)
            {
                currentViewMode = "files";
                tbSearchLibrary.Visible = false;
            }
            else if(browserView == 1)
            {
                currentViewMode = "search";
                tbSearchLibrary.Visible = true;
                tbSearchLibrary.Select();
            }
            else if (browserView == 2)
            {
                currentViewMode = "artists";
                this.GetArtist();
            }
        }

        private void GetArtist()
        {
            this.tvMediaShares.Nodes.Clear();
            string condition = (this.tbSearchLibrary.Text == "") ? null : this.tbSearchLibrary.Text;
            string[] aArtist = parent.XBMC.Database.GetArtist(condition);

            if (aArtist != null)
            {
                for (int x = 1; x < aArtist.Length; x++)
                {
                    sharesNode = new TreeNode();
                    sharesNode.Name = aArtist[x];
                    sharesNode.Text = aArtist[x];
                    tvMediaShares.Nodes.Add(sharesNode);
                    tvMediaShares.Nodes[x - 1].ImageIndex = 0;
                }
            }
        }

        private void cbShareType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.PopulateShareBrowser();
        }

        private void tvMediaShares_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (currentViewMode == "files")
                this.PopulateFileBrowser();
            else if (currentViewMode == "artists" || currentViewMode == "albums")
                this.PopulateSongBrowser();
        }

        private void lvDirectoryContent_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            parent.XBMC.Controls.PlayMedia(lvDirectoryContent.Items[lvDirectoryContent.FocusedItem.Index].ToolTipText);
        }

        private void MediaBrowserF1_FormClosing(object sender, FormClosingEventArgs e)
        {
            parent.shareBrowserOpened = false;
            this.Dispose();
        }

        private void tvMediaShares_MouseDown(object sender, MouseEventArgs e)
        {
            tvMediaShares.SelectedNode = tvMediaShares.GetNodeAt(e.X, e.Y);

            if (currentViewMode == "artists")
                this.ExpandArtistDirectory(tvMediaShares.SelectedNode.Text.ToString());
            else if (e.Button != MouseButtons.Right && tvMediaShares.SelectedNode != null && currentViewMode == "files")
                this.ExpandSharedDirectory();
        }

        private void tsiCollapseAll_Click(object sender, EventArgs e)
        {
            tvMediaShares.CollapseAll();
        }

        //START Playlis controls
        private void tsiPlayRecursive_Click(object sender, EventArgs e)
        {
            this.AddDirectoryContentToPlaylist(true, false, true);
        }

        private void tsiEnqueueRecursive_Click(object sender, EventArgs e)
        {
            this.AddDirectoryContentToPlaylist(false, true, true);
        }

        private void tsiEnqueueFiles_Click(object sender, EventArgs e)
        {
            this.AddFilesToPlaylist();
        }

        private void cmsFiles_Click(object sender, EventArgs e)
        {
            this.AddFilesToPlaylist();
            if (Settings.Default.playlistOpened)
                parent.Playlist.RefreshPlaylist();
        }

        private void tsiPlayFiles_Click(object sender, EventArgs e)
        {
            this.AddFilesToPlaylist(true);
        }
        //END Playlist controls

        private void cbBrowseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.ChangeBrowserView(cbBrowseType.SelectedIndex);
        }
    }
}