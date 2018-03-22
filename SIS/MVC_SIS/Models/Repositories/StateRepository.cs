using SIS.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SIS.Models.Repositories
{
    public class StateRepository
    {
        private static List<State> _states;

        static StateRepository()
        {
            // sample data
            _states = new List<State>
            {
                new State { StateAbbreviation="KY", StateName="Kentucky" },
                new State { StateAbbreviation="MN", StateName="Minnesota" },
                new State { StateAbbreviation="OH", StateName="Ohio" },
            };
        }

        public static IEnumerable<State> GetAll()
        {
            return _states;
        }

        public static State Get(string stateAbbreviation)
        {
            return _states.FirstOrDefault(c => c.StateAbbreviation == stateAbbreviation);
        }

        public static void Add(State state)
        {
            state.StateAbbreviation = state.StateAbbreviation.ToUpper();
            _states.Add(state);
        }

        public static void Edit(State state)
        {
            var selectedState = _states.FirstOrDefault(c => c.StateAbbreviation == state.StateAbbreviation);

            selectedState.StateName = state.StateName;
        }

        public static void Delete(string stateAbbreviation)
        {
            _states.RemoveAll(c => c.StateAbbreviation == stateAbbreviation);
        }
    }
}