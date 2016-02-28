using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Devcorner.NIdenticon;
using Devcorner.NIdenticon.BlockGenerators;
using Devcorner.NIdenticon.BrushGenerators;
using DotVVM.Framework.Hosting;
using DotVVM.Framework.Runtime;

namespace CheckBook.App
{
    public class IdenticonPresenter : IDotvvmPresenter
    {

        /// <summary>
        /// Generates the Identicon image and returns it to the client.
        /// </summary>
        public Task ProcessRequest(DotvvmRequestContext context)
        {
            // generate the identicon
            var identicon = new IdenticonGenerator("SHA512", new Size(180, 180), Color.White, new Size(8, 8))
            {
                DefaultBrushGenerator = new StaticColorBrushGenerator(Color.FromArgb(255, 41, 128, 185)),
                DefaultBlockGenerators = IdenticonGenerator.ExtendedBlockGeneratorsConfig
            };
            var name = Convert.ToString(context.Parameters["Identicon"]);
            using (var bitmap = identicon.Create(name))
            {
                // save it in the response stream
                context.OwinContext.Response.ContentType = "image/png";
                bitmap.Save(context.OwinContext.Response.Body, ImageFormat.Png);
            }

            return Task.FromResult(0);
        }

        public IViewModelSerializer ViewModelSerializer { get; }
    }
}