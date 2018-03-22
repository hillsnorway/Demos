using FlooringMastery.Models.Interfaces;
using FlooringMastery.Models.Responses;


namespace FlooringMastery.BLL
{
    public class TaxManager
    {
        private ITaxRepository _taxRepo;

        public TaxManager(ITaxRepository concrete)
        {
            _taxRepo = concrete;
        }

        public TaxesResponse GetTaxes()
        {
            return _taxRepo.GetTaxes();
        }

        public TaxResponse GetTaxByState(string stateAbbr)
        {
            return _taxRepo.GetTaxByState(stateAbbr);
        }
    }
}
