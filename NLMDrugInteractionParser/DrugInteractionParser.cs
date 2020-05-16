using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NLMDrugInteractionParser
{
    public class DrugInteractionParser : IDrugInteractionParser
    {
        public List<MedicationInteractionPair> ParseDrugInteractions(string jstring)
        {

            var interactionList = new List<MedicationInteractionPair>();
            JObject j = new JObject();
            int tokenCount = 0;
            int fullInteractiveTypeCount = 0;
            int interactionPairCount = 0;
            var minConceptTokenList = new List<JToken>();
            var urlTokenList = new List<JToken>();

            j = JObject.Parse(jstring);
            try
            {
                tokenCount = j["fullInteractionTypeGroup"].Children()["fullInteractionType"].Children()["minConcept"].ToList().Count();
                fullInteractiveTypeCount = j["fullInteractionTypeGroup"].Children()["fullInteractionType"].ToList().Count();
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


            for (int f = 0; f < fullInteractiveTypeCount; f++)
            {
                interactionPairCount = j["fullInteractionTypeGroup"][f]["fullInteractionType"].Children()["interactionPair"].ToList().Count();

                for (int i = 0; i < interactionPairCount; i++)
                {
                    var interaction = new MedicationInteractionPair() { InteractionId = Guid.NewGuid() };

                    interaction.Comment = j["fullInteractionTypeGroup"][f]["fullInteractionType"][i]["comment"].ToString();
                    interaction.MedicationPair = (new MedicationInteractionPair.MedicationViewModel() { DisplayName = j["fullInteractionTypeGroup"][f]["fullInteractionType"][i]["minConcept"][0]["name"].ToString(), RxCui = j["fullInteractionTypeGroup"][f]["fullInteractionType"][i]["minConcept"][0]["rxcui"].ToString() }, new MedicationInteractionPair.MedicationViewModel() { DisplayName = j["fullInteractionTypeGroup"][f]["fullInteractionType"][i]["minConcept"][1]["name"].ToString(), RxCui = j["fullInteractionTypeGroup"][f]["fullInteractionType"][i]["minConcept"][1]["rxcui"].ToString() });

                    //MinConcept also contains the RxCuis, which can be used to .Join these with the MedDTO object thing  

                    interactionList.Add(interaction);

                    var interactionConceptCount = j["fullInteractionTypeGroup"][f]["fullInteractionType"][i]["interactionPair"].Children()["interactionConcept"].ToList().Count();

                    //this find the term "interaction" in the comment element and returns from the term interaction to the next period, which marks the end of the sentence.
                    string PATTERN = @"interaction.(?:(?!\.).)*";
                    var m = Regex.Matches(interaction.Comment, PATTERN);
                    //int parseHelper = 5;
                    for (int p = 0; p < interactionConceptCount; p++)
                    {

                        var detail = new MedicationInteractionPair.InteractionDetail();

                        detail.InteractionAssertion = char.ToUpper(m[p].Groups[0].Value[0]) + m[p].Groups[0].Value.Substring(1);

                        detail.Description = j["fullInteractionTypeGroup"][f]["fullInteractionType"][i]["interactionPair"][p]["description"].ToString();

                        detail.Severity = j["fullInteractionTypeGroup"][f]["fullInteractionType"][i]["interactionPair"][p]["severity"].ToString();

                        //if the source is the JAMIA article, the uri is of the article.
                        detail.LinkTupList = new List<(string, Uri)>(j["fullInteractionTypeGroup"][f]["fullInteractionType"][i]["interactionPair"][p]["interactionConcept"].Children()["minConceptItem"]["name"].ToList()
                                                        .Select(x => x.ToString().ToUpper())
                                                       .Zip(j["fullInteractionTypeGroup"][f]["fullInteractionType"][i]["interactionPair"][p]["interactionConcept"].Children()["sourceConceptItem"]["url"].ToList(), (first, second) => (first,
                                                          new Uri(second.ToString().Equals("NA")
                                                         ? "https://www.ncbi.nlm.nih.gov/pmc/articles/PMC3422823/"
                                                         : second.ToString()))));

                        //detail.InteractionAssertion = (interaction.Comment.Substring(m.Index + 4)[0])+ interaction.Comment.Substring(m.Index + 5)?? "No Details on Source available";

                        //string[] stringArray = { "Drug1", "Drug2" };
                        //var splitString = interaction.Comment.Split(stringArray, StringSplitOptions.RemoveEmptyEntries);
                        //string levelOne =  p == 0 ? splitString[3] : splitString[parseHelper];
                        //var levelTwo  = levelOne.Split("and ", 2)[1];
                        //detail.InteractionAssertion = char.ToUpper(levelTwo[0]) + levelTwo.Substring(1);
                        //while (p > 0)
                        //{

                        //    parseHelper = splitString.Count() - 1 > parseHelper ? parseHelper + 2 : 5;
                        //    break;
                        //}

                        interaction.DrugInteractionDetails.Add(detail);
                    }
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
                return ParseDrugInteractions(jstring);
            });

        }
    }
}
