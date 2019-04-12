using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SimplePayTR.Model;

namespace SimplePayTR.Helper
{
    public class ModelHelper
    {
        public static string ReadEmbedXml(string name)
        {
            string result;
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"SimplePayTR.XML.{name}";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        public static string CreatePosXml(BaseRequest request, string content)
        {
            dynamic model = new System.Dynamic.ExpandoObject();
            IDictionary<string, object> property = new System.Dynamic.ExpandoObject();
            foreach (var item in request.Accounts)
                property.Add(item.Key, item.Value);

            model.Pay = request.Pos;
            model.Account = (System.Dynamic.ExpandoObject)property;

            if (request.Is3D)
            {
                dynamic extra3D = new System.Dynamic.ExpandoObject();
                extra3D.Url = request.Url;
                extra3D.SuccessUrl = request.SuccessUrl;
                extra3D.ErrorUrl = request.ErrorUrl;
                model.Extra = extra3D;
            }

            string renderResult = RazorEngine.Razor.Parse(content, model);
            return renderResult;
        }

        public static string GetInlineContent(string post, string key)
        {
            var resultCode = string.Empty;

            var inlineContent = @"(<({0})[^>]*\/?>)(.*?)(<\/(?:\2)>)";
            var result = post.Match(string.Format(inlineContent, key));

            if (result.Success)
            {
                resultCode = result.Groups[3].Value;
            }

            if (!string.IsNullOrEmpty(resultCode)) return resultCode;
            inlineContent = @"({0}=)(\x22)(.*?)(\x22)";
            result = post.Match(string.Format(inlineContent, key));
            if (result.Success)
                resultCode = result.Groups[3].Value;
            return resultCode;
        }

        
    }
}