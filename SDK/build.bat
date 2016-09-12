CD "1. Get Command Line Tools"
CALL "Get Command Line Tools.bat"

CD ..

CD "2. Create CSProj Files"

MSXSL ..\Data\CommandLineTools.csv.xml "2. Create CSProj Files.xslt" > fileset.xml
CLBCFileSetToFiles fileset.xml
