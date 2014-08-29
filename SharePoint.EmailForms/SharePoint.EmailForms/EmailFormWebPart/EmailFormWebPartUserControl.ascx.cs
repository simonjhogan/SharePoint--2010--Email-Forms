using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace SharePoint.EmailForms
{
    public partial class EmailFormWebPartUserControl : UserControl
    {
        public EmailFormWebPart WebPart { get; set; }
        private Control FormControls;

        protected void Page_Load(object sender, EventArgs e)
        {
            Button button;
            String form = this.WebPart.FormCode;

            if (String.IsNullOrEmpty(form))
            {
                FormErrorMessage.Text = "<p>No form specification found, please update the webpart.</p>";
                FormErrorMessage.Visible = true;
                return;
            }

            try
            {
                this.FormControls = ParseControl(form);
                FormPlaceHolder.Controls.Add(this.FormControls);
                button = FormPlaceHolder.FindControl("_EmailSubmit") as Button;
                if (button != null)
                {
                    button.Click += SubmitButton_Click;
                }
            }
            catch (Exception exp)
            {
                FormErrorMessage.Text = "<p>Form specification contains error, please update the webpart.<p>";
                FormErrorMessage.Text += "<p>" + exp.Message + "</p>";
                FormErrorMessage.Visible = true;
                return;
            }
        }

        protected void SubmitButton_Click(object sender, EventArgs e)
        {
            String sendto = this.WebPart.EmailSendTo;
            String sendfrom = this.WebPart.EmailSendFrom;

            System.Text.StringBuilder message = new System.Text.StringBuilder();
            System.Collections.Specialized.StringDictionary messageHeader = new System.Collections.Specialized.StringDictionary();

            //Page.Validate();

            if (Page.IsValid) {
                message.AppendLine(this.WebPart.EmailIntroduction);
                message.AppendLine();

                foreach (Control ctl in this.FormControls.Controls) {
                    switch (ctl.GetType().ToString()) {
                        case "System.Web.UI.WebControls.TextBox":
                            message.AppendLine(ctl.ID + ":");
                            message.AppendLine(((TextBox)ctl).Text);
                            message.AppendLine();

                            if (!this.WebPart.EmailSendTo.Contains("@")) {
                                if (this.WebPart.EmailSendTo.Equals(((TextBox)ctl).ID)) {
                                    sendto = ((TextBox)ctl).Text;
                                }
                            }

                            if (!this.WebPart.EmailSendFrom.Contains("@")) {
                                if (this.WebPart.EmailSendFrom.Equals(((TextBox)ctl).ID)) {
                                    sendfrom = ((TextBox)ctl).Text;
                                }
                            }
                            break;

                        case "System.Web.UI.WebControls.DropDownList":
                            message.AppendLine(ctl.ID + ":");
                            message.AppendLine(((DropDownList)ctl).Text);
                            message.AppendLine();
                            break;

                        case "System.Web.UI.WebControls.RadioButtonList":
                            message.AppendLine(ctl.ID + ":");
                            message.AppendLine(((RadioButtonList)ctl).Text);
                            message.AppendLine();
                            break;

                        case "System.Web.UI.WebControls.CheckBoxList":
                            message.AppendLine(ctl.ID + ":");
                            foreach (ListItem i in ((CheckBoxList)ctl).Items) {
                                if (i.Selected) {
                                    message.AppendLine(" - " + i.Value);
                                }
                            }
                            message.AppendLine();
                            break;

                        case "System.Web.UI.WebControls.ListBox":
                            message.AppendLine(ctl.ID + ":");
                            if (((ListBox)ctl).SelectionMode == ListSelectionMode.Multiple) {
                                foreach (ListItem i in ((ListBox)ctl).Items) {
                                    if (i.Selected) {
                                        message.AppendLine(" - " + i.Value);
                                    }
                                }
                            } else {
                                message.AppendLine(((ListBox)ctl).Text);
                            }
                            message.AppendLine();
                            break;
                    }
                }

                messageHeader.Add("to", sendto);
                messageHeader.Add("from", sendfrom);
                messageHeader.Add("subject", this.WebPart.EmailSubject);
                messageHeader.Add("content-type", "text/plain");

                Microsoft.SharePoint.Utilities.SPUtility.SendEmail(SPContext.Current.Web, messageHeader, message.ToString());
                SPUtility.Redirect(this.WebPart.SubmitRedirectUrl, SPRedirectFlags.Trusted, System.Web.HttpContext.Current);
            }
        }
    }
}
