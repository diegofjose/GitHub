using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace WebServiceSOAP.Classes
{
    public class BD
    {
        private SqlConnection objConn;

        public string CarregaConnString()
        {
            return "server=" + System.Web.Configuration.WebConfigurationManager.AppSettings["SERVERNAME"] + ";" +
                   "database=" + System.Web.Configuration.WebConfigurationManager.AppSettings["DATABASENAME"] + ";" +
                   "user id=" + System.Web.Configuration.WebConfigurationManager.AppSettings["USERNAME"] + ";" +
                   "pwd=" + System.Web.Configuration.WebConfigurationManager.AppSettings["PASSWORD"] + ";";
        }

        public bool AbreConexao()
        {
            try
            {
                objConn = new SqlConnection(CarregaConnString());
                objConn.Open();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void FechaConexao()
        {
            try
            {
                objConn.Close();
                objConn = null;
            }
            catch (Exception ex)
            {
            }
        }

        public Boolean ExecutaSQL(string sSQL)
        {
            try
            {
                AbreConexao();

                SqlCommand objCommand = new SqlCommand(sSQL, (SqlConnection)objConn);

                objCommand.ExecuteNonQuery();
                objCommand.Dispose();
                objCommand = null;

                FechaConexao();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DataTable AbreTbl(string sSQL)
        {
            try
            {
                AbreConexao();

                DataTable Tbl = new DataTable();
                DataSet Ds = new DataSet();
                SqlDataAdapter Da;

                Da = new SqlDataAdapter(sSQL, objConn);

                Tbl = null;
                Da.Fill(Ds);
                Tbl = Ds.Tables[0].Copy();

                FechaConexao();

                return Tbl;
            }
            catch (Exception ex)
            {
                FechaConexao();
                return null;
            }
        }
    }
}