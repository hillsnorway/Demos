using System.Collections.Generic;
using DVDLibWebAPI.Data.Models;


namespace DVDLibWebAPI.Data
{
    public class DvdRepositoryMock : IDvdRepository
    {
        public DVDdb Add(DVDdb dvd)
        {
            return DvdStaticMemoryRepo.Add(dvd);
        }

        public DVDdb Delete(int ID)
        {
            return DvdStaticMemoryRepo.Delete(ID);
        }

        public DVDdb Edit(DVDdb dvd, int ID)
        {
            return DvdStaticMemoryRepo.Edit(dvd, ID);
        }

        public Dictionary<int, DVDdb> GetAll()
        {
            return DvdStaticMemoryRepo.GetAll();
        }

        public DVDdb Get(int ID)
        {
            return DvdStaticMemoryRepo.Get(ID);
        }
    }
}
