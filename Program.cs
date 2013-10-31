using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;

namespace ConsoleMail
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Usage: mail user@example.com subject");
                return 1;
            }

            try
            {
                string account = System.Environment.GetEnvironmentVariable("EMAIL");
                if (account == null)
                {
                    Console.Error.WriteLine("Set an environment variable called EMAIL with YOUR e-mail address.");
                    return 2;
                }

                string message = ReadStdin();
                if (message.Length == 0)
                {
                    Console.Error.WriteLine("No mail sent since stdin was empty.");
                    return 3;
                }

                Console.WriteLine("Sending e-mail using account '{0}'", account);
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
                service.Credentials = new WebCredentials(account, "");
                service.UseDefaultCredentials = true;

                // For verbose logging
                /*service.TraceEnabled = true;
                service.TraceFlags = TraceFlags.All;*/

                service.AutodiscoverUrl(account, RedirectionUrlValidationCallback);

                EmailMessage email = new EmailMessage(service);
                email.ToRecipients.Add(args[0]);
                email.Subject = args[1];
                message = message.Replace("\n", "<br/>\n");
                email.Body = new MessageBody(message);

                email.Send();
                Console.WriteLine("Email successfully sent to '{0}'", args[0]);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 4;
            }

            return 0;
        }

        private static string ReadStdin()
        {
            string s = "";

            int ch;
            do
            {
                ch = Console.Read();
                if (ch > 0)
                    s += (char)ch;
            } while (ch != -1);

            return s;
        }

        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            Uri redirectionUri = new Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }

            return result;
        }

    }
}
