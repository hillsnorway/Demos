using FlooringMastery.Models;
using FlooringMastery.Models.Interfaces;
using FlooringMastery.Models.Responses;
using System.Collections.Generic;


namespace FlooringMastery.Data
{
    public class TaxTestRepository : ITaxRepository
    {
        private List<StateTax> _taxes;

        public TaxTestRepository()
        {
            _taxes = new List<StateTax>();
            _taxes.Add(new StateTax
            {
                StateCode = "OH",
                StateName = "Ohio",
                TaxRate = 6.25M
            });
            _taxes.Add(new StateTax
            {
                StateCode = "PA",
                StateName = "Pennsylvania",
                TaxRate = 6.75M
            });
            _taxes.Add(new StateTax
            {
                StateCode = "TX",
                StateName = "Texas",
                TaxRate = 8.19M
            });
            _taxes.Add(new StateTax
            {
                StateCode = "TN",
                StateName = "Tennessee",
                TaxRate = 9.46M
            });
        }

        public TaxesResponse GetTaxes()
        {
            TaxesResponse response = new TaxesResponse();
            response.Success = true;
            response.Taxes = _taxes;
            return response;
        }

        public TaxResponse GetTaxByState(string stateAbbr)
        {
            TaxResponse response = new TaxResponse();

            //Find matching record, or return null
            //return 
            StateTax lookedupTax = _taxes.Find(t => t.StateCode.ToUpper() == stateAbbr.ToUpper());
            if (lookedupTax != null && lookedupTax.StateCode.ToUpper() == stateAbbr.ToUpper())
            {
                response.Success = true;
                response.StateTax = lookedupTax;
                return response;
            }
            else
            {
                response.Message = $"Could not find a tax rate for the state: {stateAbbr}!";
                response.StateTax = lookedupTax;
                return response;
            }
        }
    }
}
