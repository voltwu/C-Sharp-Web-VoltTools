using System.Threading.Tasks;

namespace VoltTools.Models.Views
{
    public class PageView : BaseView
    {
        public int pageId { set; get; }
        public string title { set; get; }
        public string contents { set; get; }
        public string urltext { set; get; }
        public PageView(IDatabase database) : base(database)
        {
        }

        internal async Task LoadData()
        {
            var page = await _database.GetPageAsync(page => page.link.Equals($"/{urltext}.html"));
            contents = page.contents;
            title = page.title;
        }
    }
}
