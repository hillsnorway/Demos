using FlooringMastery.Models;
using FlooringMastery.Models.Interfaces;
using FlooringMastery.Models.Responses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using System.Text;

namespace FlooringMastery.Data
{
    public class OrderFileRepository : IOrderRepository
    {
        public DateTime OrdersDate { get; private set; }
        public string OrdersFileName { get; private set; }
        private bool _evalHeader;
        private static char _delimeter = char.Parse(ConfigurationManager.AppSettings["Delimiter"]);
        private static char _delimeterSerrogate = char.Parse(ConfigurationManager.AppSettings["DelimiterSerrogate"]);

        public OrderFileRepository(DateTime orderDate)
        {
            OrdersDate = orderDate;
            if (GetOrderFileName(orderDate, out string orderFileName))
            {
                OrdersFileName = orderFileName;
            }
            else
            {
                OrdersFileName = "";
            }
            if (ConfigurationManager.AppSettings["EvalHeader"].ToString().ToUpper() != "FALSE")
                _evalHeader = true;
            else
                _evalHeader = false;
            //if (char.TryParse(ConfigurationManager.AppSettings["Delimiter"].ToString(), out char d))
            //    _delimeter = d;
            //else
            //    _delimeter = ',';
            //if (char.TryParse(ConfigurationManager.AppSettings["DelimiterSerrogate"].ToString(),out char ds))
            //    _delimeterSerrogate = ds;
            //else
            //    _delimeterSerrogate = '~';  //Default set to (~) - another possibility the greek letter Delta (Δ)? Unicode: \u0394
        }

