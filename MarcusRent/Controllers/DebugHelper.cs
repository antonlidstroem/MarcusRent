using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MarcusRent.Controllers
{
    public static class DebugHelper
    {
        public static void DebugModelStatePostCreate(ModelStateDictionary state)
        {
            foreach (var entry in state)
            {
                var key = entry.Key;
                var errors = entry.Value.Errors;

                Console.WriteLine(state.ToString());

                foreach (var error in errors)
                {
                    Console.WriteLine($"ModelState error for {key}: {error.ErrorMessage}");
                }
            }
        }
    }
}
