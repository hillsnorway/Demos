using SGBank.Models;
using SGBank.Models.Interfaces;
using System;
using System.Configuration;
using System.IO;

namespace SGBank.Data
{


    public class FileAccountRepository : IAccountRepository
    {
        private string _filePath;
        private bool _evalHeader;

        public FileAccountRepository(): this(ConfigurationManager.AppSettings["FilePath"].ToString())
        {
        }

        public FileAccountRepository(string filePath)
        {
            //Only Evaluate the Header if EvalHeader == TRUE
            if (ConfigurationManager.AppSettings["EvalHeader"].ToString().ToUpper() == "TRUE")
                _evalHeader = true;
            else
                _evalHeader = false;
            _filePath = filePath;
            //If file does not exist, create it and add a header
            if (!File.Exists(_filePath)) this.InitFilePath(_filePath);
        }

        public Account LoadAccount(string accountNumber)
        {
            try
            {
                using (StreamReader sr = new StreamReader(_filePath))
                {
                    string line;
                    //Get the header
                    line = sr.ReadLine();
                    //Does the file contain data? 
                    if (line == null)
                    {
                        throw new Exception($"The file {_filePath} is empty. \nContact IT!");
                    }
                    else if (this.ValidateHeader(line)) //ONLY EVALUATES HEADER IF _evalHeader == true
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            string[] columns = line.Split(',');
                            if (columns[(int)FileAccountHeaderElement.AccountNumber] == accountNumber)
                            {
                                if (columns.Length != Enum.GetNames(typeof(FileAccountHeaderElement)).Length)
                                    throw new Exception($"The entry for account: {accountNumber} in the file {_filePath} is corrupt. \nContact IT!");
                                Account retVal = new Account();
                                retVal.AccountNumber = columns[(int)FileAccountHeaderElement.AccountNumber];
                                retVal.Name = columns[(int)FileAccountHeaderElement.Name];
                                //retVal.Balance = decimal.Parse(columns[(int)FileAccountHeaderElement.Balance]);
                                if (decimal.TryParse(columns[(int)FileAccountHeaderElement.Balance], out decimal balance))
                                    retVal.Balance = balance;
                                else
                                    throw new Exception($"The BALANCE value in the file {_filePath} could not be parsed. \nContact IT!");
                                //retVal.Type = retVal.ParseAccountType(char.Parse(columns[(int)FileAccountHeaderElement.Type])).Value;
                                if (char.TryParse(columns[(int)FileAccountHeaderElement.Type], out char accountType))
                                    retVal.Type = retVal.ParseAccountType(accountType).Value;
                                else
                                   throw new Exception($"The TYPE value in the file {_filePath} could not be parsed. \nContact IT!");
                                return retVal;
                            }
                        }
                    }
                }
                //The account number was not found, return null
                return null;
            }

            catch (FileNotFoundException)
            {
                throw new Exception($"The file: {_filePath} was not found. \nContact IT!");
            }
            catch (Exception ex)
            {
                throw ex; //Throw exception up to calling method for handling in catch there.
            }

            finally
            {
            }

        }

        public void SaveAccount(Account account)
        {
            bool accountWrittenToFile = false;
            string line = null;
            int lineNr = 0;
            string tmpFilePath = Path.GetTempPath(); //Gets the system temp folder path
            tmpFilePath += "SGBankTmp.txt";

            try
            {
                using (StreamReader sr = new StreamReader(_filePath))
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
                                if (columns.Length == Enum.GetNames(typeof(FileAccountHeaderElement)).Length && columns[(int)FileAccountHeaderElement.AccountNumber] == account.AccountNumber)
                                {
                                    sw.WriteLine($"{account.AccountNumber},{account.Name},{account.Balance.ToString()},{account.Type.ToString().Substring(0, 1)}");
                                    accountWrittenToFile = true;
                                }
                                else sw.WriteLine(line);
                            }
                        }
                    }
                }
                if (accountWrittenToFile)
                {
                    File.Delete(_filePath);
                    File.Copy(tmpFilePath, _filePath);
                    File.Delete(tmpFilePath);
                }
                else
                {
                    throw new Exception($"Was unable update the account info in the file: {_filePath}. The account was not found! \nContact IT!");
                }

            }

            catch (FileNotFoundException)
            {
                throw new Exception($"The file: {_filePath} was not found. \nContact IT!");
            }
            catch (Exception ex)
            {
                throw ex; //Throw exception up to calling method for handling in catch there.
            }

            finally
            {
            }

        }

        private void InitFilePath(string filePath)
        {
            try
            {
                File.Create(filePath).Close();
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.WriteLine(this.ValidHeader());
                }
            }

            catch (FileNotFoundException)
            {
                throw new Exception($"The file: {_filePath} was not found. \nContact IT!");
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
            return $"{Enum.GetNames(typeof(FileAccountHeaderElement))[0]},{Enum.GetNames(typeof(FileAccountHeaderElement))[1]},{Enum.GetNames(typeof(FileAccountHeaderElement))[2]},{Enum.GetNames(typeof(FileAccountHeaderElement))[3]}";
        }

        private bool ValidateHeader(string headerLine)
        {
            if (_evalHeader)
            {
                string[] header = headerLine.Split(',');
                if (header.Length != Enum.GetNames(typeof(FileAccountHeaderElement)).Length) //Currently 4 HeaderElements
                {
                    throw new Exception($"The data file {_filePath} has an invalid header: wrong number of header elements. \nContact IT!");
                }
                else
                {
                    for (int i = 0; i < header.Length; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                if (header[0].ToString().ToUpper() != Enum.GetNames(typeof(FileAccountHeaderElement))[0].ToString().ToUpper())
                                    throw new Exception($"The data file {_filePath} has an invalid header: first header element incorrect, or header missing. \nContact IT!");
                                else
                                    break;
                            case 1:
                                if (header[1].ToUpper().ToUpper() != Enum.GetNames(typeof(FileAccountHeaderElement))[1].ToString().ToUpper())
                                    throw new Exception($"The data file {_filePath} has an invalid header: second header element incorrect. \nContact IT!");
                                else
                                    break;
                            case 2:
                                if (header[2].ToUpper().ToUpper() != Enum.GetNames(typeof(FileAccountHeaderElement))[2].ToString().ToUpper())
                                    throw new Exception($"The data file {_filePath} has an invalid header: third header element incorrect. \nContact IT!");
                                else
                                    break;
                            case 3:
                                if (header[3].ToUpper().ToUpper() != Enum.GetNames(typeof(FileAccountHeaderElement))[3].ToString().ToUpper())
                                    throw new Exception($"The data file {_filePath} has an invalid header: fourth header element incorrect. \nContact IT!");
                                else
                                    break;
                            default:
                                break;
                        }
                    }
                }
            }
            return true;
        }
    }
}
