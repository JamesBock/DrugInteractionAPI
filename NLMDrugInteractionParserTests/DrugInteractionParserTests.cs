using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLMDrugInteractionParser;
using System;
using System.Collections.Generic;
using System.IO;
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
            var reader = new StreamReader(@"C:\Users\James\source\repos\NLMDrugInteractionParserSolution\NLMDrugInteractionParser\SingleDrugMuliIngredientString.txt");
            var jstring = reader.ReadToEnd();
            var parser = new SingleDrugInteractionParser();
            var interactions = parser.ParseDrugInteractions(jstring);
            //var interactions = _client.GetInteractions(jstring);

            //Are medication with multiple ingredients being parsed correctly?
            Assert.AreEqual("acetaminophen 33.3 MG/ML / diphenhydramine hydrochloride 1.67 MG/ML [Tylenol PM] (1092374) is resolved to acetaminophen (161)", interactions.Select(o => o.Comment).FirstOrDefault());
            //Test for JAMIA interaction inclusion. All DrugBank interactions have "N/A" as severity
            //Assert.AreEqual(true, interactions
            //                            .SelectMany(x => x.DrugInteractionDetails
            //                           .Select(o => o.Severity))
            //                            .Any(s => s == "high"));
        }
        [TestMethod()]
        public async Task ParseSingleDrugInteractionsAsyncTest()
        {
            var reader = new StreamReader(@"C:\Users\James\source\repos\NLMDrugInteractionParserSolution\NLMDrugInteractionParser\SingleDrugMuliIngredientString.txt");
            var jstring = await reader.ReadToEndAsync();
            var parser = new SingleDrugInteractionParser();
            var interactions = await  parser.ParseDrugInteractionsAsync(jstring);
            //var interactions = _client.GetInteractions(jstring);

            //Are medication with multiple ingredients being parsed correctly?
            Assert.AreEqual("acetaminophen 33.3 MG/ML / diphenhydramine hydrochloride 1.67 MG/ML [Tylenol PM] (1092374) is resolved to acetaminophen (161)", interactions.Select(o => o.Comment).FirstOrDefault());
            //Test for JAMIA interaction inclusion. All DrugBank interactions have "N/A" as severity
            //Assert.AreEqual(true, interactions
            //                            .SelectMany(x => x.DrugInteractionDetails
            //                           .Select(o => o.Severity))
            //                            .Any(s => s == "high"));
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