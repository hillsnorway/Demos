

namespace FlooringMastery.Models.Responses
{
    public class TaxResponse : Response
    {
        public StateTax StateTax { get; set; }

        public TaxResponse()
        {
            this.StateTax = new StateTax();
        }
    }
}
