using DVDLibWebAPI.Data.Models;
using System.Collections.Generic;
using System.Linq;


namespace DVDLibWebAPI.Data
{
    class DvdStaticMemoryRepo
    {
        private static Dictionary<int, DVDdb> _dvds = new Dictionary<int, DVDdb>
        {
            { 1, new DVDdb { dvdId = 1, title = "A Great Tale", realeaseYear = "2018", director = "Sam Jones", rating = "PG", notes = "This really is a great tale!" } },
            { 2, new DVDdb { dvdId = 2, title = "Wonderful Tale", realeaseYear="2016", director="Joe Smith", rating="PG-13", notes="Wonderful, just wonderful!" } },
            { 3, new DVDdb { dvdId = 3, title = "Just A Tale", realeaseYear = "1900", director = "Joe Baker", rating = "G", notes = "Just a simple plot..." } }
        };

        public static Dictionary<int, DVDdb> GetAll()
        {
            return _dvds;
        }

        public static DVDdb Get(int ID)
        {
            if (_dvds.ContainsKey(ID))
            {
                return _dvds[ID];
            }
            else
            {
                return null;
            }
        }

        public static DVDdb Add(DVDdb dvd)
        {
            int newKey;

            if (_dvds.Keys.Count() == 0)
                newKey = 1;
            else
                newKey = _dvds.Keys.Max() + 1;
            dvd.dvdId = newKey;
            _dvds.Add(newKey, dvd);
            return _dvds[newKey];
        }

        public static DVDdb Delete(int ID)
        {
            if (_dvds.ContainsKey(ID))
            {
                DVDdb old = _dvds[ID];
                _dvds.Remove(ID);
                return old;
            }
            else
            {
                return null;
            }
        }

        public static DVDdb Edit(DVDdb dvd, int ID)
        {
            if (_dvds.ContainsKey(ID))
            {
                _dvds[ID] = dvd;
                return _dvds[ID];
            }
            else
            {
                return null;
            }
        }
    }
}
