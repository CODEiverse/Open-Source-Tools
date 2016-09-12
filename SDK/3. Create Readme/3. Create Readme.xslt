<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl" xmlns:xs="http://www.w3.org/2001/XMLSchema">
  <xsl:output method="xml" indent="yes" />
  <xsl:param name="root" />
  <xsl:template match="@*|node()">
    <copy>
      <xsl:apply-templates select="@*|node()" />
    </copy>
  </xsl:template>

  <xsl:template match="/CommandLineTools">
    <FileSet>
      <FileSetFiles>
        <FileSetFile>
          <RelativePath>Readme.md</RelativePath>
          <OverwriteMode>Always</OverwriteMode>
          <FileContents>
# CODEiverse Open Source Tools

This is the Open Source Toolset that demonstrate the functionality
that CODEiverse provides.

There are currently <xsl:value-of select="count(//CommandLineTool)"/> tools.  They are.          
  <xsl:for-each select="//CommandLineTool">
  - <xsl:value-of select="Name"/>
  </xsl:for-each>

          </FileContents>
        </FileSetFile>
      </FileSetFiles>
    </FileSet>
  </xsl:template>
</xsl:stylesheet>
