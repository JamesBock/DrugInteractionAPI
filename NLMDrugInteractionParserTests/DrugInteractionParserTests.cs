using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLMDrugInteractionParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NLMDrugInteractionParser.Tests
{
    [TestClass()]
    public class DrugInteractionParserTests
    {
        List<string> rxCUIs => new List<string>() { "310964", "1113397", "1247756", "313992", "104895", "330765", "608930", "141962", "1161682", "800405" };
        HttpClient _client => new InteractionListClient();


        [TestMethod()]
        public async Task ParseDrugInteractionsTest()
        {

            var parser = new DrugInteractionParser();
            var response = await _client.GetAsync($"list.json?rxcuis={string.Join<string>("+", rxCUIs)}");
            var interactions = parser.ParseDrugInteractions(await response.Content.ReadAsStringAsync());

            //Are medication with multiple ingredients being parsed correctly?
            Assert.AreEqual(29, interactions.Count);
            //Test for JAMIA interaction inclusion. All DrugBank interactions have "N/A" as severity
            Assert.AreEqual(true, interactions
                                        .SelectMany(x => x.DrugInteractionDetails
                                        .Select(o => o.Severity))
                                        .Any(s => s == "high"));
        }

        [TestMethod()]
        public async Task ParseDrugInteractionsNullTest()
        {
        
            var parser = new DrugInteractionParser();
            var response = await _client.GetAsync($"list.json?rxcuis={string.Empty}");
            var interactions = parser.ParseDrugInteractions(await response.Content.ReadAsStringAsync());
            var first = interactions.Select(x => x.DrugInteractionDetails.First().Description).First();
            Assert.AreEqual("No Drug-Drug Interactions Found", first);  
        }
        [TestMethod()]
        public async Task ParseDrugInteractionsAsyncTest()
        {
        
            var parser = new DrugInteractionParser();
            var response = await _client.GetAsync($"list.json?rxcuis={string.Join<string>("+", rxCUIs)}");
            var task = parser.ParseDrugInteractionsAsync(await response.Content.ReadAsStringAsync());

            Assert.AreEqual(task.GetType(), typeof(Task<List<MedicationInteractionPair>>));
            var interactions = await task;
            //Are medication with multiple ingredients being parsed correctly?
            Assert.AreEqual(29, interactions.Count);
            //Test for JAMIA interaction inclusion. All DrugBank interactions have "N/A" as severity
            Assert.AreEqual(true, interactions
                                        .SelectMany(x => x.DrugInteractionDetails
                                        .Select(o => o.Severity))
                                        .Any(s => s == "high"));
        }
    }
}