using System.Collections.Generic;
using System.Linq;
using DVDLibWebAPI.Data.Models;


namespace DVDLibWebAPI.Data
{
    public class DvdStaticMemoryRepoComplex
    {
        private static Dictionary<int, DByear> _year;
        private static Dictionary<int, DBdirector> _director;
        private static Dictionary<int, DBrating> _rating;
        private static Dictionary<int, DBdvd> _dvdDictionary;

        static DvdStaticMemoryRepoComplex()
        {
            _year = new Dictionary<int, DByear>();
            _year.Add(119, new DByear
            {
                YearID = 119, YearNR = "2018"
            });
            _year.Add(101, new DByear
            {
                YearID = 101,
                YearNR = "2000"
            });
            _year.Add(70, new DByear
            {
                YearID = 70,
                YearNR = "1969"
            });

            _director = new Dictionary<int, DBdirector>();
            _director.Add(1, new DBdirector
            {
                DirectorID = 1,
                DirectorName = "Joe Cool"
            });
            _director.Add(2, new DBdirector
            {
                DirectorID = 2,
                DirectorName = "Dude Awesome"
            });
            _director.Add(3, new DBdirector
            {
                DirectorID = 3,
                DirectorName = "Nathan Hill"
            });

            _rating = new Dictionary<int, DBrating>();
            _rating.Add(1, new DBrating
            {
                RatingID = 1,
                RatingType = "G",
                RatingDescr = "G - General Audiences\nAll ages admitted. Nothing that would offend parents for viewing by children."
            });
            _rating.Add(2, new DBrating
            {
                RatingID = 2,
                RatingType = "PG",
                RatingDescr = "PG - Parental Guidance Suggested \nSome material may not be suitable for children. Parents urged to give \"parental guidance\". May contain some material parents might not like for their young children."
            });
            _rating.Add(3, new DBrating
            {
                RatingID = 3,
                RatingType = "PG-13",
                RatingDescr = "PG-13 Parents Strongly Cautioned \nSome material may be inappropriate for children under 13. Parents are urged to be cautious. Some material may be inappropriate for pre-teenagers."
            });
            _rating.Add(4, new DBrating
            {
                RatingID = 4,
                RatingType = "R",
                RatingDescr = "R – Restricted \nUnder 17 requires accompanying parent or adult guardian. Contains some adult material. Parents are urged to learn more about the film before taking their young children with them."
            });
            _rating.Add(5, new DBrating
            {
                RatingID = 5,
                RatingType = "NC-17",
                RatingDescr = "NC-17 – Adults Only \nNo One 17 and Under Admitted. Clearly adult. Children are not admitted."
            });

            _dvdDictionary = new Dictionary<int, DBdvd>();
            _dvdDictionary.Add(1, new DBdvd()
            {
                DvdID = 1,
                DvdYear = _year.FirstOrDefault(kvp => kvp.Key == 119).Value,
                DvdDirector = _director.FirstOrDefault(kvp => kvp.Key == 3).Value,
                DvdRating = _rating.FirstOrDefault(kvp => kvp.Key == 3).Value,
                DvdTitle = "A Year In The Life",
                DvdNotes = "A moving story follows a man and details the year in his life leading up to his fiftieth birthday.  You get the feeling you know what walking in his shoes feels like."
            });
            _dvdDictionary.Add(2, new DBdvd()
            {
                DvdID = 2,
                DvdYear = _year.FirstOrDefault(kvp => kvp.Key == 101).Value,
                DvdDirector = _director.FirstOrDefault(kvp => kvp.Key == 2).Value,
                DvdRating = _rating.FirstOrDefault(kvp => kvp.Key == 2).Value,
                DvdTitle = "Everything Is Awesome",
                DvdNotes = "A modern take on Pollyanna.  Most people think this movie is a spoof of \"The Lego Movie\" from 2014, but its roots actually go back to the 1960 Disney movie, and 1913 Novel by Eleanor Porter."
            });
            _dvdDictionary.Add(3, new DBdvd()
            {
                DvdID = 3,
                DvdYear = _year.FirstOrDefault(kvp => kvp.Key == 70).Value,
                DvdDirector = _director.FirstOrDefault(kvp => kvp.Key == 1).Value,
                DvdRating = _rating.FirstOrDefault(kvp => kvp.Key == 4).Value,
                DvdTitle = "Winter of Discontent",
                DvdNotes = "An utterly depressing movie.  The title forshadows that this is a dark movie with themes of Winter and Discontent.  This film truely delivers on that promise."
            });
        }
        
        public static Dictionary<int, DBdvd> GetAll()
        {
            return _dvdDictionary;
        }

        public static DBdvd Get(int ID)
        {
            if (_dvdDictionary.ContainsKey(ID))
            {
                return _dvdDictionary[ID];
            }
            else
            {
                return null;
            }
        }

        public static DBdvd Add(DBdvd dvd)
        {
            int newKey;

            if (_dvdDictionary.Keys.Count() == 0)
                newKey = 1;
            else
                newKey = _dvdDictionary.Keys.Max() + 1;
            dvd.DvdID = newKey;
            _dvdDictionary.Add(newKey, dvd);
            return _dvdDictionary[newKey];
        }

        public static DBdvd Delete(int ID)
        {
            if (_dvdDictionary.ContainsKey(ID))
            {
                DBdvd old = _dvdDictionary[ID];
                _dvdDictionary.Remove(ID);
                return old;
            }
            else
            {
                return null;
            }
        }

        public static DBdvd Edit(DBdvd dvd, int ID)
        {

            if (_dvdDictionary.ContainsKey(ID))
            {
                _dvdDictionary[ID] = dvd;
                return _dvdDictionary[ID];
            }
            else
            {
                return null;
            }
        }
    }
}
