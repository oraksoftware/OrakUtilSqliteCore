using OrakUtilDotNetCore.FiConfig;
using OrakUtilDotNetCore.FiContainer;
using OrakUtilDotNetCore.FiOrm;

namespace OrakUtilSqliteCore.FiDbHelper
{
  //using OrakYazilimLib.AdoNetHelper;
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.SQLite;


  public class FiSqLite
  {
    private string txConnString { get; set; } // private set vardı

    public static FiSqLite BuiWitProfile(string connProfile)
    {
      string prTxConnString = FiAppConfig.GetConnStringWthTest(connProfile);
      return new FiSqLite(prTxConnString);
    }


    /**
     *
     */
    public FiSqLite(string prTxConnString)
    {
      //string connstr = @"Data Source=Y:\sqlite-data\demo.db";
      this.txConnString = prTxConnString;
      //EnsureDatabaseExists();
    }
    /**
     * Database yoksa oluşturulur.
     */
    public Fdr EnsureDatabaseExists()
    {
      Fdr fdrMain = new Fdr();

      try
      {
        // Veritabanı dosyasını kontrol et.
        var connStringBuilder = new SQLiteConnectionStringBuilder(txConnString);
        string databaseFile = connStringBuilder.DataSource;

        if (!System.IO.File.Exists(databaseFile))
        {
          // Eğer dosya yoksa bir bağlantı açılarak yeni bir veritabanı oluşturulur.
          SQLiteConnection.CreateFile(databaseFile);
          fdrMain.SetBoExecAndResultTrue();
          FiAppConfig.fiLog?.Debug($"Veritabanı oluşturuldu: {databaseFile}");
          return fdrMain;
        }
        // Execution yapılmadı
        fdrMain.boResult = true;
        fdrMain.txMessage = "Veritabanı Mevcut.";
        FiAppConfig.fiLog?.Debug($"Veritabanı mevcut");
        return fdrMain;
      }
      catch (Exception ex)
      {
        fdrMain.SetBoExecAndResultFalse();
        FiAppConfig.fiLog?.Error($"Veritabanı kontrol/oluşturma sırasında bir hata oluştu: {ex.Message}");
        return fdrMain;
      }

    }

    public Fdr ExecNonQuery(string query)
    {
      FiQuery fiQuery = new FiQuery(query);
      return ExecNonQuery(fiQuery);
    }

    public Fdr SqlUpdateQuery(FiQuery fiQuery)
    {
      return ExecNonQuery(fiQuery);
    }

    public Fdr SqlInsertQuery(FiQuery fiQuery)
    {
      return ExecNonQuery(fiQuery);
    }

    public Fdr SqlDeleteQuery(FiQuery fiQuery)
    {
      return ExecNonQuery(fiQuery);
    }

    public Fdr ExecNonQuery(FiQuery fiQuery) //, params ParamItem[] parameters
    {
      Fdr fdrMain = new Fdr();

      using SQLiteConnection conn = new SQLiteConnection(this.txConnString);
      FiAppConfig.fiLog?.Debug(txConnString);

      using SQLiteCommand sqliteCmd = conn.CreateCommand();
      //Comm.Parameters.Clear();
      string query = FiQueryUtils.FixSqlProblems(fiQuery.sql);
      sqliteCmd.CommandText = query;
      //Comm.CommandType = CommandType.Text;

      try
      {
        conn.Open();
        AddSqlParametersToComm(fiQuery.fkbParams, sqliteCmd);
        int lnRowsAffected = sqliteCmd.ExecuteNonQuery();
        fdrMain.SetBoExecAndResultTrue();
        fdrMain.lnRowsAffected = lnRowsAffected;
        return fdrMain;
      }
      catch (Exception e)
      {
        fdrMain.SetBoExecAndResultFalse();
        FiAppConfig.fiLog?.Error($"{e.Message}");
        FiAppConfig.fiLog?.Error($"{e.StackTrace}");
        return fdrMain;
      }

    }

    public Fdr SqlSelectQuery(string query, FiKeybean fkbParams)
    {
      FiQuery fiQuery = new FiQuery(query, fkbParams);
      return SqlSelectQuery(fiQuery);
    }

