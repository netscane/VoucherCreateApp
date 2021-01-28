using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace VoucherCreateApp.Dao
{
    public class DbHelper
    {
        private SqlConnection conn = null;

        private static DbHelper instance;

        static Object localLock = new Object();

        public static DbHelper getInstance(){
            if(null == instance){
                lock(localLock)
                {
                    if(null == instance){
                        instance = new DbHelper();
                    }
                }
            }
            return instance;
        }

        public String ConnectionString
        { get; set; }
        public SqlConnection GetConnection()
        {
            try
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    return conn;
                }
                else
                {
                    conn = new SqlConnection(ConnectionString);
                    //conn.ConnectionString = ConnectionString;
                    conn.Open();
                    return conn;
                }
            }
            catch (SqlException se)
            {
                //logger.Error(se.StackTrace);
                conn = null;
                throw se;
            }
        }

        public void CloseConnection(SqlConnection conn)
        {
            try
            {
                if (conn != null && conn.State == ConnectionState.Open)
                {
                    conn.Close();
                    conn = null;
                }
            }
            catch (SqlException se)
            {

            }
        }

        public DataSet Query(String sql)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlConnection conn = GetConnection();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                sda.Fill(ds);
            }
            catch (SqlException se)
            {
                //logger.Error(se.StackTrace);
                try
                {
                    conn.Close();
                }
                catch (SqlException se1)
                {
                }
                conn = null;
                throw se;
            }

            return ds;
        }

        public DataTable GetTable(String sql)
        {
            DataTable dt = new DataTable();
            try
            {
                SqlConnection conn = GetConnection();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                sda.Fill(dt);
            }
            catch (SqlException se)
            {
                //logger.Error(se.StackTrace);
                try
                {
                    conn.Close();
                }
                catch (SqlException se1)
                {
                }
                conn = null;
                throw se;
            }

            return dt;
        }

        public void LongHaul(String sql)
        {
            try
            {
                SqlConnection conn = GetConnection();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch (SqlException se)
            {
                try
                {
                    conn.Close();
                }
                catch (SqlException)
                {
                }
                //logger.Error(se.StackTrace);
                conn = null;
                throw se;
            }
        }

        public void LongHaul(List<string> sqlList)
        {
            foreach (String sql in sqlList)
            {
                //logger.Info("执行事务语句:" + sql);
            }
            SqlConnection conn = null;
            SqlTransaction sqlTransaction = null;
            try
            {
                conn = GetConnection();
                SqlCommand cmd = conn.CreateCommand();
                sqlTransaction = conn.BeginTransaction(System.Data.IsolationLevel.Serializable); // 开启事务
                cmd.Connection = conn;
                cmd.Transaction = sqlTransaction;

                
                foreach (String sql in sqlList)
                {
                    // 利用sqlcommand进行数据操作 
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
                sqlTransaction.Commit(); // 成功提交 
                cmd.Dispose();
            }
            catch (Exception ex)
            {
                if (sqlTransaction != null)
                    try
                    {
                        sqlTransaction.Rollback(); // 出错回滚 
                    }
                    catch (SqlException oe) { }
                throw ex;
            }
        }

        public Object SelectOnlyValue(String sql)
        {
            Object value = null;
            try
            {
                SqlConnection conn = GetConnection();

                DataSet ds = new DataSet();
                SqlDataAdapter sda = new SqlDataAdapter(sql, conn);
                sda.Fill(ds);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    value = ds.Tables[0].Rows[0][0];
                }
            }
            catch (SqlException se)
            {
                //logger.Error(se.StackTrace);
                try
                {
                    conn.Close();
                }
                catch (SqlException se1)
                {
                }
                conn = null;
                throw se;
            }
            return value;
        }
        public bool IsEmpty(DataSet ds)
        {
            return !(ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0);
        }
    }
}
