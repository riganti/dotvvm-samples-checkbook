using DotVVM.Framework.Binding;
using DotVVM.Framework.Binding.Expressions;
using DotVVM.Framework.Controls;
using DotVVM.Framework.Hosting;
using System;

namespace CheckBook.App.Controls
{
    public class SearchTextBox : TextBox
    {
        [MarkupOptions(AllowBinding = false)]
        public int SearchDelayInMs
        {
            get { return (int)GetValue(SearchDelayInMsProperty); }
            set { SetValue(SearchDelayInMsProperty, value); }
        }
        public static readonly DotvvmProperty SearchDelayInMsProperty
            = DotvvmProperty.Register<int, SearchTextBox>(c => c.SearchDelayInMs, 200);

        /// <summary>
        /// Gets or sets the command that will be triggered when the control text is changed.
        /// </summary>
        [MarkupOptions(Required = true)]
        public Command DoneTyping
        {
            get { return (Command)GetValue(ChangedProperty); }
            set { SetValue(ChangedProperty, value); }
        }
        public static readonly DotvvmProperty DoneTypingProperty =
            DotvvmProperty.Register<Command, SearchTextBox>(t => t.DoneTyping, null);

        protected override void OnInit(IDotvvmRequestContext context)
        {
            UpdateTextAfterKeydown = true;
            base.OnInit(context);
        }

        protected override void AddAttributesToRender(IHtmlWriter writer, IDotvvmRequestContext context)
        {

            writer.AddKnockoutDataBind("doneTyping", "function () { " + KnockoutHelper.GenerateClientPostBackScript(nameof(DoneTyping), GetCommandBinding(DoneTypingProperty), this, true, null, false, "$element") + " } ");
            writer.AddAttribute("delay",SearchDelayInMs.ToString());
            base.AddAttributesToRender(writer, context);
        }
    }
}