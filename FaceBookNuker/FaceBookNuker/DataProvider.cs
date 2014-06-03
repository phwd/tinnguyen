﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.Data;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Windows.Forms;
using System.IO;
using FaceBookNuker.Models;
public class DataProvider
{
    public static SSCVNContext SSCVNDB = new SSCVNContext();
    public static DBContext DB = new DBContext();

    private static DataProvider provider;
    public static DataProvider Provider
    {
        get
        {
            if (provider == null)
                provider = new DataProvider();
            return provider;
        }
    }
    private SQLiteConnection sqlConnection;
    private SQLiteCommand sqlCommand;
    private SQLiteDataAdapter sqlDataAdapter;
    public DataProvider()
    {
        SetConnection();
    }
    private void SetConnection()
    {
        sqlConnection = new SQLiteConnection("Data Source=" + Path.GetDirectoryName(Application.ExecutablePath) + "\\db.db;");
    }
    private bool openConn()
    {
        if (sqlConnection.State == System.Data.ConnectionState.Closed)
        {
            try
            {
                sqlConnection.Open();
            }
            catch
            {
                return false;
            }
        }
        return true;
    }
    private bool closeConn()
    {
        if (sqlConnection.State == System.Data.ConnectionState.Open)
        {
            try
            {
                sqlConnection.Close();
            }
            catch
            {
                return false;
            }
        }
        return true;
    }
    public DataTable GetData(string strSQL, string strTableName)
    {
        DataTable dtReturn = null;
        try
        {
            if (openConn())
            {
                sqlCommand = sqlConnection.CreateCommand();
                sqlDataAdapter = new SQLiteDataAdapter(strSQL, sqlConnection);
                DataSet ds = new DataSet();
                sqlDataAdapter.Fill(ds);
                closeConn();
                dtReturn = ds.Tables[0];
                dtReturn.TableName = strTableName;
            }
        }
        catch
        {
            closeConn();
        }
        return dtReturn;
    }
    public DataTable GetTableStruct(string strTableName)
    {
        DataTable dtReturn = null;
        try
        {
            if (openConn())
            {
                sqlCommand = sqlConnection.CreateCommand();
                sqlDataAdapter = new SQLiteDataAdapter(string.Format("SELECT * FROM {0} WHERE 1 = 2 ", strTableName), sqlConnection);
                DataSet ds = new DataSet();
                sqlDataAdapter.Fill(ds);
                closeConn();
                dtReturn = ds.Tables[0];
                dtReturn.TableName = strTableName;
            }
        }
        catch
        {
            closeConn();
        }
        return dtReturn;
    }
    private bool ExcuteSQL(string strSQL)
    {
        int objReturn = 0;
        try
        {
            if (openConn())
            {
                sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = strSQL;
                objReturn = sqlCommand.ExecuteNonQuery();
                closeConn();
            }
        }
        catch
        {
            closeConn();
        }
        return objReturn == 1;
    }
    public bool Execute(DataTable dtExcute)
    {
        sqlDataAdapter = null;
        try
        {
            if (openConn())
            {
                sqlDataAdapter = new SQLiteDataAdapter(string.Format("SELECT * FROM [{0}]", dtExcute.TableName), sqlConnection);
                SQLiteCommandBuilder cmdBd = new SQLiteCommandBuilder(sqlDataAdapter);
                cmdBd.ConflictOption = ConflictOption.OverwriteChanges;
                sqlDataAdapter.DeleteCommand = cmdBd.GetDeleteCommand(true);
                sqlDataAdapter.InsertCommand = cmdBd.GetInsertCommand(true);
                sqlDataAdapter.UpdateCommand = cmdBd.GetUpdateCommand(true);
                int iRS = sqlDataAdapter.Update(dtExcute);
            }
        }
        catch
        {
            closeConn();
            return false;
        }
        return true;
    }
}
