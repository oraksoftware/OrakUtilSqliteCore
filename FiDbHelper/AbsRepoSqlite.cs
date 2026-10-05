using OrakUtilDotNetCore.FiContainer;
using OrakUtilDotNetCore.FiOrm;

namespace OrakUtilSqliteCore.FiDbHelper
{


  public abstract class AbsRepoSqlite : IRepoSqLite
  {
    public string connProfile { get; set; }

    //protected IFiTableMeta fiTableMeta { get; set; }

    // protected Fkf fkfAll { get; set; }

    // protected FiCol? qcfTxSqTableName { get; set; }

    public AbsRepoSqlite()
    {

    }

    public virtual Fkf GetFkfAll()
    {
      return new Fkf();
    }

    public virtual FicList GetFclDto()
    {
      return new FicList();
    }

    public virtual FiCol? GetQcfTxSqTableName()
    {
      return null;
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

    // public Fdr AbsInsertV1(FiQuery fiQuery)
    // {
    //   fiQuery.fiTableMeta ??= fiTableMeta;
    //   string sql = FiQugenSqlite.InsertFiCols(fiQuery.fiTableMeta, fiQuery.ficListCol, fiQuery.boInsertFieldsOnly);
    //   //FiAppConfig.fiLog?.Debug("Query:"+ sql);
    //   fiQuery.sql = sql;
    //
    //   return GetDbHelper().SqlInsertQuery(fiQuery);
    // }

    /**
     * Repo tanımındaki GetFkfAll metodu ile sorgu oluşturur.
     */
    public Fdr FiInsertV1(Fkb fkbEntity)
    {
      //fiQuery.fkfAll = GetFkfAll();
      FiQuery fiQuery = new FiQuery();
      fiQuery.fkfAll = GetFkfAll();
      fiQuery.fkbParams = fkbEntity;

      Fdr fdrSql = FiQugenSqlite.GenInsertV1(fiQuery);

      if(!fdrSql.IsTrueBoResult()) return fdrSql;

      return GetDbHelper().SqlInsertQuery(fiQuery);
    }

    // public Fdr AbsUpdateByIdentKey(FiQuery fiQuery)
    // {
    //   fiQuery.fiTableMeta ??= fiTableMeta;
    //   string sql = FiQugenSqlite.UpdateFiColsByIdentKey(fiQuery);
    //   //FiAppConfig.fiLog?.Debug("Query:"+ sql);
    //   fiQuery.sql = sql;
    //
    //   return GetDbHelper().SqlInsertQuery(fiQuery);
    // }

    /**
     * Required Fields: FiTableMeta (Ftm)
     *
     * @return Fdr fdDtbVal
     */
    protected Fdr AbsSelectAllByFtm(FiQuery fiQuery)
    {
      //string sql = FiQugenSqlite.SelectAllV2();
      //FiAppConfig.fiLog?.Debug("Query:"+ sql);
      //fiQuery.sql = sql;


      return GetDbHelper().SqlSelectQueryAsDtb(fiQuery);
    }

    /**
     * Required Fields: FfkAll (parent)
     *
     * @return Fdr fdDtbVal
     */
    protected Fdr AbsSelectAllV2()
    {
      //string sql = FiQugenSqlite.SelectAll1(this.fkfAll());
      //FiAppConfig.fiLog?.Debug("Query:"+ sql);
      //fiQuery.sql = sql;

      return null; // GetDbHelper().SqlSelectQueryAsDtb(fiQuery);
    }

    protected Fdr AbsDeleteById1(FiQuery fiQuery)
    {
      //if (fiQuery.fiTableMeta == null) fiQuery.fiTableMeta = fiTableMeta;

      var fdrSql = FiQugenSqlite.DeleteWhereIdCols(fiQuery.fkfAll);
      // MEDFIX burada fdrSql kontrolü eklenmeli
      fiQuery.sql = fdrSql.refValue?.ToString() ?? "";

      //fiQuery.LogQueryAndParams();

      return GetDbHelper().SqlDeleteQuery(fiQuery);
    }
  }

}