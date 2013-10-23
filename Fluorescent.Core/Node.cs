using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Routing;

namespace Fluorescent.Core
{
    public class Node
    {
        protected bool IsRoot { get; set; }
        protected bool IsAction { get; set; }
        protected readonly string Name;
        protected string Method { get; set; }
        protected string ActionName { get; set; }
        protected string ControllerName { get; set; }
        protected string AreaName { get; set; }
        protected List<string> Parameters { get; set; }
        protected IList<Node> Nodes { get; set; }

        public Node(string name, bool root = false)
        {
            Name = name;
            Nodes = new List<Node>();
            Parameters = new List<string>();
            AreaName = "";
            IsRoot = root;
        }

        public NodeDefinition Get(string name)
        {
            return Action(name, "GET");
        }

        public NodeDefinition Patch(string name)
        {
            return Action(name, "PATCH");
        }

        public NodeDefinition Put(string name)
        {
            return Action(name, "PUT");
        }

        public NodeDefinition Delete(string name)
        {
            return Action(name, "DELETE");
        }

        public NodeDefinition Post(string name)
        {
            return Action(name, "POST");
        }

        public void Group(string name, Action<Node> children)
        {
            var parent = new Node(name);
            children.Invoke(parent);
            Nodes.Add(parent);
        }

        protected virtual NodeDefinition Action(string name, string method)
        {
            var node = new Node(name) { Method = method, IsAction = true };
            Nodes.Add(node);
            return new NodeDefinition(node);
        }

        public class NodeDefinition
        {
            private readonly Node _node;

            public NodeDefinition(Node node)
            {
                _node = node;
            }

            public NodeDefinition To(string action, string controller, string area = "")
            {
                _node.ActionName = action;
                _node.ControllerName = controller;
                _node.AreaName = area;

                return this;
            }

            public NodeDefinition To<TController>(Expression<Action<TController>> action, string area = "")
            {
                _node.ActionName = (default(TController)).GetMemberName(action);
                _node.ControllerName = _node.GetControllerName<TController>();
                _node.AreaName = area;

                return this;
            }

            public NodeDefinition With(params string[] parameters)
            {
                _node.Parameters = (parameters ?? new string[0]).ToList();
                return this;
            }
        }

        public string ToJs(RouteCollection routeCollection = null)
        {
            routeCollection = routeCollection ?? RouteTable.Routes;

            var builder = new StringBuilder();
            BuildJavaScript(routeCollection, builder, Name);
            AddHelperMethod(builder);
            return builder.ToString();
        }

        public string ToJson(RouteCollection routeCollection = null)
        {
            string template = "function () { var {json}; return {routes} }();"
                .Replace("{json}", ToJs(routeCollection))
                .Replace("{routes}", Name);

            return template;
        }

        public Node Connect(Node node)
        {
            node.IsRoot = false;
            Nodes.Add(node);
            return this;
        }

        protected  virtual void AddHelperMethod(StringBuilder value)
        {
            value.AppendLine(Name + @".resolveUrl=function(e,t){var n=[];for(var r in t){if(e.indexOf(r)<0){n.push(r+'='+t[r]);}}var i=e.replace(/:(\w+)/g,function(n,r){var i=t[r];if(!i){throw'missing route value for '+r+' in '+e;}return i;});if(i.indexOf('/:')>0){throw'not all route values were matched';}return n.length===0?i:i+'?'+n.join('&');};");
        }

        protected virtual void BuildJavaScript(RouteCollection routeCollection, StringBuilder builder, string root, bool requiresComma = false)
        {
            if (IsAction)
            {
                var action =
                    string.Format("{0} : {{ url : '{1}', method : '{2}', resolve: function(params) {{ return {3}.resolveUrl(this.url, params); }} }} ",
                        Name,
                        GetUrl(routeCollection),
                        Method,
                        root);
                builder.Append(action);

                if (requiresComma)
                    builder.Append(",");
            }
            else
            {
                builder.AppendFormat(IsRoot ? "{0} = {{" : "{0} : {{", Name);

                foreach (var node in Nodes)
                {
                    node.BuildJavaScript(routeCollection, builder, root, !Nodes.Last().Equals(node));
                }

                builder.Append("}");

                if (IsRoot)
                    builder.Append(";");
                else if (requiresComma)
                    builder.Append(",");
            }
        }

        protected virtual string GetUrl(RouteCollection routes)
        {
            var httpContext = HttpContext.Current;

            if (httpContext == null)
            {
                var request = new HttpRequest("/", "http://localhost", "");
                var response = new HttpResponse(new StringWriter());
                httpContext = new HttpContext(request, response);
            }

            var values = new RouteValueDictionary();
            foreach (var parameter in Parameters)
                values.Add(parameter, string.Format(":{0}", parameter));
            values.Add("area", AreaName);
            values.Add("action", ActionName);
            values.Add("controller", ControllerName);

            var path = routes.GetVirtualPath(new RequestContext(new HttpContextWrapper(httpContext),
                new RouteData()), values);

            if (path == null)
            {
                var message = string.Format("Route not found based on route values: {0}",
                    string.Join(",", new[] {ActionName, ControllerName, AreaName}));
                throw new Exception(message);
            }

            return HttpUtility.UrlDecode(path.VirtualPath);
        }

        public string GetControllerName<T>()
        {
            var controllerName = typeof(T).Name;
            return GetControllerName(controllerName);
        }

        public string GetControllerName(string controllerName)
        {
            if (string.IsNullOrWhiteSpace(controllerName)) return null;

            return controllerName.EndsWith("Controller", StringComparison.InvariantCultureIgnoreCase) 
                ? ReplaceLastOccurrence(controllerName, "controller", "") 
                : controllerName;
        }

        public static string ReplaceLastOccurrence(string source, string find, string replace)
        {
            var place = source.LastIndexOf(find, StringComparison.InvariantCultureIgnoreCase);
            var result = source.Remove(place, find.Length).Insert(place, replace);
            return result;
        }
    }
}