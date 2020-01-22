

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace signalRR
{
    public class DataAccessLayer 
    {
        SqlConnection con =new SqlConnection(ConfigurationManager.ConnectionStrings["TheConnectionString"].ConnectionString);
        public int executeNonQuery(string sql)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);

                int i = cmd.ExecuteNonQuery();
                con.Close();
                return i;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public DataTable executeQuery(string sql)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(sql, con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                con.Close();
                return dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

   
}
