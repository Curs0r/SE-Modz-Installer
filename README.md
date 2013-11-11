Copyright (C) 2013  J. Westbrock

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

SE-Modz-Installer
=================

A simple installer for custom blocks made for Space Engineers

10 Nov, 2013 Initial Creation by Curs0r

DotNetZip Library provided by http://dotnetzip.codeplex.com/

///////////////////////////////////////////////
To package a block for use with this installer:
///////////////////////////////////////////////

Create a folder for your block. Copy the directory structure require under Content.
For example if you have an icon texture create the folder Textures and under it GUI followed by Icons, and so on.
Place your files in the appropriate directories after creating them.

Create a new file called Description.xml. Paste the following text into your new xml file:

<?xml version="1.0"?>

<MyObjectBuilder_CubeBlockDefinitions xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
<Definitions>
	
</Definitions>
</MyObjectBuilder_CubeBlockDefinitions>

Copy your block's <Definition> from CubeBlocks.sbc and paste inside the <Definitions> element.

Save your Definition.xml file.

Compress your block's folder as a zip archive. Test it by launching SE-Modz Installer and dragging
t to the colored area. If your block fails to install, revisit the previous steps or come to
http://se-modz.forumotion.com/ for help.
