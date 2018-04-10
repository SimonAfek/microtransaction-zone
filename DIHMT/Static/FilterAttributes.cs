using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Configuration;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace DIHMT.Static
{
    public class ReCaptchaAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var privateKey = WebConfigurationManager.AppSettings["ReCaptchaPrivateKey"];

            if (string.IsNullOrEmpty(privateKey))
            {
                throw new Exception("ReCaptchaPrivateKey not found in AppSettings");
            }

            var gCaptchaResponse = filterContext.RequestContext.HttpContext.Request.Form["g-recaptcha-response"];

            if (string.IsNullOrEmpty(gCaptchaResponse))
            {
                ((Controller)filterContext.Controller).ModelState.AddModelError("ReCaptcha", "Captcha error");

                return;
            }

            var postData = $"secret={privateKey}&response={gCaptchaResponse}";

            var postDataAsBytes = Encoding.UTF8.GetBytes(postData);

            // Create web request
            var request = WebRequest.Create("https://www.google.com/recaptcha/api/siteverify");
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postDataAsBytes.Length;
            var dataStream = request.GetRequestStream();
            dataStream.Write(postDataAsBytes, 0, postDataAsBytes.Length);
            dataStream.Close();

            // Get the response.
            var response = request.GetResponse();

            using (dataStream = response.GetResponseStream())
            {
                if (dataStream != null)
                {
                    using (var reader = new StreamReader(dataStream))
                    {
                        var responseFromServer = JsonConvert.DeserializeObject<ReCaptchaResponse>(reader.ReadToEnd());

                        if (!responseFromServer.success)
                        {
                            ((Controller)filterContext.Controller).ModelState.AddModelError("ReCaptcha", "Captcha error");
                        }
                    }
                }
                else
                {
                    ((Controller)filterContext.Controller).ModelState.AddModelError("ReCaptcha", "Captcha error");
                }
            }
        }

        private class ReCaptchaResponse
        {
            public bool success { get; set; }
        }
    }
}