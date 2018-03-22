using System.Collections.Generic;


namespace FlooringMastery.Models.Responses
{
    public class TaxesResponse : Response
    {
        public List<StateTax> Taxes { get; set; }

        public TaxesResponse()
        {
            this.Taxes = new List<StateTax>();
        }
    }
}
