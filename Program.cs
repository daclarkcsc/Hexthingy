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
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;


namespace Hexgridmaker {

    // The basic idea behind the program is this.  We draw a hexgrid on a flat plane.
    // We then orient that plane tangent to the sphere - it touches the sphere only
    // at its center (the centerlat, centerlong point).  We then project a line from
    // the corners of each hex to the globe, and calculate where that line hits
    // the surface of the sphere.  We find that point's latitude and longitude,
    // and write it to the .kml file, organized by hex.  Using Google Earth's 
    // polygon drawing functions, we draw the hex.

    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class Point {
        private double x;
        private double y;
        private double latitude;
        private double longitude;
        const double degperradian = 180.00 / Math.PI;

        public Point(double pos_x, double pos_y) {
            this.x = pos_x;
            this.y = pos_y;
        }

        private double degrees(double radians)
        {
            return radians * degperradian;
        }

        public void rotate(double angle, double offset)
        {
            // The user can specify a grid orientation, which rotates the grid by
            // a certain number of degrees. By doing this, we can generate grids
            // at any angle to north.
            double tmp_x;
            double tmp_y;
            this.x = this.x - offset;
            this.y = this.y + offset;
            tmp_x = this.x * Math.Cos(angle) - this.y * Math.Sin(angle);
            tmp_y = this.x * Math.Sin(angle) + this.y * Math.Cos(angle);
            this.x = tmp_x;
            this.y = tmp_y;
        }

        public void project(double centerlong, double centerlat) {

            // Here's where the magic happens.  Take a point on a plane that is
            // tangental to the sphere, and project it onto the surface of the
            // sphere.  Derive the latitude and longitude, and you're done.

            double p = Math.Sqrt((this.x * this.x) + (this.y * this.y));
            double c = 2 * Math.Asin(p / 2);
            double term1 = (this.y * Math.Sin(c) * Math.Cos(centerlat)) / p;
            this.latitude = Math.Asin(Math.Cos(c) * Math.Sin(centerlat) + term1);
            term1 = this.x * Math.Sin(c);
            double term2 = (p * Math.Cos(centerlat) * Math.Cos(c)) -
                (this.y * Math.Sin(centerlat) * Math.Sin(c));
            this.longitude = centerlong + Math.Atan2(term1, term2);
            this.latitude = degrees(this.latitude);
            this.longitude = degrees(this.longitude);
        }

        public void write(StreamWriter kml) {
            string text = "    " + this.longitude.ToString() + "," +
                this.latitude.ToString();
            kml.WriteLine(text);
        }
    }

    public class Hex
    {
        Point[] points = new Point[7];
        public static double size;
        public static double width;
        public static double height;
        public static double side_length;
        public static double rect_height;
        public static double rect_width;

        public Hex(Point[] points) {
            this.points = points;
        }

        public static void define(double size)
        {
            double angle = Math.PI / 6.00;
            Hex.size = size;
            Hex.width = size / 2.00;
            Hex.side_length = size / (2 * Math.Cos(angle));
            Hex.height = (size * Math.Tan(angle)) / 2.00;
            Hex.rect_height = Hex.height + Hex.side_length;
            Hex.rect_width = Hex.size;
        }

        public void write(StreamWriter kml)
        {
            kml.WriteLine("<Polygon>");
            kml.WriteLine("<tessellate>1</tessellate>");
            kml.WriteLine("<outerBoundaryIs>");
            kml.WriteLine("<LinearRing>");
            kml.WriteLine("  <coordinates>");
            foreach (Point point in this.points)
            {
                point.write(kml);
            }
            kml.WriteLine("  </coordinates>");
            kml.WriteLine("</LinearRing>");
            kml.WriteLine("</outerBoundaryIs>");
            kml.WriteLine("</Polygon>");
        }
    }

    public class Map
    {
        // The Map object is the grid.  It knows all the info the user entered for
        // it, in Degree Equivalents.  Creating the Map object doesn't actually
        // generate the grid - that's done by its own function.
        Hex[,] data;
        private double width;
        private double height;
        private double centerlong;
        private double centerlat;
        private double hexsize;
        private double rot;
        private int columns;
        private int rows;

