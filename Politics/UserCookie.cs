//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Politics
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserCookie
    {
        public int UserId { get; set; }
        public string Value { get; set; }
    
        public virtual User User { get; set; }
    }
}
