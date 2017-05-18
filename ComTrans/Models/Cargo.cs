//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ComTrans.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class Cargo
    {
        public int Id { get; set; }
        [DisplayName("��������")]
        [Required]
        public string Name { get; set; }
        [Required]
        [DisplayName("����")]
        public Nullable<decimal> Price { get; set; }
        [Required]
        [DisplayName("���-��")]
        public Nullable<int> Count { get; set; }
        [Required]
        [DisplayName("���")]
        public Nullable<int> Weight { get; set; }
        public int OrderId { get; set; }
    
        public virtual Order Order { get; set; }
    }
}