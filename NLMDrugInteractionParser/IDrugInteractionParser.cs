using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLMDrugInteractionParser
{
    public interface IDrugInteractionParser
    {
        public List<MedicationInteractionPair> ParseDrugInteractions(string jstring);
        public Task<List<MedicationInteractionPair>> ParseDrugInteractionsAsync(string jstring);
    }
}
