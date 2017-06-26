<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                exclude-result-prefixes="msxsl"
                xmlns:xs="http://www.w3.org/2001/XMLSchema"
>
<!--*****************************
    Project:    Codee42 (ODXML4)
    Created By: EJ Alexandra - 2016
                An Abstract Level, llc
    License:    Mozilla Public License 2.0
    *****************************  -->  
  <xsl:output method="html" indent="yes" media-type="text/html" omit-xml-declaration="yes"/>

  <xsl:include href="../CommonXsltTemplates.xslt" />

  <xsl:template match="@* | node()">
    <FileSet>
      <FileSetFiles>
        <FileSetFile>
          <RelativePath>..\DataSchema.odxml</RelativePath>
          <FileContents><Ontology>
      <OntologyGroups>
        <OntologyGroup>
          <Name>ODXMLSchema</Name>
          <Namespace>com.anabstractlevel.Schemas</Namespace>
          <ObjectDefs>
            <xsl:for-each select="//xs:element[count(xs:complexType) > 0 and count(xs:complexType/xs:sequence/xs:element) > 1]">
                <xsl:variable name="objectdef-name" select="@name" />
              <ObjectDef>
                <Name>
                  <xsl:value-of select="@name"/>
                </Name>
                <Namespace>com.anabstractlevel.Schemas.ODXMLSchema</Namespace>
                <PropertyDefs>
                    <xsl:if test="count(xs:complexType/xs:sequence/xs:element[@name = concat($objectdef-name, 'Id')]) = 0">
                      <PropertyDef>
                      <Name><xsl:value-of select="concat($objectdef-name, 'Id')"/></Name>
                      <Namespace>com.anabstractlevel.Schemas.ODXMLSchema.<xsl:value-of select="current()/@name"/></Namespace>
                          <DefaultValue>(newid()))</DefaultValue>
                      <DataType>GUID</DataType>
                      <Length></Length>
                      <IsPrimaryKey>1</IsPrimaryKey>
                    </PropertyDef>
                    </xsl:if>
                  <xsl:for-each select="xs:complexType/xs:sequence/xs:element">
                      <xsl:variable name="datatype">
                          <xsl:call-template name="xsd-to-object-def-type">
                              <xsl:with-param name="xsd-type-name" select="@type"/>
                          </xsl:call-template>
                      </xsl:variable>
                    <PropertyDef>
                      <Name><xsl:value-of select="@name"/></Name>
                      <Namespace>com.anabstractlevel.Schemas.ODXMLSchema.<xsl:value-of select="current()/@name"/></Namespace>
                      <DataType>
                          <xsl:value-of select="$datatype"/>
                      </DataType>
                      <Length>
                          <xsl:choose>
                              <xsl:when test="$datatype = 'TEXT' and @name = 'Description'">500</xsl:when>
                              <xsl:when test="$datatype = 'TEXT'">100</xsl:when>
                          </xsl:choose>
                      </Length>
                        <IsNullable>1</IsNullable>
                      <IsPrimaryKey>0</IsPrimaryKey>
                    </PropertyDef>
                  </xsl:for-each>
                </PropertyDefs>
              </ObjectDef>
            </xsl:for-each>
          </ObjectDefs>
        </OntologyGroup>
      </OntologyGroups>
    </Ontology></FileContents>
        </FileSetFile>
    </FileSetFiles>
    </FileSet>
    
  </xsl:template>
</xsl:stylesheet>
