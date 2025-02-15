using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UW.Common
{
    public class DatabaseHelper
    {
        public static DatabaseHelper _instance;
        private DataBaseType _DBType;
        private string _connectStr;
        private string lastRunTime;

        public static DatabaseHelper Instance
        {
            get
            {
                if (DatabaseHelper._instance == null)
                {
                    string appSetting = ConfigurationManager.AppSettings["DataBaseType"];
                    string connectStr = ConfigurationManager.ConnectionStrings[appSetting].ToString();
                    DatabaseHelper._instance = new DatabaseHelper((DataBaseType)Enum.Parse(typeof(DataBaseType), appSetting), connectStr);
                }
                return DatabaseHelper._instance;
            }
            set => DatabaseHelper._instance = value;
        }

        public DatabaseHelper(DataBaseType dbType, string connectStr)
        {
            DateTime dateTime = DateTime.Now;
            dateTime = dateTime.AddDays(-1.0);
            this.lastRunTime = dateTime.ToString("yyyy-MM-dd");
            // ISSUE: explicit constructor call
            //base.\u002Ector();
            this._DBType = dbType;
            this._connectStr = connectStr;
        }

        public bool CheckMysqlDataBase()
        {
            List<string> list = ((IEnumerable<string>)this._connectStr.Split(';')).ToList<string>();
            string server = list.Find((Predicate<string>)(x => x.Contains("server"))).Split('=')[1];
            string port = list.Find((Predicate<string>)(x => x.Contains("port"))).Split('=')[1];
            string userid = list.Find((Predicate<string>)(x => x.Contains("userid"))).Split('=')[1];
            string password = list.Find((Predicate<string>)(x => x.Contains("password"))).Split('=')[1];
            string dbName = list.Find((Predicate<string>)(x => x.Contains("database"))).Split('=')[1];
            string connectionString = "server=" + server + ";port=" + port + ";user=" + userid + ";password=" + password + ";";
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();
                new MySqlCommand("SET GLOBAL event_scheduler = ON", connection).ExecuteNonQuery();
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter("SELECT * FROM information_schema.SCHEMATA where SCHEMA_NAME='" + dbName + "';", connection);
                DataSet dataSet = new DataSet();
                mySqlDataAdapter.Fill(dataSet);
                if (dataSet.Tables[0].Rows.Count > 0)
                    return true;
                new MySqlCommand("CREATE DATABASE " + dbName + ";", connection).ExecuteNonQuery();
                string messageBoxText = DatabaseHelper.RestoreDB(server, port, userid, password, dbName);
                if (messageBoxText != "")
                {
                    MessageBox.Show(messageBoxText);
                }
                return messageBoxText == "";
            }
            catch (Exception ex)
            {
                connection.Close();
                MessageBox.Show(ex.Message + "\r\n" + connectionString);
                return false;
            }
        }

        public void FixedTimeBackupDB()
        {
            if (DateTime.Compare(Convert.ToDateTime(this.lastRunTime), Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"))) >= 0 
                || DateTime.Now.Hour != 20 || DateTime.Now.Minute != 15)
                return;

            this.lastRunTime = DateTime.Now.ToString("yyyy-MM-dd");
            List<string> list = ((IEnumerable<string>)this._connectStr.Split(';')).ToList<string>();
            string server = list.Find((Predicate<string>)(x => x.Contains("server"))).Split('=')[1];
            string port = list.Find((Predicate<string>)(x => x.Contains("port"))).Split('=')[1];
            string userid = list.Find((Predicate<string>)(x => x.Contains("userid"))).Split('=')[1];
            string password = list.Find((Predicate<string>)(x => x.Contains("password"))).Split('=')[1];
            string database = list.Find((Predicate<string>)(x => x.Contains("database"))).Split('=')[1];
            //"server=" + server + ";port=" + port + ";user=" + userid + ";password=" + password + ";";
            Task.Run(() => { DatabaseHelper.BackupDB(server, port, userid, password, database); });
        }

        public bool Exists(string strSql)
        {
            object single = this.GetSingle(strSql);
            return (!object.Equals(single, (object)null) && !object.Equals(single, (object)DBNull.Value) ? int.Parse(single.ToString()) : 0) != 0;
        }

        public bool Exists(string strSql, params IDbDataParameter[] cmdParms)
        {
            object single = this.GetSingle(strSql, cmdParms);
            return (!object.Equals(single, (object)null) && !object.Equals(single, (object)DBNull.Value) ? int.Parse(single.ToString()) : 0) != 0;
        }

        public string CreatePagingSql(int recordCount, int pageSize, int pageIndex, string safeSql, string orderField)
        {
            return new DBFactory(this._DBType).CreatePagingInstance().CreatePagingSql(recordCount, pageSize, pageIndex, safeSql, orderField);
        }

        public string CreateCountingSql(string safeSql)
        {
            return new DBFactory(this._DBType).CreatePagingInstance().CreateCountingSql(safeSql);
        }

        public IDbDataParameter CreateDBParameter(string paraName, DataType dataType, int size = 0)
        {
            return new DBFactory(this._DBType).CreateDBParameter(paraName, dataType, size);
        }

        public object GetSingle(string sql)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                using (IDbCommand command = instance.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Connection = connection;
                    try
                    {
                        connection.Open();
                        object objA = command.ExecuteScalar();
                        return object.Equals(objA, (object)null) || object.Equals(objA, (object)DBNull.Value) ? (object)null : objA;
                    }
                    catch (Exception ex)
                    {
                        connection.Close();
                        throw ex;
                    }
                }
            }
        }

        public object GetSingle(string sql, int timeout)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                using (IDbCommand command = instance.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Connection = connection;
                    command.CommandTimeout = timeout;
                    try
                    {
                        connection.Open();
                        object objA = command.ExecuteScalar();
                        return object.Equals(objA, (object)null) || object.Equals(objA, (object)DBNull.Value) ? (object)null : objA;
                    }
                    catch (Exception ex)
                    {
                        connection.Close();
                        throw ex;
                    }
                }
            }
        }

        public int ExecuteSql(string sql)
        {
            sql = sql.Replace("\\", "\\\\");
            sql = sql.Replace("'t", "\\'t");
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                using (IDbCommand command = instance.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Connection = connection;
                    try
                    {
                        connection.Open();
                        return command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public int ExecuteSql(string sql, int timeout)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                using (IDbCommand command = instance.CreateCommand())
                {
                    command.CommandText = sql;
                    command.Connection = connection;
                    command.CommandTimeout = timeout;
                    try
                    {
                        connection.Open();
                        return command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        connection.Close();
                        throw ex;
                    }
                }
            }
        }

        public int ExecuteSqlTran(List<string> sqlList)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                connection.Open();
                using (IDbCommand command = instance.CreateCommand())
                {
                    command.Connection = connection;
                    using (IDbTransaction transaction = instance.CreateTransaction(connection))
                    {
                        command.Transaction = transaction;
                        try
                        {
                            int num = 0;
                            for (int index = 0; index < sqlList.Count; ++index)
                            {
                                string sql = sqlList[index];
                                if (sql.Trim().Length > 1)
                                {
                                    command.CommandText = sql;
                                    num += command.ExecuteNonQuery();
                                }
                            }
                            transaction.Commit();
                            return num;
                        }
                        catch
                        {
                            transaction.Rollback();
                            return 0;
                        }
                    }
                }
            }
        }

        public DataSet Query(string sql)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                DataSet dataSet = new DataSet();
                IDbDataAdapter dbDataAdapter = (IDbDataAdapter)null;
                try
                {
                    connection.Open();
                    using (IDbCommand command = instance.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Connection = connection;
                        dbDataAdapter = instance.CreateDataAdapter();
                        dbDataAdapter.SelectCommand = command;
                        dbDataAdapter.Fill(dataSet);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    GC.SuppressFinalize((object)dbDataAdapter);
                }
                return dataSet;
            }
        }

        public DataSet Query(string sql, int timeout)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                DataSet dataSet = new DataSet();
                IDbDataAdapter dbDataAdapter = (IDbDataAdapter)null;
                try
                {
                    connection.Open();
                    using (IDbCommand command = instance.CreateCommand())
                    {
                        command.CommandText = sql;
                        command.Connection = connection;
                        command.CommandTimeout = timeout;
                        dbDataAdapter = instance.CreateDataAdapter();
                        dbDataAdapter.SelectCommand = command;
                        dbDataAdapter.Fill(dataSet);
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    GC.SuppressFinalize((object)dbDataAdapter);
                }
                return dataSet;
            }
        }

        public int ExecuteSql(string sql, params IDbDataParameter[] cmdParms)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                using (IDbCommand command = instance.CreateCommand())
                {
                    try
                    {
                        DatabaseHelper.PrepareCommand(command, connection, (IDbTransaction)null, sql, cmdParms);
                        int num = command.ExecuteNonQuery();
                        command.Parameters.Clear();
                        return num;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public int ExecuteSql(string sql, int timeout, params IDbDataParameter[] cmdParms)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                using (IDbCommand command = instance.CreateCommand())
                {
                    try
                    {
                        command.CommandTimeout = timeout;
                        DatabaseHelper.PrepareCommand(command, connection, (IDbTransaction)null, sql, cmdParms);
                        int num = command.ExecuteNonQuery();
                        command.Parameters.Clear();
                        return num;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public int ExecuteSqlTran(Hashtable sqlList)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                connection.Open();
                using (IDbTransaction transaction = instance.CreateTransaction(connection))
                {
                    IDbCommand command = instance.CreateCommand();
                    try
                    {
                        int num1 = 0;
                        foreach (DictionaryEntry sql in sqlList)
                        {
                            string cmdText = sql.Key.ToString();
                            IDbDataParameter[] cmdParms = (IDbDataParameter[])sql.Value;
                            DatabaseHelper.PrepareCommand(command, connection, transaction, cmdText, cmdParms);
                            int num2 = command.ExecuteNonQuery();
                            num1 += num2;
                            command.Parameters.Clear();
                        }
                        transaction.Commit();
                        return num1;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public object GetSingle(string strSql, params IDbDataParameter[] cmdParms)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                using (IDbCommand command = instance.CreateCommand())
                {
                    try
                    {
                        DatabaseHelper.PrepareCommand(command, connection, (IDbTransaction)null, strSql, cmdParms);
                        object objA = command.ExecuteScalar();
                        command.Parameters.Clear();
                        return object.Equals(objA, (object)null) || object.Equals(objA, (object)DBNull.Value) ? (object)null : objA;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public object GetSingle(string strSql, int timeout, params IDbDataParameter[] cmdParms)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                using (IDbCommand command = instance.CreateCommand())
                {
                    try
                    {
                        command.CommandTimeout = timeout;
                        DatabaseHelper.PrepareCommand(command, connection, (IDbTransaction)null, strSql, cmdParms);
                        object objA = command.ExecuteScalar();
                        command.Parameters.Clear();
                        return object.Equals(objA, (object)null) || object.Equals(objA, (object)DBNull.Value) ? (object)null : objA;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }

        public DataSet Query(string strSql, params IDbDataParameter[] cmdParms)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                using (IDbCommand command = instance.CreateCommand())
                {
                    DatabaseHelper.PrepareCommand(command, connection, (IDbTransaction)null, strSql, cmdParms);
                    IDbDataAdapter dataAdapter = instance.CreateDataAdapter();
                    dataAdapter.SelectCommand = command;
                    DataSet dataSet = new DataSet();
                    try
                    {
                        dataAdapter.Fill(dataSet);
                        command.Parameters.Clear();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        GC.SuppressFinalize((object)dataAdapter);
                    }
                    return dataSet;
                }
            }
        }

        private static void PrepareCommand(
          IDbCommand cmd,
          IDbConnection conn,
          IDbTransaction trans,
          string cmdText,
          IDbDataParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;
            if (cmdParms == null)
                return;
            foreach (IDbDataParameter cmdParm in cmdParms)
            {
                if ((cmdParm.Direction == ParameterDirection.InputOutput || cmdParm.Direction == ParameterDirection.Input) && cmdParm.Value == null)
                    cmdParm.Value = (object)DBNull.Value;
                cmd.Parameters.Add((object)cmdParm);
            }
        }

        public DataSet RunProcedure(string storedProcName, IDbDataParameter[] parameters)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                DataSet dataSet = new DataSet();
                IDbDataAdapter dbDataAdapter = (IDbDataAdapter)null;
                try
                {
                    connection.Open();
                    dbDataAdapter = instance.CreateDataAdapter();
                    using (IDbCommand dbCommand = DatabaseHelper.BuildQueryCommand(instance, connection, storedProcName, parameters))
                    {
                        dbDataAdapter.SelectCommand = dbCommand;
                        dbDataAdapter.Fill(dataSet);
                        connection.Close();
                    }
                    return dataSet;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    GC.SuppressFinalize((object)dbDataAdapter);
                }
            }
        }

        public DataSet RunProcedure(string storedProcName, IDbDataParameter[] parameters, int timeout)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                DataSet dataSet = new DataSet();
                IDbDataAdapter dbDataAdapter = (IDbDataAdapter)null;
                try
                {
                    connection.Open();
                    dbDataAdapter = instance.CreateDataAdapter();
                    using (IDbCommand dbCommand = DatabaseHelper.BuildQueryCommand(instance, connection, storedProcName, parameters))
                    {
                        dbCommand.CommandTimeout = timeout;
                        dbDataAdapter.SelectCommand = dbCommand;
                        dbDataAdapter.Fill(dataSet);
                        connection.Close();
                    }
                    return dataSet;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    GC.SuppressFinalize((object)dbDataAdapter);
                }
            }
        }
        public int RunProcedure(
        string storedProcName,
        IDbDataParameter[] parameters,
          out int rowsAffected)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                connection.Open();
                int num;
                using (IDbCommand dbCommand = DatabaseHelper.BuildIntCommand(instance, connection, storedProcName, parameters))
                {
                    rowsAffected = dbCommand.ExecuteNonQuery();
                    num = (int)((IDataParameter)dbCommand.Parameters["ReturnValue"]).Value;
                }
                return num;
            }
        }

        public void RunProcedureVoid(string storedProcName, IDbDataParameter[] parameters)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                try
                {
                    connection.Open();
                    DatabaseHelper.BuildIntCommand(instance, connection, storedProcName, parameters).ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public object RunProcedureReturnObj(string storedProceName, IDbDataParameter[] parameters)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                try
                {
                    connection.Open();
                    using (IDbCommand dbCommand = DatabaseHelper.BuildIntCommand(instance, connection, storedProceName, parameters))
                        return dbCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public int GetReturnValueRunProcedure(string storedProcName, IDbDataParameter[] parameters)
        {
            IDBFactory instance = new DBFactory(this._DBType).CreateInstance();
            using (IDbConnection connection = instance.CreateConnection(this._connectStr))
            {
                try
                {
                    connection.Open();
                    using (IDbCommand dbCommand = DatabaseHelper.BuildIntCommand(instance, connection, storedProcName, parameters))
                    {
                        dbCommand.ExecuteNonQuery();
                        return (int)((IDataParameter)dbCommand.Parameters["ReturnValue"]).Value;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        private static IDbCommand BuildQueryCommand(
          IDBFactory myDBFactory,
          IDbConnection connection,
          string storedProcName,
          IDbDataParameter[] parameters)
        {
            IDbCommand command = myDBFactory.CreateCommand();
            command.CommandText = storedProcName;
            command.CommandType = CommandType.StoredProcedure;
            command.Connection = connection;
            foreach (IDbDataParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) && parameter.Value == null)
                        parameter.Value = (object)DBNull.Value;
                    command.Parameters.Add((object)parameter);
                }
            }
            return command;
        }

        private static IDbCommand BuildIntCommand(
          IDBFactory myDBFactory,
          IDbConnection connection,
          string storedProcName,
          IDbDataParameter[] parameters)
        {
            IDbCommand command = myDBFactory.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = storedProcName;
            command.Connection = connection;
            command.Parameters.Add((object)parameters);
            command.Parameters.Add((object)myDBFactory.GetReturnValuePara());
            return command;
        }

        public static string BackupDB(
          string server,
          string port,
          string userid,
          string password,
          string dbName)
        {
            string mysqldumPath = "C:\\Program Files\\MariaDB 10.4\\bin";
            string path = AppDomain.CurrentDomain.BaseDirectory + "DBBackup";
            string strCmd = "mysqldump --host=" + server + " --default-character-set=utf8 --port=" + port + " --user=" + userid + " --password=" + password + " --hex-blob --single-transaction --triggers --events --routines " + string.Format("{0} > \"{1}\\{2}_{3:yyyyMMdd_HHmmss}.sql\"", (object)dbName, (object)path, (object)dbName, (object)DateTime.Now);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    if (Path.GetFileNameWithoutExtension(file) != dbName && (DateTime.Now - File.GetCreationTime(file)).TotalDays > 3.0)
                        File.Delete(file);
                }
            }
            return DatabaseHelper.RunCmd(mysqldumPath, strCmd);
        }

        public static string RestoreDB(
          string server,
          string port,
          string userid,
          string password,
          string dbName)
        {
            string mysqldumPath = "C:\\Program Files\\MariaDB 10.4\\bin";
            string path = AppDomain.CurrentDomain.BaseDirectory + "DBBackup\\" + dbName + ".sql";
            string strCmd = "mysql --host=" + server + " --default-character-set=utf8 --port=" + port + " --user=" + userid + " --password=" + password + "  " + dbName + "  < \"" + path + "\"";
            return File.Exists(path) ? DatabaseHelper.RunCmd(mysqldumPath, strCmd) : path + "文件不存在";
        }

        private static string RunCmd(string mysqldumPath, string strCmd)
        {
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WorkingDirectory = mysqldumPath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.StandardInput.WriteLine(strCmd);
            process.StandardInput.WriteLine("exit");
            process.StandardOutput.ReadToEnd();
            string end = process.StandardError.ReadToEnd();
            process.WaitForExit();
            process.Close();
            return end;
        }
    }
}
