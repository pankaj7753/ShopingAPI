using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ShopingAPI.DataLayer.Models
{
    public class CommanProperty
    {
        public CommanProperty()
        {
            CreatedDateTime = System.DateTime.Now;
            UpdatedDateTime = System.DateTime.Now;
            string ipAddress = "";
            if (Dns.GetHostAddresses(Dns.GetHostName()).Length > 0)
            {
                ipAddress = Dns.GetHostAddresses(Dns.GetHostName())[0].ToString();
            }
            IP = ipAddress;
            IsActive = true;
        }
        [Key]
        [StringLength(50)]
        public string Id { get; set; }
        public string IP { get; set; }
        [Column(TypeName = "boolean")] 
        public bool IsActive { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime UpdatedDateTime { get; set; }
        public string ModifyBy { get; set; }
    }
}
