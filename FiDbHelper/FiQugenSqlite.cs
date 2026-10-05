using OrakUtilDotNetCore.FiConfig;
using OrakUtilDotNetCore.FiContainer;
using OrakUtilDotNetCore.FiCore;
using OrakUtilDotNetCore.FiMetas;
using OrakUtilDotNetCore.FiOrm;

namespace OrakUtilSqliteCore.FiDbHelper;

using System;
using System.Collections.Generic;
using System.Text;
public static class FiQugenSqlite
{
  public static string GenCreateTableIfNotExist(Fkf fkfAll)
  {
    string tempQuery = @"
CREATE TABLE IF NOT EXISTS {{tableName}} (
{{tableFields}} 
);  
";

    //CREATE TABLE ornek (
    // id INTEGER PRIMARY KEY,
    // ad TEXT
    // );

    StringBuilder sbFields = new StringBuilder();

    //FicList ficList = ifiTbl.GenITableCols();

    int index = 0;
    foreach (KeyValuePair<string, FiCol> keyValuePair in fkfAll)
    {
      FiCol fiCol = keyValuePair.Value;

      if (fiCol.IsTransient()) continue;

      if (index > 0)
      {
        sbFields.Append(",");
      }
      //if (fiCol.ofcTxTxSqlFieldDefinition() == null) {
      sbFields.Append(fiCol.fcTxFieldName) // DbFieldName alınmalı
        .Append(" ")
        .Append(ConvertColTypeToDbType(fiCol.fcTxFieldType))
        .Append(GetLengthDef(fiCol));

      if (FiString.OrEmpty(fiCol.fcTxIdType).Equals("identity"))
      {
        sbFields.Append(" PRIMARY KEY AUTOINCREMENT");
      }

      if (FiString.OrEmpty(fiCol.fcTxIdType).Equals("user-assign"))
      {
        sbFields.Append(" PRIMARY KEY");
      }

      sbFields.Append("\n");
      index++;
    }

    fkfAll.TryGetValue(FimFtSpecFields.QcfTxSqTableName().ftTxKey, out FiCol? qcfTxSqTableName);

    if (qcfTxSqTableName == null)
    {
      return "-- table name not found";
    }

    Fkb fkbParams = new Fkb();
    fkbParams.Add("tableName", qcfTxSqTableName.fcTxHeader);
    fkbParams.Add("tableFields", sbFields.ToString());

    string createQuery = FiTemplate.ReplaceTemplateParams(tempQuery, fkbParams);

    return createQuery;
  }

  private static string ConvertColTypeToDbType(string ofcTxColType)
  {
    if (ofcTxColType == null) return "-- null type";
    if (ofcTxColType.Equals("tint", StringComparison.InvariantCultureIgnoreCase)) return "TINYINT";
    if (ofcTxColType.Equals("int", StringComparison.InvariantCultureIgnoreCase)) return "INTEGER";

    return ofcTxColType;
  }

  /**
   * Alanın genel tipini verir int,text,decimal gibi
   */
  private static string ConvertColTypeToGeneralType(string fcTxColType)
  {
    if (fcTxColType == null) return "";

    if (fcTxColType.Equals("tint", StringComparison.InvariantCultureIgnoreCase)
      || fcTxColType.Equals("int", StringComparison.InvariantCultureIgnoreCase)
    ) return "INTEGER";

    if (fcTxColType.Equals("nvarchar", StringComparison.InvariantCultureIgnoreCase)
      || fcTxColType.Equals("varchar", StringComparison.InvariantCultureIgnoreCase)
    ) return "TEXT";

    if (fcTxColType.Equals("double", StringComparison.InvariantCultureIgnoreCase)
      || fcTxColType.Equals("float", StringComparison.InvariantCultureIgnoreCase)
      || fcTxColType.Equals("decimal", StringComparison.InvariantCultureIgnoreCase)
    ) return "DECIMAL";

    return fcTxColType;
  }

  private static string GetLengthDef(FiCol fiCol)
  {
    string generalType = FiString.OrEmpty(ConvertColTypeToGeneralType(fiCol.fcTxFieldType));

    if (generalType.Equals("TEXT") && fiCol.fcLnLength != null)
    {
      return $"({fiCol.fcLnLength})";
    }

    if (generalType.Equals("DECIMAL") && fiCol.fcLnPrecision != null)
    {
      return $"({fiCol.fcLnPrecision},{FiNumber.OrIntZero(fiCol.fcLnScale)})";
    }

    return "";
  }

