using NLog;
using Number2Name;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PBChecks
{
    internal class Program
    {
        static Logger log;

        static void Main(string[] args)
        {
            log = LogManager.GetLogger("Phonebook Checker");

            log.Info("########### User {0} starting {1} ###########", Environment.UserName, AppDomain.CurrentDomain.FriendlyName);

            if (args.Length < 2)
            {
                throw new Exception("Flitz Id args required!");
            }
            CallerNames Names = new CallerNames(args[0], args[1]);

            Dictionary<string, string> pb = Names.KnownNumbers;
            log.Info($"Phonebooks contain {pb.Count} entries");

            Dictionary<string, string> pInternals = Names.Internals;
            log.Info($"Phonebooks contain {pInternals.Count} internal entries");
        }
    }
}
