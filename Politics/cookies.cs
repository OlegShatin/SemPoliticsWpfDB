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
    
    public partial class cookies
    {
        public int user_id { get; set; }
        public string value { get; set; }
    
        public virtual users users { get; set; }
    }
}
