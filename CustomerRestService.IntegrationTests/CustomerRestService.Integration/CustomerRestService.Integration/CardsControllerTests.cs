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
    public class CardsControllerTests
    {
        [TestMethod]
        public void CreateCard()
        {
            //arrange
            var client = new HttpClient();

            var card = new CardCreationDto
            {
                CustomerCode = GetFirstCustomerCode(),
                CardCode = Guid.NewGuid().ToString().Substring(0,15),
                UniqueNumber = "1234ABRCF777",
                CVVNumber = "12345678AAAA4",
                StartDate = DateTime.Parse("01/02/2017"),
                EndDate = DateTime.Parse("01/03/2017")
            };

            string url = "http://localhost:4088/api/Cards";

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { CamelCaseText = false });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            // act
            var result = client.PostAsync(url, card, formatter).Result;
            var updatedCard = result.Content.ReadAsAsync<CardCreationDto>().Result;

            // assert
            Assert.IsTrue(result.StatusCode == HttpStatusCode.Created);
            Assert.IsTrue(updatedCard != null);
        }

        [TestMethod]
        public void CreateCard_HappyFlow_ShouldReturnCreated()
        {
            //Arrange
            var client = new HttpClient();

            var cardCode = Guid.NewGuid().ToString().Substring(0,15);
            var uniNum = "12345678909876543212348765";

            var card = new CardCreationDto
            {
                CustomerCode = GetFirstCustomerCode(),
                CardCode = cardCode,
                UniqueNumber = uniNum,
                CVVNumber = "43215444",
                StartDate = DateTime.Now.AddYears(-2),
                EndDate = DateTime.Now.AddMonths(3)
            };

            string url = "http://localhost:4088/api/Cards/";

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter
                {
                    NamingStrategy = new CamelCaseNamingStrategy(false, false, false)
                });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            ////Act
            var result = client.PostAsync(url, card, formatter).Result;
            //var fetchRequest = client.GetAsync(url + cardCode + "/").Result;
            //var createdCard = fetchRequest.Content.ReadAsAsync<CardCreationDto>().Result;  
            var createdCard = result.Content.ReadAsAsync<CardCreationDto>().Result;

            ////Assert
            result.StatusCode.Should().Be(HttpStatusCode.Created);
            createdCard.Should().NotBeNull();
            createdCard.UniqueNumber.Should().Be(uniNum);
        }

        [TestMethod]
        public void CreateCard_ExistingCardCode_ShouldReturnBadRequest()
        {
            //Arrange
            var client = new HttpClient();
            string url = "http://localhost:4088/api/Cards/";

            var existingCardCode = client.GetAsync(url)
                                .Result.Content.ReadAsAsync<IEnumerable<CardCreationDto>>()
                                .Result.ToList()
                                .FirstOrDefault().CardCode;
            
            var uniNum = "13121012101022213";

            var card = new CardCreationDto
            {
                CardCode = existingCardCode,
                CustomerCode = GetFirstCustomerCode(),
                UniqueNumber = uniNum,
                CVVNumber = "3210987654321",
                StartDate = DateTime.Now.AddYears(-2),
                EndDate = DateTime.Now.AddMonths(3)
            };


            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter
                {
                    NamingStrategy = new CamelCaseNamingStrategy(false, false, false)
                });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //Act
            var result = client.PostAsync(url, card, formatter).Result;
            //var fetchRequest = client.GetAsync(url + cardCode + "/").Result;
            //var createdCard = fetchRequest.Content.ReadAsAsync<CardCreationDto>().Result;  
            var createdCard = result.Content.ReadAsAsync<CardCreationDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            createdCard.Should().BeNull();
        }

        [TestMethod]
        public void GetCards_HappyFlow_ShouldReturnOk()
        {
            //Arrange
            var client = new HttpClient();

            string url = "http://localhost:4088/api/Cards";

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //Act
            var result = client.GetAsync(url).Result;
            var fetchedCards = result.Content.ReadAsAsync<IEnumerable<CardDto>>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            fetchedCards.Should().NotBeNull();
        }

        [TestMethod]
        public void GetCard_HappyFlow_ShouldReturnOk()
        {
            //Arrange
            var client = new HttpClient();
            string url = "http://localhost:4088/api/Cards/";

            var randomCard = client.GetAsync(url)
                .Result.Content
                .ReadAsAsync<IEnumerable<CardCreationDto>>()
                .Result.ToList()
                .OrderBy(x => Guid.NewGuid())
                .First();

            url += randomCard.CardCode;

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //Act
            var result = client.GetAsync(url).Result;
            var fetchedCard = result.Content.ReadAsAsync<CardCreationDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            fetchedCard.Should().NotBeNull();
            fetchedCard.CVVNumber.Should().Be(randomCard.CVVNumber);
        }

        //This test will break or cause massive server load if dealing with large DBs. In such a case disable the controller's bare GetCustomers and GetCards Actions or wire them to redirect to their paginated variants; then create an admin-only action that hooks into the services' CodeExists methods and implement that in integration testing;
        [TestMethod]
        public void GetCard_InexistentCode_ShouldReturnNotFound()
        {
            //Arrange
            var client = new HttpClient();
            string url = "http://localhost:4088/api/Cards/";

            var badCardCode = Guid.NewGuid().ToString().Substring(0, 15);
            //bool codeDoesExist = true;
            ////This line breaks the test because some entries in the db have null values for CustomerId and it can't turn those into a value
            //var cards = client.GetAsync(url).Result.Content.ReadAsAsync<IEnumerable<CardNoCustIdDto>>().Result.ToList();

            //while (codeDoesExist)
            //{
            //    if (cards.Any(c => c.CardCode == badCardCode))
            //        badCardCode = Guid.NewGuid().ToString().Substring(0, 15);
            //    else codeDoesExist = false;
            //}

            url += badCardCode;

            //Act
            var result = client.GetAsync(url).Result;
            var fetchedCard = result.Content.ReadAsAsync<CardNoCustIdDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            fetchedCard.Should().BeNull();
        }

        [TestMethod]
        public void UpdateCard_HappyFlow_ShouldReturnOk()
        {
            //Arrange
            var client = new HttpClient();
            
            string url = "http://localhost:4088/api/Cards/f9158f43-8d1a-4";

            var newUN = "1010101310101012";

            //var card = new CardDto
            //{
            //    CustomerCode = GetFirstCustomerCode(),
            //    UniqueNumber = newUN,
            //    CVVNumber = "3210987654321",
            //    StartDate = DateTime.Now.AddYears(-2),
            //    EndDate = DateTime.Now.AddMonths(3)
            //};

            var card = new CardDto
            {
                CustomerCode = GetFirstCustomerCode(),
                UniqueNumber = newUN,
                CVVNumber = "21212121",
                StartDate = DateTime.Now.AddYears(-2),
                EndDate = DateTime.Now.AddMonths(3)
            };


            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;


            //Act
            var result = client.PutAsync(url, card, formatter).Result;
            var updatedCardReq = client.GetAsync(url).Result;
            var updatedCard = updatedCardReq.Content.ReadAsAsync<CardDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.NoContent);
            updatedCard.Should().NotBeNull();
            updatedCard.UniqueNumber.Should().Be(newUN);
        }

        [TestMethod]
        public void UpdateCard_InexistentCode_ShouldReturnNotFound()
        {
            //Arrange
            var client = new HttpClient();

            var badCardCode = Guid.NewGuid().ToString();

            string url = "http://localhost:4088/api/Cards/"+badCardCode;

            var newUN = "1010101010101012";

            var card = new CardDto
            {
                CustomerCode = GetFirstCustomerCode(),
                UniqueNumber = newUN,
                CVVNumber = "3210987654321",
                StartDate = DateTime.Now.AddYears(-2),
                EndDate = DateTime.Now.AddMonths(3)
            };


            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;


            //Act
            var result = client.PutAsync(url, card, formatter).Result;
            var fetchInexRequest = client.GetAsync(url).Result;
                var updatedCard = fetchInexRequest.Content.ReadAsAsync<CardDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            updatedCard.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow("24157778723801", "6480958", "123123123123", "123123123123")]
        [DataRow("24157778999999", "44444444444", "123123123123", "123123123123")]
        [DataRow("24157778999999", "6480958", "a23123123123", "123123123123")]
        [DataRow("24157778999999", "6480958", "123123123123", "a23123123123")]
        public void CreateCard_BadData_ShouldReturnBadRequest(string cc, string custCode, string uniNum, string cvv)
        {
            //Arrange
            var client = new HttpClient();

            //var custCodeExists = true;
            //var customers = client.GetAsync("http://localhost:4088/Customers/").Result.Content.ReadAsAsync<IEnumerable<CustomerDto>>().Result.ToList();
            //while (custCodeExists)
            //{
            //    custCode = Guid.NewGuid().ToString();
            //    custCodeExists = customers.Any(c => c.CustomerCode == custCode);
            //}

            //custCode = badCustCode ? custCode : customers.FirstOrDefault().CustomerCode;

            var card = new CardCreationDto
            {
                CardCode = cc,
                CustomerCode = custCode,
                UniqueNumber = uniNum,
                CVVNumber = cvv,
                StartDate = DateTime.Now.AddYears(-2),
                EndDate = DateTime.Now.AddMonths(3)
            };

            string url = "http://localhost:4088/api/Cards";

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //Act
            var result = client.PostAsync(url, card, formatter).Result;
            var createdCard = result.Content.ReadAsAsync<CardCreationDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            createdCard.Should().BeNull();
        }

        [TestMethod]
        public void CreateCard_CardCodeIsNull_ShouldReturnBadRequest()
        {
            //Arrange
            var client = new HttpClient();

            var card = new CardCreationDto
            {
                CardCode = null,
                CustomerCode = GetFirstCustomerCode(),
                StartDate = new DateTime(2010, 11, 11),
                EndDate = new DateTime(2012, 12, 21),
                CVVNumber = "1231231231124",
                UniqueNumber = "1234565432123478"
            };

            string url = "http://localhost:4088/api/Cards";

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //Act
            var result = client.PostAsync(url, card, formatter).Result;
            var createdCard = result.Content.ReadAsAsync<CardCreationDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            createdCard.Should().BeNull();
        }

        [DataTestMethod]
        [DataRow( null, "1231233211", "142358679123")]
        [DataRow("6480958", null, "142358679123")]
        [DataRow("6480958", "1231233211", null)]
        public void CreateCard_NullValues_ShouldReturnBadRequest(string custCode, string cvv, string uniNum)
        {
            //Arrange
            var client = new HttpClient();

            var card = new CardCreationDto
            {
                CardCode = Guid.NewGuid().ToString().Substring(0,15),
                CustomerCode = custCode,
                StartDate = new DateTime(2010, 11, 11),
                EndDate = new DateTime(2012, 12, 21),
                CVVNumber = cvv,
                UniqueNumber = uniNum
            };

            string url = "http://localhost:4088/api/Cards";

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.Converters
                .Add(new StringEnumConverter { NamingStrategy = new CamelCaseNamingStrategy(false, false, false) });
            formatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;

            //Act
            var result = client.PostAsync(url, card, formatter).Result;
            var createdCard = result.Content.ReadAsAsync<CardCreationDto>().Result;

            //Assert
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            createdCard.Should().BeNull();
        }

        public string GetFirstCustomerCode() => new HttpClient()
            .GetAsync("http://localhost:4088/api/Customers")
            .Result.Content
            .ReadAsAsync<IEnumerable<CustomerDto>>()
            .Result.ToList()
            .FirstOrDefault().CustomerCode;
    }
}
