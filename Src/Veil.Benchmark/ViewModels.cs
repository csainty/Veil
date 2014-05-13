using System.Collections.Generic;
using System.Dynamic;

namespace Veil.Benchmark
{
    public static class ViewModels
    {
        public static ViewModel CreateTypedViewModel()
        {
            return new ViewModel
            {
                Name = "Test Template",
                IsLoggedIn = true,
                Roles = new[] { "User", "Admin", "Editor", "Viewer", "Uploader", "Pick & Pack" }
            };
        }

        public static dynamic CreateDynamicViewModel()
        {
            dynamic model = new ExpandoObject();
            model.Name = "Test Template";
            model.IsLoggedIn = true;
            model.Roles = new[] { "User", "Admin", "Editor", "Viewer", "Uploader", "Pick & Pack" };
            return model;
        }

        public static IDictionary<string, object> CreateDictionaryModel()
        {
            var model = new Dictionary<string, object>();
            model.Add("Name", "Test Template");
            model.Add("IsLoggedIn", true);
            model.Add("Roles", new[] { "User", "Admin", "Editor", "Viewer", "Uploader", "Pick & Pack" });
            return model;
        }
    }

    public class ViewModel
    {
        public string Name { get; set; }

        public bool IsLoggedIn { get; set; }

        public IEnumerable<string> Roles { get; set; }
    }
}