  public static String InsertFics(List<FiCol> listFields, bool? boInsertFieldsOnly)
  {
    FimFtSql.SfTableName();
    FimFtSql.SfFields();
    FimFtSql.SfTxFieldsVar();

    String template = "INSERT INTO {{sfTableName}} ( {{sfTxFields}} ) \n"
      + " VALUES ( {{sfTxFieldsVar}} )";

    StringBuilder sbFields = new StringBuilder();
    StringBuilder sbVars = new StringBuilder();

    int indexFields = 1;
    int indexParams = 1;

    foreach (FiCol fiCol in listFields)
    {

      if (fiCol.IsPrimaryKey()) continue;

      if (FiBool.IsTrue(boInsertFieldsOnly))
      {

        if (FiBool.IsTrue(fiCol.boInsertCol))
        {

          if (indexFields != 1) sbFields.Append(", ");
          sbFields.Append(fiCol.fcTxFieldName);

          if (indexParams != 1) sbVars.Append(", ");
          sbVars.Append("@").Append(fiCol.fcTxFieldName);

          indexFields++;
          indexParams++;
        }

      }
      else
      {

        if (indexFields != 1) sbFields.Append(", ");
        sbFields.Append(fiCol.fcTxFieldName);

        if (indexParams != 1) sbVars.Append(", ");
        sbVars.Append("@").Append(fiCol.fcTxFieldName);

        indexFields++;
        indexParams++;
      }

    }

    Fkb fkbTemplate = new Fkb();
    fkbTemplate.AddFim(FimFtSql.SfTableName(), "");
    fkbTemplate.AddFim(FimFtSql.SfFields(), sbFields.ToString());
    fkbTemplate.AddFim(FimFtSql.SfTxFieldsVar(), sbVars.ToString());

    return FiTemplate.ReplaceTemplateParams(template, fkbTemplate);
  }


  public static String UpdateFiColsByIdentKey(FiQuery fiQuery)
  {
    //if(1==1) return "test";
    String template = @"UPDATE {{tableName}} SET {{csvFields}}  
WHERE {{txWhere}} ";

    StringBuilder queryFields = new StringBuilder();
    StringBuilder sbWhereFields = new StringBuilder();

    int indexUpFields = 1;
    int indexWhere = 1;

    foreach (FiCol fiCol in fiQuery.ficListCol)
    {
      //if (fiCol.CheckFiColIfPrimaryKey()) continue;
      //if (FiBool.IsTrue(fiQuery.boUseUpdateFieldsOnly))

      if (fiCol.CheckFiColIfIdentityPrimaryKey())
      {
        if (indexWhere != 1) sbWhereFields.Append(", ");
        sbWhereFields.Append(fiCol.fcTxFieldName)
          .Append("= @").Append(fiCol.fcTxFieldName);
        indexWhere++;
        continue;
      }

      if (indexUpFields != 1) queryFields.Append(", ");

      queryFields.Append(fiCol.fcTxFieldName)
        .Append("= @")
        .Append(fiCol.fcTxFieldName);

      indexUpFields++;
    }

    Fkb fkbTemplate = new Fkb();
    fkbTemplate.Add("tableName", fiQuery.fiTableMeta.GetITxTableName());
    fkbTemplate.Add("csvFields", queryFields.ToString());
    fkbTemplate.Add("txWhere", sbWhereFields.ToString());

    return FiTemplate.ReplaceTemplateParams(template, fkbTemplate);
  }


//   public static string SelectAll1(IFiTableMeta ifiTbl)
//   {
//     // tpl:template
//     string txQueryTpl = $@"
// SELECT *
// FROM {FicOksCoding.OkTableName().fnmTemplate()}
// "; //
//
//     Fkb fkbParams = new Fkb();
//     fkbParams.AddFic(FicOksCoding.OkTableName(), ifiTbl.GetITxTableName());
//
//     string query = FiTemplate.ReplaceTemplateParameters(txQueryTpl.Trim(), fkbParams);
//
//     FiAppConfig.fiLog?.Debug(query);
//
//     return query;
//   }


  public static string SelectAllV2(Fkf fkfAll)
  {
    string txTableName = fkfAll.GetFimHeaderNtn(FimFtSpecFields.QcfTxSqTableName());

    // tpl:template
    string txQueryTpl = $@"
SELECT * 
FROM {txTableName}
"; //

    Fkb fkbParams = new Fkb();
    //fkbParams.AddFic(FicOksCoding.OkTableName(), fiTbl.GetITxTableName());

    string query = FiTemplate.ReplaceTemplateParams(txQueryTpl.Trim(), fkbParams);

    FiAppConfig.fiLog?.Debug(query);

    return query;
  }

  public static Fdr DeleteWhereIdCols(Fkf? fkfAll)
  {
    Fdr fdrMain = new Fdr();

    if (fkfAll == null) return fdrMain;

    String template = $@"
DELETE FROM  {FimFtSql.SfTableName().getTempVar()}
WHERE {FimFtSql.SfWhere().getTempVar()}
".Trim();

    StringBuilder sbTxWhere = new StringBuilder();

    int indexForPriKey = 1;
    foreach (KeyValuePair<string, FiCol> fiCol in fkfAll)
    {

      if (fiCol.Value.IsPrimaryKey())
      {
        if (indexForPriKey != 1) sbTxWhere.Append(", ");
        sbTxWhere.Append(fiCol.Value.GetTxDbFieldOrTxFieldName());
        sbTxWhere.Append(" = @").Append(fiCol.Value.fcTxFieldName);
        indexForPriKey++;
        continue;
      }

    }

    // Where cümleciği gelmemişse
    if (FiString.IsEmpty(sbTxWhere.ToString()))
    {
      fdrMain.fdBoResult = false;
      fdrMain.refValue = "error:no-where condition";
      return fdrMain;
    }

    string tableName = fkfAll.GetTableName();
    if (FiString.IsEmpty(tableName))
    {
      fdrMain.fdBoResult = false;
      fdrMain.refValue = "error:no-table-name";
      return fdrMain;
    }


    Fkb fkbTemplate = new Fkb();

    fkbTemplate.AddFim(FimFtSql.SfTableName(), tableName);
    fkbTemplate.AddFim(FimFtSql.SfWhere(), sbTxWhere.ToString());

    fdrMain.fdBoResult = true;
    fdrMain.refValue = FiTemplate.ReplaceTemplateParams(template, fkbTemplate);

    return fdrMain;
  }





