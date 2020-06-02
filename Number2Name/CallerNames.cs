using Fritz;
using Fritz.Serialization;
using NLog;
using System;
using System.Collections.Generic;

namespace Number2Name
{
    /// <summary>
    /// Checks FB Phonebook to resolve Names from incoming Numbers
    /// </summary>
    public class CallerNames
    {
        static Logger Log = LogManager.GetCurrentClassLogger();
        Dictionary<string, string> KnownNumbers;
        Dictionary<string, string> Internals { get; set; }

        public CallerNames(string userName, string password)
        {
            KnownNumbers = GetPhoneBook(userName, password);
        }

        /// <summary>
        /// Resolves the specified remote number.
        /// </summary>
        /// <param name="remoteNumber">The remote number.</param>
        /// <returns>Name with number, if available</returns>
        public string Resolve(string remoteNumber)
        {
            string aligned = Align(remoteNumber);
            if (KnownNumbers.ContainsKey(aligned))
            {
                Log.Debug($"Match: { KnownNumbers[aligned]} ({ remoteNumber})");
                return string.Format($"{KnownNumbers[aligned]} ({remoteNumber})");
            }
            else
                Log.Debug($"No phonebook match: { remoteNumber}");

            return remoteNumber;
        }

        public string GetInternal(string internNumber)
        {
            if (Internals.ContainsKey(internNumber))
            {
                Log.Debug($"Match: { Internals[internNumber]} ({ internNumber})");
                return string.Format($"{Internals[internNumber]} ({internNumber})");
            }
            else
                Log.Debug($"No internals match: { internNumber}");

            return internNumber;
        }

        private Dictionary<string, string> GetPhoneBook(string userName, string password)
        {
            Dictionary<string, string> book = new Dictionary<string, string>();
            Internals = new Dictionary<string, string>();

            FritzClient _fb = new FritzClient { UserName = userName, Password = password };

            phonebooksPhonebook phonebook = _fb.GetPhonebook("Telefonbuch");
            foreach (phonebooksPhonebookContact contact in phonebook.contact)
            {
                Log.Debug($"{contact.uniqueid}\t{contact.person[0].realName}\t{contact.telephony[0].number[0].Value}");

                foreach (phonebooksPhonebookContactTelephony phone in contact.telephony)
                {
                    foreach(phonebooksPhonebookContactTelephonyNumber number in phone.number)
                    {
                        string dial = Align(number.Value);
                        if(!string.IsNullOrEmpty(dial)
                            && !book.ContainsKey(dial))
                        {
                            Log.Trace($"\t->{dial}");
                            book.Add(dial, contact.person[0].realName);
                        }
                        if(dial.StartsWith("**6"))
                        {
                            string homephone = dial.Substring(3);
                            if(!Internals.ContainsKey(homephone))
                            {
                                Log.Trace($"\t(internal)->{homephone}");
                                Internals.Add(homephone, contact.person[0].realName);
                            }

                        }

                    }
                }
            }
            return book;
        }

        private string Align(string number)
        {
            if (string.IsNullOrEmpty(number))
                return number;

            string noPrefix = number;
            if (number.StartsWith("00"))
                noPrefix = number.Substring(2);
            else if (number.StartsWith("0") || number.StartsWith("+"))
                noPrefix = number.Substring(1);
            
            string aligned = noPrefix.Replace(" ", "");
            return aligned;
        }
    }
}
