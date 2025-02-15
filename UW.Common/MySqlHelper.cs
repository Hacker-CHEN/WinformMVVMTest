using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace UW.Common
{
    public class MySqlHelper
    {
        //从App.Config文件中找到mysql连接语句
        private string dbConfig = ConfigurationManager.ConnectionStrings["mysql"].ToString();

        /// <summary>
        /// 执行数据库
        /// </summary>
        /// <param name="sql">数据库语句</param>
        /// <returns></returns>
        public int ExecuteSql(string sql)
        {
            using (MySqlConnection connection = new MySqlConnection(dbConfig))
            {
                using (MySqlCommand mySqlCommand = new MySqlCommand(sql, connection))
                {
                    try
                    {
                        connection.Open();
                        return mySqlCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        //自动回收未使用内存服务
                        GC.SuppressFinalize(mySqlCommand);
                    }
                }
            }
        }

        /// <summary>
        /// 查询数据库
        /// </summary>
        /// <param name="sql">数据库语句</param>
        /// <returns></returns>
        public DataTable Query(string sql)
        {
            DataTable dataTable = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(dbConfig))
            {
                MySqlDataAdapter mySqlDataAdapter = null;
                try
                {
                    connection.Open();
                    using (mySqlDataAdapter = new MySqlDataAdapter(sql, connection))
                        mySqlDataAdapter.Fill(dataTable);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //自动回收未使用内存服务
                    GC.SuppressFinalize(mySqlDataAdapter);
                }
            }
            return dataTable;
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名称</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public DataTable RunProcedure(string storedProcName, IDbDataParameter[] parameters)
        {
            DataTable dataTable = new DataTable();
            using (MySqlConnection mySqlConnection = new MySqlConnection(dbConfig))
            {
                MySqlDataAdapter mySqlDataAdapter = null;
                try
                {
                    mySqlConnection.Open();
                    using (MySqlCommand mySqlCommand = new MySqlCommand())
                    {
                        mySqlCommand.CommandTimeout = 3000;
                        mySqlCommand.CommandText = storedProcName;
                        mySqlCommand.CommandType = CommandType.StoredProcedure;
                        mySqlCommand.Connection = mySqlConnection;
                        mySqlCommand.Parameters.AddRange((Array)parameters);
                        using (mySqlDataAdapter = new MySqlDataAdapter())
                        {
                            mySqlDataAdapter.SelectCommand = mySqlCommand;
                            mySqlDataAdapter.Fill(dataTable);
                            mySqlConnection.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    //自动回收未使用内存服务
                    GC.SuppressFinalize(mySqlDataAdapter);
                }
            }
            return dataTable;
        }
    }
}
