using OrakUtilDotNetCore.FiContainer;
using OrakUtilDotNetCore.FiOrm;

namespace OrakUtilSqliteCore.FiDbHelper
{


  public class AbsRepoSqlite : IRepoSqLite
  {
    public string connProfile { get; set; }
    protected IFiTableMeta fiTableMeta { get; set; }

    public AbsRepoSqlite()
    {

    }

    protected AbsRepoSqlite(string connProfile)
    {
      this.connProfile = connProfile;
    }

    public FiSqLite GetDbHelper()
    {
      //Console.WriteLine($"dbhelper: {connProfile}");
      return FiSqLite.BuiWitProfile(connProfile);
    }

    public void CheckAndSetConnProfile()
    {
      // TODO metod yaz
    }

    public Fdr AbsInsert1(FiQuery fiQuery)
    {
      fiQuery.fiTableMeta ??= fiTableMeta;
      string sql = FiQugenSqlite.InsertFiCols(fiQuery.fiTableMeta, fiQuery.ficListCol, fiQuery.boInsertFieldsOnly);
      //FiAppConfig.fiLog?.Debug("Query:"+ sql);
      fiQuery.sql = sql;

      return GetDbHelper().SqlInsertQuery(fiQuery);
    }
    public Fdr AbsUpdateByIdentKey(FiQuery fiQuery)
    {
      fiQuery.fiTableMeta ??= fiTableMeta;
      string sql = FiQugenSqlite.UpdateFiColsByIdentKey(fiQuery);
      //FiAppConfig.fiLog?.Debug("Query:"+ sql);
      fiQuery.sql = sql;

      return GetDbHelper().SqlInsertQuery(fiQuery);
    }

    /**
     * Required Fields: FiTableMeta
     */
    protected Fdr AbsSelectAll1(FiQuery fiQuery)
    {
      string sql = FiQugenSqlite.SelectAll1(fiQuery.fiTableMeta);
      //FiAppConfig.fiLog?.Debug("Query:"+ sql);
      fiQuery.sql = sql;

      return GetDbHelper().SqlSelectQuery(fiQuery);
    }
    protected Fdr AbsDeleteById1(FiQuery fiQuery)
    {
      if (fiQuery.fiTableMeta == null) fiQuery.fiTableMeta = fiTableMeta;

      var fdrSql = FiQugenSqlite.DeleteWhereIdCols(fiQuery.fiTableMeta);
      // MEDFIX burada fdrSql kontrolü eklenmeli
      fiQuery.sql = fdrSql.refValue?.ToString() ?? "";

      //fiQuery.LogQueryAndParams();

      return GetDbHelper().SqlDeleteQuery(fiQuery);
    }
  }

}