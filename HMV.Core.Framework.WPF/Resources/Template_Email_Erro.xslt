<?xml version="1.0" encoding="utf-8" ?>
<!DOCTYPE xsl:stylesheet [
  <!ENTITY nbsp "&#160;">
]>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
  xmlns:msxsl="urn:schemas-microsoft-com:xslt"
  xmlns:HMV="urn:HMV">
  <xsl:param name="TitutoMensagem"></xsl:param>
  <xsl:param name="Sistema"></xsl:param>
  <xsl:param name="Mensagem"></xsl:param>
  <xsl:param name="UsuarioWindows"></xsl:param>
  <xsl:param name="Maquina"></xsl:param>
  <xsl:param name="Versao"></xsl:param>
  <xsl:param name="Imagem"></xsl:param>
  <xsl:output method="html" doctype-system="http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"
     doctype-public="-//W3C//DTD XHTML 1.0 Strict//EN"/>
  <msxsl:script implements-prefix="HMV" language="C#">
    <![CDATA[
      public string GetDateTime()
      {
          return DateTime.Now.ToString("'Porto Alegre, ' dd 'de' MMMM 'de' yyyy."); 
      }

    ]]>
  </msxsl:script>
  <xsl:template match="/">
    <html>
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
        <title>
          <xsl:value-of select="$TitutoMensagem" />
        </title>
        <style type="text/css" media="screen">
          body {
          margin: 0;
          padding: 0;
          background-color: #ffffff;
          }

          td.permission {
          padding: 20px 0 20px 0;
          }

          td.permission p {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 10px;
          font-weight: normal;
          color: #151515;
          }

          td.permission p a {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 10px;
          font-weight: normal;
          color: #a72323;
          }

          td.header {
          background-image: url('header.jpg');
          background-repeat: no-repeat;
          background-position: top center;
          background-color: #b62e2e;
          height: 130px;
          }

          td.header h1 {
          font-family: Georgia, serif;
          font-size: 30px;
          font-weight: normal;
          color: #333333;
          margin-left: 50px;
          margin-bottom: 24px;
          }

          table.content {
          background-color: #f5f5f5;
          }

          td.sidebar a {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 11px;
          font-weight: normal;
          color: #a72323;
          text-decoration: none;
          }

          td.sidebar ul {
          margin: 0 0 0 24px;
          padding: 0;
          }

          td.sidebar ul li,
          td.sidebar ul li a {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 12px;
          font-weight: normal;
          color: #a72323;
          }

          td.sidebar h3 {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 16px;
          font-weight: bold;
          color: #333333;
          margin: 10px 0 14px 0;
          padding: 0;
          }

          td.sidebar h3.buttons {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 16px;
          font-weight: bold;
          color: #333333;
          margin: 0 0 4px 0;
          padding: 0;
          }

          td.sidebar h4 {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 13px;
          font-weight: bold;
          color: #333333;
          margin: 0 0 2px 0;
          padding: 0;
          }

          td.sidebar p {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 11px;
          font-weight: normal;
          color: #505050;
          margin: 0 0 13px 0;
          padding: 0;
          }

          td.sidebar p.buttons {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 11px;
          font-weight: normal;
          color: #505050;
          margin: 0 0 4px 0;
          padding: 0;
          }

          td.border {
          border-right: 2px solid #e0e0e0;
          }

          td.mainbar a {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 12px;
          font-weight: normal;
          color: #a72323;
          text-decoration: none;
          }

          td.mainbar h2 {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 16px;
          font-weight: bold;
          color: #42607D;
          margin: 0 0 4px 0;
          padding: 0 0 4px 0;
          border-bottom: 1px solid #cbcbcb;
          }

          td.mainbar h3 {
          font-family: Georgia, serif;
          font-size: 16px;
          font-weight: normal;
          color: #333333;
          margin: 10px 0 14px 0;
          padding: 0;
          }

          td.mainbar p {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 12px;
          font-weight: normal;
          color: #333333;
          margin: 0 0 2px 0;
          padding: 0;
          }

          td.mainbar p.top {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 10px;
          font-weight: bold;
          color: #a72323;
          margin: 0 0 30px 0;
          padding: 0;
          }

          td.mainbar p.top a {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 10px;
          font-weight: bold;
          color: #a72323;
          }

          td.mainbar ul {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 12px;
          font-weight: normal;
          color: #333333;
          margin: 0 0 20px 24px;
          padding: 0;
          }

          td.footer {
          padding: 20px 0 20px 0;
          }

          td.footer p {
          font-family: 'Lucida Grande', sans-serif;
          font-size: 10px;
          font-weight: normal;
          color: #151515;
          }

          p{
          padding: 0;
          }
        </style>

      </head>
      <body>
        
        <table width="100%" border="0" cellspacing="0" cellpadding="0">
          <tr>
            <td align="center">
              <table width="580" border="0" cellspacing="0" cellpadding="0">
                <tr>
                  <td class="permission" align="center">
                  </td>
                </tr>
                <tr>
                  <td>
                    <table width="580" height="130" border="0" cellspacing="16" cellpadding="0" class="content">
                      <tr>
                        <td class="mainbar" align="left" valign="top" width="580">
                          <h2>
                            <xsl:value-of select="$TitutoMensagem" />
                          </h2>
                          <h2>
                            <xsl:value-of select="concat('Sistema: ', $Sistema)"/>
                          </h2>
                          <h2>
                            <xsl:value-of select="concat('Máquina: ', $Maquina)"/>
                          </h2>
                          <h2>
                            <xsl:value-of select="concat('Usuário: ', $UsuarioWindows)"/>
                          </h2>
                          <h2>
                            <xsl:value-of select="concat('Versão: ', $Versao)"/>
                          </h2>
                          <xsl:value-of select="$Mensagem" disable-output-escaping="yes"/>
                        </td>
                      </tr>
                      <!--<tr>
                        <td class="header" align="left" valign="bottom">
                          <img alt="ABC Widgets" width="800" height="600">
                            <xsl:attribute name="src">
                              <xsl:value-of select="concat('http://novaintra.moinhos.net/PortalMedico/EmailTemplate/LogsImg/', $Imagem)"/>
                            </xsl:attribute>
                          </img>
                        </td>
                      </tr>-->
                    </table>
                  </td>
                </tr>
              </table>
            </td>
          </tr>
        </table>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
