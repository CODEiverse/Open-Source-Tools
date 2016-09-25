<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:xs="http://www.w3.org/2001/XMLSchema">
  <xsl:output method="xml" indent="yes" />
  <xsl:param name="root" />
  <xsl:template match="@*|node()">
    <xsl:copy>
      <xsl:apply-templates select="@*|node()" />
    </xsl:copy>
  </xsl:template>

  <xsl:template match="/">
    <FileSet>
      <FileSetFiles>
        <FileSetFile>
          <RelativePath>../../README.md</RelativePath>
          <OverwriteMode>Always</OverwriteMode>
          <FileContents>
# CODEiverse Open Source Tools

This is the Open Source Toolset that demonstrate the functionality
that CODEiverse provides.

There are currently <xsl:value-of select="count(//CommandLineTool)"/> tools.  They are.          

<xsl:for-each select="//CommandLineTool"><![CDATA[
  ]]><xsl:value-of select="position()"/>. **<a><xsl:attribute name="href">https://github.com/CODEiverse/Open-Source-Tools/blob/master/Docs/CommandLineTools/CLBC<xsl:value-of select="Name"/>.md</xsl:attribute><xsl:value-of select="Name"/></a>**<![CDATA[
      ]]><xsl:value-of select="Description"/><![CDATA[
]]>
</xsl:for-each>

The current MSI Installer ([v2016-09-11](https://github.com/CODEiverse/Open-Source-Tools/raw/master/Setup/Debug/CODEiverse_OST_20160911.msi)) can be downloaded to 
them to run locally.

          </FileContents>
        </FileSetFile>
      <xsl:for-each select="//CommandLineTool">
        <FileSetFile>
          <RelativePath>..\..\Docs\CommandLineTools\CLBC<xsl:value-of select="Name"/>.md</RelativePath>
        <FileContents>
# CLBC<xsl:value-of select="Name"/>

Description:
<xsl:value-of select="Description" />
          
Executable Name: <xsl:value-of select="ExeName"/>.

<a><xsl:attribute name="href">../../README.md</xsl:attribute>Back</a>
        
        </FileContents>
        </FileSetFile>
      
      </xsl:for-each>
      </FileSetFiles>
    </FileSet>
  </xsl:template>
</xsl:stylesheet>
