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
                 <xsl:for-each select="//CommandLineTool">
                 <FileSetFile>
                   <RelativePath>../../Microsoft/CommandLineTools/CLBC<xsl:value-of select="Name"/>\CLBC<xsl:value-of select="Name"/>.csproj</RelativePath>
                   <OverwriteMode>Never</OverwriteMode>
                   <FileContents>&lt;?xml version="1.0" encoding="utf-8"?>
<Project ToolsVersion="14.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props" Condition="Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')" />
  <PropertyGroup>
    <Configuration Condition=" '$(Configuration)' == '' ">Debug</Configuration>
    <Platform Condition=" '$(Platform)' == '' ">AnyCPU</Platform>
    <ProjectGuid>{<xsl:value-of select="CommandLineToolId"/>}</ProjectGuid>
    <OutputType>Exe</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>CODEiverse.OST.CommandLineTools.CLBC<xsl:value-of select="Name"/></RootNamespace>
    <AssemblyName>CLBC<xsl:value-of select="Name"/></AssemblyName>
    <TargetFrameworkVersion>v4.5.2</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' ">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>..\binaries\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition=" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ">
    <PlatformTarget>AnyCPU</PlatformTarget>
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>..\binaries\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="System" />
    <Reference Include="System.Core" />
    <Reference Include="System.Xml.Linq" />
    <Reference Include="System.Data.DataSetExtensions" />
    <Reference Include="Microsoft.CSharp" />
    <Reference Include="System.Data" />
    <Reference Include="System.Net.Http" />
    <Reference Include="System.Xml" />
  </ItemGroup>
  <ItemGroup>
    <Compile Include="Program.cs" />
    <Compile Include="Properties\AssemblyInfo.cs" />
  </ItemGroup>
  <ItemGroup>
    <None Include="App.config" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\..\Common\Lib\CODEiverse.OST.Lib.csproj">
      <Project>{af51f8df-6f98-4210-8c79-ef3cacef0b30}</Project>
      <Name>CODEiverse.OST.Lib</Name>
    </ProjectReference>
  </ItemGroup>
  <Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />
  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
       Other similar extension points exist, see Microsoft.Common.targets.
  <Target Name="BeforeBuild">
  </Target>
  <Target Name="AfterBuild">
  </Target>
  -->
</Project></FileContents>
                 </FileSetFile>
                 <FileSetFile>
                   <RelativePath>../../Microsoft/CommandLineTools/CLBC<xsl:value-of select="Name"/>\Program.cs</RelativePath>
                   <OverwriteMode>Never</OverwriteMode>
                   <FileContents>/*****************************
Project:    CODEiverse - Open Source Tools (OST)
            Command Line Based Codee (CLBC)
            http://www.CODEiverse.com
Created By: EJ Alexandra - 2016
            An Abstract Level, llc
License:    Mozilla Public License 2.0
*****************************/
using CODEiverse.OST.Lib.CmdHelpers;
using CODEiverse.OST.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace CODEiverse.OST.CommandLineTools
{

    /// &lt;summary>
    /// <xsl:value-of select="Description"/>
    /// &lt;/summary>
    class Program : CLBCBaseProgram
    {
        static void Main(string[] args)
        {
        }
    }
}
</FileContents>
                 </FileSetFile>
                 <FileSetFile>
                   <RelativePath>../../Microsoft/CommandLineTools/CLBC<xsl:value-of select="Name"/>\Properties\AssemblyInfo.cs</RelativePath>
                   <OverwriteMode>Always</OverwriteMode>
                   <FileContents>using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("CLBC<xsl:value-of select="Name"/>")]
[assembly: AssemblyDescription("<xsl:value-of select="Description"/>")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("CODEiverse.com")]
[assembly: AssemblyProduct("CLBC<xsl:value-of select="Name"/>")]
[assembly: AssemblyCopyright("Copyright © CODEiverse.com, EJ Alexandra 2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("<xsl:value-of select="CommandLineToolId"/>")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("<xsl:value-of select="substring-after(Version, 'v')"/>")]
[assembly: AssemblyFileVersion("<xsl:value-of select="substring-after(Version, 'v')"/>")]
</FileContents>
                 </FileSetFile>
                 <FileSetFile>
                   <RelativePath>../../Microsoft/CommandLineTools/CLBC<xsl:value-of select="Name"/>\App.config</RelativePath>
                   <OverwriteMode>Never</OverwriteMode>
                   <FileContents>&lt;?xml version="1.0" encoding="utf-8" ?>
<configuration>
    <startup> 
        <supportedRuntime version="v4.0" sku=".NETFramework,Version=v4.5.2" />
    </startup>
</configuration></FileContents>
                 </FileSetFile>
               </xsl:for-each>
               </FileSetFiles>
             </FileSet>
              </xsl:template>
            </xsl:stylesheet>
          