        public OrdersResponse GetOrders()
        {
            OrdersResponse response = new OrdersResponse();
            response.Success = true; //Assume true, failed parsing turns this false!

            try
            {
                if (OrdersFileName.Length == 0)
                {
                    response.Message = $"No order data file could be found for the date {OrdersDate:d}";
                    response.Success = false;
                    return response;
                }
                using (StreamReader sr = new StreamReader(OrdersFileName))
                {
                    string line;
                    //Get the header
                    line = sr.ReadLine();
                    //Does the file contain data? 
                    if (line == null)
                    {
                        response.Message = $"The file {OrdersFileName} is empty. \nContact IT!";
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
                        OrderResponse parseOrderLineResponse = new OrderResponse();
                        parseOrderLineResponse = ParseOrderLine(line, lineNR);
                        if (!parseOrderLineResponse.Success)
                        {
                            response.Message = parseOrderLineResponse.Message;
                            response.Success = false;
                        }
                        else
                        {
                            response.Orders.Add(parseOrderLineResponse.Order);
                            //Success true by default - no need to update when orderReadSuccess...
                        }
                    }
                    if (!response.Success && response.Message.Length > 2)
                    {
                        response.Message = response.Message.Substring(0, response.Message.Length - 1); //Remove last "\n" FYI: Escaped characters apparantly count as a SINGLE character!
                        //response.Success is already set to false!
                    }
                    return response; //If there were errors reading lines, these lines will have been omitted during reading, and error messages generated explaining this.
                    //Order lines parsed w/o errors are added to the List, which can be used if so desired (in spite of the !Success result).
                }
            }

            catch (FileNotFoundException)
            {
                response.Message = $"The file: {OrdersFileName} was not found. \nContact IT!";
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

        private static string[] parseCSVLine(string csvLine)
        {
            string delimeter = OrderFileRepository._delimeter.ToString();

            using (TextFieldParser textFieldParser = new TextFieldParser(new MemoryStream(Encoding.UTF8.GetBytes(csvLine))))
            {
                textFieldParser.HasFieldsEnclosedInQuotes = true;
                textFieldParser.SetDelimiters(delimeter);

                try
                {
                    return textFieldParser.ReadFields();
                }
                catch (MalformedLineException)
                {
                    StringBuilder m_sbLine = new StringBuilder();

                    for (int i = 0; i < textFieldParser.ErrorLine.Length; i++)
                    {
                        if (i > 0 && textFieldParser.ErrorLine[i] == '"' && (textFieldParser.ErrorLine[i + 1] != ',' && textFieldParser.ErrorLine[i - 1] != ','))
                            m_sbLine.Append("\"\"");
                        else
                            m_sbLine.Append(textFieldParser.ErrorLine[i]);
                    }

                    return parseCSVLine(m_sbLine.ToString());
                }
            }
        }

        private OrderResponse ParseOrderLine(string line, int lineNR)
        {
            OrderResponse response = new OrderResponse();

            string[] columns = parseCSVLine(line); //line.Split(_delimeter); //Swapped out Split with a method that handles CSV with quotations...
            if (columns.Length != Enum.GetNames(typeof(OrderFileHeaderElement)).Length)
            {
                response.Message = $"The data on line number {lineNR} in the file {OrdersFileName} is corrupt. \nContact IT!";
                response.Success = false;
                return response;
            }

            bool orderReadSuccess = true;
            string orderReadExMessage = $"The following elements on line number {lineNR} were not able to be parsed:\n";
            Order myOrder = new Order();

            //OrderDate
            myOrder.OrderDate = OrdersDate;
            //OrderNumber
            if (int.TryParse(columns[(int)OrderFileHeaderElement.OrderNumber], out int parsedOrderNumber))
            {
                myOrder.OrderNumber = parsedOrderNumber;
            }
            else
            {
                orderReadExMessage += $"* OrderNumber\n";
                orderReadSuccess = false;
            }
            //CustomerName
            if (columns[(int)OrderFileHeaderElement.CustomerName].All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c) || c == _delimeterSerrogate))
            {
                myOrder.CustomerName = columns[(int)OrderFileHeaderElement.CustomerName];
                myOrder.CustomerName = myOrder.CustomerName.Replace(_delimeterSerrogate, _delimeter);
            }
            else
            {
                orderReadExMessage += $"* CustomerName\n";
                orderReadSuccess = false;
            }
            //State
            if (columns[(int)OrderFileHeaderElement.State].All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c) || c == _delimeterSerrogate))
            {
                myOrder.OrderStateTax.StateCode = columns[(int)OrderFileHeaderElement.State];
                myOrder.OrderStateTax.StateCode.Replace(_delimeterSerrogate, _delimeter);
            }
            else
            {
                orderReadExMessage += $"* StateCode\n";
                orderReadSuccess = false;
            }
            //TaxRate
            if (decimal.TryParse(columns[(int)OrderFileHeaderElement.TaxRate], out decimal parsedTaxRate))
            {
                myOrder.OrderStateTax.TaxRate = parsedTaxRate;
            }
            else
            {
                orderReadExMessage += $"* TaxRate\n";
                orderReadSuccess = false;
            }
            //ProductType
            if (columns[(int)OrderFileHeaderElement.ProductType].All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c) || c == _delimeterSerrogate))
            {
                myOrder.OrderProduct.ProductType = columns[(int)OrderFileHeaderElement.ProductType];
                myOrder.OrderProduct.ProductType.Replace(_delimeterSerrogate, _delimeter);
            }
            else
            {
                orderReadExMessage += $"* ProductType\n";
                orderReadSuccess = false;
            }
            //Area
            if (decimal.TryParse(columns[(int)OrderFileHeaderElement.Area], out decimal parsedArea))
            {
                myOrder.Area = parsedArea;
            }
            else
            {
                orderReadExMessage += $"* Area\n";
                orderReadSuccess = false;
            }
            //CostPerSquareFoot
            if (decimal.TryParse(columns[(int)OrderFileHeaderElement.CostPerSquareFoot], out decimal parsedCostPerSquareFoot))
            {
                myOrder.OrderProduct.CostPerSquareFoot = parsedCostPerSquareFoot;
            }
            else
            {
                orderReadExMessage += $"* CostPerSquareFoot\n";
                orderReadSuccess = false;
            }
            //LaborCostPerSquareFoot
            if (decimal.TryParse(columns[(int)OrderFileHeaderElement.LaborCostPerSquareFoot], out decimal parsedLaborCostPerSquareFoot))
            {
                myOrder.OrderProduct.LaborCostPerSquareFoot = parsedLaborCostPerSquareFoot;
            }
            else
            {
                orderReadExMessage += $"* LaborCostPerSquareFoot\n";
                orderReadSuccess = false;
            }
            //MaterialCost
            if (decimal.TryParse(columns[(int)OrderFileHeaderElement.MaterialCost], out decimal parsedMaterialCost))
            {
                myOrder.FileMaterialCost = parsedMaterialCost;
            }
            else
            {
                orderReadExMessage += $"* MaterialCost\n";
                orderReadSuccess = false;
            }
            //LaborCost
            if (decimal.TryParse(columns[(int)OrderFileHeaderElement.LaborCost], out decimal parsedLaborCost))
            {
                myOrder.FileLaborCost = parsedLaborCost;
            }
            else
            {
                orderReadExMessage += $"* LaborCost\n";
                orderReadSuccess = false;
            }
            //Tax
            if (decimal.TryParse(columns[(int)OrderFileHeaderElement.Tax], out decimal parsedTax))
            {
                myOrder.FileTax = parsedTax;
            }
            else
            {
                orderReadExMessage += $"* Tax\n";
                orderReadSuccess = false;
            }
            //Total
            if (decimal.TryParse(columns[(int)OrderFileHeaderElement.Total], out decimal parsedTotal))
            {
                myOrder.FileTotal = parsedTotal;
            }
            else
            {
                orderReadExMessage += $"* Total\n";
                orderReadSuccess = false;
            }
            //Check if alle elements were read w/o error
            if (orderReadSuccess)
            {
                response.Success = true;
                response.Order = myOrder;
                return response;
            }
            else
            {
                response.Message = orderReadExMessage;
                response.Success = false;
                return response;
            }
        }

        public OrderResponse GetOrder(int orderNumber)
        {
            OrderResponse response = new OrderResponse();
            try
            {
                OrdersResponse responseOs = new OrdersResponse();
                responseOs = GetOrders();
                if (responseOs.Success)
                {
                    Order orderToGet = responseOs.Orders.Find(o => o.OrderNumber == orderNumber);
                    if  (orderToGet == null)
                    {
                        response.Message = $"Was unable to find an order for order number: {orderNumber}!";
                        response.Success = false;
                        return response;
                    }
                    else
                    {
                        response.Success = true;
                        response.Order = orderToGet;
                        return response;
                    }
                }
                else
                {
                    response.Message = responseOs.Message;
                    response.Success = false;
                    return response;
                }
            }
            
            catch (FileNotFoundException)
            {
                response.Message = $"The file: {OrdersFileName} was not found. \nContact IT!";
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

        public OrderResponse AddOrder(Order newOrder)
        {
            OrderResponse response = new OrderResponse();
            response = OrderWriter(OrderWriterAction.Add, newOrder);
            return response;
        }

        public OrderResponse EditOrder(Order editedOrder)
        {
            OrderResponse response = new OrderResponse();
            response = OrderWriter(OrderWriterAction.Edit, editedOrder);
            return response;
        }

        public OrderResponse DeleteOrder(int orderNumber)
        {
            OrderResponse response = new OrderResponse();
            response = GetOrder(orderNumber);
            if (response.Success)
            {
                response = OrderWriter(OrderWriterAction.Delete, response.Order);
            }
            return response;
        }

        public List<string> GetAllFileNames(out string dataPath)
        {
            try
            {
                dataPath = ConfigurationManager.AppSettings["DataPath"].ToString();
                string fileSuffix = ConfigurationManager.AppSettings["DataFileSuffix"].ToString();
                List<string> fileNames = Directory.GetFiles(dataPath).Where(f => f.ToString().Contains(fileSuffix)).ToList();
                return fileNames;
            }
            catch (DirectoryNotFoundException)
            {
                throw new Exception("The data directory could not be found! \nContact IT!");
            }
            catch (PathTooLongException)
            {
                throw new Exception("The data directory path is too long to used! \nContact IT!");
            }
            catch (UnauthorizedAccessException)
            {
                throw new Exception("Insufficient permissions to process the data directory path! \nContact IT!");
            }
            catch (IOException)
            {
                throw new Exception("An IO Exception occurred (path may indicate a file, or there could be a network error)! \nContact IT!");
            }
            catch (ArgumentNullException)
            {
                throw new Exception("The data directory path can not be Null! \nContact IT!");
            }
            catch (ArgumentException)
            {
                throw new Exception("The data directory path is invalid! \nContact IT!");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }

        private OrderResponse OrderWriter(OrderWriterAction mode, Order order)
        {
            OrderResponse response = new OrderResponse();
            
            //Prepare for using a tmp file
            string tmpFilePath = Path.GetTempPath(); //Gets the system temp folder path
            tmpFilePath += "Orders_Tmp" + ConfigurationManager.AppSettings["DataFileSuffix"].ToString();

            try
            {
                bool modeActionPerformed = false;
                bool removeEmptyOrderFile = false;
                string line = null;
                int lineNr = 0;

                //Delete old tmp file if it exists
                if (File.Exists(tmpFilePath)) File.Delete(tmpFilePath);

                if (mode == OrderWriterAction.Add)
                {
                    int nextOrderNumber = 0;

                    if (OrdersFileName == "")
                    {
                        //Write to New File:
                        GetOrderFileName(order.OrderDate, out string OrdersFileName);
                        InitFilePath(OrdersFileName);
                        //OrderNumber = 1 for new file
                        nextOrderNumber = 1;
                    }
                    else
                    {
                        nextOrderNumber = GetNextOrderNumber();
                        if (nextOrderNumber == 0)
                        {
                            response.Message = "Was unable to locate the file: {OrdersFileName} to add the order to! \nContact IT!"; ;
                            response.Success = false;
                            return response;
                        }
                    }
                    order.OrderNumber = nextOrderNumber;
                }
                if (mode == OrderWriterAction.Add || mode == OrderWriterAction.Edit || mode == OrderWriterAction.Delete)
                {
                    //Write to Existing File:
                    using (StreamReader sr = new StreamReader(OrdersFileName))
                    {
                        using (StreamWriter sw = new StreamWriter(tmpFilePath))
                        {
                            while ((line = sr.ReadLine()) != null)
                            {
                                lineNr++;
                                if (lineNr == 1) sw.WriteLine(this.ValidHeader()); //ALWAYS OUTPUT A FRESH HEADER
                                else
                                {
                                    string[] columns = line.Split(',');
                                    if (!int.TryParse(columns[(int)OrderFileHeaderElement.OrderNumber], out int fileIntOrderNumber))
                                        continue; //Encountered a line with an unparseable OrderNumber, skip it
                                    if (columns.Length == Enum.GetNames(typeof(OrderFileHeaderElement)).Length && fileIntOrderNumber == order.OrderNumber)
                                    {
                                        switch (mode)
                                        {
                                            case OrderWriterAction.Add:
                                                //Wait until last line in file before writing...
                                                break;
                                            case OrderWriterAction.Edit:
                                                sw.WriteLine(BuildSeparatedOrderLine(order));
                                                modeActionPerformed = true;
                                                break;
                                            case OrderWriterAction.Delete:
                                                //By not writing here, we are deleting...
                                                //If lineNR == 2 and there is no next line in the file, it is the last order in the file being deleted...
                                                if (lineNr == 2 && sr.Peek() == -1) removeEmptyOrderFile = true;
                                                modeActionPerformed = true;
                                                break;
                                            default:
                                                break;
                                        }

                                    }
                                    else sw.WriteLine(line);
                                }
                            }
                            if (mode == OrderWriterAction.Add)
                            {
                                sw.WriteLine(BuildSeparatedOrderLine(order));
                                modeActionPerformed = true;
                            }
                        }
                    }
                    if (modeActionPerformed)
                    {
                        File.Delete(OrdersFileName);
                        if(!removeEmptyOrderFile) File.Copy(tmpFilePath, OrdersFileName);
                        File.Delete(tmpFilePath);
                        response.Order = order;
                        response.Success = true;
                    }
                    else
                    {
                        response.Message = $"Was unable update the account info in the file: {OrdersFileName}. The account was not found! \nContact IT!";
                        response.Success = false;
                    }
                }
                return response;
            }

            catch (FileNotFoundException)
            {
                response.Message = $"The file: {OrdersFileName} was not found. \nContact IT!";
                response.Success = false;
                return response;
            }
            catch (Exception ex)
            {
                throw ex; //Throw exception up to calling method for handling in catch there.
            }

            finally
            {
                //Delete tmp file if it exists
                if (File.Exists(tmpFilePath)) File.Delete(tmpFilePath);
            }
        }

        private int GetNextOrderNumber()
        {
            int retVal = 0;
            //Look in an existing file, and get the next OrderNumber
            OrdersResponse ordersResponse = new OrdersResponse();
            ordersResponse = GetOrders();
            if (ordersResponse.Success)
                retVal = ordersResponse.Orders.Max(o => o.OrderNumber) + 1;
            return retVal;
        }

        private bool GetOrderFileName(DateTime orderDate, out string orderFileName)
        {
            string dataPath = ConfigurationManager.AppSettings["DataPath"].ToString();
            string dataFilePrefix = ConfigurationManager.AppSettings["DataFilePrefix"].ToString();
            string dataFileDateParseFormat = ConfigurationManager.AppSettings["DataFileDateParseFormat"].ToString();
            string dataFileSuffix = ConfigurationManager.AppSettings["DataFileSuffix"].ToString();
            orderFileName = dataPath + dataFilePrefix + orderDate.ToString(dataFileDateParseFormat) + dataFileSuffix;
            if (File.Exists(orderFileName))
                return true;
            else
                return false;
        }

        private string BuildSeparatedOrderLine(Order order)
        {
            string retVal = "";

            retVal = order.OrderNumber.ToString();
            retVal = retVal + _delimeter + order.CustomerName.Replace(_delimeter, _delimeterSerrogate);
            retVal = retVal + _delimeter + order.OrderStateTax.StateCode.Replace(_delimeter, _delimeterSerrogate);
            retVal = retVal + _delimeter + order.OrderStateTax.TaxRate.ToString();
            retVal = retVal + _delimeter + order.OrderProduct.ProductType.Replace(_delimeter, _delimeterSerrogate);
            retVal = retVal + _delimeter + order.Area.ToString();
            retVal = retVal + _delimeter + order.OrderProduct.CostPerSquareFoot.ToString();
            retVal = retVal + _delimeter + order.OrderProduct.LaborCostPerSquareFoot.ToString();
            retVal = retVal + _delimeter + order.FileMaterialCost.ToString();
            retVal = retVal + _delimeter + order.FileLaborCost.ToString();
            retVal = retVal + _delimeter + order.FileTax.ToString();
            retVal = retVal + _delimeter + order.FileTotal.ToString();
            return retVal;
        }

        private Response InitFilePath(string filePath)
        {
            Response response = new Response();
            try
            {
                File.Create(filePath).Close();
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLine(this.ValidHeader());
                }
                OrdersFileName = filePath; //THIS IS KEY!!! OrdersFileName can NOT be empty when a file exists!
                response.Success = true;
                return response;
            }

            catch (FileNotFoundException)
            {
                response.Message = $"The file: {OrdersFileName} was not found. \nContact IT!";
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

        private string ValidHeader()
        {
            return  $"" +
                    $"{Enum.GetNames(typeof(OrderFileHeaderElement))[0]}," +
                    $"{Enum.GetNames(typeof(OrderFileHeaderElement))[1]}," +
                    $"{Enum.GetNames(typeof(OrderFileHeaderElement))[2]}," +
                    $"{Enum.GetNames(typeof(OrderFileHeaderElement))[3]}," +
                    $"{Enum.GetNames(typeof(OrderFileHeaderElement))[4]}," +
                    $"{Enum.GetNames(typeof(OrderFileHeaderElement))[5]}," +
                    $"{Enum.GetNames(typeof(OrderFileHeaderElement))[6]}," +
                    $"{Enum.GetNames(typeof(OrderFileHeaderElement))[7]}," +
                    $"{Enum.GetNames(typeof(OrderFileHeaderElement))[8]}," +
                    $"{Enum.GetNames(typeof(OrderFileHeaderElement))[9]}," +
                    $"{Enum.GetNames(typeof(OrderFileHeaderElement))[10]}," +
                    $"{Enum.GetNames(typeof(OrderFileHeaderElement))[11]}"; 
        }

        private Response ValidateHeader(string headerLine)
        {
            Response response = new Response();
            response.Success = true;

            if (_evalHeader)
            {
                string[] header = headerLine.Split(',');
                if (header.Length != Enum.GetNames(typeof(OrderFileHeaderElement)).Length) //Currently 12 HeaderElements
                {
                    throw new Exception($"The data file {OrdersFileName} has an invalid header: wrong number of header elements. \nContact IT!");
                }
                else
                {
                    for (int i = 0; i < header.Length; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                if (header[0].ToString().ToUpper() != Enum.GetNames(typeof(OrderFileHeaderElement))[0].ToString().ToUpper())
                                {
                                    response.Message = $"The data file {OrdersFileName} has an invalid header: first header element incorrect, or header missing. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 1:
                                if (header[1].ToUpper().ToUpper() != Enum.GetNames(typeof(OrderFileHeaderElement))[1].ToString().ToUpper())
                                {
                                    response.Message = $"The data file {OrdersFileName} has an invalid header: second header element incorrect. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 2:
                                if (header[2].ToUpper().ToUpper() != Enum.GetNames(typeof(OrderFileHeaderElement))[2].ToString().ToUpper())
                                {
                                    response.Message = $"The data file {OrdersFileName} has an invalid header: third header element incorrect. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 3:
                                if (header[3].ToUpper().ToUpper() != Enum.GetNames(typeof(OrderFileHeaderElement))[3].ToString().ToUpper())
                                {
                                    response.Message = $"The data file {OrdersFileName} has an invalid header: fourth header element incorrect. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 4:
                                if (header[4].ToString().ToUpper() != Enum.GetNames(typeof(OrderFileHeaderElement))[4].ToString().ToUpper())
                                {
                                    response.Message = $"The data file {OrdersFileName} has an invalid header: first header element incorrect, or header missing. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 5:
                                if (header[5].ToUpper().ToUpper() != Enum.GetNames(typeof(OrderFileHeaderElement))[5].ToString().ToUpper())
                                {
                                    response.Message = $"The data file {OrdersFileName} has an invalid header: second header element incorrect. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 6:
                                if (header[6].ToUpper().ToUpper() != Enum.GetNames(typeof(OrderFileHeaderElement))[6].ToString().ToUpper())
                                {
                                    response.Message = $"The data file {OrdersFileName} has an invalid header: third header element incorrect. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 7:
                                if (header[7].ToUpper().ToUpper() != Enum.GetNames(typeof(OrderFileHeaderElement))[7].ToString().ToUpper())
                                {
                                    response.Message = $"The data file {OrdersFileName} has an invalid header: fourth header element incorrect. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 8:
                                if (header[8].ToString().ToUpper() != Enum.GetNames(typeof(OrderFileHeaderElement))[8].ToString().ToUpper())
                                {
                                    response.Message = $"The data file {OrdersFileName} has an invalid header: first header element incorrect, or header missing. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 9:
                                if (header[9].ToUpper().ToUpper() != Enum.GetNames(typeof(OrderFileHeaderElement))[9].ToString().ToUpper())
                                {
                                    response.Message = $"The data file {OrdersFileName} has an invalid header: second header element incorrect. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 10:
                                if (header[10].ToUpper().ToUpper() != Enum.GetNames(typeof(OrderFileHeaderElement))[10].ToString().ToUpper())
                                {
                                    response.Message = $"The data file {OrdersFileName} has an invalid header: third header element incorrect. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            case 11:
                                if (header[11].ToUpper().ToUpper() != Enum.GetNames(typeof(OrderFileHeaderElement))[11].ToString().ToUpper())
                                {
                                    response.Message = $"The data file {OrdersFileName} has an invalid header: fourth header element incorrect. \nContact IT!";
                                    response.Success = false;
                                    break;
                                }
                                else
                                    break;
                            default:
                                break;
                        }
                    }
                }
            }
            return response;
        }
    }

    internal enum OrderWriterAction
    {
        Add,
        Edit,
        Delete
    }
}
