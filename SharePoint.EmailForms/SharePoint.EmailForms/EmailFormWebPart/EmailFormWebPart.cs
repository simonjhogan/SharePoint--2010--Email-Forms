using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SharePoint.EmailForms
{
    [ToolboxItemAttribute(false)]
    public class EmailFormWebPart : WebPart
    {
        // Visual Studio might automatically update this path when you change the Visual Web Part project item.
        private const string _ascxPath = @"~/_CONTROLTEMPLATES/SharePoint.EmailForms/EmailFormWebPart/EmailFormWebPartUserControl.ascx";

        [Category("Form Settings"), Personalizable(), WebBrowsable, WebDisplayName("Email From Address/Field")]
        public String EmailSendFrom { get; set; }

        [Category("Form Settings"), Personalizable(), WebBrowsable, WebDisplayName("Email To Address/Field")]
        public String EmailSendTo { get; set; }

        [Category("Form Settings"), Personalizable(), WebBrowsable, WebDisplayName("Email Subject")]
        public String EmailSubject { get; set; }

        [Category("Form Settings"), Personalizable(), WebBrowsable, WebDisplayName("Email Introduction")]
        public String EmailIntroduction { get; set; }

        [Category("Form Settings"), Personalizable(), WebBrowsable, WebDisplayName("Form Specification")]
        public String FormCode { get; set; }

        [Category("Form Settings"), Personalizable(), WebBrowsable, WebDisplayName("Success Redirect URL")]
        public String SubmitRedirectUrl { get; set; }

        protected override void CreateChildControls()
        {
            EmailFormWebPartUserControl control = Page.LoadControl(_ascxPath) as EmailFormWebPartUserControl;

            if (control != null)
            {
                control.WebPart = this;
                Controls.Add(control);
            }
        }
    }
}
