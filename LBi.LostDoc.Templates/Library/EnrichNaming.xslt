﻿<?xml version="1.0" encoding="utf-8"?>
<!-- 
  
  Copyright 2014 DigitasLBi Netherlands B.V.
  
  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at
  
      http://www.apache.org/licenses/LICENSE-2.0
  
  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License. 
  
-->
<xsl:stylesheet version="1.0"
                xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
                xmlns:msxsl="urn:schemas-microsoft-com:xslt"
                xmlns:ld="urn:lostdoc:template"
                xmlns:xna="urn:lostdoc:naming"
                exclude-result-prefixes="msxsl">

  <xsl:include href="Naming.xslt"/>

  <xsl:template match="/">
    <xsl:apply-templates select="descendant::*[@assetId]"/>
  </xsl:template>

  <xsl:template match="*[@assetId]">
    <xsl:copy>
      <moop/>
    </xsl:copy>
  </xsl:template>
</xsl:stylesheet>