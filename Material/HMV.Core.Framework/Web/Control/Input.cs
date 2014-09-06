using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HMV.Core.Framework.Web.Control
{
    public class Input : System.Web.UI.HtmlControls.HtmlInputText
    {
        public bool lower { get; set; }
        public bool normal { get; set; }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            if (this.lower)
                Attributes["onkeyup"] += "this.value=this.value.toLowerCase();";
            else if (!this.normal)
                Attributes["onkeyup"] += "this.value=this.value.toUpperCase();";
            base.Render(writer);
        }
    }
}
