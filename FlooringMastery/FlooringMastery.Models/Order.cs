using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlooringMastery.Models
{
    public class Order
    {
        public DateTime OrderDate { get; set; }
        public int OrderNumber { get; set; }
        public string CustomerName { get; set; }
        public StateTax OrderStateTax { get; set; }
        public Product OrderProduct { get; set; }
        public decimal Area { get; set; }
        public decimal FileMaterialCost { get; set; }
        public decimal FileLaborCost { get; set; }
        public decimal FileTax { get; set; }
        public decimal FileTotal { get; set; }
        public decimal CalcMaterialCost
        {
            get
            {
                return Math.Round(this.Area * this.OrderProduct.CostPerSquareFoot,2,MidpointRounding.AwayFromZero);
            }
        }
        public decimal CalcLaborCost
        {
            get
            {
                return Math.Round(this.Area * this.OrderProduct.LaborCostPerSquareFoot, 2, MidpointRounding.AwayFromZero);
            }
        }
        public decimal CalcTax
        {
            get
            {
                return Math.Round((this.CalcMaterialCost + this.CalcLaborCost) * (this.OrderStateTax.TaxRate/100),2, MidpointRounding.AwayFromZero);
            }
        }
        public decimal CalcTotal
        {
            get
            {
                return Math.Round(this.CalcMaterialCost + this.CalcLaborCost + this.CalcTax, 2, MidpointRounding.AwayFromZero);
            }
        }

        public Order()
        {
            this.OrderStateTax = new StateTax();
            this.OrderProduct = new Product();
        }

        public void Recalculate()
        {
            //Update Calculations
            Area = Math.Round(Area, 3, MidpointRounding.AwayFromZero);
            FileLaborCost = CalcLaborCost;
            FileMaterialCost = CalcMaterialCost;
            FileTax = CalcTax;
            FileTotal = CalcTotal;
        }

    }
}
