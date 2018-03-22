using FlooringMastery.Models;
using FlooringMastery.Models.Interfaces;
using FlooringMastery.Models.Responses;
using System;
using System.Configuration;
using System.IO;
using System.Linq;


namespace FlooringMastery.Data
{
    public class ProductFileRepository : IProductRepository
    {
        public string ProductsFileName { get; private set; }
        private bool _evalHeader;
        private char _delimeter, _delimeterSerrogate;

        public ProductFileRepository()
            :this ("")
        {

        }

        public ProductFileRepository(string productFileName)
        {
            if (productFileName == "")
            {
                if (GetProductFileName(out productFileName))
                {
                    ProductsFileName = productFileName;
                }
                else
                {
                    ProductsFileName = "";
                }
            }
            else
            {
                ProductsFileName = productFileName;
            }

            if (ConfigurationManager.AppSettings["EvalHeader"].ToString().ToUpper() != "FALSE")
                _evalHeader = true;
            else
                _evalHeader = false;
            if (char.TryParse(ConfigurationManager.AppSettings["Delimiter"].ToString(), out char d))
                _delimeter = d;
            else
                _delimeter = ',';
            if (char.TryParse(ConfigurationManager.AppSettings["DelimiterSerrogate"].ToString(), out char ds))
                _delimeterSerrogate = ds;
            else
                _delimeterSerrogate = '~';  //Default set to (~) - another possibility the greek letter Delta (Δ)? Unicode: \u0394
        }

        public ProductsResponse GetProducts()
        {
            ProductsResponse response = new ProductsResponse();
            response.Success = true; //Assume true, failed parsing turns this false!

            try
            {
                if (ProductsFileName.Length == 0)
                {
                    response.Message = $"The file {ProductsFileName} could be found.";
                    response.Success = false;
                    return response;
                }
                using (StreamReader sr = new StreamReader(ProductsFileName))
                {
                    string line;
                    //Get the header
                    line = sr.ReadLine();
                    //Does the file contain data? 
                    if (line == null)
                    {
                        response.Message = $"The file {ProductsFileName} is empty. \nContact IT!";
                        response.Success = false;
                        return response;
                    }
                    else
                    {
                        //Evaluate Header
                        Response headerResponse = new Response();
                        headerResponse = ValidateHeader(line); //ONLY EVALUATES HEADER IF _evalHeader == true
                        if (!headerResponse.Success)
                        {
                            response.Message = headerResponse.Message;
                            response.Success = false;
                            return response;
                        }
                    }
                    //OK to parse file for data!!
                    int lineNR = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "") continue; //Empty line in the file, skip it...
                        lineNR++;
                        //Parse the line from the file
                        ProductResponse parseProductLineResponse = new ProductResponse();
                        parseProductLineResponse = ParseProductLine(line, lineNR);
                        if (!parseProductLineResponse.Success)
                        {
                            response.Message = parseProductLineResponse.Message;
                            response.Success = false;
                        }
                        else
                        {
                            response.Products.Add(parseProductLineResponse.Product);
                            //Success true by default - no need to update when orderReadSuccess...
                        }
                    }
                    if (!response.Success && response.Message.Length > 2)
                    {
                        response.Message = response.Message.Substring(0, response.Message.Length - 2); //Remove last "\n"
                        //response.Success is already set to false!
                    }
                    return response; //If there were errors reading lines, these lines will have been omitted during reading, and error messages generated explaining this.
                    //Product lines parsed w/o errors are added to the List, which can be used if so desired (in spite of the !Success result).
                }
            }

            catch (FileNotFoundException)
            {
                response.Message = $"The file: {ProductsFileName} was not found. \nContact IT!";
                response.Success = false;
                return response;
            }
            catch (Exception ex)
            {
                throw ex; //Throw exception up to calling method for handling in catch there.
            }

