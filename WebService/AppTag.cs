//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebService
{
    using System;
    using System.Collections.Generic;
    
    public partial class AppTag
    {
        public int Id { get; set; }
        public Nullable<int> AppId { get; set; }
        public Nullable<int> TagId { get; set; }
    
        public virtual AppInfo AppInfo { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
