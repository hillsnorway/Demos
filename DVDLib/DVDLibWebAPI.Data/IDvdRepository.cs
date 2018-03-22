using DVDLibWebAPI.Data.Models;
using System.Collections.Generic;


namespace DVDLibWebAPI.Data
{
    public interface IDvdRepository
    {
        Dictionary<int, DVDdb> GetAll();
        DVDdb Get(int ID);
        DVDdb Add(DVDdb dvd);
        DVDdb Delete(int ID);
        DVDdb Edit(DVDdb dvd, int ID);
    }
}
