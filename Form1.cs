//This file is part of Hexthingy.

//    Hexthingy is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.

//    Hexthingy is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

//    You should have received a copy of the GNU General Public License
//    along with Foobar; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

// Hexthingy is copyright 2006 by David Clark


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Hexgridmaker {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        // Here's some boilerplate code to try to prevent obviously incorrect
        // input from ending up in the entryboxes.

        private bool nonNumberEntered = false;

        private void textbox_keydown(object sender, KeyEventArgs e) {
            nonNumberEntered = false;
            if (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9) {
                if (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9) {
                    if (e.KeyCode != Keys.Back && 
                        e.KeyCode != Keys.Decimal &&
                        e.KeyCode != Keys.OemPeriod  &&
                        e.KeyCode != Keys.OemMinus ) {
                            nonNumberEntered = true;
                    }
                }
            }
        }

        private void textbox_keypress(object sender, KeyPressEventArgs e)
        {
            if (nonNumberEntered == true)
            {
                e.Handled = true;
            }              
        }

        // End boilerplate code.



        private void Form1_Load(object sender, EventArgs e) {
            //  Populate the controls with reasonable default values.
            widthComboBox.SelectedIndex = 3;
            heightComboBox.SelectedIndex = 3;
            resolutionComboBox.SelectedIndex = 3;
            textBox1.Text = "0.00";
            textBox2.Text = "0.00";
            textBox3.Text = "100.00";
            textBox4.Text = "100.00";
            orientation_entry.Text = "0.0";
            resolution_entry.Text = "10.0";
        }

        private void cancel_button_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        private void save_button_Click(object sender, EventArgs e)
        {
            // The meat of the program's ui.  Parse the input, get the filename
            // to save as, generate the Map object, generate the grid, save it to disk.

            double centerlong;
            double centerlat;
            double mapwidth;
            double mapheight;
            double hexsize;
            double rotation_angle;

            try
            {
                // For each one, we convert whatever the user has typed into
                // degrees.  Then we sanity-check that value, then we
                // convert it to radians
                centerlong = textboxToDegrees(textBox2, "deg");
                validate_long(centerlong);
                centerlong = degtorad(centerlong);
                
                centerlat = textboxToDegrees(textBox1, "deg");
                validate_lat(centerlat);
                centerlat = degtorad(centerlat);

                mapwidth = textboxToDegrees(textBox3, widthComboBox.Text);
                validate_size(mapwidth);
                mapwidth = degtorad(mapwidth);

                mapheight = textboxToDegrees(textBox4, heightComboBox.Text);
                validate_widthheight(mapheight);
                mapheight = degtorad(mapheight);

                hexsize = textboxToDegrees(resolution_entry, resolutionComboBox.Text);
                validate_size(hexsize);
                hexsize = degtorad(hexsize);

                rotation_angle = textboxToDegrees(orientation_entry, "deg");
                validate_angle(rotation_angle);
                rotation_angle = degtorad(rotation_angle);
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Improper input. Click Help button for more info.",
                    "Data Entry Error", MessageBoxButtons.OK);
                return;
            }
            catch (FormatException)
            {
                MessageBox.Show("Improper input. Click Help button for more info.",
                    "Data Entry Error", MessageBoxButtons.OK);
                return;
            }

            
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Google Earth .kml|*.kml";
            saveDialog.Title = "Save .kml File";
            saveDialog.AddExtension = true;
            saveDialog.ShowDialog();
            if (saveDialog.FileName != "")
            {
                StreamWriter kml;
                try
                {
                    kml = new StreamWriter(saveDialog.FileName, false);
                }
                catch
                {
                    MessageBox.Show("Invalid filename.  Grid not written to disk.",
                        "Save Error", MessageBoxButtons.OK);
                    return;
                }
                Map map = new Map(centerlong, centerlat, mapwidth, mapheight, hexsize, rotation_angle);
                map.create_hex_grid();
                map.write(kml);
                kml.Close();
                Application.Exit();
            }
        }

        private double textboxToDegrees(TextBox textbox, string units)
        {
            double value = Convert.ToDouble(textbox.Text);

            switch (units)
            {
                // Conversion routines to turn the input into Degree Equivalents
                // A Degree Equivalent is 60 nautical miles, and is the internal
                // unit used by hexthingy.
                case "feet":
                    return (value / 364566.929);
                case "yards":
                    return (value / 121522.31);
                case "metres":
                    return (value / 111120.00);
                case "km":
                    return (value / 111.120);
                case "miles":
                    return (value / 69.046767);
                case "nm":
                    return (value / 60.00);
                case "deg":
                    return value;
                default:
                    throw new Exception("Invalid Unit.  Weird.");
            }

        }

        // Here we do some basic sanity-checking on the numeric values
        // entered by the user.

        private void validate_angle(double rotation_angle)
        {
            if ((rotation_angle < -360.0) | (rotation_angle > 360.0)) {
                throw new FormatException();
            }
        }

        private void validate_widthheight(double widthorheight)
        {
            if (widthorheight <= 0.00)
            {
                throw new FormatException();
            }
        }

        private void validate_size(double hexsize)
        {
            if (hexsize <= 0.00)
            {
                throw new FormatException();
            }
        }

        private void validate_lat(double centerlat)
        {
            if ((centerlat < -90.0) | (centerlat > 90.0)) {
                throw new FormatException();
            }
        }

        private void validate_long(double centerlong)
        {
            if ((centerlong > 360.0) | (centerlong < -360.0))
            {
                throw new FormatException();
            }
        }

        // utility functions needed by the form
      
        private double degtorad(double degrees)
        {
            return (degrees * Math.PI) / 180.0;
        }

        private void help_button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Welcome to Hexthingy - a program to generate hex grids "+
            "for later viewing in Google Earth. \n\n" +
            "Valid values for the entry fields are as follows:\n\n" +
            "Center longitude : -360.0 degrees to 360.0 degrees\n" +
            "Center latitude : -90.0 degrees to 90 degrees\n" +
            "Width & Height : values greater than about 10 000 km will produce "+
            "poor results.\n" +
            "Orientation : -360.0 degrees to 360 degrees\n" +
            "Resolution: Maps generated that have more than 100 000 hexes will "+
            "display slowly in Google Earth.\n\n"+
            "For more information, check the readme.txt file distributed with " +
            "this program.\n\n" +
            "Version 1.2\n" +
            "Written by David Clark (da_clark@shaw.ca)\n" +
            "Copyright 2006, 2014, distributed under the GPL",
            "Hexthingy", MessageBoxButtons.OK);     
        }

        
    }
}