            finally
            {
            }
        }

        private ProductResponse ParseProductLine(string line, int lineNR)
        {
            ProductResponse response = new ProductResponse();

            string[] columns = line.Split(_delimeter);
            if (columns.Length != Enum.GetNames(typeof(ProductFileHeaderElement)).Length)
            {
                response.Message = $"The data on line number {lineNR} in the file {ProductsFileName} is corrupt. \nContact IT!";
                response.Success = false;
                return response;
            }

            bool productReadSuccess = true;
            string productReadExMessage = $"The following elements on line number {lineNR} were not able to be parsed:\n";
            Product myProduct = new Product();

            //ProductType
            if (columns[(int)ProductFileHeaderElement.ProductType].All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c)))
            {
                myProduct.ProductType = columns[(int)ProductFileHeaderElement.ProductType];
            }
            else
            {
                productReadExMessage += $"* ProductType\n";
                productReadSuccess = false;
            }
            //CostPerSquareFoot
            if (decimal.TryParse(columns[(int)ProductFileHeaderElement.CostPerSquareFoot], out decimal parsedCostPerSquareFoot))
            {
                myProduct.CostPerSquareFoot = parsedCostPerSquareFoot;
            }
            else
            {
                productReadExMessage += $"* CostPerSquareFoot\n";
                productReadSuccess = false;
            }
            //LaborCostPerSquareFoot
            if (decimal.TryParse(columns[(int)ProductFileHeaderElement.LaborCostPerSquareFoot], out decimal parsedLaborCostPerSquareFoot))
            {
                myProduct.LaborCostPerSquareFoot = parsedLaborCostPerSquareFoot;
            }
            else
            {
                productReadExMessage += $"* LaborCostPerSquareFoot\n";
                productReadSuccess = false;
            }
            //Check if all elements were read w/o error
            if (productReadSuccess)
            {
                response.Success = true;
                response.Product = myProduct;
                return response;
            }
            else
            {
                response.Message = productReadExMessage;
                response.Success = true;
                return response;
            }
        }

        public ProductResponse GetProductByType(string productType)
        {
            ProductResponse response = new ProductResponse();
            try
            {
                ProductsResponse responsePs = new ProductsResponse();
                responsePs = GetProducts();
                if (responsePs.Success)
                {
                    Product productToGet = responsePs.Products.Find(p => p.ProductType.ToUpper() == productType.ToUpper());
                    if (productToGet == null)
                    {
                        response.Message = $"Was unable to find a product matching product type: {productType}!";
                        response.Success = false;
                        return response;
                    }
                    else
                    {
                        response.Success = true;
                        response.Product = productToGet;
                        return response;
                    }
                }
                else
                {
                    response.Message = responsePs.Message;
                    response.Success = false;
                    return response;
                }
            }

            catch (FileNotFoundException)
            {
                response.Message = $"The file: {ProductsFileName} was not found. \nContact IT!";
                response.Success = false;
                return response;
            }
            catch (Exception ex)
            {
                throw ex; //Throw exception up to calling method for handling in catch there.
            }

            finally
            {
            }
        }

        private bool GetProductFileName(out string productFileName)
        {
            string dataPath = ConfigurationManager.AppSettings["DataPath"].ToString();
            productFileName = dataPath + ConfigurationManager.AppSettings["ProductFileName"].ToString();
            if (File.Exists(productFileName))
                return true;
            else
                return false;
        }

        private string ValidHeader()
        {
            return $"" +
                    $"{Enum.GetNames(typeof(ProductFileHeaderElement))[0]}," +
                    $"{Enum.GetNames(typeof(ProductFileHeaderElement))[1]}," +
                    $"{Enum.GetNames(typeof(ProductFileHeaderElement))[2]},";
        }

        private Response ValidateHeader(string headerLine)
        {
            Response response = new Response();
            response.Success = true;

            if (_evalHeader)
            {
                string[] header = headerLine.Split(',');
                if (header.Length != Enum.GetNames(typeof(ProductFileHeaderElement)).Length) //Currently 3 HeaderElements
                {
                    response.Message = $"The file {ProductsFileName} has an invalid header: wrong number of header elements. \nContact IT!";
                    response.Success = false;
                    return response;
                }
                else
                {
                    for (int i = 0; i < header.Length; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                if (header[0].ToString().ToUpper() != Enum.GetNames(typeof(ProductFileHeaderElement))[0].ToString().ToUpper())
                                {
                                    response.Message = $"The file {ProductsFileName} has an invalid header: first header element incorrect, or header missing. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 1:
                                if (header[1].ToUpper().ToUpper() != Enum.GetNames(typeof(ProductFileHeaderElement))[1].ToString().ToUpper())
                                {
                                    response.Message = $"The file {ProductsFileName} has an invalid header: second header element incorrect. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 2:
                                if (header[2].ToUpper().ToUpper() != Enum.GetNames(typeof(ProductFileHeaderElement))[2].ToString().ToUpper())
                                {
                                    response.Message = $"The file {ProductsFileName} has an invalid header: third header element incorrect. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                        }
                    }
                }
            }
            return response;
        }
    }
}
