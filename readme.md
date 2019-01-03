Hexthingy
---------

Hexthingy is a small program designed to display hexagon grids (like those used in wargames) on the surface of the earth for viewing in [Google Earth](http://earth.google.com/) or [Google Maps](https://google.com/mymaps).  Hexgrids of any size and resolution may be created (but see below), and those grids may be rotated at any angle.  Once the user is happy with the parameters entered, he or she pushes a button to generate a .kml file, which is saved on the hard drive.  Double-click on this file, and view the resulting grid in your map viewer.

I created Hexthingy in the hopes that it would be useful to map designers during the research stage of wargame design.  Use it to brainstorm different map scales, positions and orientations. Then zoom in on each hex, examine its contents, and transfer the results to your game.

Obtaining
---------

Clone the repository or download it as a .zip file and build the solution in Visual Studio, or (if you feel like trusting an anonymous stranger on the internet) I've shared a [Windows binary file](https://drive.google.com/open?id=1Fb4GG01bL4xLriSYI9bhcSQZh8IcJAFX) - this was built against .net 4.5, so if you're getting version problems, try building it yourself.

Instructions
------------

Google Earth: Desktop:  Double-click on the .kml file.

Google Earth: Chrome:  Click the 'My Places' icon on the left toolbar (it kind of looks like a ribbon, I guess?).  You will see an option called 'Import KML File'.  Choose this and select the .kml file created by Hexthingy.

Google Maps:  Go to My Maps at [https://google.com/mymaps](https://google.com/mymaps) and select Create a New Map.  In the top left dialogue box, choose Import, and select the .kml file created by Hexthingy.


Parameters
----------

Center Point Latitude - this is the location of the center of the grid on the earth, in decimal degrees of latitude.  For example, entering '45' will center the grid at 45 degrees north of the equator, while '-90' will center it at the South Pole.  Values greater than 90 or less than -90 are not valid.

Center Point Longitude - this is the location of the center of the grid on the earth, in decimal degrees of longitude.  For example, entering '0' will center the grid at the prime meridian, while '-180' or 180 will center it at the International Date Line.  Positive values are east of the prime meridian, while negative values are west.  Values wrap around, so 270 degrees is the same as -90.  Values greater than 360 or less than -360 are not valid.

Width (East-West) - this is the width of the grid.  It can be specified in any of the following units: (feet, yards, metres, kilometres, miles, nautical miles and Degree Equivalents).  Note that though the distortion with this projection is surprisingly low, grids covering more than a hemisphere are not valid, and you may be unhappy with the results for grids wider than around ten thousand kilometres.  The grid is drawn, then rotated, so a grid 100 km wide, with an orientation of 90 degrees will actually be drawn 100 km North to South, not East to West.

Height (North-South) - this is the height of the grid.  It can be specified in any of the following units: (feet, yards, metres, kilometres, miles, nautical miles and Degree Equivalents).  Note that though the distortion with this projection is surprisingly low, grids covering more than a hemisphere are not valid, and you may be unhappy with the results for grids higher than around ten thousand kilometres.  The grid is drawn, then rotated, so a grid 100 km high, with an orientation of 90 degrees will actually be drawn 100 km West to East, not North to South.

Orientation - After the grid is drawn, it is rotated by the number of degrees specified here. For example, using this parameter, the user can specify a grid that runs by columns or rows, or faces in a direction other than north.

Resolution - Specify the size of each hex here.  Again, you can use any of the following units: feet, yards, metres, kilometres, miles, nautical miles and Degree Equivalents.  Note that specifying a resolution larger than the map width or height will result in no grid being drawn.

Press the 'Generate and Save Grid' button to create the grid on disk.  You will be prompted for a filename and location to save, and then the grid will be created and saved.  If no .kml extension is specified, one will be appended to the filename.  Double-click on the resulting file to view the grid in Google Earth.

Technical Notes
---------------

Hexthingy uses an orthographic projection to transfer a hex grid drawn on a plane onto the surface of the Earth.  This projection is a nice one, with zero distortion at the center and fairly minimal distortion in radial lines away from the center.  Distortion increases as the area covered approaches a hemisphere, and the projection fails if more than half the earth is being drawn.  If you want a global game, use the ISEA projection instead.

Try to use restraint while creating your grids.  Hexthingy will happily create a grid covering all of Europe at ten metres to the hex, but the resulting file will be gigabytes in size, and will fail to display in Google Earth in non-geological timeframes.   If you really need a map with more than a quarter of a million hexes in it, try drawing a grid with your ideal resolution times three. The result will have one-seventh as many hexes, and you can still zoom in on each hex, imagining seven hexes packed into it (one in the centre, and six around the perimeter).

I don't recommend simply printing off the output from Google Earth and using it as your map - most of the data displayed by Google Earth is copyrighted (as displayed near the bottom of the view window), and distributing it may open you up to civil or criminal prosectution.  Consult your local lawyer.

Changenotes
-----------

Mar 5, 2014:
Version 1.2 released.  Update in KML specification was causing grid to be compressed to a single horizontal line.  Honestly, whitespace is now significant in coordinate tuples??  DC

September 17, 2006:
Version 1.1 released.  Error in rotate function was causing grids to be rotated incorrectly.  Thanks to Mike Cox for pointing out the error.  DC

May 17, 2006:
Version 1.0 released.  DC
