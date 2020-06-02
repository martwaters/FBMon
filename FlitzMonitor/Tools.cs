using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlitzMonitor
{
    public class Tools
    {
        #region Exception Support

        /// <summary>
        /// Loops the exception message stack to assemble the messages.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>System.String.</returns>
        public static string GetMessageStack(Exception ex)
        {
            StringBuilder msg = new StringBuilder();
            Exception x = ex;
            string insert = string.Empty;
            while (x != null)
            {
                msg.AppendFormat("{0}{1}", insert, x.Message);
                x = x.InnerException;
                if (string.IsNullOrEmpty(insert))
                    insert = "\r\n";
                insert += "\t";
            };
            return msg.ToString();
        }

        /// <summary>
        /// Gets the most inner message of an exception chain.
        /// </summary>
        /// <param name="ex">The exception.</param>
        /// <returns>System.String.</returns>
        public static string GetInnerMostMessage(Exception ex)
        {
            if (ex == null) return String.Empty;

            Exception inner = ex;
            while (inner.InnerException != null)
                inner = inner.InnerException;

            return inner.Message;
        }

        #endregion Exception Support
    }
}
