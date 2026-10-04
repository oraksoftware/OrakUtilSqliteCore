using OrakUtilDotNetCore.FiConfig;
using OrakUtilDotNetCore.FiContainer;
using OrakUtilDotNetCore.FiCore;
using OrakUtilDotNetCore.FiMetas;
using OrakUtilDotNetCore.FiOrm;

namespace OrakUtilSqliteCore.FiDbHelper
{
  using System;
  using System.Collections.Generic;
  using System.Text;

  public class FiQugenSqlite
  {
    public static string CreateTable(Fkf fkfAll)
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

        if(FiBool.IsTrue(fiCol.fcBoTransient))
        {
          continue;
        }

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

      if(qcfTxSqTableName == null)
      {
        return "-- table name not found";
      }

      Fkb fkbParams = new Fkb();
      fkbParams.Add("tableName", qcfTxSqTableName.fcTxHeader);
      fkbParams.Add("tableFields", sbFields.ToString());

      string createQuery = FiTemplate.ReplaceTemplateParameters(tempQuery, fkbParams);

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
    private static string ConvertColTypeToGeneralType(string ofcTxColType)
    {
      if (ofcTxColType == null) return "";

      if (ofcTxColType.Equals("tint", StringComparison.InvariantCultureIgnoreCase)
        || ofcTxColType.Equals("int", StringComparison.InvariantCultureIgnoreCase)
      ) return "INTEGER";

      if (ofcTxColType.Equals("nvarchar", StringComparison.InvariantCultureIgnoreCase)
        || ofcTxColType.Equals("varchar", StringComparison.InvariantCultureIgnoreCase)
      ) return "TEXT";

      if (ofcTxColType.Equals("double", StringComparison.InvariantCultureIgnoreCase)
        || ofcTxColType.Equals("float", StringComparison.InvariantCultureIgnoreCase)
        || ofcTxColType.Equals("decimal", StringComparison.InvariantCultureIgnoreCase)
      ) return "DECIMAL";

      return ofcTxColType;
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

    // insert

    public static String InsertFiCols(IFiTableMeta iFiTableMeta, List<FiCol> listFields, bool? boInserFieldsOnly)
    {

      String template = "INSERT INTO {{tableName}} ( {{csvFields}} ) \n"
        + " VALUES ( {{paramFields}} )";

      StringBuilder queryFields = new StringBuilder();
      StringBuilder queryParams = new StringBuilder();

      int indexFields = 1;
      int indexParams = 1;

      foreach (FiCol fiCol in listFields)
      {

        if (fiCol.CheckFiColIfPrimaryKey()) continue;

        if (FiBool.IsTrue(boInserFieldsOnly))
        {

          if (FiBool.IsTrue(fiCol.boInsertCol))
          {

            if (indexFields != 1) queryFields.Append(", ");
            queryFields.Append(fiCol.fcTxFieldName);

            if (indexParams != 1) queryParams.Append(", ");
            queryParams.Append("@").Append(fiCol.fcTxFieldName);

            indexFields++;
            indexParams++;
          }

        }
        else
        {

          if (indexFields != 1) queryFields.Append(", ");
          queryFields.Append(fiCol.fcTxFieldName);

          if (indexParams != 1) queryParams.Append(", ");
          queryParams.Append("@").Append(fiCol.fcTxFieldName);

          indexFields++;
          indexParams++;
        }

      }

      Fkb fkbTemplate = new Fkb();
      fkbTemplate.Add("tableName", iFiTableMeta.GetITxTableName());
      fkbTemplate.Add("csvFields", queryFields.ToString());
      fkbTemplate.Add("paramFields", queryParams.ToString());

      return FiTemplate.ReplaceTemplateParameters(template, fkbTemplate);
    }


    public static String InsertFics(List<FiCol> listFields, bool? boInsertFieldsOnly)
    {

      FimFtSql.SfTableName();
      FimFtSql.SfFields();
      FimFtSql.SfTxFieldsVar();

      String template = "INSERT INTO {{sfTableName}} ( {{sfTxFields}} ) \n"
        + " VALUES ( {{sfTxFieldsVar}} )";

      StringBuilder queryFields = new StringBuilder();
      StringBuilder queryParams = new StringBuilder();

      int indexFields = 1;
      int indexParams = 1;

      foreach (FiCol fiCol in listFields)
      {

        if (fiCol.CheckFiColIfPrimaryKey()) continue;

        if (FiBool.IsTrue(boInsertFieldsOnly))
        {

          if (FiBool.IsTrue(fiCol.boInsertCol))
          {

            if (indexFields != 1) queryFields.Append(", ");
            queryFields.Append(fiCol.fcTxFieldName);

            if (indexParams != 1) queryParams.Append(", ");
            queryParams.Append("@").Append(fiCol.fcTxFieldName);

            indexFields++;
            indexParams++;
          }

        }
        else
        {

          if (indexFields != 1) queryFields.Append(", ");
          queryFields.Append(fiCol.fcTxFieldName);

          if (indexParams != 1) queryParams.Append(", ");
          queryParams.Append("@").Append(fiCol.fcTxFieldName);

          indexFields++;
          indexParams++;
        }

      }

      Fkb fkbTemplate = new Fkb();
      fkbTemplate.AddFim(FimFtSql.SfTableName(), "");
      fkbTemplate.AddFim(FimFtSql.SfFields(), queryFields.ToString());
      fkbTemplate.AddFim(FimFtSql.SfTxFieldsVar(), queryParams.ToString());

      return FiTemplate.ReplaceTemplateParameters(template, fkbTemplate);
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

      return FiTemplate.ReplaceTemplateParameters(template, fkbTemplate);
    }


    public static string SelectAll1(IFiTableMeta ifiTbl)
    {
      // tpl:template
      string txQueryTpl = $@"
SELECT * 
FROM {FicOksCoding.OkTableName().fnmTemplate()}
"; //

      Fkb fkbParams = new Fkb();
      fkbParams.AddFic(FicOksCoding.OkTableName(), ifiTbl.GetITxTableName());

      string query = FiTemplate.ReplaceTemplateParameters(txQueryTpl.Trim(), fkbParams);

      FiAppConfig.fiLog?.Debug(query);

      return query;
    }


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

      string query = FiTemplate.ReplaceTemplateParameters(txQueryTpl.Trim(), fkbParams);

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

        if (fiCol.Value.CheckFiColIfPrimaryKey())
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
      fdrMain.refValue = FiTemplate.ReplaceTemplateParameters(template, fkbTemplate);

      return fdrMain;
    }






    public static Fdr InsertFicListV2(FicList ficList)
    {
      Fdr fdrMain = new Fdr();



      return fdrMain;
    }
  } // end class

}