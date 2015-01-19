using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Xml;
using System.Data;
using WebServiceSOAP.Classes;

namespace WebServiceSOAP
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class WebServSOAP : System.Web.Services.WebService
    {
        Querys objQuerys = new Querys();

        [WebMethod]
        public string envioDeCampanha(string token, string carteira, string campanha, object[] lotes, object[] dados, string retorno)
        {
            #region VALIDAÇÃO DE ACESSO

            if (!objQuerys.ValidaAcesso(((string[])token.Split(':'))[0], ((string[])token.Split(':'))[1]))
            {
                return "Falha de login.";
            }

            #endregion

            #region ENVIO DE CAMPANHA

            List<DataTable> listTbl = new List<DataTable>();

            foreach (object[] lote in lotes)
            {
                int CODMLG = objQuerys.InsereMailling(((string[])token.Split(':'))[0], carteira, Convert.ToDateTime(lote[1]), Convert.ToDateTime(lote[2]));

                foreach (object[] dado in dados)
                {
                    listTbl.Add(objQuerys.InsereMailling_Dado_Tbl(((string[])token.Split(':'))[0], dado[1].ToString(), dado[0].ToString(), Convert.ToDateTime(lote[1]), CODMLG));
                }
            }

            DataTable tblRetorno = new DataTable();

            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlNode nodeGeral = doc.CreateElement("pgdigitalsms");
            doc.AppendChild(nodeGeral);

            foreach (DataTable tbl in listTbl)
            {
                tblRetorno.Merge(tbl);
            }

            //PREPARA XML DE RETORNO
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

            #endregion ENVIO DE CAMPANHA

            #region RETORNO

            if (doc != null)
            { return doc.InnerXml; }
            else
            { return ""; }

            #endregion RETORNO
        }

        [WebMethod]
        public string envioAvulso(string token, string carteira, string msg, string fones, string data, string retorno)
        {
            #region VALIDAÇÃO DE ACESSO

            if (!objQuerys.ValidaAcesso(((string[])token.Split(':'))[0], ((string[])token.Split(':'))[1]))
            {
                return "Falha de login.";
            }

            #endregion

            #region ENVIO AVULSO

            XmlDocument doc;

            #region VERIFICAÇÃO DE MAILING JÁ EXISTENTE PARA O DIA

            int CODMLG = objQuerys.VerificaMailingExistente(DateTime.Today, ((string[])token.Split(':'))[0], carteira);

            #endregion

            if (CODMLG != 0)
            {
                #region MAILING JÁ EXISTENTE PARA O DIA

                doc = objQuerys.InsereMailling_Dado(((string[])token.Split(':'))[0], msg, fones, Convert.ToDateTime(data), CODMLG);

                #endregion MAILING JÁ EXISTENTE PARA O DIA
            }
            else
            {
                #region MAILING NÃO EXISTENTE PARA O DIA

                //INSERE MAILING
                CODMLG = objQuerys.InsereMailling(((string[])token.Split(':'))[0], carteira, Convert.ToDateTime(data), DateTime.MinValue);

                doc = objQuerys.InsereMailling_Dado(((string[])token.Split(':'))[0], msg, fones, Convert.ToDateTime(data), CODMLG);

                #endregion MAILING NÃO EXISTENTE PARA O DIA
            }

            #endregion ENVIO AVULSO

            #region RETORNO

            if (doc != null)
            { return doc.InnerXml; }
            else
            { return ""; }

            #endregion RETORNO
        }

        [WebMethod]
        public string getResposta(string token, string retorno)
        {
            #region VALIDAÇÃO DE ACESSO

            if (!objQuerys.ValidaAcesso(((string[])token.Split(':'))[0], ((string[])token.Split(':'))[1]))
            {
                return "Falha de login.";
            }

            #endregion

            #region BUSCA DAS RESPOSTAS

            XmlDocument doc = objQuerys.RetornaRespostas(((string[])token.Split(':'))[0]);

            #endregion BUSCA DAS RESPOSTAS

            #region RETORNO

            if (doc != null)
            { return doc.InnerXml; }
            else
            { return ""; }

            #endregion RETORNO
        }
    }
}