  /**
   * fkfAll kullanarak Insert sorgusu üretir
   *
   * Table ismini qcfTxSqTableName alanından alır. Eğer bu alan yoksa hata döner.
   *
   * Sorguyu fiQuery.sql alanına yazar
   */
  public static Fdr GenInsertV1(FiQuery fiQuery)
  {
    Fdr fdrMain = new Fdr();

    string tableName = fiQuery.GetFkfAllInit().GetTableName();

    if (FiString.IsEmpty(tableName))
    {
      fdrMain.fdBoResult = false;
      fdrMain.txMessage = "error:no-table-name";
      return fdrMain;
    }

    String tempInsert = $@"INSERT INTO {FimFtSql.SfTableName().getTempVar()} 
  ( {FimFtSql.SfFields().getTempVar()} )
  VALUES ( {FimFtSql.SfFieldsVar().getTempVar()} )";

    StringBuilder sbFields = new StringBuilder();
    StringBuilder sbVars = new StringBuilder();

    //int indexFields = 1;

    foreach (KeyValuePair<string, FiCol> pair in fiQuery.GetFkfAllInit())
    {
      if (pair.Value.IsIdAutoIncrement()) continue;
      if (pair.Value.IsTransient()) continue;

      // if (FiBool.IsTrue(boInsertFieldsOnly))
      // {
      //   if (FiBool.IsTrue(pair.boInsertCol))
      //   {
      //     if (indexFields != 1) queryFields.Append(", ");
      //     queryFields.Append(pair.fcTxFieldName);
      //
      //     if (indexParams != 1) queryParams.Append(", ");
      //     queryParams.Append("@").Append(pair.fcTxFieldName);
      //
      //     indexFields++;
      //     indexParams++;
      //   }
      //
      // }
      // else

      sbFields.Append(FiQugenUtil.FormSqlFieldCommaByFic(pair.Value));
      sbVars.Append(FiQugenUtil.FormSqlVarCommaByFic(pair.Value));

      //indexFields++;
    }

    FiString.RTrim(sbFields, FiQugenUtil.GetTxComma());
    FiString.RTrim(sbVars, FiQugenUtil.GetTxComma());

    Fkb fkbTemplate = new Fkb();

    fkbTemplate.AddFim(FimFtSql.SfTableName(), tableName);
    fkbTemplate.AddFim(FimFtSql.SfFields(), sbFields.ToString());
    fkbTemplate.AddFim(FimFtSql.SfFieldsVar(), sbVars.ToString());

    string sql = FiTemplate.ReplaceTemplateParams(tempInsert, fkbTemplate);

    fiQuery.sql = sql;
    fdrMain.fdTxVal = sql;
    fdrMain.fdBoResult = true;
    return fdrMain;
  }

  /**
   * fkfAll kullanarak Select sorgusu üretir
   */
  public static Fdr GenSelectAllV1(FiQuery fiQuery)
  {
    Fdr fdrMain = new Fdr();

    string tableName = fiQuery.GetFkfAllInit().GetTableName();

    if (FiString.IsEmpty(tableName))
    {
      fdrMain.fdBoResult = false;
      fdrMain.txMessage = "error:no-table-name";
      return fdrMain;
    }

    String tempInsert = $@"SELECT {FimFtSql.SfFields().getTempVar()} 
  FROM {FimFtSql.SfTableName().getTempVar()}";

    StringBuilder sbFields = new StringBuilder();

    foreach (KeyValuePair<string, FiCol> pair in fiQuery.GetFkfAllInit())
    {
      if (pair.Value.IsTransient()) continue;

      sbFields.Append(FiQugenUtil.FormSqlFieldCommaByFic(pair.Value));
    }

    FiString.RTrim(sbFields, FiQugenUtil.GetTxComma());

    Fkb fkbTemplate = new Fkb();

    fkbTemplate.AddFim(FimFtSql.SfTableName(), tableName);
    fkbTemplate.AddFim(FimFtSql.SfFields(), sbFields.ToString());

    string sql = FiTemplate.ReplaceTemplateParams(tempInsert, fkbTemplate);

    fiQuery.sql = sql;
    fdrMain.fdTxVal = sql;
    fdrMain.fdBoResult = true;
    return fdrMain;
  }

} // end class