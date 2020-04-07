﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Band_Organizer
{
    public partial class Band_Organizer : Form
    {
        public Band_Organizer()
        {
            InitializeComponent();
            //BandAlbumTrackDB.DropDatabase();        // FOR TESTING ONLY. Comment this line out to not drop existing database
            BandAlbumTrackDB.CreateDatabase();
            BandAlbumTrackDB.CreateTables();
            FillBandListBox();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            // closes the form
            this.Close();
        }

        private void btnAddBand_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsPresent(txtBandName, "Band Name") && 
                    CheckIfInList(txtBandName, lbBandList))
                {
                    // the band name is added to the listbox 
                    // then clears the textbox and moves the focus to the album name textbox

                    Band newBand = new Band { BandName = txtBandName.Text };
                    BandAlbumTrackDB.InsertBandName(newBand);

                    FillBandListBox();

                    txtBandName.Clear();
                    txtAlbumName.Focus();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" +
                    ex.GetType().ToString() + "\n" +
                    ex.StackTrace, "Exception");
            }
        }

        private void btnAddAlbum_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsPresent(txtAlbumName, "Album Name")           && 
                    CheckIfSelected(lbBandList, lblBandName.Text)   && 
                    CheckIfInList(txtAlbumName, lbAlbumList))
                {
                    // album name is added to the listbox 
                    // then the textbox is cleared and focus is moved to the track name textbox
                    
                    Album newAlbum = new Album { 
                        AlbumTitle = txtAlbumName.Text, 
                        ReleaseDate = dtReleaseDate.Value.ToLocalTime()
                    };

                    string bandName = lbBandList.SelectedItem.ToString().Trim();
                    BandAlbumTrackDB.InsertAlbumName(newAlbum, bandName);
                    List<string> albumList = BandAlbumTrackDB.FetchAlbumData(bandName);

                    FillListBox(albumList, lbAlbumList);

                    txtAlbumName.Clear();
                    dtReleaseDate.ResetText();
                    txtTrackName.Focus();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" +
                    ex.GetType().ToString() + "\n" +
                    ex.StackTrace, "Exception");
            }
        }

        private void btnAddTrack_Click(object sender, EventArgs e)
        {
            try
            {
                if (IsPresent(txtTrackName, "Track Name")               && 
                    CheckIfSelected(lbAlbumList, lblAlbumName.Text)     && 
                    CheckIfInList(txtTrackName, lbTrackList))
                {
                    // track name is added to the listbox then the textbox is cleared

                    Tracks newTrack = new Tracks { TrackTitle = txtTrackName.Text };
                    string albumName = lbAlbumList.SelectedItem.ToString().Trim();

                    BandAlbumTrackDB.InsertTrackName(newTrack, albumName);
                    List<string> trackList = BandAlbumTrackDB.FetchTrackData(albumName);
                    FillListBox(trackList, lbTrackList);

                    txtTrackName.Clear();
                    txtTrackName.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" +
                    ex.GetType().ToString() + "\n" +
                    ex.StackTrace, "Exception");
            }
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            if (ClearAllData())
            {
                BandAlbumTrackDB.ClearAllData();

                // clears all data from textboxes and listboxes
                txtBandName.Clear();
                txtAlbumName.Clear();
                txtTrackName.Clear();

                dtReleaseDate.ResetText();

                lbBandList.Items.Clear();
                lbAlbumList.Items.Clear();
                lbTrackList.Items.Clear();
            }
        }

        private bool IsPresent(TextBox textBox, string name)
        {
            // checks if a textbox is empty, notifies user if false
            if (textBox.Text == "")
            {
                MessageBox.Show(name + " is a required field.", 
                    "Entry Error");
                textBox.Focus();
                return false;
            }
            return true;
        }

        private bool ClearAllData()
        {
            // warns user when attempting to clear all data
            string message = "Are you sure you want to clear all data?";

            DialogResult button =
                MessageBox.Show(message, "Warning",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Warning);

            if (button == DialogResult.Yes)
                return true;
            else
                return false;
        }

        private bool CheckIfSelected(ListBox listBox, string name)
        {
            // checks to make sure a band or album is selected before entering more input
            if (listBox.SelectedIndex == -1)
            {
                MessageBox.Show("You must make a selection from " + name, "Entry Error");
                return false;
            }
            else
                return true;
        }

        private void FillBandListBox()
        {
            List<string> bandList = BandAlbumTrackDB.FetchBandData();
            FillListBox(bandList, lbBandList);
        }

        private void lbBandList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbBandList.SelectedItem == null)
                MessageBox.Show("whoops");
            else
            {
                string bandName = lbBandList.SelectedItem.ToString().Trim();
                List<string> albumList = BandAlbumTrackDB.FetchAlbumData(bandName);
                FillListBox(albumList, lbAlbumList, lbTrackList);
            }
        }

        private void lbAlbumList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbAlbumList.SelectedItem == null)
                MessageBox.Show("whoops");
            else
            {
                string albumName = lbAlbumList.SelectedItem.ToString().Trim();
                List<string> trackList = BandAlbumTrackDB.FetchTrackData(albumName);
                FillListBox(trackList, lbTrackList);
            }
        }

        private bool CheckIfInList(TextBox textBox, ListBox listBox)
        {
            if (listBox.Items.Contains(textBox.Text))
            {
                MessageBox.Show("This already exists in the database.",
                    "Entry Error");
                textBox.Focus();
                return false;
            }
            return true;
        }

        private void FillListBox(List<string> list, ListBox listBox, 
                                 ListBox secondListBox = default(ListBox))
        {
            listBox.HorizontalScrollbar = true;
            listBox.Items.Clear();
            if (secondListBox != null)
                secondListBox.Items.Clear();

            foreach (string item in list)
                listBox.Items.Add(item);
        }
    }
}
