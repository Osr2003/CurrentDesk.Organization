/* **************************************************************
* File Name     :- CurrentDeskSecurity.cs
* Author        :- Mukesh Nayak
* Copyright     :- Mindfire Solutions 
* Date          :- 10th Jan 2013
* Modified Date :- 10th Jan 2013
* Description   :- This file do encryption and decryption 
****************************************************************/

namespace CurrentDesk.Common
{
    /// <summary>
    /// Current desk Security class will encrypt and decrypt the data
    /// </summary>
    public class CurrentDeskSecurity
    {
       /// <summary>
       /// This Function will encrypted password
       /// </summary>
       /// <param name="password">password</param>
       /// <returns>return encrypted string</returns>
        public string SetPassEncrypted(string password)
        {            
            if (password != null)
            {
                return Encryption.EncryptAES(password, Constants.K_ENC_KEY, Constants.K_ENC_SALT);
            }
            return null;
        }

        /// <summary>
        /// This function will decrypted password
        /// </summary>
        /// <param name="password">password</param>
        /// <returns>decrypted</returns>
        public string GetPassDecrypted(string password)
        {           
            if (password != null)
            {
                return Encryption.DecryptAES(password, Constants.K_ENC_KEY, Constants.K_ENC_SALT);
            }
            return null;
        }
    }
}
