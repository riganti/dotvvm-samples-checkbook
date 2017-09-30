using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Hosting;

namespace CheckBook.App.Controls
{
    public class ExpressionTextBox : TextBox
    {
        protected override void OnPreRender(IDotvvmRequestContext context)
        {
            context.ResourceManager.AddRequiredResource("ExpressionTextBox");

            base.OnPreRender(context);
        }

        protected override void AddAttributesToRender(IHtmlWriter writer, IDotvvmRequestContext context)
        {
            writer.AddKnockoutDataBind("cc-ExpressionTextBox", "true");

            base.AddAttributesToRender(writer, context);
        }
    }
}