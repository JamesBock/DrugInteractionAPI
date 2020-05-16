using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NLMDrugInteractionParser
{
    public class InteractionClient : HttpClient
    {   
        //was initially thinking of using a facade pattern to have a client for each endpoint but in what scenario would you need to make call to the different endpoint simutaneously? 
        IDrugInteractionParser parser;
        IDrugInteractionParser singleParser;
        public InteractionClient()
        {
         BaseAddress = new Uri("https://rxnav.nlm.nih.gov/REST/interaction/");
            parser = new DrugInteractionParser();
            singleParser = new SingleDrugInteractionParser();
        }

        public Task<List<MedicationInteractionPair>> GetInteractionListAsync(IEnumerable<string> rxcuis)
        {
          return parser.ParseDrugInteractionsAsync(
                          GetAsync($"list.json?rxcuis={string.Join<string>(" +", rxcuis)}")
                           .GetAwaiter()
                           .GetResult()
                           .Content.ReadAsStringAsync()
                           .GetAwaiter()
                           .GetResult());

        }

        public List<MedicationInteractionPair> GetInteractionList(IEnumerable<string> rxcuis)
        {
          return parser.ParseDrugInteractions(
                          GetAsync($"list.json?rxcuis={string.Join<string>(" +", rxcuis)}")
                           .GetAwaiter()
                           .GetResult()
                           .Content.ReadAsStringAsync()
                           .GetAwaiter()
                           .GetResult());

        }
        
        public List<MedicationInteractionPair> GetInteractions(string rxcui)
        {
          return singleParser.ParseDrugInteractions(
                          GetAsync($"interaction.json?rxcui={rxcui}")
                           .GetAwaiter()
                           .GetResult()
                           .Content.ReadAsStringAsync()
                           .GetAwaiter()
                           .GetResult());

        }
        
        public Task<List<MedicationInteractionPair>> GetInteractionsAsync(string rxcui)
        {
          return singleParser.ParseDrugInteractionsAsync(
                          GetAsync($"interaction.json?rxcui={rxcui}")
                           .GetAwaiter()
                           .GetResult()
                           .Content.ReadAsStringAsync()
                           .GetAwaiter()
                           .GetResult());

        }
    }
}
