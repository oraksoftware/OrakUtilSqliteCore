namespace OrakUtilSqliteCore.FiDbHelper
{
  public interface IRepoSqLite
  {
    string connProfile { get; set; }
    FiSqLite GetDbHelper();
    // FiSqLite.BuiWitProfile(connProfile) ile helper objesi oluşturulur.
  }


}