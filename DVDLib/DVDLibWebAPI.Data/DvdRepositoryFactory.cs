using System;


namespace DVDLibWebAPI.Data
{
    public class DvdRepositoryFactory
    {
        public static IDvdRepository GetRepository()
        {
            switch (Settings.GetRepositoryType())
            {
                case "SampleData":
                    return new DvdRepositoryMock();
                case "ADO":
                    return new DvdRepositoryADO();
                case "EntityFramework":
                    return new DvdRepositoryEF();
                default:
                    throw new Exception("Invalid Repo Type Set");
            }
        }

    }
}
