using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Text;
using System.Xml;

namespace WebServiceSOAP.Classes
{
    public class Querys
    {
        BD _bd = new BD();
        StringBuilder sSQL = new StringBuilder();

        public bool ValidaAcesso(string login, string senha)
        {
            try
            {
                sSQL = new StringBuilder();

                sSQL.AppendLine("SELECT 1 FROM tbl_login ");
                sSQL.AppendLine("WHERE tbl_login.codlgntp = 7 ");
                sSQL.AppendFormat("AND tbl_login.usuariolgn = '{0}' ", login);
                sSQL.AppendFormat("AND tbl_login.senhalgn = SUBSTRING(sys.fn_sqlvarbasetostr(HASHBYTES('MD5', '{0}')),3,999) ", senha);

                DataTable tblRetorno = _bd.AbreTbl(sSQL.ToString());

                if (tblRetorno != null && tblRetorno.Rows.Count == 1 && tblRetorno.Rows[0][0].ToString() == "1")
                { return true; }
                else
                { return false; }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int VerificaMailingExistente(DateTime data, string login, string carteira, string campanha)
        {
            try
            {
                sSQL = new StringBuilder();

                sSQL.AppendLine("SELECT codmlg FROM tbl_mailling tm ");

                sSQL.AppendLine("JOIN tbl_login tl ON tl.codemp = tm.codemp ");
                sSQL.AppendFormat("   AND tl.usuariolgn = '{0}' ", login);

                sSQL.AppendLine("JOIN tbl_centro_custo tcc on tcc.codemp = tl.codemp ");
                sSQL.AppendLine("   AND tcc.statuscc = 1 ");
                sSQL.AppendFormat("   AND tcc.dcrcc = '{0}' ", carteira);

                sSQL.AppendLine("WHERE CONVERT(VARCHAR(10), tm.datainimlg, 112) = CONVERT(VARCHAR(10), GETDATE(), 112) ");

                if (campanha != "")
                {
                    sSQL.AppendFormat(" AND dcrmlg = '{0}' ", campanha);
                }

                sSQL.AppendFormat("   AND tm.codmlgsts IN ({0},{1},{2},{3}) ", (int)Enums.MaillingStatus.Processado_Dados_Sucesso
                                                                                       , (int)Enums.MaillingStatus.Em_Envio
                                                                                       , (int)Enums.MaillingStatus.Envio_concluido
                                                                                       , (int)Enums.MaillingStatus.Concluido);

                DataTable tblRetorno = _bd.AbreTbl(sSQL.ToString());

                if (tblRetorno != null && tblRetorno.Rows.Count == 1)
                { return Convert.ToInt32(tblRetorno.Rows[0][0]); }
                else
                { return 0; }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int VerificaMailingExistente(DateTime data, string login, string carteira)
        {
            return VerificaMailingExistente(data, login, carteira, "");
        }

        public int InsereMailling(string login, string carteira, DateTime dataInicio, DateTime dataFim)
        {
            int CODEMP = 0, CODLGN = 0, CODCC = 0;

            //BUSCA EMPRESA
            sSQL = new StringBuilder();

            sSQL.AppendLine("SELECT tl.codemp, tl.codlgn, MAX(tcc.codcc) codcc ");
            sSQL.AppendLine("FROM tbl_login tl ");

            sSQL.AppendLine("JOIN tbl_centro_custo tcc on tcc.codemp = tl.codemp ");
            sSQL.AppendLine("   AND tcc.statuscc = 1 ");
            sSQL.AppendFormat("   AND tcc.dcrcc = '{0}' ", carteira);

            sSQL.AppendFormat("WHERE tl.usuariolgn = '{0}' ", login);
            sSQL.AppendLine("GROUP BY tl.codemp, tl.codlgn");

            DataTable tblRetorno = _bd.AbreTbl(sSQL.ToString());

            if (tblRetorno != null && tblRetorno.Rows.Count == 1)
            {
                CODEMP = Convert.ToInt32(tblRetorno.Rows[0][0]);
                CODLGN = Convert.ToInt32(tblRetorno.Rows[0][1]);
                CODCC = Convert.ToInt32(tblRetorno.Rows[0][2]);
            }
            else
            {
                return 0;
            }

            sSQL = new StringBuilder();

            //INSERE DADOS
            sSQL.AppendLine("INSERT INTO tbl_mailling VALUES ");
            sSQL.AppendLine("( ");

            sSQL.AppendFormat("{0}, ", CODEMP);
            sSQL.AppendFormat("{0}, ", CODLGN);
            sSQL.AppendFormat("{0}, ", CODCC);
            sSQL.AppendFormat("{0}, ", (int)Enums.Modelo.WebService);
            sSQL.AppendLine("'Webservice SOAP ' + CONVERT(VARCHAR(10), GETDATE(), 103), ");
            sSQL.AppendLine("'Webservice SOAP ' + CONVERT(VARCHAR(10), GETDATE(), 103), ");
            sSQL.AppendLine("'Webservice SOAP ' + CONVERT(VARCHAR(10), GETDATE(), 103), ");
            sSQL.AppendLine("'Webservice SOAP ' + CONVERT(VARCHAR(10), GETDATE(), 103), ");
            sSQL.AppendFormat("{0}, ", (int)Enums.MaillingStatus.Processado_Dados_Sucesso);
            sSQL.AppendFormat("{0}, ", (dataInicio == null ? "CONVERT(VARCHAR(10), GETDATE(), 120)" : "'" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", dataInicio.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute).AddSeconds(DateTime.Now.Second)) + "'"));
            sSQL.AppendFormat("{0}, ", (dataFim == DateTime.MinValue ? (dataInicio == null ? "CONVERT(VARCHAR(10), GETDATE(), 120)" : "'" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", dataInicio.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute + 5).AddSeconds(DateTime.Now.Second)) + "'") : String.Format("{0:yyyy-MM-dd HH:mm:ss}", dataFim)));
            sSQL.AppendLine("NULL, ");
            sSQL.AppendLine("NULL, ");
            sSQL.AppendLine("NULL, ");
            sSQL.AppendLine("NULL, ");
            sSQL.AppendFormat("{0}, ", (int)Enums.Prioridade.NORMAL);
            sSQL.AppendLine("NULL, ");
            sSQL.AppendLine("NULL, ");
            sSQL.AppendLine("NULL, ");
            sSQL.AppendLine("NULL ");

            sSQL.AppendLine(") ");

            if (_bd.ExecutaSQL(sSQL.ToString()))
            {
                sSQL = new StringBuilder();

                sSQL.AppendFormat("SELECT MAX(codmlg) codmlg FROM tbl_mailling WHERE codemp = {0} AND codcc = {1} AND codlgn = {2} ", CODEMP, CODCC, CODLGN);

                tblRetorno = _bd.AbreTbl(sSQL.ToString());

                return Convert.ToInt32(tblRetorno.Rows[0][0]);
            }
            else
            { return 0; }
        }

        public XmlDocument InsereMailling_Dado(string login, string msg, string fones, DateTime data, int CODMLG)
        {
            DataTable tblRetorno = InsereMailling_Dado_Tbl(login, msg, fones, data, CODMLG);

            if (tblRetorno != null && tblRetorno.Rows.Count > 0)
            {
                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(docNode);

                XmlNode nodeGeral = doc.CreateElement("pgdigitalsms");
                doc.AppendChild(nodeGeral);

                foreach (DataRow oRow in tblRetorno.Rows)
                {
                    XmlNode nodeElemento = doc.CreateElement("retorno");
                    XmlAttribute att = doc.CreateAttribute("id");
                    att.Value = oRow["codmlgdd"].ToString();
                    nodeElemento.Attributes.Append(att);
                    nodeGeral.AppendChild(nodeElemento);

                    XmlAttribute att2 = doc.CreateAttribute("data");
                    att2.Value = String.Format("{0:dd-MM-yyyy hh:mm:ss}", Convert.ToDateTime(oRow["dataenvmlgdd"]));
                    nodeElemento.Attributes.Append(att2);
                    nodeGeral.AppendChild(nodeElemento);

                    nodeElemento.InnerText = oRow["msgmlgdd"].ToString();
                }

                return doc;
            }
            else
            {
                return null;
            }
        }


        public DataTable InsereMailling_Dado_Tbl(string login, string msg, string fones, DateTime data, int CODMLG)
        {
            try
            {
                int CODEMP = 0;

                //BUSCA EMPRESA
                sSQL = new StringBuilder();

                sSQL.AppendLine("SELECT codemp FROM tbl_login ");
                sSQL.AppendFormat("WHERE tbl_login.usuariolgn = '{0}' ", login);

                DataTable tblRetorno = _bd.AbreTbl(sSQL.ToString());

                if (tblRetorno != null && tblRetorno.Rows.Count == 1)
                {
                    CODEMP = Convert.ToInt32(tblRetorno.Rows[0][0]);
                }
                else
                {
                    return null;
                }

                sSQL = new StringBuilder();

                //INSERE DADOS
                foreach (string fone in fones.Split(','))
                {
                    sSQL.AppendLine("");
                    sSQL.AppendLine("INSERT INTO tbl_mailling_dado VALUES ");
                    sSQL.AppendLine("( ");

                    sSQL.AppendFormat("{0}, ", CODEMP);
                    sSQL.AppendFormat("{0}, ", CODMLG);
                    sSQL.AppendFormat("{0}, ", (int)Enums.MaillingDadoStatus.Processado);
                    sSQL.AppendFormat("'{0}', ", fone.Substring(0, 3));
                    sSQL.AppendFormat("'{0}', ", (fone.Length == 12 ? fone.Substring(3, 9) : fone.Substring(3, 8)));
                    sSQL.AppendFormat("'{0}', ", msg);
                    sSQL.AppendLine("NULL, ");
                    sSQL.AppendLine("NULL, ");
                    sSQL.AppendLine("NULL, ");
                    sSQL.AppendLine("NULL, ");
                    sSQL.AppendLine("NULL, ");
                    sSQL.AppendLine("ABS(CHECKSUM(NEWID()) % 5) + 1, ");
                    sSQL.AppendLine("NULL, ");
                    sSQL.AppendLine("GETDATE(), ");
                    sSQL.AppendFormat("{0}, ", (data == null ? "CONVERT(VARCHAR(10), GETDATE(), 120)" : "'" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", data.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute).AddSeconds(DateTime.Now.Second + 1)) + "'"));
                    sSQL.AppendLine("NULL, ");
                    sSQL.AppendFormat("CEILING(CAST({0} AS DECIMAL(10,2)) / CAST(160 AS DECIMAL(10,2))), ", msg.Length);
                    sSQL.AppendLine("NULL, ");
                    sSQL.AppendLine("NULL, ");
                    sSQL.AppendLine("NULL, ");
                    sSQL.AppendLine("NULL, ");
                    sSQL.AppendLine("NULL, ");
                    sSQL.AppendLine("NULL, ");
                    sSQL.AppendLine("0, ");
                    sSQL.AppendLine("NULL, ");
                    sSQL.AppendLine("NULL ");

                    sSQL.AppendLine(") ");
                }

                //ATUALIZA STATUS DO MAILING
                sSQL.AppendLine("");
                sSQL.AppendFormat("UPDATE tbl_mailling SET codmlgsts = {0}, datainimlg = {1}, datafimmlg = {2} WHERE codmlg = {3} ", (int)Enums.MaillingStatus.Em_Envio
                                                                                                                                   , (data == null ? "CONVERT(VARCHAR(10), GETDATE(), 120)" : "'" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", data.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute).AddSeconds(DateTime.Now.Second)) + "'")
                                                                                                                                   , (data == null ? "CONVERT(VARCHAR(10), GETDATE(), 120)" : "'" + String.Format("{0:yyyy-MM-dd HH:mm:ss}", data.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute + 5).AddSeconds(DateTime.Now.Second)) + "'")
                                                                                                                                   , CODMLG);

                if (_bd.ExecutaSQL(sSQL.ToString()))
                {
                    sSQL = new StringBuilder();

                    sSQL.AppendLine("SELECT TOP " + (fones.Split(',').Length == 0 ? "1" : fones.Split(',').Length.ToString()) + " tmd.codmlgdd, tmd.dataenvmlgdd, tmd.msgmlgdd ");
                    sSQL.AppendLine("FROM tbl_mailling_dado tmd ");
                    sSQL.AppendFormat("WHERE tmd.codmlg = {0} ", CODMLG);
                    sSQL.AppendLine("   AND tmd.dataenvmlgdd >= CONVERT(VARCHAR(10), GETDATE(), 120) ");
                    sSQL.AppendFormat("   AND tmd.msgmlgdd = '{0}' ", msg);
                    sSQL.AppendLine("ORDER BY tmd.codmlgdd DESC ");

                    tblRetorno = _bd.AbreTbl(sSQL.ToString());

                    return tblRetorno;
                }
                else
                { return null; }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public XmlDocument RetornaRespostas(string login)
        {
            try
            {
                //BUSCA RESPOSTAS
                sSQL = new StringBuilder();

                sSQL.AppendLine("SELECT tmdr.codmlgdd, tmdr.dthrrespmlgddrpt, tmdr.msgmlgddrpt ");
                sSQL.AppendLine("FROM tbl_mailling_dado_resposta tmdr ");
                sSQL.AppendLine("JOIN tbl_mailling_dado tmd ON tmd.codmlgdd = tmdr.codmlgdd ");
                sSQL.AppendLine("JOIN tbl_login tl ON tl.codemp = tmd.codemp ");
                sSQL.AppendFormat(" AND tl.usuariolgn = '{0}' ", login);
                sSQL.AppendLine("WHERE CONVERT(VARCHAR(10), tmdr.dthrrespmlgddrpt, 120) = CONVERT(VARCHAR(10), GETDATE(), 120) ");

                DataTable tblRetorno = _bd.AbreTbl(sSQL.ToString());

                XmlDocument doc = new XmlDocument();
                XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                doc.AppendChild(docNode);

                XmlNode nodeGeral = doc.CreateElement("pgdigitalsms");
                doc.AppendChild(nodeGeral);

                foreach (DataRow oRow in tblRetorno.Rows)
                {
                    XmlNode nodeElemento = doc.CreateElement("retorno");
                    XmlAttribute att = doc.CreateAttribute("id");
                    att.Value = oRow["codmlgdd"].ToString();
                    nodeElemento.Attributes.Append(att);
                    nodeGeral.AppendChild(nodeElemento);

                    XmlAttribute att2 = doc.CreateAttribute("data");
                    att2.Value = String.Format("{0:dd-MM-yyyy hh:mm:ss}", Convert.ToDateTime(oRow["dthrrespmlgddrpt"]));
                    nodeElemento.Attributes.Append(att2);
                    nodeGeral.AppendChild(nodeElemento);

                    nodeElemento.InnerText = oRow["msgmlgddrpt"].ToString();
                }

                return doc;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}