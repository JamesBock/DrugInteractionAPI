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
        InteractionClient _client => new InteractionClient();
       
        [TestMethod()]
        public void ParseSingleDrugInteractionsTest()
        {
            var parser = new SingleDrugInteractionParser();
            var interactions = parser.ParseDrugInteractions(SingleInteractionString.JString);
           // var interactions = _client.GetInteractions("88014");

            //Are medication with multiple ingredients being parsed correctly?
            Assert.AreEqual("rizatriptan (88014) is resolved to rizatriptan (88014)", interactions.Select(o=>o.Comment).FirstOrDefault());
            //Test for JAMIA interaction inclusion. All DrugBank interactions have "N/A" as severity
            Assert.AreEqual(true, interactions
                                        .SelectMany(x => x.DrugInteractionDetails
                                        .Select(o => o.Severity))
                                        .Any(s => s == "high"));
        }

         [TestMethod()]
        public async Task ParseSingleDrugInteractionsAsyncTest()
        {
            var parser = new SingleDrugInteractionParser();
            var interactions = await parser.ParseDrugInteractionsAsync(SingleInteractionString.JString);
            //var interactions = await _client.GetInteractionsAsync("88014");

            //Are medication with multiple ingredients being parsed correctly?
            Assert.AreEqual("rizatriptan (88014) is resolved to rizatriptan (88014)", interactions.Select(o=>o.Comment).FirstOrDefault());
            //Test for JAMIA interaction inclusion. All DrugBank interactions have "N/A" as severity
            Assert.AreEqual(true, interactions
                                        .SelectMany(x => x.DrugInteractionDetails
                                        .Select(o => o.Severity))
                                        .Any(s => s == "high"));
        }

        [TestMethod()]
        public void ParseDrugInteractionsTest()
        {            
            var interactions = _client.GetInteractionList(rxCUIs);
          

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
            var interactions = await _client.GetInteractionListAsync(new List<string>());
           
            var first = interactions.Select(x => x.DrugInteractionDetails.First().Description).First();
            Assert.AreEqual("No Drug-Drug Interactions Found", first);  
        }
        [TestMethod()]
        public async Task ParseDrugInteractionsAsyncTest()
        {
            var response = _client.GetInteractionListAsync(rxCUIs);
            

            Assert.AreEqual(response.GetType(), typeof(Task<List<MedicationInteractionPair>>));
            var interactions = await response;
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