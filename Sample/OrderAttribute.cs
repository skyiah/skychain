using Greatbone;

namespace Core
{
    public class OrderAttribute : StateAttribute
    {
        readonly char state;

        public OrderAttribute(char state)
        {
            this.state = state;
        }

        public override bool Check(WebContext ac, object obj)
        {
            var o = obj as Order;
            if (state == 'A')
                return o.addr != null;
            return false;
        }
    }
}