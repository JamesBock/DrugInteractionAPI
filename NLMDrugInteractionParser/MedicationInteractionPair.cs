using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NLMDrugInteractionParser
{
    public class MedicationInteractionPair
    {
        /// <summary>
        /// Created for logging purposes
        /// </summary>
        public Guid InteractionId { get; set; }

        /// <summary>
        /// blob that contains all interaction assertions between to medications. May be multiple if a medication has multiple ingredients.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// This was previously a tuple of string but i needed to have more data related to each specific entity not the pair as a whole. 
        /// </summary>
        public (MedicationViewModel, MedicationViewModel) MedicationPair { get; set; }
        
        /// <summary>
        /// Interaction details are for medications or their subcomponents. One Medication Pair can generate many interactions between its constituents.
        /// </summary>
        public List<InteractionDetail> DrugInteractionDetails { get; set; } = new List<InteractionDetail>();


        public class MedicationViewModel
        {
            public string DisplayName { get; set; }

            public string RxCui { get; set; }

        }

        public class InteractionDetail
        {
            /// <summary>
            /// This string is returned using a regex on the Comment element of the GET response blob. Describes the assertion of the interaction.
            /// </summary>
            public string InteractionAssertion { get; set; }

            /// <summary>
            /// DrugBank is always N/A as of 5/15/20. All interactions from JAMIA article are high.
            /// </summary>
            public string Severity { get; set; }

            /// <summary>
            /// The potential outcome of the interaction or why interaction should be noted.
            /// </summary>
            public string Description { get; set; }

            /// <summary>
            /// Links the medication name and the url togetherfor easy linking in the UI
            /// </summary>
            public List<(string, Uri)> LinkTupList { get; set; }
        }
    }
}
