using System.Xml.Linq;

namespace CustomerService.Services
{
    public class CustomerPersonService : ICustomerService
    {
        private readonly HttpClient _httpClient;

        public CustomerPersonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> DoesCustomerExist(int customerId)
        {
            if (customerId <= 0)
                return false;

            string soapApiUrl = $"https://www.crcind.com/csp/samples/SOAP.Demo.cls?soap_method=FindPerson&id={customerId}";
            HttpResponseMessage response = await _httpClient.GetAsync(soapApiUrl);
            if (!response.IsSuccessStatusCode)
                return false;

            string xmlResponse = await response.Content.ReadAsStringAsync();
            XDocument xmlDoc = XDocument.Parse(xmlResponse);
            XNamespace ns = "http://tempuri.org";
            return xmlDoc.Descendants(ns + "FindPersonResult").FirstOrDefault() != null;
        }
    }
}
