using System;

namespace Fluorescent.Core
{
    public static class Routes
    {
        public static Node New(string name, Action<Node> config)
        {
            var node = new Node(name, true);
            config.Invoke(node);
            return node;
        }
    }
}
