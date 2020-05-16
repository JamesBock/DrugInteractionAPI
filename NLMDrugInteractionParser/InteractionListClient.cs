using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NLMDrugInteractionParser
{
    public class InteractionListClient : HttpClient
    {
        public InteractionListClient()
        {
            BaseAddress = new Uri("https://rxnav.nlm.nih.gov/REST/interaction/");
        }
    }
}
