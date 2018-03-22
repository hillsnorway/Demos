using System.Configuration;


namespace DVDLibWebAPI.Data
{
    public class Settings
    {
        private static string _repositoryType;

        public static string GetRepositoryType()
        {
            if (string.IsNullOrEmpty(_repositoryType))
                _repositoryType = ConfigurationManager.AppSettings["RepositoryType"].ToString();
            return _repositoryType;
        }
    }
}