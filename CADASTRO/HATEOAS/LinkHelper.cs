using System.Collections.Generic;
using System.Linq;

namespace CADASTRO.HATEOAS
{
    public class LinkHelper
    {
        private string _url;
        private string _protocol = "https://";
        public List<Link> ActionsById = new List<Link>();
        public List<Link> GeneralActions = new List<Link>();

        public LinkHelper(string url)
        {
            _url = url;
        }
        public LinkHelper(string url, string protocol)
        {
            _url = url;
            _protocol = protocol;
        }
        public void AddActionById(string rel, string method)
        {
            ActionsById.Add(new Link(_protocol + _url, rel, method));
        }
        public void AddGeneralActions(string rel, string method)
        {
            GeneralActions.Add(new Link(_protocol + _url, rel, method));
        }
        public Link[] GetActionsById(string sufix)
        {
            Link [] tempLinks = new Link[ActionsById.Count];

            for (int i = 0; i < tempLinks.Length; i++)
            {
                tempLinks[i] = new Link(ActionsById[i].Href, ActionsById[i].Rel, ActionsById[i].Method);
            }

            foreach (var link in tempLinks)
            {
                link.Href = link.Href + "/" + sufix;
            }
            return tempLinks;
        }
        public Link[] GetGeneralActions()
        {
            return GeneralActions.ToArray();
        }

        public Link[] GetOpenActions(string sufix)
        {  
            var linkBd = ActionsById.Where(action => action.Method.Equals("GET")).First();
            
            Link [] tempLink = new Link[1];

            tempLink[0] = new Link(linkBd.Href + "/" + sufix, linkBd.Rel, linkBd.Method);

            return tempLink;
        }
    }
}