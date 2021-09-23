using CustomerRestService.Integration.Dtos;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace CustomerRestService.Integration
{
    [TestClass]
    public class CustomersControllerTests
    {
        [TestMethod]
        public void CreateCustomer_HappyFlow_ShouldReturnCreated()
        {
            //Arrange
            var client = new HttpClient();

            var customer = new CustomerDto
            {
                CustomerCode = Guid.NewGuid().ToString(),
                Name = "Yuri Bravoooo",
                CNP = "1890915060590",
                Address = "str. Mexico nr. 678"
            };

            string url = "http://localhost:4088/api/Customers";

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false)});
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //Act
            var result = client.PostAsync(url, customer, formatter).Result;
            var createdCustomer = result.Content.ReadAsAsync<CustomerDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            createdCustomer.Should().NotBeNull();
        }

        [TestMethod]
        public void CreateCustomer_ExistingCustomerCode_ShouldReturnBadRequest()
        {
            //Arrange
            var client = new HttpClient();

            string url = "http://localhost:4088/api/Customers";

            var existingCustomerCode = client.GetAsync(url).Result.Content
                                .ReadAsAsync<IEnumerable<CustomerDto>>()
                                .Result.ToList()
                                .FirstOrDefault().CustomerCode;

            var customer = new CustomerDto
            {
                CustomerCode = existingCustomerCode,
                Name = "Carlos Bravoooo",
                CNP = "1890915060590",
                Address = "str. Mexico nr. 678"
            };


            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //Act
            var result = client.PostAsync(url, customer, formatter).Result;
                var createdCustomer = result.Content.ReadAsAsync<CustomerDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            createdCustomer.Should().BeNull();

        }

        [TestMethod]
        public void GetCustomers_HappyFlow_ShouldReturnOk()
        {
            //Arrange
            var client = new HttpClient();

            string url = "http://localhost:4088/api/Customers";

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //Act
            var result = client.GetAsync(url).Result;
            var fetchedCustomers = result.Content.ReadAsAsync<IEnumerable<CustomerDto>>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            fetchedCustomers.Should().NotBeNull();
        }

        [TestMethod]
        public void GetCustomer_HappyFlow_ShouldReturnOk()
        {
            //Arrange
            var client = new HttpClient();

            string url = "http://localhost:4088/api/Customers/6480958";

                var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //Act
            var result = client.GetAsync(url).Result;
            var fetchedCustomer = result.Content.ReadAsAsync<CustomerDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            fetchedCustomer.Should().NotBeNull();
            fetchedCustomer.Name.Should().Be("Ambrosi Loudian");
        }


        [TestMethod]
        public void GetCustomer_InexistentCode_ShouldReturnNotFound()
        {
            //Arrange
            var client = new HttpClient();
            string url = "http://localhost:4088/api/Customers/";

            var badCustCode = Guid.NewGuid().ToString().Substring(0,15);
            bool codeDoesExist = true;
            var customers = client.GetAsync(url).Result.Content.ReadAsAsync<IEnumerable<CustomerDto>>().Result.ToList();
            
            while (codeDoesExist)
            {
                if (customers.Any(c => c.CustomerCode == badCustCode))
                    badCustCode = Guid.NewGuid().ToString().Substring(0, 15);
                else codeDoesExist = false;
            }

            url += badCustCode;

            //Act
            var result = client.GetAsync(url).Result;
            var fetchedCustomer = result.Content.ReadAsAsync<CustomerDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            fetchedCustomer.Should().BeNull();
        }

        [TestMethod]
        public void UpdateCustomer_HappyFlow_ShouldReturnNoContent()
        {
            //Arrange
            var client = new HttpClient();

            string url = "http://localhost:4088/api/Customers/8eedcafc-c782-45af-b997-278f7da11552/";

            var customer = new CustomerUpdateDto
            {
                Name = "Caezar Bravo",
                CNP = "1890915060590",
                Address = "str. Alarmelor nr. 678"
            };

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //Act
            var result = client.PutAsync(url, customer, formatter).Result;
            var fetchedCustomerReq = client.GetAsync(url).Result;
            var fetchedUpdatedCustomer = fetchedCustomerReq.Content.ReadAsAsync<CustomerUpdateDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
            fetchedUpdatedCustomer.Should().NotBeNull();
            //fetchedUpdatedCustomer.Name.Should().Be("Carlos Bravos");
        }

        [TestMethod]
        public void UpdateCustomer_InexistentCode_ShouldReturnNotFound()
        {
            //Arrange
            var client = new HttpClient();

            var badCustCode = Guid.NewGuid().ToString();

            string url = "http://localhost:4088/api/Customers/" + badCustCode;

            var customer = new CustomerUpdateDto
            {
                Name = "Carlito Bravissimo",
                CNP = "1890915060590",
                Address = "str. Mexico nr. 678"
            };


            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;


            //Act
            var result = client.PutAsync(url, customer, formatter).Result;
            var fetchInexRequest = client.GetAsync(url).Result;
            var updatedCustomer = fetchInexRequest.Content.ReadAsAsync<CustomerUpdateDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            updatedCustomer.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("6480958", "Carlos Bravo", "1890915999999", "str. Mexico nr. 678")]
        [DataRow("6444445", "CarlosBravo", "1890915999999", "str. Mexico nr. 678")]
        [DataRow("6444445", "carlos bravo", "1890915999999", "str. Mexico nr. 678")]
        [DataRow("6444445", "Carlos Bravo", "1891315999999", "str. Mexico nr. 678")]
        [DataRow("6444445", "Carlos Bravo", "1890932999999", "str. Mexico nr. 678")]
        [DataRow("6444445", "Carlos Bravo", "3890911999999", "str. Mexico nr. 678")]
        [DataRow("6444445", "Carlos Bravo", "3890911999999", "54FoPh5q4lh2cusGEfDawbkX4XDTYBGKMzWDwmBCPmEXBYR3gTcbGooS2N1zitgwkdHeNQBLvXcejAwjl3025dS1Og7nvsMi4fR490gEYe4RABM0rarwexPfDJDwzOqlA7ohhfRLW6c6M1HEEOElpqTCLG1SQaXcgIMTkYbps4QalpqPvYqdRXHs2qcWXY0jlRMOl9Bud")]
        [DataRow("6444445", "Ezekiel Zabesaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabb", "1890915999999", "str. Mexico nr. 678")]
        public void CreateCustomer_BadData_ShouldReturnBadRequest(string cc, string name, string cnp, string adr)
        {
            //Arrange
            var client = new HttpClient();

            var customer = new CustomerDto
            {
                CustomerCode = cc,
                Name = name,
                CNP = cnp,
                Address = adr
            };

            string url = "http://localhost:4088/api/Customers";

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //Act
            var result = client.PostAsync(url, customer, formatter).Result;
            var createdCustomer = result.Content.ReadAsAsync<CustomerDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            createdCustomer.Should().BeNull();
        }

        [TestMethod]
        public void CreateCustomer_CustomerCodeIsNull_ShouldReturnBadRequest()
        {
            //Arrange
            var client = new HttpClient();

            var customer = new CustomerDto
            {
                CustomerCode = null,
                Name = "Giovani Nullo",
                CNP = "1951104789654",
                Address = "str. Vrabiei nr. 10"
            };

            string url = "http://localhost:4088/api/Customers";

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //Act
            var result = client.PostAsync(url, customer, formatter).Result;
            var createdCustomer = result.Content.ReadAsAsync<CustomerDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            createdCustomer.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(null, "1190506789123", "al. Penei, nr 25")]
        [DataRow("Horation Bana", null, "al. Penei, nr 25")]
        [DataRow("Horation Bana", "1190506789123", null)]
        public void CreateCustomer_NullValues_ShouldReturnBadRequest(string name, string cnp, string adr)
        {
            //Arrange
            var client = new HttpClient();

            var customer = new CustomerDto
            {
                CustomerCode = Guid.NewGuid().ToString(),
                Name = name,
                CNP = cnp,
                Address = adr
            };

            string url = "http://localhost:4088/api/Cards";

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //Act
            var result = client.PostAsync(url, customer, formatter).Result;
            var createdCustomer = result.Content.ReadAsAsync<CardCreationDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            createdCustomer.Should().BeNull();
        }
    }
}
