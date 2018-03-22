using FlooringMastery.Models;
using FlooringMastery.Models.Interfaces;
using FlooringMastery.Models.Responses;
using System;
using System.Configuration;
using System.IO;
using System.Linq;


namespace FlooringMastery.Data
{
    public class TaxFileRepository : ITaxRepository
    {
        public string TaxesFileName { get; private set; }
        private bool _evalHeader;
        private char _delimeter, _delimeterSerrogate;

        public TaxFileRepository()
            :this ("")
        {

        }

        public TaxFileRepository(string taxFileName)
        {
            if (taxFileName == "")
            {
                if(GetTaxFileName(out taxFileName))
                {
                    TaxesFileName = taxFileName;
                }
                else
                {
                    TaxesFileName = "";
                }
            }
            else
            {
                TaxesFileName = taxFileName;
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

        public TaxesResponse GetTaxes()
        {
            TaxesResponse response = new TaxesResponse();
            response.Success = true; //Assume true, failed parsing turns this false!

            try
            {
                if (TaxesFileName.Length == 0)
                {
                    response.Message = $"The file {TaxesFileName} could be found.";
                    response.Success = false;
                    return response;
                }
                using (StreamReader sr = new StreamReader(TaxesFileName))
                {
                    string line;
                    //Get the header
                    line = sr.ReadLine();
                    //Does the file contain data? 
                    if (line == null)
                    {
                        response.Message = $"The file {TaxesFileName} is empty. \nContact IT!";
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
                        TaxResponse parseTaxLineResponse = new TaxResponse();
                        parseTaxLineResponse = ParseTaxLine(line, lineNR);
                        if (!parseTaxLineResponse.Success)
                        {
                            response.Message = parseTaxLineResponse.Message;
                            response.Success = false;
                        }
                        else
                        {
                            response.Taxes.Add(parseTaxLineResponse.StateTax);
                            //Success true by default - no need to update when orderReadSuccess...
                        }
                    }
                    if (!response.Success && response.Message.Length > 2)
                    {
                        response.Message = response.Message.Substring(0, response.Message.Length - 2); //Remove last "\n"
                        //response.Success is already set to false!
                    }
                    return response; //If there were errors reading lines, these lines will have been omitted during reading, and error messages generated explaining this.
                    //StateTax lines parsed w/o errors are added to the List, which can be used if so desired (in spite of the !Success result).
                }
            }

            catch (FileNotFoundException)
            {
                response.Message = $"The file: {TaxesFileName} was not found. \nContact IT!";
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

        private TaxResponse ParseTaxLine(string line, int lineNR)
        {
            TaxResponse response = new TaxResponse();

            string[] columns = line.Split(_delimeter);
            if (columns.Length != Enum.GetNames(typeof(TaxFileHeaderElement)).Length)
            {
                response.Message = $"The data on line number {lineNR} in the file {TaxesFileName} is corrupt. \nContact IT!";
                response.Success = false;
                return response;
            }

            bool taxReadSuccess = true;
            string taxReadExMessage = $"The following elements on line number {lineNR} were not able to be parsed:\n";
            StateTax myStateTax = new StateTax();

            //StateAbbreviation
            if (columns[(int)TaxFileHeaderElement.StateAbbreviation].All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c)))
            {
                myStateTax.StateCode = columns[(int)TaxFileHeaderElement.StateAbbreviation];
            }
            else
            {
                taxReadExMessage += $"* StateAbbreviation\n";
                taxReadSuccess = false;
            }
            //StateName
            if (columns[(int)TaxFileHeaderElement.StateName].All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c)))
            {
                myStateTax.StateName = columns[(int)TaxFileHeaderElement.StateName];
            }
            else
            {
                taxReadExMessage += $"* StateName\n";
                taxReadSuccess = false;
            }
            //TaxRate
            if (decimal.TryParse(columns[(int)TaxFileHeaderElement.TaxRate], out decimal parsedTaxRate))
            {
                myStateTax.TaxRate = parsedTaxRate;
            }
            else
            {
                taxReadExMessage += $"* TaxRate\n";
                taxReadSuccess = false;
            }
            //Check if all elements were read w/o error
            if (taxReadSuccess)
            {
                response.Success = true;
                response.StateTax = myStateTax;
                return response;
            }
            else
            {
                response.Message = taxReadExMessage;
                response.Success = true;
                return response;
            }
        }

        public TaxResponse GetTaxByState(string stateAbbr)
        {
            TaxResponse response = new TaxResponse();
            try
            {
                TaxesResponse responseTs = new TaxesResponse();
                responseTs = GetTaxes();
                if (responseTs.Success)
                {
                    StateTax statetaxToGet = responseTs.Taxes.Find(t => t.StateCode.ToUpper() == stateAbbr.ToUpper());
                    if (statetaxToGet == null)
                    {
                        response.Message = $"Was unable to find a state matching state abbreviation: {stateAbbr}!";
                        response.Success = false;
                        return response;
                    }
                    else
                    {
                        response.Success = true;
                        response.StateTax = statetaxToGet;
                        return response;
                    }
                }
                else
                {
                    response.Message = responseTs.Message;
                    response.Success = false;
                    return response;
                }
            }

            catch (FileNotFoundException)
            {
                response.Message = $"The file: {TaxesFileName} was not found. \nContact IT!";
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

        private bool GetTaxFileName(out string taxFileName)
        {
            string dataPath = ConfigurationManager.AppSettings["DataPath"].ToString();
            taxFileName = dataPath + ConfigurationManager.AppSettings["TaxFileName"].ToString();
            if (File.Exists(taxFileName))
                return true;
            else
                return false;
        }

        private string ValidHeader()
        {
            return $"" +
                    $"{Enum.GetNames(typeof(TaxFileHeaderElement))[0]}," +
                    $"{Enum.GetNames(typeof(TaxFileHeaderElement))[1]}," +
                    $"{Enum.GetNames(typeof(TaxFileHeaderElement))[2]},";
        }

        private Response ValidateHeader(string headerLine)
        {
            Response response = new Response();
            response.Success = true;

            if (_evalHeader)
            {
                string[] header = headerLine.Split(',');
                if (header.Length != Enum.GetNames(typeof(TaxFileHeaderElement)).Length) //Currently 3 HeaderElements
                {
                    response.Message = $"The file {TaxesFileName} has an invalid header: wrong number of header elements. \nContact IT!";
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
                                if (header[0].ToString().ToUpper() != Enum.GetNames(typeof(TaxFileHeaderElement))[0].ToString().ToUpper())
                                {
                                    response.Message = $"The file {TaxesFileName} has an invalid header: first header element incorrect, or header missing. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 1:
                                if (header[1].ToUpper().ToUpper() != Enum.GetNames(typeof(TaxFileHeaderElement))[1].ToString().ToUpper())
                                {
                                    response.Message = $"The file {TaxesFileName} has an invalid header: second header element incorrect. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 2:
                                if (header[2].ToUpper().ToUpper() != Enum.GetNames(typeof(TaxFileHeaderElement))[2].ToString().ToUpper())
                                {
                                    response.Message = $"The file {TaxesFileName} has an invalid header: third header element incorrect. \nContact IT!";
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
