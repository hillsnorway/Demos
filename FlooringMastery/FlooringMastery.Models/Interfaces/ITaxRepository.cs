using FlooringMastery.Models.Responses;


namespace FlooringMastery.Models.Interfaces
{
    public interface ITaxRepository
    {
        TaxesResponse GetTaxes();
        TaxResponse GetTaxByState(string stateAbbr);
    }
}