    public Fdr SqlSelectQuery(FiQuery fiQuery)
    {
      Fdr fdrDtb = new Fdr();

      using SQLiteConnection conn = new SQLiteConnection(txConnString);
      using SQLiteCommand comm = conn.CreateCommand();

      try
      {
        //comm.Parameters.Clear();
        string query = FiQueryUtils.FixSqlProblems(fiQuery.sql);
        comm.CommandText = query;
        //comm.CommandType = CommandType.Text;
        AddSqlParametersToComm(fiQuery.fkbParams, comm);

        // Microsoft SqLite de yok
        SQLiteDataAdapter da = new SQLiteDataAdapter(comm);
        DataTable dt = new DataTable();
        da.Fill(dt);

        fdrDtb.SetBoExecAndResultTrue();
        fdrDtb.refValue = dt;
        fdrDtb.refDtbVal = dt;
        return fdrDtb;
      }
      catch (Exception e)
      {
        FiAppConfig.fiLog?.Error(e.Message);
        fdrDtb.boResult = false;
        return fdrDtb;
      }

    }
    private void AddSqlParametersToComm(FiKeybean fkbParams, SQLiteCommand comm)
    {

      if (fkbParams != null && fkbParams.Count > 0)
      {
        comm.Parameters.AddRange(GetArrSqlParams(fkbParams));
      }

    }

    public static SQLiteParameter[] GetArrSqlParams(FiKeybean fkbParams)
    {
      if (fkbParams == null) return new List<SQLiteParameter>().ToArray();

      List<SQLiteParameter> list = new List<SQLiteParameter>();

      foreach (var fkbItem in fkbParams)
      {
        string sqlParamName = "@" + fkbItem.Key;
        list.Add(new SQLiteParameter(sqlParamName, fkbItem.Value));
      }

      return list.ToArray();
    }

    public DataTable RunProc(string procName) //, params ParamItem[] parameters
    {
      //Comm.Parameters.Clear();
      //Comm.CommandText = procName;
      //Comm.CommandType = CommandType.StoredProcedure;

      //if (parameters != null && parameters.Length > 0)
      //{
      //	Comm.Parameters.AddRange(ProcessParameters(parameters));
      //}

      //DataTable dt = new DataTable();
      //SqlDataAdapter adapter = new SqlDataAdapter(Comm);
      //adapter.Fill(dt);

      //return dt;
      return null;
    }


  } // end class


//Comm.Parameters.Clear();
//Comm.CommandText = query;
//Comm.CommandType = CommandType.Text;

//if (parameters != null && parameters.Length > 0)
//{
//	Comm.Parameters.AddRange(ProcessParameters(parameters));
//}

//int result = 0;

//Conn.Open();
//try
//{
//	result = Comm.ExecuteNonQuery();
//	if (result == -1) result = 1;
//}
//catch (Exception e)
//{
//	Console.WriteLine(e);
//	result = -2;
//	//throw;
//}

//Conn.Close();

//return result;


// Open the connection for testing :
// using (var conn = new SQLiteConnection(pTxConnString))
// {
//   try
//   {
//     conn.Open();
//     FiAppConfig.fiLogManager?.LogMessage("connected");
//     boConnStatus = true;
//   }
//   catch (Exception e)
//   {
//     FiAppConfig.fiLogManager?.ErrorMessage(e.Message);
//     this.boConnStatus = false;
//     this.exConn = e;
//   }

//}


//private SQLiteParameter[] ProcessParameters(params ParamItem[] parameters)
//{
//	//SqlParameter[] pars = parameters.Select(x => new SqlParameter()
//	//{
//	//	ParameterName = x.ParamName,
//	//	Value = x.ParamValue
//	//}).ToArray();

//	//return pars;
//	return null;
//}


//Conn.Close();
//return result;

// public int ExecQuery(string query) //, params ParamItem[] parameters
// {
//   using (var conn = new SQLiteConnection(txConnString))
//   {
//     SQLiteCommand sqliteCmd = conn.CreateCommand();
//     sqliteCmd.CommandText = query;
//     // "INSERT INTO SampleTable (Col1, Col2) VALUES('Test Text ', 1); ";
//
//     var lnRowsAffected = sqliteCmd.ExecuteNonQuery();
//
//     return lnRowsAffected;
//   }
//
// }

//if (parameters != null && parameters.Length > 0)
//{
//	Comm.Parameters.AddRange(ProcessParameters(parameters));
//}

// "INSERT INTO SampleTable (Col1, Col2) VALUES('Test Text ', 1); ";
}