        public Map(double centerlong, double centerlat, double width, double height,
            double hexsize, double rotation_angle)
        {
            this.width = width;
            this.height = height;
            this.centerlong = centerlong;
            this.centerlat = centerlat;
            this.hexsize = hexsize;
            Hex.define(hexsize);
            this.columns = (int)(this.width / Hex.rect_width);
            this.rows = (int)(this.height / Hex.rect_height);
            this.data = new Hex[rows, columns];
            this.rot = rotation_angle;
        }

        public void create_hex_grid()
        {
            double x_offset;
            double pos_x;
            double pos_y;

            // Draw the grid on a plane.

            for (int columnnum = 0; columnnum < this.columns; columnnum++)
            {
                for (int rownum = 0; rownum < this.rows; rownum++)
                {
                    if (rownum % 2 == 0)
                    {
                        x_offset = Hex.width;
                    }
                    else
                    {
                        x_offset = 0.00;
                    }

                    // plot the points for each hex.

                    pos_x = x_offset + (columnnum * Hex.rect_width) + Hex.width;
                    pos_y = -(rownum * Hex.rect_height);
                    Point toppoint = new Point(pos_x, pos_y);
                    pos_x = x_offset + (columnnum * Hex.rect_width);
                    pos_y = -((rownum * Hex.rect_height) + Hex.height);
                    Point topleftpoint = new Point(pos_x, pos_y);
                    pos_x = x_offset + (columnnum + 1) * Hex.rect_width;
                    pos_y = -((rownum * Hex.rect_height) + Hex.height);
                    Point toprightpoint = new Point(pos_x, pos_y);
                    pos_x = x_offset + (columnnum * Hex.rect_width);
                    pos_y = -((rownum + 1) * Hex.rect_height);
                    Point bottomleftpoint = new Point(pos_x, pos_y);
                    pos_x = x_offset + (columnnum + 1) * Hex.rect_width;
                    pos_y = -((rownum + 1) * Hex.rect_height);
                    Point bottomrightpoint = new Point(pos_x, pos_y);
                    pos_x = x_offset + (columnnum * Hex.rect_width) + Hex.width;
                    pos_y = -(((rownum + 1) * Hex.rect_height) + Hex.height);
                    Point bottompoint = new Point(pos_x, pos_y);
                    
                    // rotate the set of points based on the orientation

                    toppoint.rotate(this.rot, (this.width / 2));
                    topleftpoint.rotate(this.rot, (this.width / 2));
                    toprightpoint.rotate(this.rot, (this.width / 2));
                    bottomleftpoint.rotate(this.rot, (this.width / 2));
                    bottomrightpoint.rotate(this.rot, (this.width / 2));
                    bottompoint.rotate(this.rot, (this.width / 2));

                    // we mention toppoint twice - otherwise Google Earth won't
                    // draw it.

                    Point[] points = {toppoint, topleftpoint, bottomleftpoint,
                        bottompoint, bottomrightpoint, toprightpoint, toppoint};

                    // project each point onto the globe

                    foreach (Point point in points) {
                        point.project(this.centerlong, this.centerlat);
                    }

                    // Assemble them into a Hex object, and move onto the next one.

                    this.data[rownum, columnnum] = new Hex(points);


                }
            }
        }

        public void write(StreamWriter kml) {
            kml.WriteLine("<kml xmlns=\"http://earth.google.com/kml/2.0\">");
            kml.WriteLine("<Folder>");
            kml.WriteLine("<name>HexThingy</name>");
            kml.WriteLine("<Placemark>");
            kml.WriteLine("<Style>");
            kml.WriteLine("<PolyStyle>");
            kml.WriteLine("  <fill>0</fill>");
            kml.WriteLine("  <outline>1</outline>");
            kml.WriteLine("</PolyStyle>");
            kml.WriteLine("</Style>");
            kml.WriteLine("<MultiGeometry>");
            for (int rownum = 0; rownum < this.rows; rownum++) {
                for (int columnnum = 0; columnnum < this.columns; columnnum++) {
                    this.data[rownum, columnnum].write(kml);
                }
            }
            kml.WriteLine("</MultiGeometry>");
            kml.WriteLine("</Placemark>");
            kml.WriteLine("</Folder>");
            kml.WriteLine("</kml>");
        }
    }
}
