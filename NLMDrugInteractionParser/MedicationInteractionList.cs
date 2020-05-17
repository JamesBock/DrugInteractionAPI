using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLMDrugInteractionParser
{
    public class MedicationInteractionList 
    {
        public List<Constituent> Constituents { get; set; }


    }
    public class Constituent
    {       
        public string Interactor { get; set; }
        public string RxCUI { get; set; }

    }
}
