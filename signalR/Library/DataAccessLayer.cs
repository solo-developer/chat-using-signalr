/*
     * Class: Library.DataAccessLayer
     * Description: Data Access Layer
     * Created By: Sagar Dahal
     * Created Date: May 24, 2015
     * ---------------
     * Modification Log: 
     *
 */

using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Configuration;
using System.Diagnostics;

namespace signalR
{
    public class DataAccessLayer : IDisposable
    {

        private static DataAccessLayer instance = null;

        private IDbConnection _sharedConnection;
        private IDbTransaction _sharedTransaction;

        public DataAccessLayer()
        {
            try
            {
                _sharedConnection = new SqlConnection(WebConfigurationManager.ConnectionStrings["TheConnectionString"].ConnectionString);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create connection. " + ex.Message);
            }
        }

        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["TheConnectionString"].ConnectionString;
        }

        public static DataAccessLayer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataAccessLayer();
                }
                return instance;
            }
        }

        public string ExecuteScalar(string query)
        {
            OpenSharedConnection();

            SqlCommand cmd = new SqlCommand(query, ((SqlConnection)_sharedConnection));
            if (_sharedTransaction != null)
            {
                cmd.Transaction = (SqlTransaction)_sharedTransaction;
            }
            var val = cmd.ExecuteScalar();

            if (_sharedTransaction == null)
            {
                CloseSharedConnection();
            }

            if (val == null)
                return "-1";
            else
                return val.ToString();
        }

        public string ExecuteScalar(SqlCommand cmd)
        {
            OpenSharedConnection();

            cmd.Connection = (SqlConnection)_sharedConnection;
            if (_sharedTransaction != null)
            {
                cmd.Transaction = (SqlTransaction)_sharedTransaction;
            }

            string val = cmd.ExecuteScalar().ToString();

            if (_sharedTransaction == null)
            {
                CloseSharedConnection();
            }

            return val;
        }

        public DataTable ExecuteQuery(string query)
        {
            OpenSharedConnection();

            SqlCommand cmd = new SqlCommand(query, ((SqlConnection)_sharedConnection));
            if (_sharedTransaction != null)
            {
                cmd.Transaction = (SqlTransaction)_sharedTransaction;
            }
            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);

            if (_sharedTransaction == null)
            {
                CloseSharedConnection();
            }

            return dt;
        }

        public DataTable ExecuteQuery(SqlCommand cmd)
        {
            OpenSharedConnection();

            cmd.Connection = (SqlConnection)_sharedConnection;
            if (_sharedTransaction != null)
            {
                cmd.Transaction = (SqlTransaction)_sharedTransaction;
            }

            SqlDataAdapter da = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            da.Fill(dt);

            if (_sharedTransaction == null)
            {
                CloseSharedConnection();
            }

            return dt;
        }

        public int ExecuteNonQuery(string query)
        {
            OpenSharedConnection();

            SqlCommand cmd = new SqlCommand(query, (SqlConnection)_sharedConnection);
            if (_sharedTransaction != null)
            {
                cmd.Transaction = (SqlTransaction)_sharedTransaction;
            }

            int val = cmd.ExecuteNonQuery();

            if (_sharedTransaction == null)
            {
                CloseSharedConnection();
            }

            return val;
        }

        public int ExecuteNonQuery(SqlCommand cmd)
        {
            OpenSharedConnection();

            cmd.Connection = (SqlConnection)_sharedConnection;
            if (_sharedTransaction != null)
            {
                cmd.Transaction = (SqlTransaction)_sharedTransaction;
            }

            int val = cmd.ExecuteNonQuery();

            if (_sharedTransaction == null)
            {
                CloseSharedConnection();
            }

            return val;
        }

        public string GetLastInsertedId()
        {
            OpenSharedConnection();

            SqlConnection conn = (SqlConnection)GetSharedConnection();
            SqlCommand cmd = new SqlCommand("SELECT SCOPE_IDENTITY();", conn);
            if (_sharedTransaction != null)
            {
                cmd.Transaction = (SqlTransaction)_sharedTransaction;
            }

            string val = cmd.ExecuteScalar().ToString();

            if (_sharedTransaction == null)
            {
                CloseSharedConnection();
            }

            return val;
        }

        public string GetLastInsertedId(string tableName)
        {
            OpenSharedConnection();

            SqlConnection conn = (SqlConnection)GetSharedConnection();
            SqlCommand cmd = new SqlCommand("SELECT IDENT_CURRENT('" + tableName + "');", conn);
            if (_sharedTransaction != null)
            {
                cmd.Transaction = (SqlTransaction)_sharedTransaction;
            }

            string val = cmd.ExecuteScalar().ToString();

            if (_sharedTransaction == null)
            {
                CloseSharedConnection();
            }

            return val;
        }


        public void CreateSharedConnection(string connectionString_)
        {
            if (_sharedConnection == null)
            {
                _sharedConnection = new SqlConnection(connectionString_);
            }
            else
            {
                throw new Exception("The connection is already created!");
            }
        }

        public void OpenSharedConnection()
        {
            try
            {
                if (_sharedConnection.State == ConnectionState.Closed)
                    _sharedConnection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to open connection. " + ex.Message);
            }
        }

        public void CloseSharedConnection()
        {
            try
            {
                if (_sharedConnection.State == ConnectionState.Open)
                    _sharedConnection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to close connection. " + ex.Message);
            }

        }

        public void BeginTransaction()
        {
            try
            {
                if (_sharedTransaction == null)
                {
                    OpenSharedConnection();
                    _sharedTransaction = _sharedConnection.BeginTransaction();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CommitTransaction()
        {
            try
            {
                if (_sharedTransaction != null)
                    _sharedTransaction.Commit();
                _sharedTransaction = null;
                CloseSharedConnection();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                if (_sharedTransaction != null)
                    _sharedTransaction.Rollback();
                _sharedTransaction = null;
            }
            catch (Exception)
            {
                CloseSharedConnection();
                throw;
            }
        }

        public IDbConnection GetSharedConnection()
        {
            if (_sharedConnection != null)
                return _sharedConnection;
            else
                throw new Exception("The connection is not created.");
        }

        public IDbTransaction GetSharedTransaction()
        {
            if (_sharedTransaction != null)
                return _sharedTransaction;
            else
                throw new Exception("The transaction is not created.");
        }

        public void Dispose()
        {
            if (_sharedConnection != null)
            {
                _sharedConnection.Dispose();
                _sharedConnection = null;
            }

            if (_sharedTransaction != null)
            {
                _sharedTransaction.Dispose();
                _sharedTransaction = null;
            }
        }
    }

    public class CatalogFreeDataAccessLayer
    {

        SqlConnection con;

        public CatalogFreeDataAccessLayer()
        {
            con = new SqlConnection(GetConnectionString());
        }

        public string GetConnectionString()
        {

            try
            {
                string tempCS = ConfigurationManager.ConnectionStrings["TheConnectionString"].ConnectionString;
                SqlConnectionStringBuilder csb = new SqlConnectionStringBuilder(tempCS);
                if ((!string.IsNullOrEmpty(csb.Password)))
                {
                    csb.Password = EncryptDecrypt.Decrypt(csb.Password, "crystalsoftnepal");
                }
                if ((!string.IsNullOrEmpty(csb.UserID)))
                {
                    csb.UserID = EncryptDecrypt.Decrypt(csb.UserID, "crystalsoftnepal");
                }
                return csb.ConnectionString;
            }

            catch (Exception)
            {
                throw new Exception("Failed to retrieve the connection string.");
            }
        }

        public string ExecuteNonQuery(string query)
        {
            OpenConnection();

            SqlCommand cmd = new SqlCommand(query, con);

            string returnVal = cmd.ExecuteNonQuery().ToString();

            CloseConnection();

            return returnVal;
        }

        public DataTable ExecuteQuery(string query)
        {
            OpenConnection();

            SqlCommand cmd = new SqlCommand(query, con);

            SqlDataAdapter sda = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            sda.Fill(dt);

            CloseConnection();

            return dt;
        }

        public void OpenConnection()
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CloseConnection()
        {
            try
            {
                if (con.State == ConnectionState.Open)
                    con.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
