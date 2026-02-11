using OrakUtilDotNetCore.FiCollections;
using OrakUtilDotNetCore.FiConfig;
using OrakUtilDotNetCore.FiContainer;
using OrakUtilDotNetCore.FiCore;
using OrakUtilDotNetCore.FiDataContainer;
using OrakUtilDotNetCore.FiMetas;
using OrakUtilDotNetCore.FiOrm;

namespace OrakUtilSqliteCore.FiDbHelper
{
  using System;
  using System.Collections.Generic;
  using System.Text;

  public class FiQugenSqlite
  {
    public static string CreateTable(IFiTableMeta ifiTbl)
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

      FicList ficList = ifiTbl.GenITableCols();

      int index = 0;
      foreach (FiCol fiCol in ficList)
      {
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

      FiKeybean fkbParams = new FiKeybean();
      fkbParams.Add("tableName", ifiTbl.GetITxTableName());
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

      FiKeybean fkbTemplate = new FiKeybean();
      fkbTemplate.Add("tableName", iFiTableMeta.GetITxTableName());
      fkbTemplate.Add("csvFields", queryFields.ToString());
      fkbTemplate.Add("paramFields", queryParams.ToString());

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

      FiKeybean fkbTemplate = new FiKeybean();
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

      FiKeybean fkbParams = new FiKeybean();
      fkbParams.AddField(FicOksCoding.OkTableName(), ifiTbl.GetITxTableName());

      string query = FiTemplate.ReplaceTemplateParameters(txQueryTpl.Trim(), fkbParams);

      FiAppConfig.fiLog?.Debug(query);

      return query;
    }


    public static Fdr DeleteWhereIdCols(IFiTableMeta iFiTableMeta)
    {
      Fdr fdrMain = new Fdr();

      if (iFiTableMeta == null) return fdrMain;

      String template = $@"
DELETE FROM {FicOksCoding.OkTableName().fnmTemplate()}
WHERE {FicOksCoding.OkTxWhere().fnmTemplate()}
".Trim();

      StringBuilder sbTxWhere = new StringBuilder();

      int indexForPriKey = 1;
      foreach (FiCol fiCol in iFiTableMeta.GenITableCols())
      {

        if (fiCol.CheckFiColIfPrimaryKey())
        {
          if (indexForPriKey != 1) sbTxWhere.Append(", ");
          sbTxWhere.Append(fiCol.GetTxDbFieldOrTxFieldName());
          sbTxWhere.Append(" = @").Append(fiCol.fcTxFieldName);
          indexForPriKey++;
          continue;
        }

      }

      // Where cümleciği gelmemişse
      if (FiString.IsEmpty(sbTxWhere.ToString()))
      {
        fdrMain.boResult = false;
        fdrMain.refValue = "no-where condition-query cleared";
        return fdrMain;
      }

      FiKeybean fkbTemplate = new FiKeybean();
      fkbTemplate.AddField(FicOksCoding.OkTableName(), iFiTableMeta.GetITxTableName());
      fkbTemplate.AddField(FicOksCoding.OkTxWhere(), sbTxWhere.ToString());

      fdrMain.boResult = true;
      fdrMain.refValue = FiTemplate.ReplaceTemplateParameters(template, fkbTemplate);

      return fdrMain;
    }






  } // end class

}