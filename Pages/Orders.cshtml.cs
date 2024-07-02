using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace woodgrovedemo.Pages
{

    //[Authorize]
    //[Authorize(Roles = "Orders.Manager")]
    public class OrdersModel : PageModel
    {
        public bool UserHasAccess { get; private set; } = false;
        public List<Order> Orders { get; private set; }

        private readonly IConfiguration Configuration;
        private TelemetryClient _telemetry;

        public OrdersModel(IConfiguration configuration, TelemetryClient telemetry)
        {
            Configuration = configuration;
            _telemetry = telemetry;
        }

        public IActionResult OnGet()
        {
            _telemetry.TrackPageView("Orders");

            // User must login to access the online orders
            if (!User.Identity?.IsAuthenticated == true)
            {
                return Page();
            }

            // Check if user is assigned to the Orders.Manager application role
            UserHasAccess = User.IsInRole("Orders.Manager");

            // Add orders
            GenerateOrders();

            return Page();
        }

        private void GenerateOrders()
        {
            Orders = new List<Order>();
            Random r = new Random();
            string[] accounts = { "David12", "Yoel", "Siobhan", "Jas", "Karen", "Avni", "Omar", "Pawan", "Katherin", "Colleen", "Vinu", "Chad", "Jams", "Joseph", "Edward", "Jun", "Berna", "Sameeksha", "Derdus", "Mikki", "Namita", "Mykaela", "Sloan", "Sasha", "Fabian", "Raquel", "Chris", "Abdul", "Levent", "Steven", "Iris", "Kaushik", "Inbar", "Rajesh", "Razi", "Michele", "Celeste", "Rachel", "Erman", "Katherine", "Austin", "Santosh", "Gayan", "Anton", "Lukas", "Sophia", "Charlotte", "Anna", "Alice", "Olivia", "Travis", "Bill", "Ipshita", "Ciara", "Hannah" };
            int NuberOfOrders = r.Next(5, 15);

            for (int i = 0; i < NuberOfOrders; i++)
            {
                Order order = new Order();
                order.ID = r.Next(1324653, 9987342).ToString("D8");
                order.Items = r.Next(1, 23);
                order.Total = r.Next(3, 420);
                order.account = accounts[r.Next(0, accounts.Length - 1)] + r.Next(1, 23).ToString();
                Orders.Add(order);
            }


        }
    }

    public class Order
    {
        public string ID { get; set; } = "";
        public string account { get; set; } = "";
        public int Items { get; set; } = 0;
        public int Total { get; set; } = 0;
    }
}
