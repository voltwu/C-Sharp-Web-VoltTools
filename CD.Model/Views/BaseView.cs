using CD.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoltTools.Models.Views
{
    public class BaseView
    {
        protected readonly IDatabase _database;
        public BaseView(IDatabase database)
        {
            _database = database;
        }
        public async Task<String> LoadNavigation()
        {
            StringBuilder sb = new StringBuilder();
            var pages = await _database.GetAllPagesAsync(page => page.isnavshow);

            var firstNaviPages = GetListClone(pages.Where(page => page.pid == 0).ToList<Page>());
            foreach (var firstNavi in firstNaviPages.OrderBy(page => page.order))
            {
                if (hasSubPages(firstNavi, pages))
                {
                    sb.Append($"<b-nav-item-dropdown text=\"{firstNavi.title}\" right>");
                    AppendSecondLevelPageOn(firstNavi.id, pages, sb);
                    sb.Append($"</b-nav-item-dropdown>");
                }
                else
                {
                    sb.Append($"<b-nav-item href=\"{firstNavi.link}\">{firstNavi.title}</b-nav-item>");
                }
            }

            return sb.ToString();
        }
        private void AppendSecondLevelPageOn(int pid, List<Page> pages, StringBuilder sb)
        {
            var secondPages = GetListClone(pages.Where(page => page.pid == pid).ToList());
            foreach (var secondPage in secondPages.OrderBy(page => page.order))
            {
                sb.Append($"<b-dropdown-item href=\"{secondPage.link}\">{secondPage.title}</b-dropdown-item>");
            }
        }
        private bool hasSubPages(Page currentPage, List<Page> pages)
        {
            return pages.Any(page => page.pid == currentPage.id);
        }

        private List<Page> GetListClone(List<Page> oldPages)
        {
            return oldPages.GetRange(0, oldPages.Count);
        }
    }
}
