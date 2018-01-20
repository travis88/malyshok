using System;
using System.ComponentModel.DataAnnotations;

namespace cms.dbModel.entity
{
    public class ObjectLinks
    {
        /// <summary>
        /// События
        /// </summary>
        public Guid[] EventsId { get; set; }
        
        /// <summary>
        /// События
        /// </summary>
        public Guid[] OrgsId { get; set; }
        
        /// <summary>
        /// События
        /// </summary>
        public Guid[] SitesId { get; set; }
        /// <summary>
        /// Сайты
        /// </summary>
        public SitesModel[] Sites { get; set;}

        /// <summary>
        /// Привязка к персоне/ главному специалисту
        /// </summary>
        public Guid[] PersonsId { get; set; }
    }
}