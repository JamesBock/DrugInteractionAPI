using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLMDrugInteractionParser
{
    public class SingleDrugInteractionParser : IDrugInteractionParser
    {
        public List<MedicationInteractionPair> ParseDrugInteractions(string jstring)
        {
            var interactionList = new List<MedicationInteractionPair>();
            JObject j = new JObject();
            int tokenCount = 0;
            int interactiveTypeCount = 0;
            int interactionPairCount = 0;
            int interactionConceptCount = 0;
            var minConceptTokenList = new List<JToken>();
            var urlTokenList = new List<JToken>();

            j = JObject.Parse(jstring);
            try
            {
                tokenCount = j["interactionTypeGroup"].Children()["interactionType"].Children()["interactionPair"].ToList().Count();
                interactiveTypeCount = j["interactionTypeGroup"].Children()["interactionType"].ToList().Count();
            }
            catch (NullReferenceException)
            {
                var emptyDrug = new MedicationInteractionPair();
                emptyDrug.DrugInteractionDetails.Add(

                                    new MedicationInteractionPair.InteractionDetail()
                                    { Description = "No Drug-Drug Interactions Found", Severity = "N/A", LinkTupList = new List<(string, Uri)>() { ("NIH", new Uri("https://rxnav.nlm.nih.gov/REST/interaction/")) } });
                interactionList.Add(emptyDrug);
                return interactionList;
            }


            for (int f = 0; f < interactiveTypeCount; f++)
            {
                interactionPairCount = j["interactionTypeGroup"][f]["interactionType"].Children()["interactionPair"].ToList().Count();
                interactionConceptCount = j["interactionTypeGroup"][f]["interactionType"].Children()["interactionPair"].Children()["interactionConcept"].ToList().Count();

                for (int i = 0; i < interactionConceptCount; i++)
                {
                    var interaction = new MedicationInteractionPair() { InteractionId = Guid.NewGuid() };

                    interaction.Comment = j["interactionTypeGroup"][f]["interactionType"][0]["comment"].ToString();
                    interaction.MedicationPair = (new MedicationInteractionPair.MedicationViewModel() { DisplayName = j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["interactionConcept"][0]["minConceptItem"]["name"].ToString(), RxCui = j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["interactionConcept"][0]["minConceptItem"]["rxcui"].ToString() }, new MedicationInteractionPair.MedicationViewModel() { DisplayName = j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["interactionConcept"][1]["minConceptItem"]["name"].ToString(), RxCui = j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["interactionConcept"][1]["minConceptItem"]["rxcui"].ToString() });





                        var detail = new MedicationInteractionPair.InteractionDetail();

                        //detail.InteractionAssertion = char.ToUpper(m[p].Groups[0].Value[0]) + m[p].Groups[0].Value.Substring(1);

                        detail.Description = j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["description"].ToString();

                        detail.Severity = j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["severity"].ToString();

                        //if the source is the JAMIA article, the uri is of the article.
                        detail.LinkTupList = new List<(string, Uri)>(j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["interactionConcept"].Children()["minConceptItem"]["name"].ToList()
                                                        .Select(x => x.ToString().ToUpper())
                                                       .Zip(j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["interactionConcept"].Children()["sourceConceptItem"]["url"].ToList(), (first, second) => (first,
                                                          new Uri(second.ToString().Equals("NA")
                                                         ? "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC3422823/"
                                                         : second.ToString()))));

                        interaction.DrugInteractionDetails.Add(detail);
                        interactionList.Add(interaction);
                   
                }
                if (interactionPairCount == 0)
                {
                    var emptyDrug = new MedicationInteractionPair();
                    emptyDrug.DrugInteractionDetails.Add(

                                        new MedicationInteractionPair.InteractionDetail()
                                        { Description = "No Drug-Drug Interactions Found", Severity = "N/A", LinkTupList = new List<(string, Uri)>() { ("NIH", new Uri("https://rxnav.nlm.nih.gov/REST/interaction/")) } });

                }
            }

            return interactionList;
        }


        public Task<List<MedicationInteractionPair>> ParseDrugInteractionsAsync(string jstring)
        {
            return Task.Run(() =>
            {
                var interactionList = new List<MedicationInteractionPair>();
                JObject j = new JObject();
                int tokenCount = 0;
                int interactiveTypeCount = 0;
                int interactionPairCount = 0;
                int interactionConceptCount = 0;
                var minConceptTokenList = new List<JToken>();
                var urlTokenList = new List<JToken>();

                j = JObject.Parse(jstring);
                try
                {
                    tokenCount = j["interactionTypeGroup"].Children()["interactionType"].Children()["interactionPair"].ToList().Count();
                    interactiveTypeCount = j["interactionTypeGroup"].Children()["interactionType"].ToList().Count();
                }
                catch (NullReferenceException)
                {
                    var emptyDrug = new MedicationInteractionPair();
                    emptyDrug.DrugInteractionDetails.Add(

                                        new MedicationInteractionPair.InteractionDetail()
                                        { Description = "No Drug-Drug Interactions Found", Severity = "N/A", LinkTupList = new List<(string, Uri)>() { ("NIH", new Uri("https://rxnav.nlm.nih.gov/REST/interaction/")) } });
                    interactionList.Add(emptyDrug);
                    return interactionList;
                }


                for (int f = 0; f < interactiveTypeCount; f++)
                {
                    interactionPairCount = j["interactionTypeGroup"][f]["interactionType"].Children()["interactionPair"].ToList().Count();
                    interactionConceptCount = j["interactionTypeGroup"][f]["interactionType"].Children()["interactionPair"].Children()["interactionConcept"].ToList().Count();

                    for (int i = 0; i < interactionConceptCount; i++)
                    {
                        var interaction = new MedicationInteractionPair() { InteractionId = Guid.NewGuid() };

                        interaction.Comment = j["interactionTypeGroup"][f]["interactionType"][0]["comment"].ToString();
                        interaction.MedicationPair = (new MedicationInteractionPair.MedicationViewModel() { DisplayName = j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["interactionConcept"][0]["minConceptItem"]["name"].ToString(), RxCui = j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["interactionConcept"][0]["minConceptItem"]["rxcui"].ToString() }, new MedicationInteractionPair.MedicationViewModel() { DisplayName = j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["interactionConcept"][1]["minConceptItem"]["name"].ToString(), RxCui = j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["interactionConcept"][1]["minConceptItem"]["rxcui"].ToString() });





                        var detail = new MedicationInteractionPair.InteractionDetail();

                        //detail.InteractionAssertion = char.ToUpper(m[p].Groups[0].Value[0]) + m[p].Groups[0].Value.Substring(1);

                        detail.Description = j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["description"].ToString();

                        detail.Severity = j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["severity"].ToString();

                        //if the source is the JAMIA article, the uri is of the article.
                        detail.LinkTupList = new List<(string, Uri)>(j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["interactionConcept"].Children()["minConceptItem"]["name"].ToList()
                                                        .Select(x => x.ToString().ToUpper())
                                                       .Zip(j["interactionTypeGroup"][f]["interactionType"][0]["interactionPair"][i]["interactionConcept"].Children()["sourceConceptItem"]["url"].ToList(), (first, second) => (first,
                                                          new Uri(second.ToString().Equals("NA")
                                                         ? "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC3422823/"
                                                         : second.ToString()))));

                        interaction.DrugInteractionDetails.Add(detail);
                        interactionList.Add(interaction);

                    }
                    if (interactionPairCount == 0)
                    {
                        var emptyDrug = new MedicationInteractionPair();
                        emptyDrug.DrugInteractionDetails.Add(

                                            new MedicationInteractionPair.InteractionDetail()
                                            { Description = "No Drug-Drug Interactions Found", Severity = "N/A", LinkTupList = new List<(string, Uri)>() { ("NIH", new Uri("https://rxnav.nlm.nih.gov/REST/interaction/")) } });

                    }
                }

                return interactionList;
            });
        }
    }

}