using Npgsql;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sufinn.Visitor.Repository
{
    public static class Utils
    {
        public static NpgsqlParameter[] ConvertDictionaryToNpgsqlParameters(Dictionary<string, object> parameters)
        {
            return parameters.Select(kvp =>
            {
                if (kvp.Value is byte[] byteArray)
                {
                    return new NpgsqlParameter(kvp.Key, NpgsqlTypes.NpgsqlDbType.Bytea) { Value = byteArray };
                }
                return new NpgsqlParameter(kvp.Key, kvp.Value ?? DBNull.Value);
            }).ToArray();
        }
        public static void WriteExceptionText(string e)
        {
            string projectRoot = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            string logfileadd = Path.Combine(projectRoot, "Exceptions\\", "Exception.txt");
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logfileadd,true))
            {
                writer.WriteLine("----------------------------------------------------------------------------------------------------------------------");
                writer.WriteLine("Exception :" + e.ToString() + "\r\n");
                writer.WriteLine("Time :" + DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss") + "\r\n");
                writer.WriteLine("----------------------------------------------------------------------------------------------------------------------");
                writer.Close();
            }

        }
        public static async Task<string> GetSufinnPayQrCode(string loanId)
        {
            var options = new RestClientOptions("https://sugmya.finpage.in")
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/open/jlg/upi/getPaymentQr?loanId="+ loanId, Method.Get);
            request.AddHeader("client-id", "CpQyfMwHbSmOymJEHyn");
            request.AddHeader("client-secret", "fBCxdWMHBRyYxgjGWgW");
            request.AddHeader("info-header", "upicollqr");
            RestResponse response = await client.ExecuteAsync(request);
            return response.Content;
        }
    }
}
