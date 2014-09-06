using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HMV.Core.Framework.Web.Control
{
    public enum ListViewExSelectableRowClickActions
    {
        None,
        Select,
        Execute
    }

    [ToolboxData("<{0}:ListViewExSelectableRow  ClickAction=\"Select\" runat=\"server\"></{0}:ListViewExSelectableRow>")]
    public class ListViewExSelectableRow : HtmlTableRow
    {
        public ListViewExSelectableRowClickActions ClickAction { get; set; }

        private Button _dummyCommand;
        protected override void OnInit(EventArgs e)
        {
            _dummyCommand = new Button();
            _dummyCommand.CommandName = this.ClickAction.ToString();
            _dummyCommand.Style["display"] = "none";

            HtmlTableCell c = new HtmlTableCell();
            c.Controls.Add(_dummyCommand);
            this.Controls.Add(c);

            base.OnInit(e);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.WriteBeginTag("tr");

            if (this.DesignMode == false)
            {
                if (this.ClickAction != ListViewExSelectableRowClickActions.None)
                    writer.WriteAttribute("onclick",
                        "document.getElementById('" + _dummyCommand.ClientID + "').click();");
            }

            writer.Write(HtmlTextWriter.TagRightChar);

            RenderChildren(writer);

            writer.WriteEndTag("tr");
        }
    }
}
